using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class DuckLevelManager : MonoBehaviour
{
    [Header("Timelines")]
    [SerializeField] private PlayableDirector introDirector;
    [SerializeField] private PlayableDirector roundWinDirector;
    [SerializeField] private float tubAppearInterval;
    
    [Space]
    [SerializeField] private int numberOfRounds;
    [SerializeField] private List<DragDuckToTubTask> duckTasks;
    [SerializeField] private List<DuckTaskData> taskDatas;
    
    [Header("Scene Data")]
    [SerializeField] private List<Transform> tubPositions;

    [Header("Audio")]
    [SerializeField] private AudioSource narratorAudioSource;
    [SerializeField] private AudioClip tryAgainClip;
    
    [Space]
    [SerializeField] private DuckLevelHud hud;
    
    public event Action OnRoundTasksStart;
    public event Action OnRoundTasksComplete;
    public event Action OnAnyProgressMade;
    public static event Action<int> OnRoundWin;
    
    public List<DragDuckToTubTask> DuckTasks => duckTasks;
    
    private int _currentRoundNumber;

    private void Start()
    {
        PlayIntro(HandleIntroFinished);
    }

    private async void PlayIntro(Action endCallback)
    {
        introDirector.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(introDirector.duration));
        endCallback?.Invoke();
    }

    private void HandleIntroFinished()
    {
        hud.Initialize(numberOfRounds);
        hud.RevealRounds();
        StartRound();
    }

    private void StartRound()
    {
        _currentRoundNumber++;
        ResetTasks();
        InitializeTasks();
        StartTaskIntros();
    }
    
    private void ResetTasks()
    {
        foreach (var duckTask in duckTasks)
        {
            duckTask.ResetTask();
        }
    }
    
    private void InitializeTasks()
    {
        var shuffledTubPosition = tubPositions.OrderBy(a => Guid.NewGuid()).ToList();
        var shuffledPuzzleDatas = taskDatas.OrderBy(a => Guid.NewGuid()).ToList();
        
        for (var i = 0; i < duckTasks.Count; i++)
        {
            var duckTask = duckTasks[i];
            duckTask.Initialize(shuffledTubPosition[i], shuffledPuzzleDatas[i]);
        }
    }

    private void StartTaskIntros()
    {
        foreach (var duckTask in duckTasks)
        {
            duckTask.PlayEntrance(HandleEntranceFinish);
        }
    }

    private async void HandleEntranceFinish()
    {
        if (!duckTasks.All(i => i.DuckEntranceFinished)) return;
        
        await EnableTubs();
        StartTasks();
    }

    private async UniTask EnableTubs()
    {
        var sortedLeftRight = duckTasks.OrderBy(i => i.TubPosition.position.x).ToList();
        
        for (int i = 0; i < sortedLeftRight.Count; i++)
        {
            sortedLeftRight[i].EnableTub();
            await UniTask.Delay(TimeSpan.FromSeconds(tubAppearInterval));
        }
    }

    private void StartTasks()
    {
        foreach (var task in duckTasks)
        {
            task.StartTask();
            task.OnComplete += HandleComplete;
            task.OnFailedAttempt += HandleFailedAttempt;
            task.OnProgressMade += HandleProgressMade;
        }

        OnRoundTasksStart?.Invoke();
    }

    private void HandleProgressMade()
    {
        OnAnyProgressMade?.Invoke();
    }

    private void HandleFailedAttempt()
    {
        narratorAudioSource.PlayOneShot(tryAgainClip);
    }

    private void HandleComplete(DragDuckToTubTask task)
    {
        task.OnComplete -= HandleComplete;
        task.OnFailedAttempt -= HandleFailedAttempt;
        task.OnProgressMade -= HandleProgressMade;
        narratorAudioSource.PlayOneShot(task.Data.ColorAudio);
        
        if (duckTasks.All(i => i.Completed))
        {
            HandleAllTasksCompleted();
        }
    }

    private void HandleAllTasksCompleted()
    {
        OnRoundWin?.Invoke(_currentRoundNumber);
        OnRoundTasksComplete?.Invoke();
        PlayRoundWin(PlayOutros);
    }
    
    private async void PlayRoundWin(Action endCallback)
    {
        roundWinDirector.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(roundWinDirector.duration));
        endCallback?.Invoke();
    }

    private async void PlayOutros()
    {
        var sortedLeftRight = duckTasks.OrderBy(i => i.TubPosition.position.x).ToList();
        
        for (int i = 0; i < sortedLeftRight.Count; i++)
        {
            await sortedLeftRight[i].PlayOutro();
            await UniTask.Delay(TimeSpan.FromSeconds(tubAppearInterval));
        }

        HandleAllOutrosFinished();
    }

    private void HandleAllOutrosFinished()
    {
        if (_currentRoundNumber < numberOfRounds)
        {
            StartRound();
        }
        else
        {
            Debug.Log("END");
        }
    }
}
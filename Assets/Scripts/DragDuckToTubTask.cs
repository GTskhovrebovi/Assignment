using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class DragDuckToTubTask : MonoBehaviour
{
    [Header("Actors")]
    [SerializeField] private Duck duck;
    [SerializeField] private Tub tub;
    
    [Header("Key Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform idlePoint;
    
    [Space]
    [SerializeField] private PlayableDirector outroDirector;
    [SerializeField] private GameObject outroVfx;
    
    [Header("Helper")]
    [SerializeField] private GameObject helperHand;
    [SerializeField] private float helperAnimationDuration;
    
    public Transform TubPosition { get; private set; }
    public DuckTaskData Data { get; private set; }
    public bool Completed { get; private set; }
    public bool DuckEntranceFinished { get; private set; }
    
    public event Action OnFailedAttempt;
    public event Action OnProgressMade;
    public event Action<DragDuckToTubTask> OnComplete;
    
    public void Initialize(Transform tubPosition, DuckTaskData data)
    {
        TubPosition = tubPosition;
        Data = data;
        tub.transform.position = TubPosition.transform.position;
        tub.Initialize(data.Color);
        duck.Initialize(data.Color);
        outroVfx.transform.position = tub.transform.position;
    }

    public void PlayEntrance(Action finishCallback)
    {
        duck.gameObject.SetActive(true);
        duck.SetInteractable(false);
        duck.transform.position = startPoint.position;
        duck.SwimToPosition(idlePoint.position, () =>
        {
            DuckEntranceFinished = true;
            duck.SetInteractable(true);
            duck.Quack();
            finishCallback?.Invoke();
        });
    }
    
    public void EnableTub()
    {
        tub.gameObject.SetActive(true);
        tub.SetInteractable(true);
    }
    
    public void StartTask()
    {
        Completed = false;
        
        duck.SetInteractable(true);
        tub.SetInteractable(true);
        
        duck.OnJumpInTubStart += HandleJumpInTubStart;
        duck.OnJumpInTubFinish += HandleJumpInTubFinish;
        duck.OnTriedToJumpInWrongTub += HandleTriedToJumpInWrongTub;
        duck.OnDuckGrab += HandleDuckGrab;
    }

    private void HandleDuckGrab()
    {
        DisableHelper();
    }

    private void HandleTriedToJumpInWrongTub()
    {
        OnFailedAttempt?.Invoke();
    }

    private void HandleJumpInTubStart()
    {
        OnProgressMade?.Invoke();
    }

    private void HandleJumpInTubFinish()
    {
        tub.Splash();
        CompleteTask();
    }
    
    private void CompleteTask()
    {
        Completed = true;
        duck.SetInteractable(false);
        tub.SetInteractable(false);
        
        duck.OnJumpInTubStart -= HandleJumpInTubStart;
        duck.OnJumpInTubFinish -= HandleJumpInTubFinish;
        duck.OnTriedToJumpInWrongTub -= HandleTriedToJumpInWrongTub;
        duck.OnDuckGrab -= HandleDuckGrab;
        
        OnComplete?.Invoke(this);
    }
    
    public void ResetTask()
    {
        Completed = false;
        DuckEntranceFinished = false;
        duck.gameObject.SetActive(false);
        tub.gameObject.SetActive(false);
    }
    
    public async UniTask PlayOutro()
    {
        outroDirector.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(outroDirector.duration));
        
        duck.gameObject.SetActive(false);
        tub.gameObject.SetActive(false);
    }

    public void EnableHelper()
    {
        duck.SetGlowActive(true);
        helperHand.SetActive(true);
        helperHand.transform.position = idlePoint.position;
        helperHand.transform.DOMove(TubPosition.position, helperAnimationDuration).SetEase(Ease.InOutQuart)
            .SetLoops(2)
            .OnComplete(DisableHelper);
        
    }

    private void DisableHelper()
    {
        helperHand.SetActive(false);
        duck.SetGlowActive(false);
    }
}

[Serializable]
public class DuckTaskData
{
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public AudioClip ColorAudio { get; private set; }
}
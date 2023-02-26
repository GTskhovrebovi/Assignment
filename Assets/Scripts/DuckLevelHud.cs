using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DuckLevelHud : MonoBehaviour
{
    [SerializeField] private float revealInterval;
    [SerializeField] private List<RoundUI> roundUIs;

    private int _numberOfRounds;

    private void OnEnable()
    {
        DuckLevelManager.OnRoundWin += HandleRoundWin;
    }

    private void OnDisable()
    {
        DuckLevelManager.OnRoundWin -= HandleRoundWin;
    }

    public void Initialize(int numberOfRounds)
    {
        _numberOfRounds = numberOfRounds;
    }
    
    private void HandleRoundWin(int numberOfRound)
    {
        roundUIs[numberOfRound-1].Complete();
    }

    public async void RevealRounds()
    {
        for (int i = 0; i < _numberOfRounds; i++)
        {
            roundUIs[i].gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(revealInterval));
        }
    }
}
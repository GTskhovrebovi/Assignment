using System.Linq;
using UnityEngine;

namespace Gameplay.DuckGame
{
    public class HelpManager : MonoBehaviour
    {
        [SerializeField] private DuckLevelManager duckLevelManager;
        [SerializeField] private float helperCooldown;

        private float _timeSinceLastProgress;
        private bool _gameActive;
    
        private void OnEnable()
        {
            duckLevelManager.OnRoundTasksStart += HandleRoundStart;
            duckLevelManager.OnRoundTasksComplete += HandleRoundComplete;
            duckLevelManager.OnAnyProgressMade += HandleAnyProgressMade;
        }
    
        private void OnDisable()
        {
            duckLevelManager.OnRoundTasksStart -= HandleRoundStart;
            duckLevelManager.OnRoundTasksComplete -= HandleRoundComplete;
            duckLevelManager.OnAnyProgressMade -= HandleAnyProgressMade;
        }

        private void Update()
        {
            if (!_gameActive) return;
        
            _timeSinceLastProgress += Time.deltaTime;
            CheckForHelp();
        }

        private void CheckForHelp()
        {
            if (helperCooldown < _timeSinceLastProgress)
            {
                EnableHelpForRandomTask();
            }
        }

        private void EnableHelpForRandomTask()
        {
            var remainingTasks = duckLevelManager.DuckTasks.Where(i => !i.Completed).ToList();
            if (remainingTasks.Count == 0) return;

            var randomIndex = Random.Range(0, remainingTasks.Count);
            var randomTask = remainingTasks[randomIndex];
            randomTask.EnableHelper();
            ResetCooldown();
        }

        private void HandleRoundStart()
        {
            _gameActive = true;
            ResetCooldown();
        }
    
        private void HandleRoundComplete()
        {
            _gameActive = false;
            ResetCooldown();
        }

        private void HandleAnyProgressMade()
        {
            ResetCooldown();
        }

        private void ResetCooldown()
        {
            _timeSinceLastProgress = 0;
        }

    }
}
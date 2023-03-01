using System;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Gameplay.Quiz
{
    public class QuestionsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text question;
        [SerializeField] private List<AnswerButton> answerButtons;

        [Header("Animations")]
        [SerializeField] private AnimationSequencerController startAnimation;
        [SerializeField] private AnimationSequencerController endAnimation;

        private readonly Dictionary<AnswerButton, AnswerData> _activeButtons = new();
        private Action<bool> _gameEndCallback;
        
        public void Initialize(QuestionData questionData)
        {
            _activeButtons.Clear();
            
            question.text = questionData.QuestionText;
            
            foreach (var answerButton in answerButtons)
                answerButton.gameObject.SetActive(false);

            for (var i = 0; i < questionData.Answers.Count; i++)
            {
                var answerButton = answerButtons[i];
                var answerData = questionData.Answers[i];
                
                answerButton.Initialize(questionData.Answers[i].AnswerText);
                answerButton.SetInteractable(true);
                answerButton.OnPress += HandleAnswerButtonPress;
                _activeButtons.Add(answerButtons[i], answerData);
            }
        }

        public void StartQuestion(Action<bool> gameEndCallback)
        {
            _gameEndCallback = gameEndCallback;
            startAnimation.ResetToInitialState();
            startAnimation.Play(EnableButtons);
        }

        private async void EnableButtons()
        {
            foreach (var button in _activeButtons.Keys)
            {
                button.gameObject.SetActive(true);
                button.AnimateEnable();
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }

        private void HandleAnswerButtonPress(AnswerButton answerButton)
        {
            if (!_activeButtons.ContainsKey(answerButton)) return;
            
            foreach (var button in _activeButtons.Keys)
            {
                button.SetInteractable(false);
            }

            var clickedCorrect = _activeButtons[answerButton].IsCorrect;
            
            if (clickedCorrect)
            {
                answerButton.AnimateCorrect(() =>
                {
                    EndGame(true);
                });

            }
            else
            {
                answerButton.AnimateIncorrect(() =>
                {
                    var endGameScheduled = false;
                    //in case if there are multiple correct answers
                    foreach (var (button, data) in _activeButtons)
                    {
                        if (data.IsCorrect)
                        {
                            if (endGameScheduled) button.AnimateCorrect();
                            else button.AnimateCorrect(() =>
                            {
                                endGameScheduled = true;
                                EndGame(false);
                            });
                        }
                    }
                });
            }
            
        }

        private async void EndGame(bool hasWon)
        {
            await AnimateOutro();
            foreach (var button in _activeButtons.Keys)
            {
                button.OnPress -= HandleAnswerButtonPress;
            }
            _activeButtons.Clear();
            _gameEndCallback?.Invoke(hasWon);
        }
        
        private async UniTask AnimateOutro()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            endAnimation.Play();
            foreach (var button in _activeButtons.Keys)
            {
                button.AnimateDisable();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }

        public void SetProgressText(int current, int total)
        {
            progressText.text = $"{current}/{total}";
        }
        
    }
}
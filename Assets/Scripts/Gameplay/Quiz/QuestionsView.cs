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
        [SerializeField] private TMP_Text question;
        [SerializeField] private List<AnswerButton> answerButtons;

        [SerializeField] private AnimationSequence introAnimation;
        [SerializeField] private AnimationSequence outroAnimation;

        private Dictionary<AnswerButton, AnswerData> _activeButtons = new();
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
            introAnimation.Play(EnableButtons);
        }

        private async void EnableButtons()
        {
            foreach (var button in _activeButtons.Keys)
            {
                button.gameObject.SetActive(true);
                button.AnimateEnable();
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            }
        }

        private async void HandleAnswerButtonPress(AnswerButton answerButton)
        {
            if (!_activeButtons.ContainsKey(answerButton)) return;
            
            foreach (var button in _activeButtons.Keys)
            {
                button.SetInteractable(false);
            }

            var clickedCorrect = _activeButtons[answerButton].IsCorrect;
            
            if (clickedCorrect)
            {
                answerButton.AnimateCorrect(async () =>
                {
                    await AnimateOutro();
                    _gameEndCallback?.Invoke(true);
                });

            }
            else
            {
                answerButton.AnimateIncorrect(async () =>
                {
                    //if there are multiple correct answers
                    foreach (var (button, data) in _activeButtons)
                    {
                        if (data.IsCorrect)
                        {
                            button.AnimateCorrect();
                        }
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                    
                    await AnimateOutro();
                    _gameEndCallback?.Invoke(false);
                });

            }
        }


        private async UniTask AnimateOutro()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            outroAnimation.Play();
            foreach (var button in _activeButtons.Keys)
            {
                button.AnimateDisable();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            Debug.Log("End");
        }
        
    }
}
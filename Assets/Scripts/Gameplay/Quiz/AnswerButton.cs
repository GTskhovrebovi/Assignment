using System;
using BrunoMikoski.AnimationSequencer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Quiz
{
    public class AnswerButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text answerText;
        [SerializeField] private AnimationSequencerController correctAnimation;
        [SerializeField] private AnimationSequencerController incorrectAnimation;
        [SerializeField] private AnimationSequencerController enableAnimation;
        [SerializeField] private AnimationSequencerController disableAnimation;
        
        public event Action<AnswerButton> OnPress;
        public void Initialize(string answer)
        {
            Reset();
            answerText.text = answer;
        }

        public void SetInteractable(bool interactable)
        {
            button.interactable = interactable;
        }
        
        public void Press()
        {
            OnPress?.Invoke(this);
        }

        public void AnimateCorrect(Action finishCallback = null)
        {
            correctAnimation.Play(() => finishCallback?.Invoke());
        }
        
        public void AnimateIncorrect(Action finishCallback = null)
        {
            incorrectAnimation.Play(() => finishCallback?.Invoke());
        }
        
        public void AnimateEnable(Action finishCallback = null)
        {
            enableAnimation.Play(() => finishCallback?.Invoke());
        }
        
        public void AnimateDisable(Action finishCallback = null)
        {
            disableAnimation.Play(() => finishCallback?.Invoke());
        }

        private void Reset()
        {
            correctAnimation.ResetToInitialState();
            incorrectAnimation.ResetToInitialState();
            enableAnimation.ResetToInitialState();
            disableAnimation.ResetToInitialState();
        }
    }
}
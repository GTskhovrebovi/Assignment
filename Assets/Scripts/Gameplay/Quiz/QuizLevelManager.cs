using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Quiz
{
    public class QuizLevelManager : MonoBehaviour
    {
        [SerializeField] private List<QuestionData> questions;
        [SerializeField] private int numberOfQuestions;
        [SerializeField] private GameObject startView;
        [SerializeField] private QuestionsView questionsView;
        [SerializeField] private GameObject endView;

        private List<QuestionData> _currentGameQuestions = new ();
        private int _currentQuestionIndex;
        
        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        public void StartPress()
        {
            StartGame();
            startView.gameObject.SetActive(false);
        }

        private void StartGame()
        {
            var shuffledQuestions = questions.OrderBy(a => Guid.NewGuid()).ToList();
            
            _currentGameQuestions = shuffledQuestions.Take(numberOfQuestions).ToList();
            _currentQuestionIndex = 0;
            
            questionsView.gameObject.SetActive(true);
            StartNextQuestion();
        }

        private void StartNextQuestion()
        {
            questionsView.Initialize(_currentGameQuestions[_currentQuestionIndex]);
            questionsView.StartQuestion(HandleQuestionEnd);
        }
        
        private void HandleQuestionEnd(bool hasWon)
        {
            _currentQuestionIndex++;

            if (_currentQuestionIndex < _currentGameQuestions.Count)
            {
                StartNextQuestion();
            }
            else
            {
                questionsView.gameObject.SetActive(false);
                endView.gameObject.SetActive(true);
            }
        }
    }

    

}

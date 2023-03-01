using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Quiz
{
    public class QuizLevelManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private List<QuestionData> questions;
        [SerializeField] private int numberOfQuestions;
        [SerializeField] private GradeSystem gradeSystem;
        
        [Header("References")]
        [SerializeField] private GameObject startView;
        [SerializeField] private QuestionsView questionsView;
        [SerializeField] private ResultView resultView;

        private List<QuestionData> _currentGameQuestions = new();
        private int _currentQuestionIndex;
        private int _numberOfCorrectAnswers;
        
        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        public void StartPress()
        {
            StartGame();
            startView.gameObject.SetActive(false);
            resultView.gameObject.SetActive(false);
        }

        private void StartGame()
        {
            var shuffledQuestions = questions.OrderBy(a => Guid.NewGuid()).ToList();
            
            _currentGameQuestions = shuffledQuestions.Take(numberOfQuestions).ToList();
            _currentQuestionIndex = 0;
            _numberOfCorrectAnswers = 0;
            
            questionsView.gameObject.SetActive(true);
            StartNextQuestion();
        }

        private void StartNextQuestion()
        {
            questionsView.Initialize(_currentGameQuestions[_currentQuestionIndex]);
            questionsView.SetProgressText(_currentQuestionIndex+1, _currentGameQuestions.Count);
            questionsView.StartQuestion(HandleQuestionEnd);
        }
        
        private void HandleQuestionEnd(bool hasWon)
        {
            if (hasWon) _numberOfCorrectAnswers++;
            _currentQuestionIndex++;
            if (_currentQuestionIndex < _currentGameQuestions.Count)
            {
                StartNextQuestion();
            }
            else
            {
                questionsView.gameObject.SetActive(false);
                resultView.gameObject.SetActive(true);
                var grade = gradeSystem.GetGrade(_numberOfCorrectAnswers, _currentGameQuestions.Count);
                resultView.Initialize(grade.GradeName, grade.GradeColor, _numberOfCorrectAnswers, _currentGameQuestions.Count);
            }
        }
    }
}

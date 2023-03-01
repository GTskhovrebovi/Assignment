using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Quiz
{
    [Serializable]
    [CreateAssetMenu(fileName = "Question", menuName = "Data/Question Data", order = 1)]
    public class QuestionData : ScriptableObject
    {
        [field: SerializeField] public string QuestionText { get; private set; }
        [field: SerializeField] public List<AnswerData> Answers { get; private set; }
    }

    [Serializable]
    public class AnswerData
    {
        [field: SerializeField] public string AnswerText { get; private set; }
        [field: SerializeField] public bool IsCorrect { get; private set; }
    }
}
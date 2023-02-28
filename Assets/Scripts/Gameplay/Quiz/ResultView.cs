using TMPro;
using UnityEngine;

namespace Gameplay.Quiz
{
    public class ResultView : MonoBehaviour
    {
        [SerializeField] private TMP_Text gradeNameText;
        [SerializeField] private TMP_Text correctAmountText;

        public void Initialize(string gradeName, Color gradeColor, int correctAnswers, int numberOfQuestions)
        {
            gradeNameText.text = gradeName;
            gradeNameText.color = gradeColor;
            correctAmountText.text = $"{correctAnswers}/{numberOfQuestions}";
        }
    }
}
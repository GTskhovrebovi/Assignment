using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Quiz
{
    [CreateAssetMenu(fileName = "Grade System", menuName = "Data/Grade System", order = 1)]
    public class GradeSystem : ScriptableObject
    {
        [SerializeField] private List<Grade> grades = new();
        
        public Grade GetGrade(int amount, int total)
        {
            for (var i = grades.Count-1; i >=0; i--)
            {
                if (grades[i].RequirementSatisfied(amount, total))
                    return grades[i];
            }
            
            return grades.First();
        }
    }
    
    [Serializable]
    public class Grade
    {
        [field: SerializeField] public string GradeName { get; private set; }
        [field: SerializeField] public Color GradeColor { get; private set; }
        [field: SerializeField, Range(0, 1)] public float Requirement { get; private set; }
        
        public bool RequirementSatisfied(int amount, int total)
        {
            return Mathf.CeilToInt(total * Requirement) <= amount;
        }
    }
}
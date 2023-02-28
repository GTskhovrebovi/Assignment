using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Quiz
{
    [CreateAssetMenu(fileName = "Grade System", menuName = "Data/Grade System", order = 1)]
    public class GradeSystem : ScriptableObject
    {
        [field: SerializeField] public List<Grade> Grades { get; private set; }
        
        public Grade GetGrade(int amount, int total)
        {
            for (var i = Grades.Count-1; i >=0; i--)
            {
                if (Grades[i].RequirementSatisfied(amount, total))
                    return Grades[i];
            }
            
            return Grades.First();
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
            var x = Mathf.CeilToInt(total * Requirement);
            return Mathf.CeilToInt(total * Requirement) <= amount;
        }
    }
}
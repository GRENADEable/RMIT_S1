using UnityEngine;

namespace Khatim
{
    [CreateAssetMenu(fileName = "Objective_Data", menuName = "Objectives/ObjectiveData")]
    public class ObjectiveData : ScriptableObject
    {
        [TextArea(10, 10)] public string objectiveMessage;
    }
}
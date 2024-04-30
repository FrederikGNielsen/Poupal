
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public abstract class Mission : ScriptableObject
{
    [Header("Text")]
    public string missionName = "PouWork";
    [TextArea]
    public string missionDesc = "This will make Pou strong";

    [Header("EXP")]
    public float STREXP = 0f;
    public float STMNEXP = 0f;
    public float AGLEXP = 0f;
    public float HPEXP = 0f;
    
    [Header("Difficulty")]
    public DifficultyLevel difficultyLevel;

    public abstract void ExecuteMission();
    
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }
}
    
[CreateAssetMenu(fileName = "GymMission", menuName = "Mission/Gym Mission", order = 1)]
public class GymMission : Mission
{
    [Header("Gym Specific")]
    public GymExerciseType gymExerciseType;

    public override void ExecuteMission()
    {
        // Implement execution logic specific to gym missions
    }

    public enum GymExerciseType
    {
        UpperBody,
        LowerBody,
        Cardio,
        // Add more as needed
    }
}

[CreateAssetMenu(fileName = "RunningMission", menuName = "Mission/Running Mission", order = 2)]
public class RunningMission : Mission
{
    [Header("Running Specific")]
    public float distance;

    public override void ExecuteMission()
    {
        // Implement execution logic specific to running missions
    }
}

[CreateAssetMenu(fileName = "SwimmingMission", menuName = "Mission/Swimming Mission", order = 3)]
public class SwimmingMission : Mission
{
    [Header("Swimming Specific")]
    public float laps;

    public override void ExecuteMission()
    {
        // Implement execution logic specific to swimming missions
    }
}

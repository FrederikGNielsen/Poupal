    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "Mission/new mission", order = 1)]
public class MissionImpossible : ScriptableObject
{
    [Header("Text")]
    public string missionName = "PouWork";
    [TextArea]
    public string missionDesc = "This will make Pou strong";

    [Header("EXP")]
    public float STREXP = 10f;
    public float STMNEXP = 10f;
    public float AGLEXP = 10f;
    public float HPEXP = 10f;

}

using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/Quest Data")]
public class Quest : ScriptableObject
{
    public string questID;
    public string title;
    public string description;
    public string questPlace;
    public bool isCompleted;
}
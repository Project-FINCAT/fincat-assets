using UnityEngine;

[System.Serializable]
public class OX
{
    public string questionText;

    public bool answer;

    [TextArea(3, 10)]
    public string explanation;
}
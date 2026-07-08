using UnityEngine;
using System.Collections.Generic;

public class FIResultUIBinder : MonoBehaviour
{
    public FISurveyResult result;

    public List<FIStatItem> statItems;

    public void Bind()
    {
        var axis = result.GetAllAxisPercent();

        int i = 0;
        foreach (var a in axis)
        {
            statItems[i].SetData(a.Key, a.Value);
            i++;
        }
    }
}
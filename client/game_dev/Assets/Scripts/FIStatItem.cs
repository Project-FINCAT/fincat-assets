using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FIStatItem : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Image fill;
    public TextMeshProUGUI percentText;

    public void SetData(string name, float percent)
    {
        label.text = name;
        percentText.text = Mathf.RoundToInt(percent) + "%";
        fill.fillAmount = percent / 100f;
    }
}
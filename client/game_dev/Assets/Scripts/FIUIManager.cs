using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FIUIManager : MonoBehaviour
{
    public static FIUIManager Instance;
    public TextMeshProUGUI optionAText;
    public TextMeshProUGUI optionBText;
    public TextMeshProUGUI optionCText;
    public TextMeshProUGUI situationText;
    public Slider progressBar;

    private void Awake()
    {
        Instance = this;
    }

    public void SetQuestion(FIQuestion q)
    {
        situationText.text = q.situation;

        optionAText.text = q.optionA;
        optionBText.text = q.optionB;
        optionCText.text = q.optionC;
    }
    
    public void UpdateProgress(float value)
    {
        progressBar.value = value;
    }

    public void OnClickOption(int score)
    {
        FIGameManager.Instance.SubmitAnswer(score);
    }
}
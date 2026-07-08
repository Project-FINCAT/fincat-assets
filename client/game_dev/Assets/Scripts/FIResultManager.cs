using UnityEngine;
using TMPro;
using System.Linq;

public class FIResultManager : MonoBehaviour
{
    public static FIResultManager Instance;
    public GameObject resultPanel;
    public FIResultUIBinder uiBinder;

    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gradeText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowResult(FISurveyResult result)
    {
        resultPanel.SetActive(true);

        scoreText.text = Mathf.RoundToInt(result.GetTotalPercent()) + "점";
        gradeText.text = result.GetGrade();

        feedbackText.text = GenerateFeedback(result);

        uiBinder.result = result;
        uiBinder.Bind();
    }

    string GenerateFeedback(FISurveyResult result)
    {
        var axis = result.GetAllAxisPercent();

        var sorted = axis.OrderBy(x => x.Value).ToList();

        var weakest = sorted[0];
        var second = sorted[1];

        string text = "";

        text += $"가장 취약한 영역: {weakest.Key}\n\n";

        text += GetAction(weakest.Key) + "\n";
        text += GetAction(second.Key);

        return text;
    }

    string GetAction(string axis)
    {
        switch (axis)
        {
            case "지출파악": return "지출을 기록해보세요.";
            case "소비절제": return "충동 소비를 줄여보세요.";
            case "부채관리": return "부채 상환 계획을 세워보세요.";
            case "비상금": return "비상금을 마련하세요.";
            case "목표설정": return "재무 목표를 설정하세요.";
        }
        return "";
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class RiskAnswerData
{
    public string text;
    public int score;
}

[System.Serializable]
public class RiskQuestionData
{
    public string question;
    public RiskAnswerData[] answers;
}

public class RiskTestManager : MonoBehaviour
{
    // =========================
    // UI
    // =========================
    [Header("UI")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;

    // =========================
    // DATA
    // =========================
    [Header("Data")]
    [SerializeField] private RiskQuestionData[] questions;

    // =========================
    // REFERENCE
    // =========================
    [Header("Reference")]
    [SerializeField] private TutorialManager tutorialManager;

    // =========================
    // STATE
    // =========================
    private int currentIndex = 0;
    private int totalScore = 0;

    // =================================================
    // START
    // =================================================
    void Start()
    {
        currentIndex = 0;
        totalScore = 0;

        ShowQuestion();
    }

    // =================================================
    // QUESTION DISPLAY
    // =================================================
    void ShowQuestion()
    {
        RiskQuestionData q = questions[currentIndex];

        questionText.text = q.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < q.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);

                TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
                btnText.text = q.answers[i].text;

                int score = q.answers[i].score; // ⭐ 클로저 문제 방지

                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => SelectAnswer(score));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // =================================================
    // ANSWER SELECT
    // =================================================
    void SelectAnswer(int score)
    {
        totalScore += score;
        currentIndex++;

        if (currentIndex < questions.Length)
        {
            ShowQuestion();
        }
        else
        {
            FinishTest();
        }
    }

    // =================================================
    // FINISH TEST
    // =================================================
    void FinishTest()
    {
        string result = GetRiskType(totalScore);

        Debug.Log("총 점수: " + totalScore);
        Debug.Log("투자 성향: " + result);

        // ⭐ 튜토리얼로 결과 전달
        tutorialManager.OnRiskTestComplete(result);

        // UI 종료
        gameObject.SetActive(false);
    }

    // =================================================
    // RISK TYPE CALC
    // =================================================
    string GetRiskType(int score)
    {
        if (score <= 10) return "안정형";
        else if (score <= 15) return "안정추구형";
        else if (score <= 21) return "위험중립형";
        else if (score <= 25) return "적극투자형";
        else return "공격투자형";
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OXManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI resultHeaderText;
    public TextMeshProUGUI explanationText;

    public TextMeshProUGUI nextButtonText;

    public Slider progressSlider;

    public Button oButton;
    public Button xButton;
    public Button nextButton;

    private Image oImage;
    private Image xImage;

    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    public List<OX> questions = new List<OX>();

    public InventoryController inventoryController;
    public GameObject q1_3_Panel;
    public NPCBank bankNPC;

    [SerializeField] private WorldMapController worldMap;

    int currentIndex = 0;
    bool isAnswered = false;

    void Start()
    {
        oImage = oButton.GetComponent<Image>();
        xImage = xButton.GetComponent<Image>();

        progressSlider.maxValue = questions.Count;
        progressSlider.value = 0;

        nextButton.gameObject.SetActive(false);

        ShowQuestion();
    }

    void ShowQuestion()
    {
        isAnswered = false;

        if (currentIndex >= questions.Count)
        {
            EndQuiz();
            return;
        }

        OX q = questions[currentIndex];

        questionText.text = q.questionText;
        resultHeaderText.text = "";
        explanationText.text = "";

        ResetButtonColor();

        oButton.interactable = true;
        xButton.interactable = true;

        nextButton.gameObject.SetActive(false);

        // 마지막 문제면 버튼 텍스트 변경
        if (currentIndex == questions.Count - 1)
        {
            nextButtonText.text = "퀘스트종료";
        }
        else
        {
            nextButtonText.text = "다음문제";
        }
    }

    public void Answer(bool userAnswer)
    {
        if (isAnswered) return;
        isAnswered = true;

        OX q = questions[currentIndex];

        bool isCorrect = (userAnswer == q.answer);

        resultHeaderText.text = isCorrect ? "정답입니다!" : "오답입니다!";
        explanationText.text = q.explanation;

        ShowAnswerColor(q.answer);

        progressSlider.value = currentIndex + 1;

        oButton.interactable = false;
        xButton.interactable = false;

        nextButton.gameObject.SetActive(true);
    }

    public void NextQuestion()
    {
        // 마지막 문제였으면 종료 처리
        if (currentIndex >= questions.Count - 1)
        {
            EndQuiz();
            return;
        }

        currentIndex++;
        ShowQuestion();
    }

    void EndQuiz()
    {
    
        if (bankNPC != null)
        {
            bankNPC.UnlockBank();
        }

        // 보상 지급
        if (inventoryController != null)
        {
            inventoryController.AddItem(3);
            Debug.Log("item3입력");
        }

        // 퀘스트 완료
        QuestManager.Instance.CompleteQuest("Bank");

        q1_3_Panel.SetActive(false);

        PauseController.SetPause(false);

        if (worldMap != null)
        {
            worldMap.canEnterSecurities = true;
            Debug.Log("증권사 입장 가능!");
        }
        else
        {
            Debug.LogError("WorldMap 연결 안됨!");
        }

        if (bankNPC != null)
        {
            bankNPC.OnQuestFinished();
        }
    }

    void ShowAnswerColor(bool correctAnswer)
    {
        if (correctAnswer)
        {
            oImage.color = correctColor;
            xImage.color = wrongColor;
        }
        else
        {
            oImage.color = wrongColor;
            xImage.color = correctColor;
        }
    }

    void ResetButtonColor()
    {
        oImage.color = defaultColor;
        xImage.color = defaultColor;
    }
}
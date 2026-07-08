using UnityEngine;
using System.Collections.Generic;

public class FIGameManager : MonoBehaviour
{
    public static FIGameManager Instance;

    public GameObject testPanel;

    public GameObject q2_3_Panel;

    public List<FIQuestion> questions;
    public FISurveyResult result;

    public InventoryController inventoryController;

    [SerializeField] private WorldMapController worldMap;

    public NPCOwl owlNPC;

    int index = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        result = new FISurveyResult(); // 초기화
        ShowQuestion();
    }

    void ShowQuestion()
    {
        FIUIManager.Instance.SetQuestion(questions[index]);
        FIUIManager.Instance.UpdateProgress((float)(index + 1) / questions.Count);
    }

    public void SubmitAnswer(int score)
    {
        result.totalScore += score;

        switch (questions[index].category)
        {
            case 0: result.expenseTracking += score; break;
            case 1: result.spendingControl += score; break;
            case 2: result.debtManagement += score; break;
            case 3: result.emergencyFund += score; break;
            case 4: result.goalSetting += score; break;
        }

        index++;

        if (index >= questions.Count)
        {
            testPanel.SetActive(false);
            FIResultManager.Instance.ShowResult(result);
        }
        else
        {
            ShowQuestion();
        }
    }

    public void EndQ2()
    {
        if (owlNPC != null)
        {
            owlNPC.UnlockAI();
        }

        Debug.Log("EndQ2 called");
        if (inventoryController != null)
        {
            inventoryController.AddItem(2);
            Debug.Log("item2입력");
        }

        // 퀘스트 완료
        QuestManager.Instance.CompleteQuest("Education");
        q2_3_Panel.SetActive(false);
        PauseController.SetPause(false);

        if (worldMap != null)
        {
            worldMap.canEnterBank = true;
            Debug.Log("은행 입장 가능!");
        }
        else
        {
            Debug.LogError("WorldMap 연결 안됨!");
        }

        if (owlNPC != null)
        {
            owlNPC.OnQuestFinished();
        }
    }
}
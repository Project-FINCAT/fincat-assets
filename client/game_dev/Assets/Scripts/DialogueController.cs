using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject ChoiceButtonPrefab;

    public UnityEngine.Events.UnityAction onForceCloseCallback; // X버튼이 눌렸을 때 실행할 예비용 콜백

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Update is called once per frame
    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void ClearChoices()
    {
        foreach(Transform child in choiceContainer) Destroy(child.gameObject);
    }

    public GameObject CreatChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(ChoiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
        return choiceButton;
    }

   // 새로 추가한 부분: X 버튼을 눌렀을 때 실행될 강제 종료 함수!

    public void ForceCloseDialogue()
    {
        if (onForceCloseCallback != null)
        {
            onForceCloseCallback.Invoke();
        }
        else
        {
            ClearChoices();
            ShowDialogueUI(false);
            PauseController.SetPause(false);
        }

        // 핵심 1: UI 선택 상태 초기화
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        // 핵심 2: 콜백 초기화 (중요)
        onForceCloseCallback = null;
    }
}

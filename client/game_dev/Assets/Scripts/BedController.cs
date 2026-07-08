using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class BedController : MonoBehaviour, IInteractable
{
    [Header("Messages")]
    public string promptMessage;
    public string sleepMessage;

    private bool isBedInteracting = false;

    public bool CanInteract() => !isBedInteracting;

    public void Interact()
    {
        if (DialogueController.Instance == null)
        {
            PassTurnDirectly();
            return;
        }

        isBedInteracting = true;

        // 1. 게임 일시정지
        PauseController.SetPause(true);

        // 2. Dialogue UI 활성화
        DialogueController.Instance.ShowDialogueUI(true);
        DialogueController.Instance.SetDialogueText(promptMessage);
        DialogueController.Instance.ClearChoices();

        // Portrait와 Name 숨기기 (시스템 메시지처럼 보이게)
        DialogueController.Instance.portraitImage.gameObject.SetActive(false);
        DialogueController.Instance.nameText.gameObject.SetActive(false);

        // 3. Yes 버튼 생성
        DialogueController.Instance.CreatChoiceButton("Yes", HandleYesSleep);

        // 4. No 버튼 생성
        DialogueController.Instance.CreatChoiceButton("No", HandleNoSleep);

        // X 버튼 눌렀을 때 No를 누른 것과 똑같이 처리하도록 연결
        DialogueController.Instance.onForceCloseCallback = HandleNoSleep; 
    }

    // Yes 버튼 클릭 시 처리 (async void로 안전하게)
    private async void HandleYesSleep()
    {
        // 자는 도중(페이드아웃 중)에 플레이어가 X를 눌러서 꼬이는 걸 방지하기 위해 즉시 콜백 해제
        DialogueController.Instance.onForceCloseCallback = null;

        // 메시지 변경
        DialogueController.Instance.SetDialogueText(sleepMessage);
        DialogueController.Instance.ClearChoices();

        // 화면 페이드 아웃
        await ScreenFader.Instance.FadeOut();

        // 턴 전환
        TurnManager.Instance.PassTurn();

        // 화면 페이드 인
        await ScreenFader.Instance.FadeIn();

        // UI 닫기 및 초기화 함수 호출
        CloseBedInteraction();
    }

    // No 버튼 클릭 시 처리
    private void HandleNoSleep()
    {
        // UI 닫기 및 초기화 함수 호출
        CloseBedInteraction();
    }

    // 중복되는 종료 코드 함수로 통일
    private void CloseBedInteraction()
    {
        DialogueController.Instance.ShowDialogueUI(false);
        DialogueController.Instance.ClearChoices();
        
        // 꺼뒀던 초상화와 이름을 다시 켜줌! (이거 안 하면 다음 NPC 대화 때 초상화 안 나옴)
        DialogueController.Instance.portraitImage.gameObject.SetActive(true);
        DialogueController.Instance.nameText.gameObject.SetActive(true);

        // 주머니(콜백)를 확실하게 비워줌!
        DialogueController.Instance.onForceCloseCallback = null;

        isBedInteracting = false;

        // 플레이어 이동 허용
        PauseController.SetPause(false);
    }

    // DialogueController 없으면 바로 턴 넘기기
    private void PassTurnDirectly()
    {
        TurnManager.Instance.PassTurn();
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    
    // 추가: 퀘스트 리스트 갱신을 담당할 컨트롤러 참조
    private QuestUIController questUI;

    void Start()
    {
        menuCanvas.SetActive(false);

        // [핵심] MenuCanvas 하위 오브젝트에 부착된 QuestUIController를 찾습니다.
        // (하이러키 구조상 QuestPage에 부착되어 있을 것을 상정합니다)
        questUI = menuCanvas.GetComponentInChildren<QuestUIController>(true);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            // 다른 사유로 게임이 일시정지 중인데 메뉴가 꺼져 있다면, 메뉴를 켜지 않습니다.
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }

            // 메뉴 활성화 상태 토글 (켜기/끄기)
            bool nextState = !menuCanvas.activeSelf;
            menuCanvas.SetActive(nextState);
            
            // 메뉴 상태에 맞춰 게임 일시정지 설정
            PauseController.SetPause(nextState);

            // [추가된 로직] 메뉴가 켜지는 순간에 실시간으로 퀘스트 목록을 다시 그려줍니다.
            if (nextState && questUI != null)
            {
                questUI.RefreshQuestList();
            }
        }
    }
}
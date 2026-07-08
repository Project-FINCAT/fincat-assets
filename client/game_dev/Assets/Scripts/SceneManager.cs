using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Threading.Tasks;

public class SceneChanger : MonoBehaviour
{
    public PlayableDirector director;

async void Start()
{
    var fader = ScreenFader.Instance;

    if (fader != null)
    {
        var cg = fader.GetComponentInChildren<CanvasGroup>();

        if (cg != null)
        {
            cg.alpha = 1; // 시작은 검정
        }

        await fader.FadeIn(); // 부드럽게 등장
    }
}

    // 1. 시작 버튼을 눌렀을 때 실행
    public void GoToCutScene()
    {
        SceneManager.LoadScene("CutScene");
    }

    // 2. 타임라인이 끝났을 때 혹은 특정 시점에 호출할 함수
    public async void GoToMainGame()
    {
        if (ScreenFader.Instance != null)
        {
            await ScreenFader.Instance.FadeOut(); // 🔥 여기서 끝까지 기다림
        }

    SceneManager.LoadScene("Main");
}

    // 버튼에서 호출할 함수
     public async void SkipCutscene()
    {
        // 1. 타임라인 스킵
        if (director != null)
        {
            director.time = director.duration;
            director.Evaluate();
        }

        // 2. 페이드 아웃
        await ScreenFader.Instance.FadeOut();

        // 3. 씬 이동
        SceneManager.LoadScene("Main");
    }

}
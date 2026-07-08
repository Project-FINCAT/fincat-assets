using System;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeInDuration = 1.5f;
    [SerializeField] float fadeOutDuration = 0.5f;
    [SerializeField] CinemachineCamera vcam;

    CinemachinePositionComposer transposer;
    Vector3 originalDamping;

    private void Awake()
    {
        // Instance = this; // 항상 최신 instance 유지

        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        transposer = vcam.GetComponent<CinemachinePositionComposer>();
        originalDamping = transposer.Damping;
    }

    async Task Fade(float targetTransparency, float duration)
    {
        float start = canvasGroup.alpha, t = 0;

        while(t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, t / duration);
            await Task.Yield();
        }

        canvasGroup.alpha = targetTransparency;
    }

    public async Task FadeOut()
    {
        //await Fade(1);
        SetDamping(Vector3.zero);
        await Fade(1, fadeOutDuration);
    }

    public async Task FadeIn()
    {
        //await Fade(0);
        SetDamping(originalDamping);
        await Fade(0, fadeInDuration);
    }

    void SetDamping(Vector3 d)
    {
        if(!transposer) return;
        transposer.Damping = d;
    }
}

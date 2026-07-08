using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public DropSlot[] slots;
    public GameObject nextButton;

    void Awake()
    {
        instance = this;
    }

    public void CheckAllCorrect()
    {
        foreach (var slot in slots)
        {
            if (!slot.isCorrect)
                return;
        }

        // 모두 정답
        nextButton.SetActive(true);
    }
}
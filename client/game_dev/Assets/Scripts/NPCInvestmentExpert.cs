using UnityEngine;

public class NPCInvestmentExpert : MonoBehaviour, IInteractable
{
    public string myType; // Aggressive, Neutral, Stable

    public bool CanInteract() => true;

    public void Interact()
    {
        Debug.Log($"[상호작용 시도] {gameObject.name} / 성향: {myType}");
        if (NewsAnalyzer.Instance != null)
        {
            // 인자에서 id를 빼고 성향만 보냅니다. (Analyzer가 Manager에게 물어볼 것임)
            NewsAnalyzer.Instance.StartAIAnalysis(myType);
        }
    }

    public void StopInteract() { }
}
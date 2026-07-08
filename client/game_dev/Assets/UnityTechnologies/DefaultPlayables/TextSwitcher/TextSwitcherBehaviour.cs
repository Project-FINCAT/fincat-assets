using System;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

[Serializable]
public class TextSwitcherBehaviour : PlayableBehaviour
{
    public Color color = Color.white;
    public float fontSize = 36f;
    
    [TextArea(3, 10)] 
    public string textContent; // 변수명을 'text'에서 'textContent'로 바꿔서 중복을 피합니다.
}
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TextSwitcherClip : PlayableAsset, ITimelineClipAsset
{
    // 1. template을 통해 Behaviour에 정의된 변수(text, color 등)를 가져옵니다.
    public TextSwitcherBehaviour template = new TextSwitcherBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; } // 자막 간의 부드러운 전환을 지원
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // 2. 타임라인이 실행될 때 template 데이터를 바탕으로 Playable을 생성합니다.
        var playable = ScriptPlayable<TextSwitcherBehaviour>.Create(graph, template);
        return playable;
    }
}
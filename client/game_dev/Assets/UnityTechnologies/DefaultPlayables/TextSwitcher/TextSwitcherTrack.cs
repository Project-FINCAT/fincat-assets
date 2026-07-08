using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

[TrackColor(0.1394896f, 0.4411765f, 0.3413077f)]
[TrackClipType(typeof(TextSwitcherClip))]
[TrackBindingType(typeof(TextMeshProUGUI))] // 1. 잘 수정하셨습니다!
public class TextSwitcherTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TextSwitcherMixerBehaviour>.Create (graph, inputCount);
    }

    public override void GatherProperties (PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        // 2. 여기 Text를 TextMeshProUGUI로 변경해야 합니다.
        TextMeshProUGUI trackBinding = director.GetGenericBinding(this) as TextMeshProUGUI;
        if (trackBinding == null)
            return;

        var serializedObject = new UnityEditor.SerializedObject (trackBinding);
        var iterator = serializedObject.GetIterator();
        while (iterator.NextVisible(true))
        {
            if (iterator.hasVisibleChildren)
                continue;

            // 3. 여기도 TextMeshProUGUI로 변경!
            driver.AddFromName<TextMeshProUGUI>(trackBinding.gameObject, iterator.propertyPath);
        }
#endif
        base.GatherProperties (director, driver);
    }
}
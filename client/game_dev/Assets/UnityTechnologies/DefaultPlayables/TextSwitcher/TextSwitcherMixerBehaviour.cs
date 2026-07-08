using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class TextSwitcherMixerBehaviour : PlayableBehaviour
{
    Color m_DefaultColor;
    float m_DefaultFontSize;
    string m_DefaultText;

    TextMeshProUGUI m_TrackBinding;
    bool m_FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as TextMeshProUGUI;

        if (m_TrackBinding == null) return;

        if (!m_FirstFrameHappened)
        {
            m_DefaultColor = m_TrackBinding.color;
            m_DefaultFontSize = m_TrackBinding.fontSize;
            m_DefaultText = m_TrackBinding.text;
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();
        string currentText = m_DefaultText;
        Color currentColor = m_DefaultColor;
        float currentFontSize = m_DefaultFontSize;
        float greatestWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<TextSwitcherBehaviour> inputPlayable = (ScriptPlayable<TextSwitcherBehaviour>)playable.GetInput(i);
            TextSwitcherBehaviour input = inputPlayable.GetBehaviour();

            if (inputWeight > greatestWeight)
            {
                currentText = input.textContent;
                currentColor = input.color;
                currentFontSize = input.fontSize;
                greatestWeight = inputWeight;
            }
        }

        // 실제 텍스트 오브젝트에 데이터 반영
        m_TrackBinding.text = currentText;
        m_TrackBinding.color = new Color(currentColor.r, currentColor.g, currentColor.b, greatestWeight);
        m_TrackBinding.fontSize = currentFontSize;
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        m_FirstFrameHappened = false;
        if (m_TrackBinding != null)
        {
            m_TrackBinding.text = m_DefaultText;
            m_TrackBinding.color = m_DefaultColor;
            m_TrackBinding.fontSize = m_DefaultFontSize;
        }
    }
}
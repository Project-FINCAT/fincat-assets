using System;

// 1. 개별 에이전트의 분석 결과 (이름, 요약, 분석, 예측)
[Serializable] // JSON으로 변환 가능하게 만드는 속성
public class AgentResult
{
    public string agent_name;
    public string summary;
    public string analysis;
    public string prediction;
}

// 2. 서버가 보내주는 전체 결과 묶음 (growth, value, technical)
[Serializable]
public class NewsAnalysisResponse
{
    public AgentResult growth;
    public AgentResult value;
    public AgentResult technical;
}
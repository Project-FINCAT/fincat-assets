from llm.gemini import call_llm

def decide_action(agent, news, reflection, memories):
    prompt = f"""
    너는 투자자 {agent.name}이다.

    성격:
    {agent.personality}

    최근 뉴스:
    {news}

    관련 기억:
    {memories}

    현재 심리 상태:
    {reflection}

    아래 중 하나를 선택하라:
    - 매수
    - 매도
    - 관망

    그리고 이유를 설명하라.
    """

    return call_llm(prompt)
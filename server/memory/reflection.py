from llm.gemini import call_llm

def reflect(memories):
    prompt = f"""
    다음 투자 관련 기억을 보고 투자자의 현재 심리 상태를 요약하라:

    {memories}
    """

    return call_llm(prompt)
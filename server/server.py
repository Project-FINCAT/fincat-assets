from fastapi import FastAPI, HTTPException, Request
from fastapi.responses import JSONResponse
from pydantic import BaseModel
import requests
import json
import re
import itertools
import time
from pathlib import Path
import os
from dotenv import load_dotenv
from datetime import datetime

app = FastAPI()
# python -m uvicorn server:app --reload

# --- 측정 데이터 저장소 ---
MEASUREMENT_FILE = Path("measurement_data.jsonl")  # JSON Lines 형식

def log_measurement(stage: str, agent_type: str = None, prompt_tokens: int = 0, 
                   output_tokens: int = 0, latency: float = 0.0):
    """토큰 수와 지연시간을 JSONL 파일에 기록"""
    measurement = {
        "timestamp": datetime.now().isoformat(),
        "stage": stage,  # "dialogue1" 또는 "refute_judge"
        "agent_type": agent_type,  # "growth", "value", "technical" (dialogue1만)
        "prompt_tokens": prompt_tokens,
        "output_tokens": output_tokens,
        "latency_seconds": latency
    }
    
    with open(MEASUREMENT_FILE, 'a', encoding='utf-8') as f:
        f.write(json.dumps(measurement, ensure_ascii=False) + '\n')

# --- 데이터 모델 정의 ---
class NewsRequest(BaseModel):
    news_content: str

class DebateRequest(BaseModel):
    news_content: str
    growth_opinion: str
    value_opinion: str
    technical_opinion: str
    player_opinion: str

# --- API 키 및 로테이션 설정 ---

load_dotenv()  # .env 파일에서 환경 변수 로드

api_keys_str = os.getenv("GEMINI_API_KEYS", "")

API_KEYS = [key.strip() for key in api_keys_str.split(",") if key.strip()]

if not API_KEYS:
    raise ValueError("API 키가 설정되지 않았습니다. .env 파일에 GEMINI_API_KEYS 환경 변수를 쉼표로 구분하여 설정하세요.")

key_cycle = itertools.cycle(API_KEYS)

# --- 전체 퀘스트 세션 대기시간 추적용 글로벌 변수 ---
# 하나의 뉴스 분석으로 시작해서 토론 판결까지 전체 흐름을 추적합니다.
session_start_time = 0.0
dialogue1_latency = 0.0

# --- 공통 제미나이 호출 함수 (토큰 메타데이터 포함) ---
def call_gemini(prompt):
    current_key = next(key_cycle)
    url = f"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={current_key}"
    
    headers = {'Content-Type': 'application/json'}
    payload = {
        "contents": [{"parts": [{"text": prompt}]}],
        "generationConfig": {
            "temperature": 1.0
        }
    }

    start_time = time.time()
    
    try:
        response = requests.post(url, headers=headers, data=json.dumps(payload))
        latency = time.time() - start_time
        
        if response.status_code != 200:
            print(f"❌ API 호출 실패: {response.status_code}")
            return None, latency, None

        result = response.json()
        text_answer = result['candidates'][0]['content']['parts'][0]['text']
        
        # 토큰 메타데이터 추출 (카멜케이스 사용)
        usage_metadata = result.get('usageMetadata', {})
        prompt_tokens = usage_metadata.get('promptTokenCount', 0)
        output_tokens = usage_metadata.get('candidatesTokenCount', 0)
        
        match = re.search(r'\{.*\}', text_answer, re.DOTALL)
        if match:
            token_info = {
                'prompt_tokens': prompt_tokens,
                'output_tokens': output_tokens,
                'total_tokens': prompt_tokens + output_tokens
            }
            return json.loads(match.group(0)), latency, token_info
        
        print("⚠️ JSON 파싱 실패")
        return None, latency, None
    except Exception as e:
        print(f"💥 에러 발생: {e}")
        import traceback
        traceback.print_exc()
        return None, 0.0, None

# --- 엔드포인트 1: 뉴스 분석 (Dialogue 1) ---
@app.post("/api/analyze-news")
def analyze_news_all(request_data: NewsRequest):
    global session_start_time, dialogue1_latency
    
    # 퀘스트 세션 전체 타이머 시작
    session_start_time = time.time()
    
    prompt = f"""
    당신은 3인의 투자 전문가(공격형, 중립형, 안정형)입니다. 
    제공된 뉴스 데이터를 금융 4대 지표(금리, 유동성, 위험도, 수수료)와 
    연계하여 분석하고 반드시 아래 JSON 형식으로만 응답하세요.
    
    [작성 규칙]
    1. 각 에이전트의 analysis는 유니티 말풍선 UI 가독성을 위해 반드시 '2줄 이내(120자 내외)'로 작성하세요.
    2. 에이전트의 페르소나를 완벽히 유지하고 "AI로서" 같은 로봇 같은 문구는 절대 금지합니다.
    3. 마크다운(```json 등) 없이 순수 JSON만 반환하세요.

    [Few-Shot 예시]
    뉴스 데이터: "정부, 스타트업 육성을 위해 1조 원 규모의 유동성 공급 정책 발표"
    반환 구조 예시:
    {{
      "growth": {{ "agent_name": "공격형 투자자", "summary": "성장주 폭등의 기회구먼!", "analysis": "시장에 유동성이 풀리면 리스크가 큰 기술주와 스타트업이 가장 먼저 반응해. 위험도를 감수하고 공격적으로 매수할 타이밍일세!", "prediction": "상승" }},
      "value": {{ "agent_name": "중립형 투자자", "summary": "시장 과열을 경계해야 합니다.", "analysis": "통화량이 늘어나면 단기 자산 상승은 있겠지만, 향후 금리 인상 압박으로 돌아올 수 있습니다. 우량주 중심으로 비중을 유지하십시오.", "prediction": "보합" }},
      "technical": {{ "agent_name": "안정형 투자자", "summary": "원금 보존이 먼저라네.", "analysis": "유동성 파티 뒤에는 인플레이션과 위험도가 도사리고 있네. 차라리 수수료가 낮고 안전한 국채나 정기 예금으로 자산을 대피시키는 게 현명하구먼.", "prediction": "하락" }}
    }}

    [실제 분석할 뉴스 데이터]: {request_data.news_content}
    """
    
    for _ in range(len(API_KEYS)):
        result, latency, token_info = call_gemini(prompt)
        if result: 
            dialogue1_latency = latency
            print("\n==================================================")
            print(f"⏱️ [1] Dialogue 1 (뉴스 분석 응답): {dialogue1_latency:.3f}초")
            if token_info:
                print(f"📊 토큰: 입력={token_info['prompt_tokens']}, 출력={token_info['output_tokens']}, 총={token_info['total_tokens']}")
                
                # 3가지 에이전트별로 토큰 기록 (한 번의 API 호출을 3으로 분배)
                prompt_per_agent = token_info['prompt_tokens'] // 3
                output_per_agent = token_info['output_tokens'] // 3
                
                for agent_type in ['growth', 'value', 'technical']:
                    log_measurement(
                        stage="dialogue1",
                        agent_type=agent_type,
                        prompt_tokens=prompt_per_agent,
                        output_tokens=output_per_agent,
                        latency=latency
                    )
                
                print(f"   에이전트별 기록: 공격형(growth), 중립형(value), 안정형(technical)")
                print(f"   → 입력={prompt_per_agent}개, 출력={output_per_agent}개 (각 에이전트)")
            
            print("==================================================")
            
            # 토큰 정보를 응답 헤더에 포함
            response_headers = {}
            if token_info:
                response_headers['X-Prompt-Tokens'] = str(token_info['prompt_tokens'])
                response_headers['X-Output-Tokens'] = str(token_info['output_tokens'])
            
            return JSONResponse(content=result, headers=response_headers)
            
    raise HTTPException(status_code=503, detail="분석 실패")

# --- 엔드포인트 2: AI 토론 (Refute & Judge) ---
@app.post("/api/debate")
async def handle_debate(data: DebateRequest):
    global session_start_time, dialogue1_latency
    
    prompt = f"""
    아래 뉴스 및 각 투자자들의 의견을 바탕으로, 서로의 논리를 비판하는 전문가 토론을 시뮬레이션하세요.
    마크다운 없이 순수 JSON만 반환해야 합니다.

    [토론 데이터]
    - 뉴스: {data.news_content}
    - 각 에이전트 의견: [공격]{data.growth_opinion}, [중립]{data.value_opinion}, [안정]{data.technical_opinion}
    - 플레이어(사용자) 주장: {data.player_opinion}

    [토론 규칙]
    1. 안정형이 공격형의 의견 지적하며 2줄 이내로 반박하고 더 나은 행동 제안.
    2. 공격형이 안정형의 의견을 비판하며 2줄 이내로 반박하고 더 나은 행동 제안.
    3. 중립형은 공격형의 의견을 지적하며 2줄 이내로 반박하고 더 나은 행동 제안.
    4. 판사(Judge)가 플레이어 의견을 포함해 2줄 이내로 모든 반박에 대해 최종 결론을 낼 것.

    [프롬프트 엔지니어링 규칙]
    1. 역할 이름 반복 금지
    2. '안정형 의견:' 같은 접두어 금지
    3. 의견 내용만 작성
    4. JSON 외 텍스트 금지

    [Few-Shot 예시]
    {{
    "stable_argument": "자네는 유동성 리스크를 전혀 고려하지 않았구먼. 당장 현금화하기 어려운 자산에 올인하는 건 파산의 지름길일세.",
    "aggressive_argument": "공격적 투자도 좋지만 근거가 빈약해. 금리 변동성에 대한 대비책도 없이 무작정 상승에 베팅하는 건 투기일 뿐이라네.",
    "neutral_argument": "수수료와 세금 계산까지 놓치다니 실망스럽습니다. 겉보기에 화려한 수익률 뒤에 숨겨진 실질 수익을 전혀 계산하지 못하셨군요.",
    "final_summary": "세 전문가의 지적대로 플레이어는 위험도와 유동성 분석이 미흡했네. 금융 지식을 더 쌓고 오기 전까지 시장의 신뢰도는 하락할 걸세."
    }}

    형식:
    {{
    "stable_argument": "의견 내용만",
    "aggressive_argument": "의견 내용만",
    "neutral_argument": "의견 내용만",
    "final_summary": "최종 판결 내용만"
    }}
    """
    
    for _ in range(len(API_KEYS)):
        result, latency, token_info = call_gemini(prompt)
        if result:
            # 유저가 중간에 고민하거나 조작한 실제 물리적 대기 시간까지 포함한 토탈 시간 계산
            total_session_time = time.time() - session_start_time
            
            print("\n==================================================")
            print(f"⏱️ [2] Refute & Judge (토론 반박 생성): {latency:.3f}초")
            if token_info:
                print(f"📊 토큰: 입력={token_info['prompt_tokens']}, 출력={token_info['output_tokens']}, 총={token_info['total_tokens']}")
                
                # Refute & Judge 토큰 기록
                log_measurement(
                    stage="refute_judge",
                    agent_type=None,  # Judge는 특정 에이전트가 아님
                    prompt_tokens=token_info['prompt_tokens'],
                    output_tokens=token_info['output_tokens'],
                    latency=latency
                )
                print(f"   ✓ 측정 데이터 기록 완료")
            
            print(f"⏳ [3] 전체 퀘스트 세션 총 대기시간: {total_session_time:.3f}초")
            print(f"   (※ LLM 순수 연산 총합: {dialogue1_latency + latency:.3f}초)")
            print("==================================================")
            
            # 토큰 정보를 응답 헤더에 포함
            response_headers = {}
            if token_info:
                response_headers['X-Prompt-Tokens'] = str(token_info['prompt_tokens'])
                response_headers['X-Output-Tokens'] = str(token_info['output_tokens'])
            
            return JSONResponse(content=result, headers=response_headers)
            
    raise HTTPException(status_code=503, detail="토론 생성 실패")

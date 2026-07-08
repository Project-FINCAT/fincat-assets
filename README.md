# FINCAT
> **An Open-Source Financial Education Game Framework via Multi-LLM Agent Orchestration**

## 1. Project Overview
**Catbonism** is an open-source, Unity-based financial education simulation framework designed to enhance financial literacy among young adults. Since the core gaming interface is optimized in Korean for target demographic testing, this repository provides the underlying software architecture, asynchronous data pipelines, and the multi-agent logic layer described in the primary manuscript. 

By integrating multi-persona LLM (Large Language Model) agents into a gamified economic ecosystem, the framework allows users to experience adaptive financial debates and simulations, bridging the gap between theoretical macroeconomics and practical personal finance.

---

## 2. Software Architecture & Components

### Client Layer (Unity Engine)
- **Environment:** Unity 2022.3 LTS (C#)
- **Core Modules:** Asynchronous Web Request Manager, Quest & Logic Engine, Interactive Dialogue UI Controller.

### Backend & Orchestration Layer (FastAPI)
- **Environment:** Python 3.10+ / FastAPI
- **Concurrency:** Asynchronous ASGI server optimizing multi-agent text generation and JSON data routing.
- **Agent Orchestration:** Independent multi-agent prompt-engineering and output parsing pipeline using Google Gemini API (`gemini-2.5-flash-lite`).

---

## 3. Multi-LLM Agent System
The framework orchestrates **4 specialized AI agents** to generate dynamic market debates and scaffold user investment decision-making:

1. **Conservative Agent:** Prioritizes risk mitigation, capital preservation, and defensive asset allocation models.
2. **Moderate Agent:** Evaluates structural market trends and delivers balanced, neutralized macroeconomic analysis.
3. **Aggressive Agent:** High-risk, high-return oriented persona focusing on growth stocks and volatile assets.
4. **Moderator (System) Agent:** Manages the overall state machine of the debate, token context windows, and drives the financial news injection pipeline.

---

## 4. Prompt Engineering Methodologies
To maintain consistent agent personas and optimize text readability within the Unity client interface, three core prompt engineering techniques are implemented in the backend orchestration layer:

### 4.1 Persona-based Chain-of-Thought (CoT)
Agents are implicitly guided to analyze economic news through four core financial indicators (**Interest Rates, Liquidity, Risk, and Transaction Fees**) before rendering their final investment posture (`prediction`). This mechanism ensures logical cohesion and prevents generic, repetitive, or erratic agent behavior.

### 4.2 Constraint Engineering
Rigid functional boundaries are established to guarantee robust API-to-client JSON serialization and UI compatibility:
* **Length Constraints:** Agent dialogue is restricted to "within 2 lines (approx. 120 characters)" to prevent text overflow or rendering bugs in Unity's dialogue bubbles.
* **Format Constraints:** Strict negative prompts are utilized to forbid natural language prefaces, markdown wrappers (e.g., \`\`\`json), and robotic disclaimers (e.g., *"As an AI..."*), enforcing a pure JSON state.

### 4.3 Few-Shot Prompting
To eliminate schema violations during high-concurrency parsing, downstream endpoints inject structured input-output pairs. This grounds the LLM’s response format and structural token distribution under a zero-temperature environment.

---

## 5. Configuration (Getting Started)
To run this project locally, you must provide your own Google Gemini API key.

1. **Get your API Key:** Visit [Google AI Studio](https://aistudio.google.com/) and create a new API key.
2. **Setup Environment Variables:**
  - Create a file named `.env` in the root directory of the `server` folder.
   - Add your API key(s) to the file. You can provide multiple keys separated by commas for load balancing (Round-robin):
     ```text
     GEMINI_API_KEYS=your_api_key_1,your_api_key_2,your_api_key_3
     ```
   - The system will automatically rotate through the provided keys to handle high-concurrency requests effectively.
3. **Security Note:** Never commit your `.env` file to GitHub. This repository includes a `.gitignore` file that prevents the `.env` file from being uploaded to the server.

---

## 6. Production Prompt Snippets
As specified in the primary manuscript, the exact, unabridged prompt payloads utilized in the framework's core endpoints are detailed below to ensure full scientific reproducibility.

### 6.1 Endpoint 1: News Analysis Engine (`/api/analyze-news`)
```text
당신은 3인의 투자 전문가(공격형, 중립형, 안정형)입니다. 
제공된 뉴스 데이터를 금융 4대 지표(금리, 유동성, 위험도, 수수료)와 
연계하여 분석하고 반드시 아래 JSON 형식으로만 응답하세요.

[작성 규칙]
1. 각 에이전트의 analysis는 유니티 말풍선 UI 가독성을 위해 반드시 '2줄 이내(120자 내외)'로 작성하세요.
2. 에이전트의 페르소나를 완벽히 유지하고 "AI로서" 같은 로봇 같은 문구는 절대 금지합니다.
3. 마크다운(```json 등) 없이 순수 JSON만 반환하세요.

[실제 분석할 뉴스 데이터]: {news_content}
```

### 6.2 Endpoint 2: AI Refutation & Judgment Engine (/api/debate)
```text
아래 뉴스 및 각 투자자들의 의견을 바탕으로, 서로의 논리를 비판하는 전문가 토론을 시뮬레이션하세요.
마크다운 없이 순수 JSON만 반환해야 합니다.

[토론 규칙]
1. 안정형이 공격형의 의견 지적하며 2줄 이내로 반박하고 더 나은 행동 제안.
2. 공격형이 안정형의 의견을 비판하며 2줄 이내로 반박하고 더 나은 행동 제안.
3. 중립형은 공격형의 의견을 지적하며 2줄 이내로 반박하고 더 나은 행동 제안.
4. 판사(Judge)가 플레이어 의견을 포함해 2줄 이내로 모든 반박에 대해 최종 결론을 낼 것.
```
### 7. License
This project is licensed under the MIT License - see the LICENSE file for details.

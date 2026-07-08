from memory.memory_store import MemoryStore
from memory.reflection import reflect
from decision.planner import decide_action

class BaseAgent:
    def __init__(self, name, personality):
        self.name = name
        self.personality = personality
        self.memory = MemoryStore()

    def process_news(self, news):
        # 1. 뉴스 저장
        self.memory.add(f"[뉴스] {news}", importance=0.8)

        # 2. 기억 검색
        memories = self.memory.retrieve(news)

        # 3. reflection
        summary = reflect(memories)
        self.memory.add(f"[성찰] {summary}", importance=0.9)

        # 4. 행동 결정
        action = decide_action(self, news, summary, memories)

        # 5. 행동도 기억
        self.memory.add(f"[행동] {action}", importance=0.7)

        return action
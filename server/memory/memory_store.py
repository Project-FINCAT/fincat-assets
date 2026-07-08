import time

class MemoryStore:
    def __init__(self):
        self.memories = []

    def add(self, text, importance=0.5):
        self.memories.append({
            "text": text,
            "importance": importance,
            "timestamp": time.time()
        })

    def retrieve(self, query, top_k=5):
        scored = []

        for m in self.memories:
            relevance = 1 if query in m["text"] else 0
            recency = 1 / (time.time() - m["timestamp"] + 1)
            importance = m["importance"]

            score = relevance + recency + importance
            scored.append((score, m))

        scored.sort(key=lambda x: x[0], reverse=True)
        return [m["text"] for _, m in scored[:top_k]]
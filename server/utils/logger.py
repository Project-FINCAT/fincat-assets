import os
import json
import datetime

LOG_DIR = "logs"

if not os.path.exists(LOG_DIR):
    os.makedirs(LOG_DIR)


def save_log_json(npc_id, message, response, memories=None, reflection=None):
    filename = os.path.join(LOG_DIR, f"{npc_id}.jsonl")

    log_data = {
        "time": datetime.datetime.now().isoformat(),
        "npc_id": npc_id,
        "input": message,
        "response": response,
        "memories": memories,
        "reflection": reflection
    }

    with open(filename, "a", encoding="utf-8") as f:
        f.write(json.dumps(log_data, ensure_ascii=False) + "\n")
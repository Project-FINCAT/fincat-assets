import google.generativeai as genai
from config import GEMINI_API_KEY, MODEL_NAME

genai.configure(api_key=GEMINI_API_KEY)
model = genai.GenerativeModel(MODEL_NAME)

def call_llm(prompt: str) -> str:
    response = model.generate_content(prompt)
    return response.text
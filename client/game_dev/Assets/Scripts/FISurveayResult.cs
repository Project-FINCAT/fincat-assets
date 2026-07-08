using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class FISurveyResult
{
    // 원점수 (0~6)
    public int expenseTracking;
    public int spendingControl;
    public int debtManagement;
    public int emergencyFund;
    public int goalSetting;

    // 총점 (0~30)
    public int totalScore;

    // 축 % 계산
    public float GetAxisPercent(int value)
    {
        return (value / 6f) * 100f;
    }

    public Dictionary<string, float> GetAllAxisPercent()
    {
        return new Dictionary<string, float>
        {
            { "지출파악", GetAxisPercent(expenseTracking) },
            { "소비절제", GetAxisPercent(spendingControl) },
            { "부채관리", GetAxisPercent(debtManagement) },
            { "비상금", GetAxisPercent(emergencyFund) },
            { "목표설정", GetAxisPercent(goalSetting) }
        };
    }

    // 종합 점수 %
    public float GetTotalPercent()
    {
        return (totalScore / 30f) * 100f;
    }

    // 등급 계산
    public string GetGrade()
    {
        if (totalScore >= 24) return "재무 습관 우수";
        if (totalScore >= 16) return "성장 중";
        if (totalScore >= 8) return "습관 형성 필요";
        return "재무 습관 재건";
    }

    // 가장 낮은 축 1개
    public string GetWeakestAxis()
    {
        return GetAllAxisPercent()
            .OrderBy(x => x.Value)
            .First().Key;
    }

    // 가장 낮은 축 2개
    public List<string> GetWeakestTwo()
    {
        return GetAllAxisPercent()
            .OrderBy(x => x.Value)
            .Take(2)
            .Select(x => x.Key)
            .ToList();
    }
}
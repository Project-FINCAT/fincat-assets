using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BankLoanUI : MonoBehaviour
{
    public TMP_InputField loanInput;

    public void TakeLoan()
    {
        int amount;

        if(int.TryParse(loanInput.text, out amount))
        {
            FinanceManager.Instance.EarnMoney(amount);
        }
    }
}
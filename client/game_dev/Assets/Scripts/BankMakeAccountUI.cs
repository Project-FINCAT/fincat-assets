using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MakeAccountUI : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public BankTransactionUI transactionUI;

    private string selectedType = "";

    public void SelectChecking()
    {
        selectedType = "Checking";
    }
    public void SelectSaving()
    {
        selectedType = "Saving";
    }
    public void SelectDeposit()
    {
        selectedType = "Deposit";
    }

    public void CreateAccount()
    {
        if(selectedType == "")
        {
            Debug.Log("계좌 종류 선택 필요");
            return;
        }

        FinanceManager.Instance.CreateAccount(
            selectedType,
            passwordInput.text
        );

        transactionUI.RefreshAccountList();

        Debug.Log("게좌 생성 완료");
    }
}
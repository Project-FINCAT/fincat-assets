using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BankTransactionUI : MonoBehaviour
{
    public TMP_InputField moneyInput;
    public TMP_InputField passwordInput;

    public TMP_Dropdown accountDropdown;

    private int selectedAccountIndex;

    void Start()
    {
        RefreshAccountList();
        accountDropdown.onValueChanged.AddListener(OnAccountChanged);
    }

    public void RefreshAccountList()
    {
        accountDropdown.ClearOptions();

        var accounts = FinanceManager.Instance.accounts;

        List<string> options = new List<string>();

        if(accounts.Count == 0)
        {
            options.Add("No Account");
        }
        else
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                options.Add(accounts[i].accountType + " Account " + (i + 1));
            }
        }

        accountDropdown.AddOptions(options);

        accountDropdown.value = 0;
        selectedAccountIndex = 0;
    }

    public void OnAccountChanged(int index)
    {
        selectedAccountIndex = index;
    }

    // ===== 입금 =====
    public void Deposit()
    {
        if (!int.TryParse(moneyInput.text, out int amount))
        {
            Debug.Log("금액 입력 오류");
            return;
        }

        string inputPassword = passwordInput.text;

        var account = FinanceManager.Instance.accounts[selectedAccountIndex];

        if (account.password != inputPassword)
        {
            Debug.Log("비밀번호가 틀렸습니다.");
            return;
        }

        bool success = FinanceManager.Instance.DepositMoney(amount, selectedAccountIndex);

        if (success)
            Debug.Log("입금 성공");
        else
            Debug.Log("입금 실패");
    }

    // ===== 출금 =====
    public void Withdraw()
    {
        if (!int.TryParse(moneyInput.text, out int amount))
        {
            Debug.Log("금액 입력 오류");
            return;
        }

        string inputPassword = passwordInput.text;

        var account = FinanceManager.Instance.accounts[selectedAccountIndex];

        if (account.password != inputPassword)
        {
            Debug.Log("비밀번호가 틀렸습니다.");
            return;
        }

        bool success = FinanceManager.Instance.WithdrawMoney(amount, selectedAccountIndex);

        if (success)
            Debug.Log("출금 성공");
        else
            Debug.Log("출금 실패");
    }

    // 계좌 선택
    public void SetSelectedAccount(int index)
    {
        selectedAccountIndex = index;
    }

    // 비밀번호 확인
    public bool CheckPassword()
    {
        string inputPassword = passwordInput.text;

        var account = FinanceManager.Instance.accounts[selectedAccountIndex];

        return account.password == inputPassword;
    }

}
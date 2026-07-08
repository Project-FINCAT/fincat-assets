using UnityEngine;
using TMPro;

public class BankPageController : MonoBehaviour
{
    public GameObject homePage;
    public GameObject makeAccountPage;
    public GameObject makeAccountPage1;
    public GameObject transactionPage;
    public GameObject transactionPage1;
    public GameObject transactionPage2;
    public TMP_Dropdown accountDropdown;
    public BankTransactionUI transactionUI;
    public GameObject loanPage;

    // BankPanel실행시 Home화면으로
    void OnEnable()
    {
        OpenHome();
    }

    // 계좌 개설 확인
    public void CheckAccountAndNext()
    {
        if (FinanceManager.Instance.accounts.Count == 0)
        {
            Debug.Log("개설된 계좌가 없습니다.");
            return;
        }

        transactionUI.SetSelectedAccount(accountDropdown.value);
        ClearAllInputs(transactionPage1);
        DisableAll();
        transactionPage1.SetActive(true);
    }

    // 비밀번호 확인
    public void CheckPasswordAndNext()
    {
        if (!transactionUI.CheckPassword())
        {
            Debug.Log("비밀번호가 틀렸습니다.");
            return;
        }

        ClearAllInputs(transactionPage2);
        DisableAll();
        transactionPage2.SetActive(true);
    }

    //InputField 초기화
    public void ClearAllInputs(GameObject page)
    {
        TMP_InputField[] inputs = page.GetComponentsInChildren<TMP_InputField>();

        foreach (TMP_InputField input in inputs)
        {
            input.text = "";
        }
    }

    public void OpenHome()
    {
        DisableAll();
        homePage.SetActive(true);
    }

    public void OpenMakeAccount()
    {
        DisableAll();
        makeAccountPage.SetActive(true);

        ClearAllInputs(makeAccountPage1);
    }

    public void OpenMakeAccountStep2()
    {
        DisableAll();
        makeAccountPage1.SetActive(true);
        
    }

    public void OpenTransaction()
    {
        DisableAll();
        transactionPage.SetActive(true);

        transactionUI.RefreshAccountList();
    }
    public void OpenTransactionSetp2()
    {
        DisableAll();
        transactionPage1.SetActive(true);
    }

    public void OpenTransactionStep3()
    {
        DisableAll();
        transactionPage2.SetActive(true);
    }

    public void OpenLoan()
    {
        DisableAll();
        loanPage.SetActive(true);
    }

    void DisableAll()
    {
        homePage.SetActive(false);
        makeAccountPage.SetActive(false);
        makeAccountPage1.SetActive(false);
        transactionPage.SetActive(false);
        transactionPage1.SetActive(false);
        transactionPage2.SetActive(false);
        loanPage.SetActive(false);
    }
}
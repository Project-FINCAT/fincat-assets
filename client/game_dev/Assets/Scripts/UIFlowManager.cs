using UnityEngine;

public class UIFlowManager : MonoBehaviour
{
    public GameObject q1_1_Panel;
    public GameObject q1_2_Panel;
    public GameObject q1_3_Panel;

    public GameObject q2_1_Panel; 
    public GameObject q2_2_Panel;
    public GameObject q2_3_Panel;

    public GameObject q3_1_Panel; 
    public GameObject q3_2_Panel;
    public GameObject q3_3_Panel;
    
    public GameObject q4_Panel;
    public GameObject q4_1_Panel; 
    public GameObject q4_2_Panel;
    public GameObject q4_3_Panel;

    public void GoToQ1_2()
    {
        q1_1_Panel.SetActive(false);
        q1_2_Panel.SetActive(true);
    }

    public void GoToQ1_3()
    {
        q1_2_Panel.SetActive(false);
        q1_3_Panel.SetActive(true);
    }

        public void GoToQ2_2()
    {
        q2_1_Panel.SetActive(false);
        q2_2_Panel.SetActive(true);
    }

    public void GoToQ2_3()
    {
        q2_2_Panel.SetActive(false);
        q2_3_Panel.SetActive(true);
    }

        public void GoToQ3_2()
    {
        q3_1_Panel.SetActive(false);
        q3_2_Panel.SetActive(true);
    }

    public void GoToQ3_3()
    {
        q3_2_Panel.SetActive(false);
        q3_3_Panel.SetActive(true);
    }

    public void GoToQ4_1()
    {
        q4_Panel.SetActive(false);
        q4_1_Panel.SetActive(true);
    }
    public void GoToQ4_2()
    {
        q4_1_Panel.SetActive(false);
        q4_2_Panel.SetActive(true);
    }

    public void GoToQ4_3()
    {
        q4_2_Panel.SetActive(false);
        q4_3_Panel.SetActive(true);
    }

}

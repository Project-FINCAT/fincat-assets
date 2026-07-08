using System.IO;
using Unity.Cinemachine;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;

    [SerializeField] CinemachineConfiner2D confiner;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);

        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        inventoryController = FindAnyObjectByType<InventoryController>();

        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = confiner.BoundingShape2D.gameObject.name,

            inventorySaveData = inventoryController.GetInventoryItems(),

            // ===== Finance =====
            playerMoney = FinanceManager.Instance.playerMoney,

            // ===== Bank Accounts =====
            accounts = FinanceManager.Instance.accounts
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));

    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;

            confiner.BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();


            inventoryController.SetInventoryItems(saveData.inventorySaveData);

            // ===== Finance Load =====
            FinanceManager.Instance.playerMoney = saveData.playerMoney;

            if (saveData.accounts != null)
            {
                FinanceManager.Instance.accounts = saveData.accounts;
            }

        }
        else
        {
            SaveGame();
        }
    }
}


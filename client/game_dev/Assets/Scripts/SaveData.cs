using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public List<InventorySaveData> inventorySaveData;


    // Finance
    public int playerMoney;
    public int bankMoney;

    // Bank
    public List<BankAccountData> accounts;
}


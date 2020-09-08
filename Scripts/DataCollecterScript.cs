using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class SyncListdataTobeSaved : SyncList<dataTobeSaved> {}

public class DataCollecterScript : NetworkBehaviour
{
    public static DataCollecterScript Instance;

    
    public SyncListdataTobeSaved playerData = new SyncListdataTobeSaved();

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class V2_PlayerSpawnSystem : NetworkBehaviour
{
   [SerializeField] private GameObject playerPrefab = null;
    //Tehdään lista spawnipaikoista
   private static List<Transform> spawnPoints = new List<Transform>();
   private int nextIndex = 0;

    //Lisätään spawnpointlistaan
   public static void AddSpawnPoint(Transform transform){
       spawnPoints.Add(transform);
       spawnPoints = spawnPoints.OrderBy(x=> x.GetSiblingIndex()).ToList();
   }

   public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);
    //Kun joku on valmis me lisäämme hänet spawn playeriin ja viceversa    
   public override void OnStartServer() => CustomNetworkManager.OnServerReadied += SpawnPlayer;
   [ServerCallback]
   private void OnDestroy() => CustomNetworkManager.OnServerReadied -= SpawnPlayer;

   [Server]
   public void SpawnPlayer(NetworkConnection conn){
       Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

       if (spawnPoint == null){
           Debug.LogError($"Missing spawn point for player{nextIndex}");
           return;
       }
       //Seuraavasta valmiista pelaajasta luodaan gameobject spawnpointin trasformin mukaan
       GameObject playerInstance = Instantiate(playerPrefab,spawnPoints[nextIndex].position,spawnPoints[nextIndex].rotation);
       //Annetaan networkserville käsky spawnata pelaaja
       NetworkServer.Spawn(playerInstance,conn);
        //kasvatetaan indexiä;
       nextIndex++;
   }


}

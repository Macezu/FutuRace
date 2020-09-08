using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    //SpawniPisteen skripti jossa luodaan ja poistetaan spawni piste listalta trasformin perusteella.
    private void Awake() => V2_PlayerSpawnSystem.AddSpawnPoint(transform);
        
    
    private void OnDestroy() => V2_PlayerSpawnSystem.RemoveSpawnPoint(transform);

//Scenessä näkyvä pallo joka osoittaa spawnpoint pisteen
    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position,1f);
        //Pirretään spawn suuntaan osoittava vihreä nuoli joka ylettää 2 yksikköä eteenpäin
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,transform.position + transform.forward * 2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboAsItem : MonoBehaviour
{

    [SerializeField] private GameObject efekti;
    private void Start() {
        StartCoroutine("destroy");
    }

    private IEnumerator destroy(){
        Instantiate(efekti,transform.position,transform.rotation);
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }

}

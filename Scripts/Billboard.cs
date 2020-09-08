using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    MeshRenderer meshRenderer;
    Material m_material;
    public Material[] folder;
    // Start is called before the first frame update
    void Start()
    {   
        meshRenderer = GetComponent<MeshRenderer>();
        //Haetaan Materiaali GameObjectin renderiltä
        
    }

    // Update is called once per frame
    private void Update() {
        RandomImage();
    }
    void RandomImage(){
        int temp = Random.Range(0,folder.Length);
        m_material= folder[temp];
        
    }
}

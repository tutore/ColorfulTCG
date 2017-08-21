using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour {

    public GameObject UIparent;
    public GUIText statusText;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatus(GameObject target)
    {
        Instantiate(statusText, UIparent.transform.position + new Vector3(100, 100), Quaternion.Euler(0, 0, 0), UIparent.transform);
    }
}

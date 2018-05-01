using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {

        transform.Rotate(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
    }
	
	// Update is called once per frame
	void Update () {
        ///transform.Rotate(new Vector3(0, 2, 0));
    }
}

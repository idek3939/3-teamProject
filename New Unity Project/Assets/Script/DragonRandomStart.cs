using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonRandomStart : MonoBehaviour {

    public int start = 0;
    Vector3 pos;
    // Use this for initialization
    void Start () {
        start = UnityEngine.Random.Range(0, 5);
        switch (start) {
            case 1:
            case 3:
                pos = new Vector3(-108,10,-72);
                break;
            case 2:
            case 4:
                pos = new Vector3(108, 10, 72);
                break;
            default:
                pos = new Vector3(-15, 10, 110);
                break;

        }
        transform.position = pos;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

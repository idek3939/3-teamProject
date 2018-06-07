using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpen : MonoBehaviour {
    public Vector3 m_pos;
    public bool flg = false;
    public int timer = 0;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!flg && Input.GetKey("space")) {
            flg = true;
        }

        if(flg && (timer <= 300)) {
            m_pos = transform.localPosition;
            m_pos.y += 0.01f;
            transform.localPosition = m_pos;  // 移動を更新
            timer++;
        }
    }
}

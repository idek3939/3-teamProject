using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCPlayer : MonoBehaviour {

    private Rigidbody myRigidbody;

    private NetworkView netView = null;
    //移動速度
    public float fSpeed = 10f;

    // Use this for initialization
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        netView = GetComponent<NetworkView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!netView.isMine)
        {
            return;
        }

        Vector3 move = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));
        myRigidbody.velocity = fSpeed * move.normalized;
    }
}

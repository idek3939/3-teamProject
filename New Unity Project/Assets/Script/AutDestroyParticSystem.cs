using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutDestroyParticSystem : MonoBehaviour {

    private ParticleSystem ptclSys;

    void Awake()
    {
        ptclSys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ptclSys.IsAlive())
        {
            return;
        }
        Destroy(gameObject);
    }
}

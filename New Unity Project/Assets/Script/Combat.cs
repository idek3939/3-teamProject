using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//プレイヤーの体力などを管理する
//開始位置も

public class Combat : NetworkBehaviour
{
    [SyncVar] public int health = 100;

    public int maxHealth = 100;

    [SerializeField] private bool destroyOnDeath = false;

    private Vector3 spawnPoint;
    private Vector3 spawnForward;

    void Awake()
    {
        health = maxHealth;
    }

    public override void OnStartClient()
    {
        spawnPoint = transform.position;
        spawnForward = transform.forward;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour {

    public Transform[] points;
    private int destPoint;
    private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        GotoNextPoint();
	}
	
    void GotoNextPoint()
    {
        if (points.Length == 0) //ポイントの数が0だった場合動かない
            return;

        //次の巡回先をエージェントに指定する
       agent.SetDestination(points[destPoint].position);

        //次の巡回先を設定
        destPoint = (destPoint + Random.Range(1,(points.Length - 1))) % points.Length;
    }

	// Update is called once per frame
	void Update () {

        agent.SetDestination(points[destPoint].position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f) //目的地に近づいた場合次の巡回先に切り替え
            GotoNextPoint();
	}
    
}

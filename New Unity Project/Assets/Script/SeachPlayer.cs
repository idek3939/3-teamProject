using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SeachPlayer : MonoBehaviour {

    public Transform[] points;
    public int destPoint;
    private NavMeshAgent agent;
    public GameObject player;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        GotoNextPoint();

        player = GameObject.Find("Cubits_Warrior_Body_LOD1");
        
    }

    void GotoNextPoint()
    {
        if (points.Length == 0) //ポイントの数が0だった場合動かない
            return;

        //次の巡回先をエージェントに指定する
        agent.SetDestination(points[destPoint].position);

        //次の巡回先を設定
        destPoint = (destPoint + Random.Range(1, (points.Length - 1))) % points.Length;
    }

    void OnTriggerStay(Collider col){
        if(col.tag == "Player") {  //見つけたお！
            if (!Physics.Linecast(transform.position + Vector3.up, col.gameObject.transform.position + Vector3.up, LayerMask.GetMask("Field"))){
                agent.SetDestination(player.transform.position);
            }
        }
    }

    private void Update() {
        if (Input.GetKey("space")) {
            agent.SetDestination(player.transform.position);
            destPoint = (destPoint + Random.Range(1, (points.Length - 1))) % points.Length;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f) { //目的地に近づいた場合次の巡回先に切り替え
            GotoNextPoint();
        }
    }
}

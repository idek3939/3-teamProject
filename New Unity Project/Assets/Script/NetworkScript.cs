using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkScript : NetworkBehaviour
{
    public Canvas canvas;

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void OnHostButton()
    {
        canvas.gameObject.SetActive(false);
        //ホストとして起動
        NetworkManager.singleton.StartHost();
    }

    public void OnClientBotton()
    {
        canvas.gameObject.SetActive(false);
        //クライアントとして起動
        NetworkManager.singleton.StartClient();
    }

    public void OnServerBotton()
    {
        canvas.gameObject.SetActive(false);
        //サーバーとして起動
        NetworkManager.singleton.StartServer();
    }

}

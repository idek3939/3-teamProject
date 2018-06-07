using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SphereScript : NetworkBehaviour
{

    //ステータス表示用text
    Text text;

    //startTimeをもとに数秒経過するとどのクライアントでも同じ値が表示される
   // private static System.DateTime startTime = System.DateTime.Now;

    private static System.DateTime startTime;

    //[SyncVar]とはサーバー上の値などをクライアント側に同期させるためのもの
    //使える数は32まででintやfloatなど基本型のみ
    //新たにhookを追加。hookとは値が同期された際に実行される処理を設定することを意味する

    //[SyncVar(hook = "OnCountChange")]
    //int count = 0;

    void Start()
    {
        if (isLocalPlayer)
        {
            //textの値を設定
            text = GameObject.Find("MsgText").GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //サーバーなら実行
        if (isServer) Count();

        //ローカルなら実行
        if (isLocalPlayer)
        {
            //※プレイヤーの移動処理などを書きます


            //UpdateCount();
            //Move();
        }
    }
    //[ClientCallback]
    //はクライアント側で実行する

    //[ClientCallback]
    //void OnCountChange(int newval)
    //{
    //    Debug.Log(this.netId + "Value:" + newval);
    //    //値を同期させる
    //    text.text = "Client: " + count;
    //}

    //OnStartServer()はサーバー起動時に実行されるメソッド
    public override void OnStartServer()
    {
        //初期化される
        startTime = System.DateTime.Now;
    }

    //[ServerCallback] はサーバー側で実行する
    [ServerCallback]
    void Count()
    {
        if (!isServer) return;
        int count = (int)((System.DateTime.Now - startTime).TotalSeconds);
        RpcSetCount(count);
    }

    //サーバーからクライアントへコマンドを送るためのもの
    //[Command]の逆です。
    [ClientRpc]
    void RpcSetCount(int n)
    {
        if (text != null)
        {
            text.text = "Client: " + n;
        }
    }

    //[ClientCallback] はクライアント側で実行する
    //[ClientCallback]
    //void UpdateCount()
    //{
    //    text.text = "Client: " + count;
    //}


    //[ClientCallback]
    //void Move()
    //{
    //    //Mainカメラの位置調節
    //    //vを初期位置に
    //    Vector3 v = transform.position;
    //    v.z = -90;
    //    v.y = 15;
    //    Camera.main.transform.position = v;
    //}

    //[ClientCallback]
    //void FixedUpdate()
    //{
    //    if (!isLocalPlayer) return;
    //    float x = Input.GetAxis("Horizontal");
    //    float z = Input.GetAxis("Vertical");

    //    CmdMoveSphere(x, z);
    //}

    // Sphereの移動
    //[Command]はローカルプレイヤーからサーバーのプレイヤーオブジェクトへ送信される命令
    //クライアントから呼び出されてサーバー側で実行されます
    //Command宣言したら必ずメソッド名にCmdをつけないと実行されません(注意)
    //[Command]
    //public void CmdMoveSphere(float x, float z)
    //{
    //    //数値は適当に調整
    //    Vector3 v = new Vector3(x, 0, z) * 10f;

    //    GetComponent<Rigidbody>().AddForce(v);
    //}
}

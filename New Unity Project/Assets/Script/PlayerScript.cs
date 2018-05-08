using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour
{

    //ステータス表示用text
    Text text;

    //startTimeをもとに数秒経過するとどのクライアントでも同じ値が表示される
    // private static System.DateTime startTime = System.DateTime.Now;
    private static System.DateTime startTime;

    //[SyncVar]とはサーバー上の値などをクライアント側に同期させるためのもの
    //使える数は32まででintやfloatなど基本型のみ
    //そして新たにhookを追加。hookとは値が同期された際に実行される処理を設定することを意味する

    //[SyncVar(hook = "OnCountChange")]
    //int count = 0;

    //移動方向
    [SerializeField] private Vector3 velocity;
    //playerの移動(通常)
    [SerializeField] private float playerSpeed = 5.0f;
    //playerの移動(走り)
    [SerializeField] private float playerSpeedDash = 7.0f;
    //拡大率
    [SerializeField] private float playerScale = 1f;
    //振り向き適応速度
    [SerializeField] private float applySpeed = 0.2f;
    //カメラの水平回転を参照する用
    [SerializeField] private Test_camera refCamera;

    //コマンド周り
    [SerializeField] private KeyCode keyMoveUp = KeyCode.W;//前
    [SerializeField] private KeyCode keyMoveLeft = KeyCode.A;//左
    [SerializeField] private KeyCode keyMoveDown = KeyCode.S;//後ろ
    [SerializeField] private KeyCode keyMoveRight = KeyCode.D;//右

    [SyncVar] public string playerName = "player";

    void Start()
    {
        if (isLocalPlayer)
        {
            //textの値を設定
            text = GameObject.Find("MsgText").GetComponent<Text>();
        }
    }

    void Update()
    {
        //サーバーなら実行
        if (isServer) Count();

        //ローカルなら実行
        if (isLocalPlayer)
        {
            //※プレイヤーの移動処理などを書きます
            Move();
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
    void Move()
    {
        if (!isLocalPlayer) return;

        // WASD入力から、XZ平面(水平な地面)を移動する方向(velocity)を得ます
        velocity = Vector3.zero;

        if (Input.GetKeyDown(keyMoveUp)) velocity.z += 10;
        if (Input.GetKeyDown(keyMoveLeft)) velocity.x -= 10;
        if (Input.GetKeyDown(keyMoveDown)) velocity.z -= 10;
        if (Input.GetKeyDown(keyMoveRight)) velocity.x += 10;


        // 速度ベクトルの長さを1秒でplayerSpeedだけ進むように調整
        velocity = velocity.normalized * playerSpeed * Time.deltaTime;

        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0)
        {
            // プレイヤーの回転(transform.rotation)の更新
            // 無回転状態のプレイヤーのZ+方向(後頭部)を、
            // カメラの水平回転(refCamera.hRotation)で回した移動の反対方向(-velocity)に回す回転に段々近づけます
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(refCamera.hRotation * -velocity),
                                                  applySpeed);

            // プレイヤーの位置(transform.position)の更新
            // カメラの水平回転(refCamera.hRotation)で回した移動方向(velocity)を足し込みます

            transform.position += refCamera.hRotation * velocity;
        }
    }

    private void Log(string message, bool noFrameCount = false)
    {
        var frameCount = Time.frameCount.ToString() + " ";
        if (noFrameCount)
        {
            frameCount = "";
        }
        Debug.Log(frameCount + message
                    + " isServer:" + isServer + " isClient:" + isClient + " isLocalPlayer:" + isLocalPlayer + " hasAuthority:" + hasAuthority
                    + " netid:" + netId.Value
                    + " playerControllerId:" + playerControllerId
                    + " instanceId=" + gameObject.GetInstanceID(),
                    gameObject);
    }

    //[ClientCallback]
    //void FixedUpdate()
    //{
    //    if (!isLocalPlayer) return;
    //    //カメラの方向から、X-Z平面の単位ベクトルを取得
    //    Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

    //    //方向キーの入力値とカメラの向きから、移動方向を決定
    //    Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;

    //    //移動方向にスピードを掛ける
    //    rb.velocity = moveForward * playerSpeed + new Vector3(0, rb.velocity.y, 0);

    //    //キャラクターの向きを進行方向に
    //    if (moveForward != Vector3.zero)
    //    {
    //        transform.rotation = Quaternion.LookRotation(moveForward);
    //    }
    //    //CmdMovePlayer(v);
    //}

    //playerの移動
    //[Command]はローカルプレイヤーからサーバーのプレイヤーオブジェクトへ送信される命令
    //クライアントから呼び出されてサーバー側で実行されます
    //Command宣言したら必ずメソッド名にCmdをつけないと実行されません(注意)

    //[Command]
    //public void CmdMovePlayer()
    //{
    //    // いずれかの方向に移動している場合
    //    if (velocity.magnitude > 0)
    //    {
    //        // プレイヤーの回転(transform.rotation)の更新
    //        // 無回転状態のプレイヤーのZ+方向(後頭部)を、
    //        // カメラの水平回転(refCamera.hRotation)で回した移動の反対方向(-velocity)に回す回転に段々近づけます
    //        transform.rotation = Quaternion.Slerp(transform.rotation,
    //                                              Quaternion.LookRotation(refCamera.hRotation * -velocity),
    //                                              applySpeed);

    //        // プレイヤーの位置(transform.position)の更新
    //        // カメラの水平回転(refCamera.hRotation)で回した移動方向(velocity)を足し込みます
    //        transform.position += refCamera.hRotation * velocity;
    //    }

    //}
}

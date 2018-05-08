using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Linq;
using UnityEngine.SceneManagement;

//ロビー処理

public class PlayingLobbyManager : NetworkLobbyManager
{
    //プレイヤーのロビー座標
    [SerializeField] private Transform[] lobbyPlayerPositions;
    //プレイヤープレファブ
    [SerializeField] private Transform[] gamePlayerPrefabs;
    //デバッグログ
    [SerializeField] private bool debugLog = false;

    //自動実装 ホストの状態
    public MatchInfo roomHostInfo { get; private set; }
    //自動実装 クライアントの状態
    public MatchInfo joinedInfo { get; private set; }
    //部屋名前取得用
    public string createdRoomName { get; private set; }
    //マッチング情報取得用
    public MatchInfoSnapshot selectMatchInfo { get; private set; }

    //localDiscoveryコンポーネントのgetter(UnityEvent)
    private LocalDiscovery localDiscovery
    {
        get { return gameObject.GetComponent<LocalDiscovery>(); }
    }

    // Use this for initialization
    void Start (){
		
	}

    //オブジェクトが有効になると呼ばれる
    void OnEnable()
    {
        //コールバック関数をリスナーとして追加
        localDiscovery.onReceieveBroadcast.AddListener(OnReceiveDiscoveryBroadcast);
    }

    //オブジェクトが無効の時に呼ばれる
    void OnDisable()
    {
        //コールバック関数のリスナーを削除
        localDiscovery.onReceieveBroadcast.RemoveListener(OnReceiveDiscoveryBroadcast);
    }

    //イベントコールバック関数(LocalDiscoveryで実行)
    private void OnReceiveDiscoveryBroadcast(string address, string data)
    {
        //アドレスを設定
        networkAddress = address;
    }
    
    //各プレイヤーのPotision取得
    internal Transform GetPlayerPosition(int slot)
    {
        return lobbyPlayerPositions[slot];
    }

    //各プレイヤーのscene nameがmainじゃなければアクティブに設定
    private void EnablePlayerPositions(bool b)
    {
        lobbyPlayerPositions.ToList().ForEach(t => t.gameObject.SetActive(b));
    }

    //エラーテキスト表示用関数
    protected virtual void OnError(string errorMessage)
    {
        Debug.LogError("OnError: " + errorMessage);
    }

    // Update is called once per frame
    void Update () {
		
	}

    //overrideを用いた警告表示処理
    //--------------------------------------ロビーserver---------------------------------------------------------
    #region server

    //新しいクライアントがサーバーに接続するときサーバー上で呼び出される
    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        base.OnLobbyServerConnect(conn);
        Info("OnLobbyServerConnect: conn=" + conn.hostId + "/" + conn.connectionId);
    }

    //サーバー上のゲーム中のプレイヤーオブジェクトの作成をカスタマイズ
    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        Info("OnLobbyServerCreateGamePlayer");
        var lp = conn.playerControllers.FirstOrDefault(p => p.playerControllerId == playerControllerId).unetView.GetComponent<LobbyPlayerScript>();
        var prefab = gamePlayerPrefabs[lp.characterType];
        var spawnPos = GetStartPosition();
        var obj = Instantiate(prefab, spawnPos.position, spawnPos.rotation);
        return obj.gameObject;
    }

    //サーバー上のロビープレイヤーオブジェクトの作成をカスタマイズ
    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        Info("OnLobbyServerCreateLobbyPlayer conn=" + conn + " playercontrollerId=" + playerControllerId);
        return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
    }

    //プレイヤーが削除されたときサーバー上で呼び出される
    public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
    {
        base.OnLobbyServerPlayerRemoved(conn, playerControllerId);
        Info("OnLobbyServerPlayerRemoved! conn=" + conn + " playerControllerId=" + playerControllerId);
    }

    //クライアントを切断する時にサーバー上で呼び出される
    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        base.OnLobbyServerDisconnect(conn);
        Info("OnLobbyServerDisconnect " + conn.hostId + "/" + conn.connectionId);
    }

    //すべてのプレイヤーがロビー内で準備完了となったときにサーバー上で呼び出される
    public override void OnLobbyServerPlayersReady()
    {
        base.OnLobbyServerPlayersReady();
        Info("OnLobbyServerPlayersReady");
    }

    //ネットワークシーンの読み込みが終了したときサーバー上で呼び出される
    public override void OnLobbyServerSceneChanged(string sceneName)
    {
        base.OnLobbyServerSceneChanged(sceneName);
        Info("OnLobbyServerSceneChanged " + sceneName);
    }

    //クライアントがロビーのシーンからゲームシーンに切り替えが終了したことを伝えられたときサーバー上で呼び出される
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayerObj, GameObject gamePlayerObj)
    {
        Info("OnLobbyServerSceneLoadedForPlayer lobbyPlayer=" + lobbyPlayerObj + " / gamePlayer=" + gamePlayerObj);
        var lobbyPlayer = lobbyPlayerObj.GetComponent<LobbyPlayerScript>();
        var playerChar = gamePlayerObj.GetComponent<PlayerScript>();
        //プレイヤーの名前をゲーム中に設定
        playerChar.playerName = lobbyPlayer.playerName;
        return true;
    }

    //ホストとしてゲームを開始したときを含めサーバーを起動したときに呼び出される
    public override void OnLobbyStartServer()
    {
        base.OnLobbyStartServer();
        Info("OnLobbyStartServer");
    }

    //クライアントで新しいプレイヤーが追加されたときにサーバー上で呼び出される
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        Info("OnServerAddPlayer " + conn.hostId + "/" + conn.connectionId + " playerControllerId=" + playerControllerId);
    }

    #endregion

    //---------------------------------------ロビーclient--------------------------------------------------

    #region client

    //満室などでプレイヤーをロビーに追加することが失敗するとクライアント上で呼び出される
    public override void OnLobbyClientAddPlayerFailed()
    {
        base.OnLobbyClientAddPlayerFailed();
    }

    //クライアントがサーバーに接続するときにクライアント上で呼び出される
    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        base.OnLobbyClientConnect(conn);
        Info("OnLobbyClientConnect! connection id =" + conn.connectionId);
    }

    //サーバーから切断されたときにクライアント上で呼び出されます。
    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        base.OnLobbyClientDisconnect(conn);
        Info("OnLobbyClientDisconnect! connection id =" + conn.connectionId);
    }

    //ロビーに入室時に独自の動作を組み込むためのフック
    public override void OnLobbyClientEnter()
    {
        base.OnLobbyClientEnter();
        Info("OnLobbyClientEnter! ");
    }

    //ロビーを退室時に独自の動作を組み込むためのフック
    public override void OnLobbyClientExit()
    {
        base.OnLobbyClientExit();
        Info("OnLobbyClientExit! ");
    }

    //新しいネットワークの読み込みを終了したときにクライアント上で呼び出される
    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);
        Info("OnLobbyClientSceneChanged: conn=" + conn.hostId + "/" + conn.connectionId + " scene=" + SceneManager.GetSceneAt(0).name);
        var sceneName = SceneManager.GetSceneAt(0).name;
        EnablePlayerPositions(sceneName != "main");
    }

    //クライアントが入場したときにクライアント上で呼び出される
    public override void OnLobbyStartClient(NetworkClient lobbyClient)
    {
        Info("OnLobbyStartClient client conn=" + lobbyClient);
    }

    //クライアントが停止したときにクライアント上で呼び出される
    public override void OnLobbyStopClient()
    {
        base.OnLobbyStopClient();
        Info("OnLobbyStopClient");
    }

    #endregion

    //--------------------------------------------ロビーhost---------------------------------------------------

    #region host

    //ホストとしてゲームを開始したときホスト上で呼び出される
    public override void OnLobbyStartHost()
    {
        Info("OnLobbyStartHost");
    }

    //ホストとしてゲームをストップしたときホスト上で呼び出される
    public override void OnLobbyStopHost()
    {
        base.OnLobbyStopHost();
        Info("OnLobbyStopHost");
    }

    #endregion

    //------------------------------------------マッチメイク--------------------------------------------------

    #region match make

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Info("OnMatchCreate " + success + " /" + extendedInfo + "/" + matchInfo);
        if (success == false)
        {
            OnError("ルーム作成に失敗しました。ネットワーク状況を確認してください。" + extendedInfo);
        }
        else
        {
            roomHostInfo = matchInfo;
        }
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Info("OnMatchJoined " + success + " /" + extendedInfo + "/" + matchInfo);
        if (success == false)
        {
            OnError("ルームjoinに失敗しました");
        }
        else
        {
            joinedInfo = matchInfo;
        }
    }
    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        Info("OnMatchList " + success + " /" + extendedInfo + "\nmatchlist:\n" + string.Join("\n", matchList.Select(m => m.ToString()).ToArray()));
    }
    public override void OnSetMatchAttributes(bool success, string extendedInfo)
    {
        Info("OnMatchCreate " + success + " /" + extendedInfo);
    }
    public override void OnDestroyMatch(bool success, string extendedInfo)
    {
        Info("OnDestroyMatch " + success + "/" + extendedInfo);
    }
    protected void CreateRoom(string roomName)
    {
        roomHostInfo = null;
        matchMaker.CreateMatch(roomName, matchSize, true, "", "", "", 0, requestDomain, OnMatchCreate);
    }

    protected void ListMatches()
    {
        matchMaker.ListMatches(0, 100, "", false, 0, requestDomain, OnMatchList);
    }

    protected void JoinRoom(MatchInfoSnapshot m)
    {
        joinedInfo = null;
        matchMaker.JoinMatch(m.networkId, "", "", "", 0, requestDomain, OnMatchJoined);
    }

    private const int requestDomain = 1;

    #endregion

    //----------------------------------------NetworkManager------------------------------------------------

    #region NetworkManager

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Info("OnServerConnect");
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Info("OnServerDisconnect");
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        Info("OnServerSceneChanged " + sceneName);
    }
    public override void OnStartClient(NetworkClient lobbyClient)
    {
        base.OnStartClient(lobbyClient);
        Info("OnStartClient");
    }
    public override void OnStartHost()
    {
        base.OnStartHost();
        Info("OnStartHost");
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Info("OnStartServer");
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        Info("OnStopClient");
    }
    public override void OnStopHost()
    {
        base.OnStopHost();
        Info("OnStopHost");
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        Info("OnClientConnect");
        base.OnClientConnect(conn);
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Info("OnClientDisconnect");
        base.OnClientDisconnect(conn);
    }
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Info("OnClientSceneChanged");
        base.OnClientSceneChanged(conn);
    }

    #endregion

    //-------------------------------------------ext----------------------------------------------

    public void InitializeLocalDiscovery()
    {
        localDiscovery.Initialize();
    }

    public IEnumerator CreateRoomAndStartHost(string roomName)
    {
        createdRoomName = roomName;
        StartMatchMaker();
        CreateRoom(roomName);
        yield return new WaitUntil(() => roomHostInfo != null);
        NetworkServer.Listen(roomHostInfo, networkPort);
        StartHost(roomHostInfo);
    }

    public void StartMatchMakerAndRoomSearch()
    {
        StartMatchMaker();
        selectMatchInfo = null;
        ListMatches();
    }

    public void SetJoinRoom(MatchInfoSnapshot selectInfo)
    {
        selectMatchInfo = selectInfo;
    }

    public IEnumerator JoinRoomAndStartClient()
    {
        JoinRoom(selectMatchInfo);
        yield return new WaitUntil(() => joinedInfo != null);
        StartClient(joinedInfo);
    }

    //予期せぬ事態が発生したときに呼び出す
    public void StopAll()
    {
        StopLan();
        StopMatchMaker();
    }

    public void StopLan()
    {
        //ホスト停止
        StopHost();
        //クライアント停止
        StopClient();

        if (localDiscovery.isClient || localDiscovery.isServer)
        {
            localDiscovery.StopBroadcast();
        }
    }

    public bool StartHostDiscovery()
    {
        localDiscovery.Initialize();
        if (localDiscovery.StartAsClient() == false)
        {
            Debug.LogError("cannot start localDiscovery as client");
            OnError("LAN discovery ソケットを開けません。");
            return false;
        }
        return true;
    }

    //警告表示用関数
    protected void Info(string msg, UnityEngine.Object origin = null)
    {
        if (debugLog)
        {
            //どのタイミングで発生したかのLog
            Debug.LogWarning("[INFO]:" + Time.frameCount + "/" + Time.realtimeSinceStartup + " " + msg, origin);
        }
    }
}

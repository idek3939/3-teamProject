using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

//ロビーのUI処理

public class PlayingLobbyUi : PlayingLobbyManager
{
    public static PlayingLobbyUi instance { get; private set; }

    //メニューアイコン
    public GameObject menuTop;
    public GameObject menuCharSelect;
    public GameObject menuSelectRoom;
    public GameObject menuInputRoomName;
    public GameObject menuRoomNameLabelRoot;

    private GameObject[] allMenues
    {
        get
        {
            return new GameObject[] 
            {
                menuTop,
                menuCharSelect,
                menuSelectRoom,
                menuInputRoomName,
                menuRoomNameLabelRoot
            };
        }
    }

    public RoomList roomList;
    public InputField menuRoomNameInputField;
    public Text menuRoomNameLabel;
    public Dialog dialog;

    //localDiscoveryコンポーネントのgetter(UnityEvent)
    private LocalDiscovery localDiscovery
    {
        get { return gameObject.GetComponent<LocalDiscovery>(); }
    }

    private const int buttonIdNone = 0;
    private const int buttonIdBack = -1;
    private int uiButtonId = buttonIdNone;
    private string lastModeBeforePlay;
    private string currMode;

    // Use this for initialization
    void Start () {
        instance = this;
        InitializeLocalDiscovery();
        SetModeTop();
    }
	
	// Update is called once per frame
	void Update () {
	}

    void OnEnable()
    {
        roomList.onSelect.AddListener(SetJoinRoom);
    }

    void OnDisable()
    {
        roomList.onSelect.RemoveListener(SetJoinRoom);
    }

    private void SetMode(string modeMethodName)
    {
        StopAllCoroutines();
        currMode = modeMethodName;
        if (!string.IsNullOrEmpty(modeMethodName))
        {
            StartCoroutine(modeMethodName);
        }
    }

    private void SetModeTop()
    {
        SetMode("ModeTop");
    }
    private void SetupMenu(params GameObject[] menuObject)
    {
        foreach (var obj in allMenues.Where(o => o != null))
        {
            obj.SetActive(menuObject != null && menuObject.Contains(obj));
        }
        uiButtonId = buttonIdNone;
    }

    protected override void OnError(string errorMessage)
    {
        Debug.LogError("OnError: " + errorMessage);
        SetupMenu();
        StopLan();
        dialog.Open("エラー", errorMessage, () => SetModeTop());
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        SetModeTop();
    }

    //ルーム初期画面
    private IEnumerator ModeTop()
    {
        SetupMenu(menuTop);
        yield return new WaitUntil(() => uiButtonId != buttonIdNone);
        switch (uiButtonId)
        {
            case 1:
                SetMode("ModeCreateRoom");
                break;
            case 2:
                SetMode("ModelSearchAndJoinRoom");
                break;
            case 3:
                SetMode("ModeHost");
                break;
            case 4:
                SetMode("ModeClient");
                break;
        }
    }

    //ルーム：作成（ホスト）
    private IEnumerator ModeCreateRoom()
    {
        SetupMenu(menuInputRoomName);
        yield return new WaitUntil(() => uiButtonId != buttonIdNone);
        if (uiButtonId == buttonIdBack)
        {
            SetModeTop();
        }
        else
        {
            yield return CreateRoomAndStartHost(menuRoomNameInputField.text);
            SetMode("ModeHostInRoom");
        }
    }

    private IEnumerator ModeHostInRoom()
    {
        SetupMenu(menuCharSelect, menuRoomNameLabelRoot);
        menuRoomNameLabel.text = "ルーム " + createdRoomName;
        yield return new WaitUntil(() => uiButtonId != buttonIdNone);
        switch (uiButtonId)
        {
            case 1:
                SetReadyToPlay();
                break;
            case buttonIdBack:
                StopAll();
                SetModeTop();
                break;
        }
    }

    //ルーム選択join画面
    private IEnumerator ModelSearchAndJoinRoom()
    {
        SetupMenu(menuSelectRoom);
        StartMatchMakerAndRoomSearch();
        yield return new WaitUntil(() => selectMatchInfo != null || uiButtonId != buttonIdNone);
        if (uiButtonId == buttonIdBack)
        {
            StopMatchMaker();
            SetModeTop();
            yield break;
        }
        yield return JoinRoomAndStartClient();
        SetMode("ModeClientInRoom");
    }

    private IEnumerator ModeClientInRoom()
    {
        SetupMenu(menuCharSelect, menuRoomNameLabelRoot);
        menuRoomNameLabel.text = "ルーム " + selectMatchInfo.name;
        yield return new WaitUntil(() => uiButtonId != buttonIdNone);
        switch (uiButtonId)
        {
            case 1:
                SetReadyToPlay();
                break;
            case buttonIdBack:
                StopAll();
                SetModeTop();
                break;
        }
    }

    //ルーム：LANホスト
   
    private IEnumerator ModeHost()
    {
        if (false == StartLanHost())
        {
            yield break;
        }
        SetMode("ModeHostMain");
    }

    private IEnumerator ModeHostMain()
    {
        SetupMenu(menuCharSelect);
        yield return new WaitUntil(() => uiButtonId != buttonIdNone);
        switch (uiButtonId)
        {
            case 1:
                SetReadyToPlay();
                break;
            case buttonIdBack:
                StopLan();
                SetModeTop();
                break;
        }
    }
    //ルーム：LANクライアント
    private IEnumerator ModeClient()
    {
        if (false == StartHostDiscovery())
        {
            SetModeTop();
            yield break;
        }
        SetMode("ModeclientMain");
    }
    private IEnumerator ModeclientMain()
    {
        SetupMenu();
        dialog.OpenWithWaitingIcon("通信中", "LANからホストを探しています", null, OnUiBackButton);
        networkAddress = null;
        yield return new WaitUntil(() => uiButtonId != buttonIdNone || !string.IsNullOrEmpty(networkAddress));
        if (uiButtonId != buttonIdNone)
        {
            StopLan();
            SetModeTop();
            yield break;
        }
        dialog.Close();

        StartClient();
        SetupMenu(menuCharSelect);
        yield return new WaitUntil(() => uiButtonId != buttonIdNone);
        switch (uiButtonId)
        {
            case 1:
                SetReadyToPlay();
                break;
            case buttonIdBack:
                StopLan();
                SetModeTop();
                break;
        }
    }

    public void OnUiButton(int buttonId)
    {
        uiButtonId = buttonId;
    }

    public void OnUiBackButton()
    {
        uiButtonId = buttonIdBack;
    }

    private bool StartLanHost()
    {
        if (localDiscovery.StartAsServer() == false)
        {
            Debug.LogError("cannot start localDiscovery as server");
        }
        var ntwk = StartHost();
        if (ntwk == null)
        {
            OnError("ネットワークをサーバーとして起動できませんでした。");
            return false;
        }
        else
        {
            return true;
        }
    }

    private void SetReadyToPlay()
    {
        lastModeBeforePlay = currMode;
        LobbyPlayerScript.localPlayer.SendReadyToBeginMessage();
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt(0).name;
        if (sceneName == lobbyScene)
        {
            SetMode(lastModeBeforePlay);
        }
        else if (sceneName == playScene)
        {
            SetupMenu();  // disable
        }
    }

    public void OnUiMmSelectRoomReload()
    {
        roomList.StartLoading();
        ListMatches();
    }

    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        base.OnMatchList(success, extendedInfo, matchList);
        if (success)
        {
            roomList.SetMatchList(matchList);
        }
        roomList.StopLoading();
    }
}

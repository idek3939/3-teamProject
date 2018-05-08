using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// ロビーで表示される3Dのプレーヤーオブジェクト設定。

public class LobbyPlayerScript : NetworkLobbyPlayer
{
    [SyncVar(hook = "OnNameChanged")]
    public string playerName;

    [SyncVar(hook = "OnCharacterTypeChanged")]
    public int characterType;

    [SerializeField]
    public LobbyUiScript[] playerNameInput;

    [SerializeField]
    private GameObject[] characterTypeModels;

    public static LobbyPlayerScript localPlayer { get; private set; }

    private static PlayingLobbyManager lobbyManager { get { return NetworkLobbyManager.singleton as PlayingLobbyManager; } }

    public override void OnStartAuthority()
    {
        //default name
        CmdNameChange("idek");
    }
    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
        playerNameInput[slot].SetAsLocalPlayer(true);
    }

    public override void OnStartClient()
    {
        var pos = lobbyManager.GetPlayerPosition((int)slot);
        transform.SetParent(pos);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        SetupUI();
    }

    private void SetupUI()
    {
        for (var i = 0; i < playerNameInput.Length; ++i)
        {
            var inp = playerNameInput[i];
            if (slot != i)
            {
                inp.Hide();
            }
            else
            {
                inp.SetAsLocalPlayer(false);
                inp.inputField.onValueChanged.AddListener(OnNameFieldChanged);
                inp.leftArrow.onClick.AddListener(OnClickLeftArrow);
                inp.rightArrow.onClick.AddListener(OnClickRightArrow);
                OnNameChanged(playerName);
            }
        }
        SetCharacterType(characterType);
    }

    private void OnClickLeftArrow()
    {
        CmdChangeCharacterType(-1);
    }

    private void OnClickRightArrow()
    {
        CmdChangeCharacterType(1);
    }

    public void OnNameFieldChanged(string str)
    {
        CmdNameChange(str);
    }

    public void OnNameChanged(string str)
    {
        playerNameInput[slot].SetNameText(str);
    }

    [Command]
    public void CmdNameChange(string str)
    {
        playerName = str;
    }

    public void OnCharacterTypeChanged(int charType)
    {
        SetCharacterType(charType);
    }

    private void SetCharacterType(int charType)
    {
        for (var i = 0; i < characterTypeModels.Length; ++i)
        {
            characterTypeModels[i].SetActive(i == charType);
        }
    }

    [Command]
    private void CmdChangeCharacterType(int step)
    {
        characterType += step;
        if (characterType < 0)
        {
            characterType = characterTypeModels.Length - 1;
        }
        if (characterType >= characterTypeModels.Length)
        {
            characterType = 0;
        }
        SetCharacterType(characterType);
    }
}

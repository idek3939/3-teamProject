using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.Events;

//ルーム選択で取得するルーム列データ

public class RoomList : MonoBehaviour
{

    public Transform menuRoomItemButton;

    public GameObject menuRoomWaitingIcon;

    public class SelectEvent : UnityEvent<MatchInfoSnapshot> { }

    public SelectEvent onSelect = new SelectEvent();

    void Start()
    {
        menuRoomItemButton.gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        menuRoomWaitingIcon.SetActive(true);
    }

    public void StopLoading()
    {
        menuRoomWaitingIcon.SetActive(false);
    }

    //マッチングするルーム列取得時の処理
    public void SetMatchList(List<MatchInfoSnapshot> matchList)
    {
        for (var i = 0; i < menuRoomItemButton.parent.childCount; ++i)
        {
            var c = menuRoomItemButton.parent.GetChild(i);
            if (c != menuRoomItemButton)
            {
                Destroy(c.gameObject);
            }
        }
        foreach (var m in matchList)
        {
            var label = string.Format("{0} / {1}", m.name, m.hostNodeId.ToString(), m.networkId);
            var bobj = Instantiate(menuRoomItemButton, menuRoomItemButton.parent) as Transform;
            bobj.GetComponentInChildren<Text>().text = label;
            var arg = m; // local 変数にしておかないと次のlambda funcでうまくclosure化されない
            bobj.GetComponent<Button>().onClick.AddListener(() => onSelect.Invoke(arg));
            bobj.gameObject.SetActive(true);
        }
        menuRoomWaitingIcon.SetActive(false);
    }
}

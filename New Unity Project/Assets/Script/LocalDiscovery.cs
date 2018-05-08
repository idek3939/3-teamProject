using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine;

//UnityEventを継承したクラスを取り扱うクラス

public class LocalDiscovery : NetworkDiscovery
{
    //UnityEventを継承したクラスを作成　string , string　を引数に
    public class ReceiveBroadcastEvent : UnityEvent<string, string> { }

    public ReceiveBroadcastEvent onReceieveBroadcast = new ReceiveBroadcastEvent();

    //イベントのコールバック実行用関数
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("from:" + fromAddress + "\ndata:" + data);
        //全てのコールバックを実行
        onReceieveBroadcast.Invoke(fromAddress, data);
    }
}

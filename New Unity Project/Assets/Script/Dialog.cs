using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

//ロビーのダイアログ
public class Dialog : MonoBehaviour {


    public Text[] titleText;
    public Text bodyText;
    public Button okButton;
    public Button cancelButton;

    public GameObject waitingIcon;

    private Action actionOk;
    private Action actionCancel;

    void OnEnable()
    {
        okButton.onClick.AddListener(OnClickOk);
        cancelButton.onClick.AddListener(OnClickCancel);
    }

    void OnDisable()
    {
        waitingIcon.gameObject.SetActive(false);
        okButton.onClick.RemoveListener(OnClickOk);
        cancelButton.onClick.RemoveListener(OnClickCancel);
    }

    private void OnClickOk()
    {
        Close();
        if (actionOk != null)
        {
            actionOk.Invoke();
        }
    }

    private void OnClickCancel()
    {
        Close();
        if (actionCancel != null)
        {
            actionCancel.Invoke();
        }
    }

    public void Open(string title, string message, Action ok, Action cancel = null)
    {
        gameObject.SetActive(true);
        titleText.ToList().ForEach(t => { t.text = title; });
        bodyText.text = message;
        actionOk = ok;
        actionCancel = cancel;
        okButton.gameObject.SetActive(ok != null);
        cancelButton.gameObject.SetActive(cancel != null);
    }

    public void OpenWithWaitingIcon(string title, string message, Action ok, Action cancel = null)
    {
        Open(title, message, ok, cancel);
        ShowWaitingIcon();
    }

    public void ShowWaitingIcon()
    {
        waitingIcon.gameObject.SetActive(true);
    }

    public void Close()
    {
        waitingIcon.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}

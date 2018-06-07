using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ロビーで表示されるプレーヤーのUI。名前入力部分

public class LobbyUiScript : MonoBehaviour
{
    public InputField inputField;
    public Image[] backgrounds;

    public Button leftArrow;
    public Button rightArrow;

    // Use this for initialization
    void Start(){

    }

    // Update is called once per frame
    void Update(){

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetAsLocalPlayer(bool b)
    {
        inputField.interactable = b;
        leftArrow.gameObject.SetActive(b);
        rightArrow.gameObject.SetActive(b);
        foreach (var img in backgrounds)
        {
            img.enabled = b;
        }
    }

    public void SetNameText(string str)
    {
        inputField.text = str;
    }
}

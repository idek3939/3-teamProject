using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //各scene遷移関数群

    //TitleScene遷移用
    public void SceneLoadTitle()
    {
        Application.LoadLevel("Title");
    }

    //MenuScene遷移用
    public void SceneLoadMenu()
    {
        Application.LoadLevel("Menu");
    }

    //OptionScene遷移用
    public void SceneLoadOption()
    {
        Application.LoadLevel("Option");
    }

    //SelectScene遷移用
    public void SceneLoadSelect()
    {
        Application.LoadLevel("Select");
    }
    
    //MatchingScene遷移用
    public void SceneLoadMatching()
    {
        Application.LoadLevel("Matching");
    }

    //MainScene遷移用
    public void SceneLoadMain()
    {
        Application.LoadLevel("Main");
    }

    //ResultScene遷移用
    public void SceneLoadResult()
    {
        Application.LoadLevel("Result");
    }

}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rogue;

public class ButtonManager : Singleton<ButtonManager>
{
    public void YesBtn()
    {
    	GameManager.Instance.ToNewLevel();
    }

    public void NoBtn()
    {
    	GameManager.Instance.DestroyBtn();
    }

    public void StartGameBtn()
    {
    	SceneManager.LoadScene ("Game");
    }

    public void ReturnMenuButton()
    {
    	SceneManager.LoadScene ("MainMenu");
    }
}
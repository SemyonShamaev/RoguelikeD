using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rogue;

public class ButtonManager : Singleton<ButtonManager>
{
	public AudioMixer masterMixer;

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
    	SceneManager.LoadScene("Game");
    }

    public void ReturnMenuButton()
    {
    	SceneManager.LoadScene("MainMenu");
    	AudioManager.Instance.RestoreMusic();
    }

    public void PauseButton()
    {
    	GameManager.Instance.PauseGame();
    }

    public void SettingsButton()
    {
    	GameManager.Instance.OpenSettings();
    }

    public void NextButton()
    {
    	GameManager.Instance.NextGame();
    }

    public void ExitButton()
    {
    	GameManager.Instance.ExitGame();
    }

    public void changeVolumeMusic(Slider slid)
    {
    	masterMixer.SetFloat("VolumeMusic",  Mathf.Lerp(-80, 0, slid.value));
    }

    public void changeVolumeEffects(Slider slid)
    {
    	masterMixer.SetFloat("VolumeEffects", Mathf.Lerp(-80, 0, slid.value));
    }
}

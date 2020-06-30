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

    public void StartGameBtn()
    {
    	SceneManager.LoadScene("Game");
    }

    public void ReturnMenuButton()
    {
        SaveLoadManager.Instance.SaveGame();
    	SceneManager.LoadScene("MainMenu");
    	AudioManager.Instance.RestoreMusic();
    }

    public void SettingsButton()
    {
    	GameManager.Instance.PausePanel.SetActive(false);
        GameManager.Instance.SettingsPanel.SetActive(true);
    }

    public void RestartBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitButton()
    {
    	Application.Quit();
    }

    public void InventoryButton()
    {
        if(!GameManager.Instance.onPause)
            GameManager.Instance.OpenInventory();
        else
        {
            GameManager.Instance.onPause = false;
            GameManager.Instance.InventoryPlayerPanel.SetActive(false);
        }
    }

    public void Next()
    {
        GameManager.Instance.onPause = false;
        Time.timeScale = 1;
    }

    public void changeVolumeMusic(Slider slid)
    {
    	masterMixer.SetFloat("VolumeMusic",  Mathf.Lerp(-80, 0, slid.value));
    }

    public void changeVolumeEffects(Slider slid)
    {
    	masterMixer.SetFloat("VolumeEffects", Mathf.Lerp(-80, 0, slid.value));
    }

    public void ExitEnemyInventoryButton()
    {
        for(int i = 0; i < Generator.Instance.enemies.Length; i++)
        {
            Generator.Instance.Invtr[i].SetActive(false);
            GameManager.Instance.onPause = false;
        }
    }

    public void PauseGame()
    {
        GameManager.Instance.onPause = true;
        Time.timeScale = 0;
    }
}

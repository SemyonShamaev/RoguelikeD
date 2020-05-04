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
    	GameManager.Instance.NextLevelPanel.SetActive(false);
        GameManager.Instance.onPause = false;
        Time.timeScale = 1;
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
    	GameManager.Instance.PausePanel.SetActive(false);
        GameManager.Instance.SettingsPanel.SetActive(true);
    }

    public void NextButton()
    {
    	GameManager.Instance.NextGame();
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
        GameManager.Instance.OpenInventory();
    }

    public void ExitInventoryButton()
    {
        GameManager.Instance.InventoryPlayerPanel.SetActive(false);
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
            if(Player.Instance.transform.position == Generator.Instance.enemies[i].transform.position)
            {
                Generator.Instance.Invtr[i].SetActive(false);
                GameManager.Instance.onPause = false;
            } 
        }
    }
}

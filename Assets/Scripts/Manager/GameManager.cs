using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rogue;

public class GameManager : Singleton<GameManager>
{
   	public int level = 1;

   	public GameObject Canvas;

   	public GameObject GameOverPanel;

   	public GameObject NextLevelPanel;
   	public GameObject PausePanel;
   	public GameObject SettingsPanel;
   	public GameObject InventoryPlayerPanel;

   	public Text levelCount;

	public AudioClip BackgroundMusic;
	public AudioClip DeathSound;

	public bool onPause = false;

	private Generator Generator;	

	Animation TransitionAnim;

   	void Awake()
   	{
   		Time.timeScale = 1;
   		
   		AudioManager.Instance.RestoreMusic();
   		AudioManager.Instance.PlayMusic(BackgroundMusic);

   		Generator = GetComponent<Generator>();
   		
   		InitGame();
   	}

   	void InitGame()
	{
		Generator.setupScene(level);
	}

	public void NewLevelMessage()
	{
		NextLevelPanel.SetActive(true);
		onPause = true;
		Time.timeScale = 0;
	}

	public void PauseGame()
	{
		if(SettingsPanel.activeSelf || PausePanel.activeSelf)
		{
			SettingsPanel.SetActive(false);
			PausePanel.SetActive(false);
			onPause = false;
			Time.timeScale = 1;
		}
		else
		{
			SettingsPanel.SetActive(true);
			PausePanel.SetActive(true);
			onPause = true;
			Time.timeScale = 0;
		}
	}

	public void NextGame()
	{
		PausePanel.SetActive(false);
		SettingsPanel.SetActive(false);
		onPause = false;
		Time.timeScale = 1;
	}

	public void OpenInventory()
	{
		InventoryPlayerPanel.SetActive(!InventoryPlayerPanel.activeSelf);
	}

	public void ToNewLevel()
	{
		onPause = false;
		Time.timeScale = 1;
		
		TransitionAnim = GameObject.Find("Panel").GetComponent<Animation>();
		TransitionAnim.Play("Transition");

		NextLevelPanel.SetActive(false);

		foreach (Transform child in transform) Destroy(child.gameObject);
		level++;
		levelCount.text = level.ToString();

		Generator.Instance.setupScene(level);
		
		Player.Instance.transform.position = new Vector3(100,100,0);
		Player.Instance.stepPoint = Player.Instance.transform.position;
		Camera.main.transform.position = new Vector3(100, 100, -5);	
	}

	public void GameOver()
	{
		AudioManager.Instance.PauseMusic();
		AudioManager.Instance.PlayEffects(DeathSound);

		GameObject GameOverPnl = Instantiate(GameOverPanel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    	GameOverPnl.transform.SetParent(Canvas.transform, false);
	}
}

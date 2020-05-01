using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rogue;

public class GameManager : Singleton<GameManager>
{
	public int PlayerLifes;
	public int PlayerFoodPoints;
   	public int level = 1;

   	public GameObject Canvas;

   	public GameObject GameOverPanel;

   	public GameObject NextLevelPanel;
   	public GameObject PausePanel;
   	public GameObject SettingsPanel;
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

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void NewLevelMessage()
	{
		NextLevelPanel.SetActive(true);
	}

	public void PauseGame()
	{
		if(PausePanel.activeInHierarchy == false)
		{
			Debug.Log("fdf");
			onPause = true;
			Time.timeScale = 0;

			PausePanel.SetActive(true);
    	}
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void NextGame()
	{
		PausePanel.SetActive(false);
		SettingsPanel.SetActive(false);
		onPause = false;
		Time.timeScale = 1;
	}

	public void DestroyBtn()
	{
		NextLevelPanel.SetActive(false);
	}

	public void OpenSettings()
	{
		PausePanel.SetActive(false);
		SettingsPanel.SetActive(true);
	}

	public void ToNewLevel()
	{
		TransitionAnim = GameObject.Find("Panel").GetComponent<Animation>();
		TransitionAnim.Play("Transition");

		DestroyBtn();
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

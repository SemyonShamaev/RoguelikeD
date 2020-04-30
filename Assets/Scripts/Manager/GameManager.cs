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

   	public GameObject PauseButton;

   	public GameObject PausePanel;
   	public GameObject GameOverPanel;
   	public GameObject NextLevelPanel;

	public AudioClip BackgroundMusic;
	public AudioClip DeathSound;

	public bool onPause = false;

	private Generator Generator;	

	private GameObject PausePnl;
	private GameObject NextLevelPnl;

	Animation TransitionAnim;

   	void Awake()
   	{
   		AudioManager.Instance.RestoreMusic();
   		AudioManager.Instance.PlayMusic(BackgroundMusic);

   		GameObject PauseBtn = Instantiate(PauseButton, new Vector3(850, 1600,0), Quaternion.identity) as GameObject;
    	PauseBtn.transform.SetParent(Canvas.transform, false);
    	PauseBtn.transform.SetAsLastSibling();
   		
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
		NextLevelPnl = Instantiate(NextLevelPanel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    	NextLevelPnl.transform.SetParent(Canvas.transform, false);
	}

	public void PauseGame()
	{
		if(PausePnl == null)
		{
			onPause = true;
			Time.timeScale = 0;

			PausePnl = Instantiate(PausePanel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    		PausePnl.transform.SetParent(Canvas.transform, false);
    	}
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void NextGame()
	{
		Destroy(PausePnl);
		onPause = false;
		Time.timeScale = 1;
	}

	public void DestroyBtn()
	{
		Destroy(NextLevelPnl);
	}

	public void ToNewLevel()
	{
		TransitionAnim = GameObject.Find("Panel").GetComponent<Animation>();
		TransitionAnim.Play("Transition");

		DestroyBtn();
		foreach (Transform child in transform) Destroy(child.gameObject);
		level++;
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

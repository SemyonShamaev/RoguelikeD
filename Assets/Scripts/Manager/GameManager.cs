using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rogue;

public class GameManager : Singleton<GameManager>
{
	public int PlayerLifes;
	public int PlayerFoodPoints;
   	public int level = 1;

   	private Generator Generator;	

   	public GameObject GameOverPicture;
   	public GameObject ReButton;
   	public GameObject ReturnMenuButton;
   	public GameObject YesButton;
   	public GameObject NoButton;
   	public GameObject Canvas;

   	private GameObject YesBtn;
   	private GameObject NoBtn;
   	Animation TransitionAnim;

   	void Start()
   	{		

   	}
   	void Awake()
   	{
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
		YesBtn = Instantiate(YesButton, new Vector3(-200,-700,0), Quaternion.identity) as GameObject;
    	YesBtn.transform.SetParent(Canvas.transform, false);
    	NoBtn = Instantiate(NoButton, new Vector3(200,-700,0), Quaternion.identity) as GameObject;
    	NoBtn.transform.SetParent(Canvas.transform, false);
	}
	public void DestroyBtn()
	{
		Destroy(YesBtn);
		Destroy(NoBtn);
	}

	public void ToNewLevel()
	{
		TransitionAnim = GameObject.Find("Panel").GetComponent<Animation>();
   		TransitionAnim["Transition"].layer = 123;
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
		GameObject GameOver = Instantiate(GameOverPicture);
		GameOver.transform.parent = Player.Instance.transform;
		GameOver.transform.position = Player.Instance.transform.position;

		GameObject ReBtn = Instantiate(ReButton, new Vector3(400,-700,0), Quaternion.identity) as GameObject;
    	ReBtn.transform.SetParent(Canvas.transform, false);
    	GameObject ReturnMenuBtn = Instantiate(ReturnMenuButton, new Vector3(-400,-700,0), Quaternion.identity) as GameObject;
    	ReturnMenuBtn.transform.SetParent(Canvas.transform, false);
	}
}

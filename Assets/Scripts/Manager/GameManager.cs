using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rogue;

public class GameManager : Singleton<GameManager>
{
   	public int level;
    public int playerLevel;
    public float gameSpeed;

   	public GameObject Canvas;
   	public GameObject GameOverPanel;
   	public GameObject NextLevelPanel;
   	public GameObject PausePanel;
   	public GameObject SettingsPanel;
   	public GameObject InventoryPlayerPanel;
    public GameObject UpLevelMessage;
    public GameObject HitParent, HitTextPrefab;
    public GameObject PlayerUpButton;
    public GameObject WinPanel;

    public Text levelCount;
   	public Text healthCount;
    public Text satietyCount;
    public Text playerLevelCount;
    public Text playerCharacteristic;
    public Text goldCount;

	public AudioClip BackgroundMusic;
	public AudioClip DeathSound;
    public AudioClip soundOfUpLevel;
    public AudioClip WinSound;

	public bool onPause = false;

	private Animation TransitionAnim;
    private Camera cam;

    private void Start()
	{
		Application.targetFrameRate = 60;
		AudioManager.Instance.PlayMusic(BackgroundMusic);

		healthCount.text = Player.Instance.currentLifes.ToString() + 
            "/" + Player.Instance.maxLifes.ToString();

        satietyCount.text = Player.Instance.currentSatiety.ToString() +
            "/" + Player.Instance.maxSatiety.ToString();

        playerLevelCount.text = playerLevel.ToString();
        goldCount.text = Inventory.Instance.items[0].count.ToString();
	}

   	private void Awake()
   	{
   		Time.timeScale = 1;
   		AudioManager.Instance.RestoreMusic();
        Generator.Instance.setupScene(level);
    }

    private void Update()
    {
        foreach (Transform child in HitParent.transform)
            child.gameObject.transform.position = Vector2.MoveTowards(child.gameObject.transform.position, new Vector3(child.gameObject.transform.position.x, child.gameObject.transform.position.y + 1000, 0), gameSpeed * Time.deltaTime / 2);
    }

	public void NewLevelMessage()
	{
		NextLevelPanel.SetActive(true);
		onPause = true;
		Time.timeScale = 0;
	}

    public void UpLevel()
    {
        UpLevelMessage.SetActive(true);
        AudioManager.Instance.PlayEffects(soundOfUpLevel);
        PlayerUpButton.SetActive(true);
    }

	public void PauseGame()
	{
		onPause = true;
		Time.timeScale = 0;
	}

	public void NextGame()
	{
		onPause = false;
		Time.timeScale = 1;
	}

	public void OpenInventory()
	{
         onPause = true;
         InventoryPlayerPanel.SetActive(!InventoryPlayerPanel.activeSelf);
	}

	public void ToNewLevel()
    { 
        onPause = false;
		Time.timeScale = 1;

		TransitionAnim = GameObject.Find("Panel").GetComponent<Animation>();
		TransitionAnim.Play("Transition");

        NextLevelPanel.SetActive(false);

		DestroyAllObjects();

		level++;
		levelCount.text = level.ToString();

		Generator.Instance.setupScene(level);
		
		Player.Instance.transform.position = new Vector3(100, 100, 0);
		Player.Instance.stepPoint = Vector3Int.FloorToInt(Player.Instance.transform.position);
		Camera.main.transform.position = new Vector3(100, 100, -5);	
	}

	public void DestroyAllObjects()
	{
		foreach (Transform child in transform) Destroy(child.gameObject);

        foreach (Transform child in HitParent.transform) Destroy(child.gameObject);
    }

	public void GameOver()
	{
		AudioManager.Instance.PauseMusic();
		AudioManager.Instance.PlayEffects(DeathSound);
		onPause = true;
		GameOverPanel.SetActive(true);
	}

    public void Win()
    {
        AudioManager.Instance.PauseMusic();
        AudioManager.Instance.PlayEffects(WinSound);
        onPause = true;
        WinPanel.SetActive(true);
    }

    public void spawnHitText(int x, int y, int damageCount)
    {
        GameObject HitText = Instantiate(HitTextPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
        HitText.transform.SetParent(HitParent.transform, false);
        if (damageCount > 0)
            HitText.GetComponent<Text>().text = damageCount.ToString();
        else if(damageCount < 0)
        {
            HitText.GetComponent<Text>().text = (-damageCount).ToString();
            HitText.GetComponent<Text>().color = Color.green;
        }
        else
            HitText.GetComponent<Text>().text = "ПРОМАХ";
    }

    public void updateCharacteristic()
    {
       playerCharacteristic.text = Player.Instance.skillPoints.ToString() + "\n" +
                                   Player.Instance.currentLifes.ToString() + "/" + Player.Instance.maxLifes.ToString() + "\n" +
                                   Player.Instance.attack.ToString() + "\n" +
                                   Player.Instance.defense.ToString() + "\n" +
                                   Player.Instance.agility.ToString() + "\n" +
                                   Player.Instance.stamina.ToString();
    }

	public void LoadData(Save.GameManagerSaveData save)
    {
        level = save.levelCount;
        playerLevel = save.playerLevel;
        levelCount.text = level.ToString();
        playerLevelCount.text = playerLevel.ToString();

        UpLevelMessage.SetActive(save.levelUp);
        PlayerUpButton.SetActive(save.levelUp);

        healthCount.text = Player.Instance.currentLifes.ToString() + "/" + Player.Instance.maxLifes.ToString();

        goldCount.text = Inventory.Instance.items[0].count.ToString();
    }
}
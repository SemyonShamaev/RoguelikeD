using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Rogue;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public Slider musicSlider;
    public Slider effectsSlider;

	private string filePath;
 	private Animation anim;

    private void Start()
    {
    	filePath = Application.persistentDataPath + "/save.gamesave";
    	DontDestroyOnLoad(this.gameObject);
    	anim = GetComponent<Animation>();
        musicSlider.GetComponent<Sliders>().setSettings();
        effectsSlider.GetComponent<Sliders>().setSettings();
    }

    public void SaveGame()
    {
    	BinaryFormatter bf = new BinaryFormatter();
    	FileStream fs = new FileStream(filePath, FileMode.Create);

   		Save save = new Save();

		save.SaveMap();
   		save.SaveEnemies();
   		save.SavePlayerInventory();
   		save.SaveEnemyInventory();
   		save.SavePlayerInventory();
   		save.SaveContainersInventory();
   		save.SaveContainers();
        save.SaveOtherObjects();
        save.SaveGameManager();
   		save.SavePlayer();

   		bf.Serialize(fs, save);
   		fs.Close();
    }

    public void LoadGame()
    {	
    	if(!File.Exists(filePath))
    		return;

    	BinaryFormatter bf = new BinaryFormatter();
    	FileStream fs = new FileStream(filePath, FileMode.Open);

    	Save save = (Save)bf.Deserialize(fs);
    	fs.Close();

    	Generator.Instance.LoadData(save.mapData);

    	int i = 0;
    	foreach(var enemy in save.enemiesData)
    	{
    		Generator.Instance.enemies[i].GetComponent<Enemy>().LoadData(enemy);
    		i++;
    	}

    	int n = 0;
    	for(int j = 0; j < Generator.Instance.Invtr.Length; j++)
    	{
    		for(int k = 0; k < 9; k++)
    		{
    			Generator.Instance.Invtr[j].GetComponentInChildren<InventoryEnemy>().LoadData(save.enemyInventoryData[n], k);
    			n++;
    		}
    	}

    	int m = 0;
    	for(int b = 0; b < Generator.Instance.InvtrContainers.Length; b++)
    	{
    		for(int p = 0; p < 9; p++)
    		{
    			Generator.Instance.InvtrContainers[b].GetComponentInChildren<InventoryEnemy>().LoadDataContainers(save.containersInventoryData[m], p);
    			m++;
    		}
    	}

    	int x = 0;
    	foreach(var cont in save.containersData)
    	{
    		Generator.Instance.containers[x].transform.position = new Vector3(cont.position.x, cont.position.y, cont.position.z);
    		x++;
    	}

        int f = 0;
        foreach (var obj in save.otherObjectsData)
        {
            Generator.Instance.otherObjects[f].transform.position = new Vector3(obj.position.x, obj.position.y, obj.position.z);
            f++;
        }

        Inventory.Instance.GetComponent<Inventory>().LoadData(save.inventoryData);
    	Player.Instance.GetComponent<Player>().LoadData(save.playerData);
    	GameManager.Instance.LoadData(save.gameManagerData);
    }

    public void Load()
	{
		anim.Play("Load");
	}

	private void OnApplicationQuit()
    {
    	if(SceneManager.GetActiveScene().name == "Game")
     		SaveGame();   
    }
}

[System.Serializable]
public class Save
{
	public PlayerSaveData playerData = new PlayerSaveData();
    public GameManagerSaveData gameManagerData = new GameManagerSaveData();
	public List<MapSaveData> mapData = new List<MapSaveData>();
	public List<EnemySaveData> enemiesData = new List<EnemySaveData>();
	public List<ContainersSaveData> containersData = new List<ContainersSaveData>();
	public List<PlayerInventorySaveData> inventoryData = new List<PlayerInventorySaveData>();
	public List<EnemyInventorySaveData> enemyInventoryData = new List<EnemyInventorySaveData>();
	public List<ContainersInventorySaveData> containersInventoryData = new List<ContainersInventorySaveData>();
    public List<OtherObjectsSaveData> otherObjectsData = new List<OtherObjectsSaveData>();

	[System.Serializable]
	public struct ContainersSaveData
	{
		public Vec3 position;

		public ContainersSaveData(Vec3 position)
		{
			this.position = position;
		}
	}
	public void SaveContainers()
	{
		foreach(var cont in Generator.Instance.containers)
		{
			Vec3 position = new Vec3(cont.transform.position.x, cont.transform.position.y, cont.transform.position.z);
			containersData.Add(new ContainersSaveData(position));
		}
	}

    [System.Serializable]
    public struct OtherObjectsSaveData
    {
        public Vec3 position;

        public OtherObjectsSaveData(Vec3 position)
        {
            this.position = position;
        }
    }
    public void SaveOtherObjects()
    {
        foreach (var obj in Generator.Instance.otherObjects)
        {
            Vec3 position = new Vec3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
            otherObjectsData.Add(new OtherObjectsSaveData(position));
        }
    }

    [System.Serializable]
	public struct PlayerSaveData
	{
		public Vec3 position, stepPoint;
		public Vec2 point;
		public int currentLifes;
        public int maxLifes;
        public int currentSatiety;
        public int maxSatiety;
        public int levelLimit;
        public int currentExp;
        public int skillPoints;
        public int attack;
        public int defense;
        public int agility;
        public int stamina;
        public int invulnerableCount;
        public int invisibleCount;
		public bool isMoving;
        public bool isInvulnerable;
        public bool isInvisible;

		public PlayerSaveData(Vec3 position, Vec3 stepPoint, Vec2 point, int currentLifes, int maxLifes, int currentSatiety, int maxSatiety,int levelLimit, int currentExp, int skillPoints, int attack, int defense, int agility, int stamina, bool isMoving, bool isInvulnerable, bool isInvisible, int invulnerableCount, int invisibleCount)
		{
			this.position = position;
			this.stepPoint = stepPoint;
			this.point = point;
			this.currentLifes = currentLifes;
            this.maxLifes = maxLifes;
            this.currentSatiety = currentSatiety;
            this.maxSatiety = maxSatiety;
            this.levelLimit = levelLimit;
            this.currentExp = currentExp;
            this.skillPoints = skillPoints;
            this.attack = attack;
            this.defense = defense;
            this.agility = agility;
            this.stamina = stamina;
			this.isMoving = isMoving;
            this.isInvulnerable = isInvulnerable;
            this.isInvisible = isInvisible;
            this.invulnerableCount = invulnerableCount;
            this.invisibleCount = invisibleCount;
		}
	}
	public void SavePlayer()
	{
		var player = Player.Instance.GetComponent<Player>();

		Vec3 position = new Vec3(Player.Instance.transform.position.x, Player.Instance.transform.position.y, Player.Instance.transform.position.z);
		Vec3 stepPoint = new Vec3(player.stepPoint.x, player.stepPoint.y, player.stepPoint.z);
		Vec2 point = new Vec2(player.point.x, player.point.y);
		int currentLifes = player.currentLifes;
        int maxLifes = player.maxLifes;
        int currentSatiety = player.currentSatiety;
        int maxSatiety = player.maxSatiety;
        int levelLimit = player.levelLimit;
        int currentExp = player.currentExp;
        int skillPoints = player.skillPoints;
        int attack = player.attack;
        int defense = player.defense;
        int agility = player.agility;
        int stamina = player.stamina;
		bool isMoving = player.isMoving;
        bool isInvulnerable = player.isInvulnerable;
        bool isInvisible = player.isInvisible;
        int invulnerableCount = player.invulnerableCount;
        int invisibleCount = player.invisibleCount;
		playerData = new PlayerSaveData(position, stepPoint, point, currentLifes, maxLifes, currentSatiety, maxSatiety, levelLimit, currentExp, skillPoints, attack, defense, agility, stamina, isMoving, isInvulnerable, isInvisible, invulnerableCount, invisibleCount);
	}

	[System.Serializable]
	public struct PlayerInventorySaveData
	{
		public int id, count;
		public PlayerInventorySaveData(int id, int count)
		{
			this.id = id;
			this.count = count;
		}
	}
	public void SavePlayerInventory()
	{
		for(int i = 0; i < Inventory.Instance.maxCount; i++)
		{
			int id = Inventory.Instance.items[i].id;
			int count = Inventory.Instance.items[i].count;
			inventoryData.Add(new PlayerInventorySaveData(id, count));
		}
	}

	[System.Serializable]
	public struct EnemyInventorySaveData
	{
		public int id, count;
		public EnemyInventorySaveData(int id, int count)
		{
			this.id = id;
			this.count = count;
		}
	}
	public void SaveEnemyInventory()
	{
		foreach(var inv in Generator.Instance.Invtr)
		{
			var invtr = inv.GetComponentInChildren<InventoryEnemy>();

			for(int i = 0; i < invtr.maxCount; i++)
			{
				int id = invtr.items[i].id;
				int count = invtr.items[i].count;
				enemyInventoryData.Add(new EnemyInventorySaveData(id, count));
			}
		}
	}

	[System.Serializable]
	public struct ContainersInventorySaveData
	{
		public int id, count;
		public ContainersInventorySaveData(int id, int count)
		{
			this.id = id;
			this.count = count;
		}
	}
	public void SaveContainersInventory()
	{
		foreach(var inv in Generator.Instance.InvtrContainers)
		{
			var invtr = inv.GetComponentInChildren<InventoryEnemy>();

			for(int i = 0; i < invtr.maxCount; i++)
			{
				int id = invtr.items[i].id;
				int count = invtr.items[i].count;
				containersInventoryData.Add(new ContainersInventorySaveData(id, count));
			}
		}
	}

	[System.Serializable]
	public struct EnemySaveData
	{
		public Vec3 position, stepPoint;
		public int lifes;
		public bool isStep, isSleep;

		public EnemySaveData(Vec3 position, Vec3 stepPoint, int lifes, bool isStep, bool isSleep)
		{
			this.position = position;
			this.stepPoint = stepPoint;
			this.lifes = lifes;
			this.isStep = isStep;
			this.isSleep = isSleep;
		}
	}
	public void SaveEnemies()
	{
		foreach(var go in Generator.Instance.enemies)
		{
			var em = go.GetComponent<Enemy>();

			Vec3 position = new Vec3(go.transform.position.x, go.transform.position.y, go.transform.position.z);
			Vec3 stepPoint = new Vec3(em.stepPoint.x, em.stepPoint.y, em.stepPoint.z);
			int lifes = em.lifes;
			bool isStep = em.isStep;
			bool isSleep = em.isSleep;

			enemiesData.Add(new EnemySaveData(position, stepPoint, lifes, isStep, isSleep));
		}
	}

	[System.Serializable]
	public struct MapSaveData
	{
		public int tiles;
		public int enemyCount, containersCount, otherObjectsCount;
        public int[] enemyTypes;
        public int[] otherObjectsType;

		public MapSaveData(int tiles, int enemyCount, int containersCount, int otherObjectsCount, int[] enemyTypes, int[] otherObjectsType)
		{
			this.tiles = tiles;
			this.enemyCount = enemyCount;
			this.containersCount = containersCount;
            this.otherObjectsCount = otherObjectsCount;
            this.enemyTypes = enemyTypes;
            this.otherObjectsType = otherObjectsType;
		}
	}
	public void SaveMap()
	{
		int enemyCount = Generator.Instance.enemies.Length;
		int containersCount = Generator.Instance.containers.Length;
        int otherObjectsCount = Generator.Instance.otherObjects.Length;
        int[] enemyTypes = Generator.Instance.enemyTypes;
        int[] otherObjectsType = Generator.Instance.otherObjectsType;

		for(int i = 0; i < Generator.Instance.MapColumns; i++)
		{
			for(int j = 0; j < Generator.Instance.MapRows; j++)
			{
				int tiles = (int)Generator.Instance.tiles[i][j];
				mapData.Add(new MapSaveData(tiles, enemyCount, containersCount, otherObjectsCount, enemyTypes, otherObjectsType));
			}
		}
	}

    [System.Serializable]
    public struct GameManagerSaveData
    {
        public int levelCount;
        public int playerLevel;
        public bool levelUp;
        public GameManagerSaveData(int levelCount, int playerLevel, bool levelUp)
        {
            this.levelCount = levelCount;
            this.playerLevel = playerLevel;
            this.levelUp = levelUp;
        }
    }

    public void SaveGameManager()
    {
        int levelCount = GameManager.Instance.level;
        int playerLevel = GameManager.Instance.playerLevel;
        bool levelUp = GameManager.Instance.UpLevelMessage.activeSelf;
        gameManagerData = new GameManagerSaveData(levelCount, playerLevel, levelUp);
    }

    [System.Serializable]
	public struct Vec3
	{
		public float x,y,z;

		public Vec3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	[System.Serializable]
	public struct Vec2
	{
		public float x,y;

		public Vec2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}
}
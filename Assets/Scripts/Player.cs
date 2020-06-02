using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rogue;

public class Player : Singleton<Player>, IPointerDownHandler, IPointerUpHandler 
{
    public GameObject shopPanel;
 
    public Vector3 point;
    public Vector3 stepPoint;

    public Slider HealthBar;
    public Slider SatietyBar;
  
    public AudioClip getDamage;
    public AudioClip PunchSound;
    public AudioClip EatSound;
    public AudioClip DrinkPotionSound;

    public bool isMoving;
    public bool isAnimation;

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

    private Camera cam;
    private Animator anim;
    private bool isDeath = false;
    private SpriteRenderer sprite;

    public void OnPointerDown(PointerEventData eventData) { } 
    public void OnPointerUp(PointerEventData eventData)
    {   
        if(isMoving)
            point = stepPoint;

        else if(!GameManager.Instance.onPause)
        {
            point = Camera.main.ScreenToWorldPoint(new Vector3((int)Mathf.Round(eventData.position.x), (int)Mathf.Round(eventData.position.y), 0)); 
            point = new Vector3((int)Mathf.Round(point.x), (int)Mathf.Round(point.y), 0);
            isMoving = true;
            (stepPoint.x, stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y, (int)point.x, (int)point.y);
            ChangeSprite();
            if(Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Floor ||
                Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.CorridorFloor)
                GiveStepEnemies();
        }    
    }

    private void Start()
    {
        stepPoint = transform.position;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {   
        if (isMoving)
            if(!isDeath)
                Move(); 
    }

    private void Move()
    {
        if(transform.position == stepPoint)
        {
            if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.End)
                GameManager.Instance.NewLevelMessage();

            (stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y, (int)point.x, (int)point.y);  

            if(stepPoint != transform.position) 
                GiveStepEnemies();

            currentSatiety--;

            if (currentSatiety < 0)
            {
                currentSatiety = 0;
                currentLifes--;
                if(currentLifes <= 0)
                    GameManager.Instance.GameOver();

            }

            updateBars(0, 0);
        }

        if(Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Enemy)
        {
            isMoving = false;
            HitEnemy((int)stepPoint.x, (int)stepPoint.y);
            stepPoint = transform.position;
        }

        else if(Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Seller)
        {
            isMoving = false;
            stepPoint = transform.position;
            shopPanel.SetActive(true);

            int ItemId = 2;
            int i = 0;

            while(i < 25)
            {
                if (ItemId < 23)
                {
                    Shop.Instance.AddItem(i, DataBase.Instance.items[ItemId], 1, DataBase.Instance.items[ItemId].type);
                    i++;
                    ItemId++;

                    if (DataBase.Instance.items[ItemId].type != DataBase.Instance.items[ItemId - 1].type)
                    {
                        float j = (float)(i + 5) / 5;
                        Debug.Log(j);

                        if (j > 1 && j < 2)
                            i = 5;
                        else if (j > 2 && j < 3)
                            i = 10;
                        else if (j > 3 && j < 4)
                            i = 15;
                        else if (j > 4)
                            i = 20;
                    }
                }
                else
                {
                    i = 26;
                }
            }
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Chest)
        {
            isMoving = false;
            stepPoint = transform.position;
            GameManager.Instance.Win();
        }

        else if(Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Object)
        {
            for(int i = 0; i < Generator.Instance.containers.Length; i++)
            {
                if(Generator.Instance.containers[i].transform.position == stepPoint)
                {
                    Generator.Instance.InvtrContainers[i].SetActive(true);
                }
            }

            isMoving = false;
            stepPoint = transform.position;
            GameManager.Instance.onPause = true;
        }

        else if(Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Wall || 
            Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Object)
        {        
            isMoving = false;
            stepPoint = transform.position;
        }

        else if (transform.position.x == (int)point.x && transform.position.y == (int)point.y)
        {
            isMoving = false; 

            if(Generator.Instance.tiles[(int)point.x][(int)point.y] == Generator.TileType.Drop)
                OpenDrop(); 
        }

        else
        {
            if(!isAnimation && stepPoint != transform.position)
            {
                isAnimation = true;
                anim.Play("PlayerWalking", 0, 0.1f);
            }

            transform.position = Vector2.MoveTowards(transform.position, stepPoint, GameManager.Instance.gameSpeed * Time.deltaTime);  
            Camera.main.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
        }  
    }

    private void HitEnemy(int x, int y)
    {
        for(int i = 0; i < Generator.Instance.enemies.Length; i++)
        {
            Enemy enemy = Generator.Instance.enemies[i].GetComponent<Enemy>(); 
            enemy.isStep = true;

            if(x == Generator.Instance.enemies[i].transform.position.x && 
                y == Generator.Instance.enemies[i].transform.position.y)
            {
                int damage = Random.Range(attack - 4, attack + 4);

                if (Generator.Instance.tiles[(int)transform.position.x + 1][(int)transform.position.y] == Generator.TileType.Enemy)
                    GameManager.Instance.spawnHitText(220, 0, damage);
                else if (Generator.Instance.tiles[(int)transform.position.x][(int)transform.position.y - 1] == Generator.TileType.Enemy)
                    GameManager.Instance.spawnHitText(0, -220, damage);
                else if (Generator.Instance.tiles[(int)transform.position.x - 1][(int)transform.position.y] == Generator.TileType.Enemy)
                   GameManager.Instance.spawnHitText(-220, 0, damage);
                else if (Generator.Instance.tiles[(int)transform.position.x][(int)transform.position.y + 1] == Generator.TileType.Enemy)
                   GameManager.Instance.spawnHitText(0, 220, damage);

                enemy.GetDamage(damage);

                AudioManager.Instance.PlayEffects(PunchSound);
                anim.Play("PlayerHit", 0, 0.1f);
            }
        }
    }

    private void GiveStepEnemies()
    {
        for(int i = 0; i < Generator.Instance.enemies.Length; i++)
        {
            Enemy enemy = Generator.Instance.enemies[i].GetComponent<Enemy>();
            enemy.isStep = true;
        }        
    }

    public void GetDamage(int l)
    {
        int accuracy = Random.Range(0, 200);

        if (agility < accuracy)
        {
            l -= defense;

            if (l <= 0)
                l = 1;

            point = stepPoint;

            AudioManager.Instance.PlayEffects(getDamage);

            updateBars(-l, 0);
            GameManager.Instance.spawnHitText(0, 0, l);

            if (currentLifes <= 0)
            {
                isDeath = true;
                anim.Play("PlayerDeath", 0, 0.1f);
                GameManager.Instance.GameOver();
            }
        }
        else
            GameManager.Instance.spawnHitText(0, 0, 0);
    }

    public void EatPlayer(int healthRecovery, int satietyRecovery)
    {
        AudioManager.Instance.PlayEffects(EatSound);
        if (currentSatiety < maxSatiety)
            updateBars(0, satietyRecovery);
        if (currentLifes < maxLifes)
            updateBars(healthRecovery, 0);
    }

    public void DrinkPotion(string potionName)
    {
        AudioManager.Instance.PlayEffects(DrinkPotionSound);

        switch (potionName)
        {
            case "Potion_1":
                updateBars(50, 0);
                break;
            case "Potion_2":
                Debug.Log("Вы выпили зелье");
                break;
            case "Potion_3":
                Debug.Log("Вы выпили зелье");
                break;
            case "Potion_4":
                Debug.Log("Вы выпили зелье");
                break;
            default:
                break;
        }
    }

    public void updateBars(int health, int satiety)
    {
        currentSatiety += satiety;

        if (currentSatiety > maxSatiety)
            currentSatiety = maxSatiety;

        currentLifes += health;

        if (currentLifes > maxLifes)
            currentLifes = maxLifes;

        HealthBar.value = (float)currentLifes / (float)maxLifes;
        GameManager.Instance.healthCount.text = Player.Instance.currentLifes.ToString() + "/" + Player.Instance.maxLifes.ToString();

        SatietyBar.value = (float)currentSatiety / (float)maxSatiety;
        GameManager.Instance.satietyCount.text = Player.Instance.currentSatiety.ToString() + "/" + Player.Instance.maxSatiety.ToString();
    }

    public void getExp(int expCount)
    {
        currentExp += expCount;
        if(currentExp >= levelLimit)
        {
            currentExp -= levelLimit;
            GameManager.Instance.playerLevel++;
            GameManager.Instance.playerLevelCount.text = GameManager.Instance.playerLevel.ToString();
            currentLifes = maxLifes;
            HealthBar.value = 1;
            GameManager.Instance.healthCount.text = "100/100";
            levelLimit += 50;
            GameManager.Instance.UpLevel();
            skillPoints += 15;
        }
    }

    public void addAttack()
    {
        if (skillPoints > 0)
        {
            attack++;
            skillPoints--;
        }
    }

    public void addDefense()
    {
        if (skillPoints > 0)
        {
            defense++;
            skillPoints--;
        }
    }

    public void addAgility()
    {
        if (skillPoints > 0)
        {
            agility++;
            skillPoints--;
        }
    }

    public void addStamina()
    {
        if (skillPoints > 0)
        {
            stamina++;
            skillPoints--;
            maxLifes++;
            currentLifes++;
            GameManager.Instance.healthCount.text = Player.Instance.currentLifes.ToString() + "/" + Player.Instance.maxLifes.ToString();
            HealthBar.value = (float)currentLifes / (float)maxLifes;

            maxSatiety++;
            currentSatiety++;
            GameManager.Instance.satietyCount.text = Player.Instance.currentSatiety.ToString() + "/" + Player.Instance.maxSatiety.ToString();
            SatietyBar.value = (float)currentSatiety / (float)maxSatiety;
        }
    }

    private void OpenDrop()
    {
        for(int i = 0; i < Generator.Instance.enemies.Length; i++)
        {
            if(transform.position == Generator.Instance.enemies[i].transform.position)
            {
                Generator.Instance.Invtr[i].SetActive(true);
                GameManager.Instance.onPause = true;
            } 
        }
    }

    private (int a, int b) FindWave(int startX, int startY, int targetX, int targetY) 
    {
        int x, y,step=0;
        int stepX = 0, stepY = 0;
        int[,] cMap = new int[Generator.Instance.MapColumns, Generator.Instance.MapRows];

        for (x = 0; x < Generator.Instance.MapColumns; x++) 
            for (y = 0; y < Generator.Instance.MapRows; y++)
            {
                if (Generator.Instance.tiles[x][y] != Generator.TileType.Floor && 
                    Generator.Instance.tiles[x][y] != Generator.TileType.CorridorFloor && 
                    Generator.Instance.tiles[x][y] != Generator.TileType.End  && 
                    Generator.Instance.tiles[x][y] != Generator.TileType.Drop)
                    cMap[x, y] = -2;
                else
                    cMap[x, y] = -1; 
            }

        
        if(startX == targetX && startY == targetY)
        {
            isMoving = false;
            return (startX, startY);
        }

        cMap[targetX,targetY]=0; 

        while (true)
        {
            for (x = startX - 8; x < startX + 8; x++)
                for (y = startY - 8; y < startY + 8; y++)
                {
                    if (cMap[x, y] == step)
                    {
                        if (x - 1 >= 0)
                            if (cMap[x - 1, y] == -1)
                                cMap[x - 1, y] = step + 1;
                        
                            if (y - 1 >= 0)
                            if (cMap[x, y - 1] == -1)
                                cMap[x, y - 1] = step + 1;
                        
                            if (x + 1 < Generator.Instance.MapColumns)
                            if (cMap[x + 1, y] == -1)
                                cMap[x + 1, y] = step + 1;

                            if (y + 1 < Generator.Instance.MapRows)
                            if (cMap[x, y + 1] == -1)
                                cMap[x, y + 1] = step + 1;
                    }
                }
            step++;
            if (cMap[startX, startY] != -1)
                break;
            if (step > 20 * 20){
                isMoving = false;
                return (startX, startY);
            }
        }

        x = startX; 
        y = startY;
        step = int.MaxValue;

        if (x - 1 >= 0)
            if (cMap[x - 1, y] >= 0 && cMap[x - 1, y] < step)
            {
                step = cMap[x - 1, y];
                stepX = x - 1;
                stepY = y;
                return (stepX,stepY);
            }    
        if (y - 1 >= 0)
            if (cMap[x, y - 1] >= 0 && cMap[x, y - 1] < step)
            {
                step = cMap[x, y - 1];
                stepX = x;
                stepY = y - 1;
                return (stepX,stepY);
            }      
        if (x + 1 < Generator.Instance.MapRows)
            if (cMap[x + 1, y] < step && cMap[x + 1, y] >= 0)
            {
                step = cMap[x + 1, y];
                stepX = x + 1;
                stepY = y;
                return (stepX,stepY);
            }                
        if (y + 1 < Generator.Instance.MapColumns )
            if (cMap[x, y + 1] < step && cMap[x, y + 1] >= 0)
            {
                step = cMap[x, y + 1];
                stepX = x;
                stepY = y + 1;
                return (stepX,stepY);
            }
            
        isMoving = false;
        return (startX,startY);
    }

    private void ChangeSprite()
    {
        if(point.x < transform.position.x)
            sprite.flipX = true;
        else
            sprite.flipX = false;
    }

    private void startAnimation()
    {
        isAnimation = true;
    }

    private void endAnimation()
    {
        isAnimation = false;
    }

    public void LoadData(Save.PlayerSaveData save)
    {
        transform.position = new Vector3(save.position.x, save.position.y, save.position.z);
        stepPoint = new Vector3(save.stepPoint.x, save.stepPoint.y, save.stepPoint.z);
        point = new Vector2(save.point.x, save.point.y);

        currentLifes = save.currentLifes;
        levelLimit = save.levelLimit;
        currentExp = save.currentExp;
        isMoving = save.isMoving;

        skillPoints = save.skillPoints;
        attack = save.attack;
        defense = save.defense;
        agility = save.agility;
        stamina = save.stamina;

        Camera.main.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
        HealthBar.value = (float)currentLifes / (float)maxLifes;

        if(currentLifes <= 0)
        {
            isDeath = true;
            anim.Play("PlayerDeath", 0, 0.1f);
            GameManager.Instance.GameOver();
        }

        isAnimation = false;
    }
}
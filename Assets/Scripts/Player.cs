using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rogue;

public class Player : Singleton<Player>, IPointerDownHandler, IPointerUpHandler 
{
    public GameObject shopPanel;
    public GameObject learnPanel;
    public GameObject buyPanel;
 
    public Vector3 point;
    public Vector3 stepPoint;

    public Slider HealthBar;
    public Slider SatietyBar;
  
    public AudioClip getDamage;
    public AudioClip PunchSound;
    public AudioClip EatSound;
    public AudioClip DrinkPotionSound;
    public AudioClip PotionInvulnerableSound;
    public AudioClip Vomiting;

    public bool isMoving;
    public bool isAnimation;
    public bool isInvulnerable;
    public bool isInvisible;

    public Text learnText;

    public int currentLifes;
    public int maxLifes;
    public int invulnerableCount;
    public int invisibleCount;

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

            invulnerableCount--;
            invisibleCount--;

            if(invulnerableCount == 0)
            {
                isInvulnerable = false;
                AudioManager.Instance.PlayEffects(PotionInvulnerableSound);
                this.GetComponent<SpriteRenderer>().material.color = Color.white;
            }

            if(invisibleCount == 0)
            {
                isInvisible = false;
                AudioManager.Instance.PlayEffects(PotionInvulnerableSound);
                this.GetComponent<SpriteRenderer>().material.color = Color.white;
            }

            updateBars(0, 0);
        }

        if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Enemy)
        {
            isMoving = false;
            HitEnemy((int)stepPoint.x, (int)stepPoint.y);
            stepPoint = transform.position;
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.SellerPotion)
        {
            isMoving = false;
            stepPoint = transform.position;
            shopPanel.SetActive(true);

            Shop.Instance.DeleteAllItems();
            Shop.Instance.AddItem(0, DataBase.Instance.items[14], 1, DataBase.Instance.items[14].type);
            Shop.Instance.AddItem(1, DataBase.Instance.items[15], 1, DataBase.Instance.items[15].type);
            Shop.Instance.AddItem(2, DataBase.Instance.items[16], 1, DataBase.Instance.items[16].type);
            Shop.Instance.AddItem(3, DataBase.Instance.items[17], 1, DataBase.Instance.items[17].type);
            Shop.Instance.AddItem(4, DataBase.Instance.items[24], 1, DataBase.Instance.items[24].type);
            Shop.Instance.AddItem(5, DataBase.Instance.items[25], 1, DataBase.Instance.items[25].type);
            Shop.Instance.AddItem(6, DataBase.Instance.items[26], 1, DataBase.Instance.items[26].type);

            GameManager.Instance.PauseGame();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.SellerArmor)
        {
            isMoving = false;
            stepPoint = transform.position;
            shopPanel.SetActive(true);

            Shop.Instance.DeleteAllItems();
            Shop.Instance.AddItem(0, DataBase.Instance.items[18], 1, DataBase.Instance.items[18].type);
            Shop.Instance.AddItem(1, DataBase.Instance.items[19], 1, DataBase.Instance.items[19].type);
            Shop.Instance.AddItem(2, DataBase.Instance.items[20], 1, DataBase.Instance.items[20].type);
            Shop.Instance.AddItem(3, DataBase.Instance.items[21], 1, DataBase.Instance.items[21].type);
            Shop.Instance.AddItem(4, DataBase.Instance.items[22], 1, DataBase.Instance.items[22].type);
            Shop.Instance.AddItem(5, DataBase.Instance.items[37], 1, DataBase.Instance.items[37].type);
            Shop.Instance.AddItem(6, DataBase.Instance.items[38], 1, DataBase.Instance.items[38].type);
            Shop.Instance.AddItem(7, DataBase.Instance.items[39], 1, DataBase.Instance.items[39].type);
            Shop.Instance.AddItem(8, DataBase.Instance.items[40], 1, DataBase.Instance.items[40].type);
            Shop.Instance.AddItem(9, DataBase.Instance.items[41], 1, DataBase.Instance.items[41].type);

            GameManager.Instance.PauseGame();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.SellerFood)
        {
            isMoving = false;
            stepPoint = transform.position;
            shopPanel.SetActive(true);

            Shop.Instance.DeleteAllItems();
            Shop.Instance.AddItem(0, DataBase.Instance.items[2], 1, DataBase.Instance.items[2].type);
            Shop.Instance.AddItem(1, DataBase.Instance.items[3], 1, DataBase.Instance.items[3].type);

            GameManager.Instance.PauseGame();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.SellerShield)
        {
            isMoving = false;
            stepPoint = transform.position;
            shopPanel.SetActive(true);

            Shop.Instance.DeleteAllItems();
            Shop.Instance.AddItem(0, DataBase.Instance.items[9], 1, DataBase.Instance.items[9].type);
            Shop.Instance.AddItem(1, DataBase.Instance.items[10], 1, DataBase.Instance.items[10].type);
            Shop.Instance.AddItem(2, DataBase.Instance.items[11], 1, DataBase.Instance.items[11].type);
            Shop.Instance.AddItem(3, DataBase.Instance.items[12], 1, DataBase.Instance.items[12].type);
            Shop.Instance.AddItem(4, DataBase.Instance.items[13], 1, DataBase.Instance.items[13].type);
            Shop.Instance.AddItem(5, DataBase.Instance.items[32], 1, DataBase.Instance.items[32].type);
            Shop.Instance.AddItem(6, DataBase.Instance.items[33], 1, DataBase.Instance.items[33].type);
            Shop.Instance.AddItem(7, DataBase.Instance.items[34], 1, DataBase.Instance.items[34].type);
            Shop.Instance.AddItem(8, DataBase.Instance.items[35], 1, DataBase.Instance.items[35].type);
            Shop.Instance.AddItem(9, DataBase.Instance.items[36], 1, DataBase.Instance.items[36].type);

            GameManager.Instance.PauseGame();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Buyer)
        {
            isMoving = false;
            stepPoint = transform.position;
            buyPanel.SetActive(true);

            Buy.Instance.AddItem(0, DataBase.Instance.items[Inventory.Instance.items[5].id], Inventory.Instance.items[5].count, Inventory.Instance.items[5].type);
            Buy.Instance.AddItem(1, DataBase.Instance.items[Inventory.Instance.items[6].id], Inventory.Instance.items[6].count, Inventory.Instance.items[6].type);
            Buy.Instance.AddItem(2, DataBase.Instance.items[Inventory.Instance.items[7].id], Inventory.Instance.items[7].count, Inventory.Instance.items[7].type);
            Buy.Instance.AddItem(3, DataBase.Instance.items[Inventory.Instance.items[8].id], Inventory.Instance.items[8].count, Inventory.Instance.items[8].type);
            Buy.Instance.AddItem(4, DataBase.Instance.items[Inventory.Instance.items[9].id], Inventory.Instance.items[9].count, Inventory.Instance.items[9].type);
            Buy.Instance.AddItem(5, DataBase.Instance.items[Inventory.Instance.items[10].id], Inventory.Instance.items[10].count, Inventory.Instance.items[10].type);
            Buy.Instance.AddItem(6, DataBase.Instance.items[Inventory.Instance.items[11].id], Inventory.Instance.items[11].count, Inventory.Instance.items[11].type);
            Buy.Instance.AddItem(7, DataBase.Instance.items[Inventory.Instance.items[12].id], Inventory.Instance.items[12].count, Inventory.Instance.items[12].type);
            Buy.Instance.AddItem(8, DataBase.Instance.items[Inventory.Instance.items[13].id], Inventory.Instance.items[13].count, Inventory.Instance.items[13].type);
            Buy.Instance.AddItem(9, DataBase.Instance.items[Inventory.Instance.items[14].id], Inventory.Instance.items[14].count, Inventory.Instance.items[14].type);
            Buy.Instance.AddItem(10, DataBase.Instance.items[Inventory.Instance.items[15].id], Inventory.Instance.items[15].count, Inventory.Instance.items[15].type);
            Buy.Instance.AddItem(11, DataBase.Instance.items[Inventory.Instance.items[16].id], Inventory.Instance.items[16].count, Inventory.Instance.items[16].type);
            Buy.Instance.AddItem(12, DataBase.Instance.items[Inventory.Instance.items[17].id], Inventory.Instance.items[17].count, Inventory.Instance.items[17].type);
            Buy.Instance.AddItem(13, DataBase.Instance.items[Inventory.Instance.items[18].id], Inventory.Instance.items[18].count, Inventory.Instance.items[18].type);
            Buy.Instance.AddItem(14, DataBase.Instance.items[Inventory.Instance.items[19].id], Inventory.Instance.items[19].count, Inventory.Instance.items[19].type);
            Buy.Instance.AddItem(15, DataBase.Instance.items[Inventory.Instance.items[20].id], Inventory.Instance.items[20].count, Inventory.Instance.items[20].type);
            Buy.Instance.AddItem(16, DataBase.Instance.items[Inventory.Instance.items[21].id], Inventory.Instance.items[21].count, Inventory.Instance.items[21].type);
            Buy.Instance.AddItem(17, DataBase.Instance.items[Inventory.Instance.items[22].id], Inventory.Instance.items[22].count, Inventory.Instance.items[22].type);
            Buy.Instance.AddItem(18, DataBase.Instance.items[Inventory.Instance.items[23].id], Inventory.Instance.items[23].count, Inventory.Instance.items[23].type);
            Buy.Instance.AddItem(19, DataBase.Instance.items[Inventory.Instance.items[24].id], Inventory.Instance.items[24].count, Inventory.Instance.items[24].type);

            GameManager.Instance.PauseGame();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.SellerWeapon)
        {
            isMoving = false;
            stepPoint = transform.position;
            shopPanel.SetActive(true);

            Shop.Instance.DeleteAllItems();
            Shop.Instance.AddItem(0, DataBase.Instance.items[4], 1, DataBase.Instance.items[4].type);
            Shop.Instance.AddItem(1, DataBase.Instance.items[5], 1, DataBase.Instance.items[5].type);
            Shop.Instance.AddItem(2, DataBase.Instance.items[6], 1, DataBase.Instance.items[6].type);
            Shop.Instance.AddItem(3, DataBase.Instance.items[7], 1, DataBase.Instance.items[7].type);
            Shop.Instance.AddItem(4, DataBase.Instance.items[8], 1, DataBase.Instance.items[8].type);
            Shop.Instance.AddItem(5, DataBase.Instance.items[27], 1, DataBase.Instance.items[27].type);
            Shop.Instance.AddItem(6, DataBase.Instance.items[28], 1, DataBase.Instance.items[28].type);
            Shop.Instance.AddItem(7, DataBase.Instance.items[29], 1, DataBase.Instance.items[29].type);
            Shop.Instance.AddItem(8, DataBase.Instance.items[30], 1, DataBase.Instance.items[30].type);
            Shop.Instance.AddItem(9, DataBase.Instance.items[31], 1, DataBase.Instance.items[31].type);

            GameManager.Instance.PauseGame();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Well)
        {
            AudioManager.Instance.PlayEffects(DrinkPotionSound);
            Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] = Generator.TileType.Wall;
            GameManager.Instance.spawnHitText(0, 0, -50);
            isMoving = false;
            stepPoint = transform.position;
            updateBars(50, 0);
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Sign)
        {
            isMoving = false;
            stepPoint = transform.position;
            learnPanel.SetActive(true);
            int type = Random.Range(0, 15);

            if (type == 0)
                learnText.GetComponent<Text>().text = "Õ¿œ¿ƒ≈Õ»≈ ”¬≈À»◊»¬¿≈“ —»À” ”ƒ¿–¿";
            if (type == 1)
                learnText.GetComponent<Text>().text = "ÀŒ¬ Œ—“‹ ”¬≈À»◊»¬¿≈“ ÿ¿Õ— œ–ŒÃ¿’¿ œŒ ¬¿Ã";
            if (type == 2)
                learnText.GetComponent<Text>().text = "¬€ÕŒ—À»¬Œ—“‹ ”¬≈À»◊»¬¿≈“ Ã¿ —»Ã¿À‹ÕŒ≈ Œ◊ Œ «ƒŒ–Œ¬‹ﬂ » —€“Œ—“»";
            if (type == 3)
                learnText.GetComponent<Text>().text = "«¿Ÿ»“¿ ”Ã≈Õ‹ÿ¿≈“ ¬’ŒƒﬂŸ»… ”–ŒÕ œŒ ¬¿Ã";
            if (type == 4)
                learnText.GetComponent<Text>().text = "œ–ŒƒŒÀ∆»“≈À‹ÕŒ—“‹ ›‘‘≈ “¿ «≈À»… «¿¬»—»“ Œ“ ¬€ÕŒ—À»¬Œ—“»";
            if (type == 5)
                learnText.GetComponent<Text>().text = "œŒƒ«≈Ã≈À‹ﬂ Œ◊≈Õ‹ Œœ¿—Õ€≈, ¡”ƒ‹“≈ Œ—“Œ–Œ∆Õ€!!!";
            if (type == 6)
                learnText.GetComponent<Text>().text = "≈—À» ¬¿ÿ¿ ÿ ¿À¿ —€“Œ—“» ”œ¿ƒ≈“ ƒŒ Õ”Àﬂ, ¬€ ¡”ƒ≈“≈ œŒÀ”◊¿“‹ ”–ŒÕ  ¿∆ƒ€… ’Œƒ";
            if (type == 7)
                learnText.GetComponent<Text>().text = "—À≈ƒ»“≈ «¿ ¬¿ÿ»Ã» ÿ ¿À¿Ã» «ƒŒ–Œ¬‹ﬂ » —€“Œ—“»";
            if (type == 8)
                learnText.GetComponent<Text>().text = "¬—≈√ƒ¿ »Ã≈…“≈ «¿œ¿— ≈ƒ€";
            if (type == 9)
                learnText.GetComponent<Text>().text = "≈—À» ¬€ ¬€…ƒ≈“≈ »« »√–€, ¬¿ÿ œ–Œ√–≈—— —Œ’–¿Õ»“—ﬂ";
            if (type == 10)
                learnText.GetComponent<Text>().text = " ¿∆ƒŒ≈ œŒƒÕﬂ“»≈ ”–Œ¬Õﬂ ¡”ƒ≈“ ƒ¿¬¿“‹—ﬂ —ÀŒ∆Õ≈≈ —  ¿∆ƒ€Ã –¿«ŒÃ";
            if (type == 11)
                learnText.GetComponent<Text>().text = "¬€ Õ≈ —ÃŒ∆≈“≈ ¡–¿“‹ œ–≈ƒÃ≈“€, ≈—À» ¬¿ÿ »Õ¬≈Õ“¿–‹ ¡”ƒ≈“ «¿œŒÀÕ≈Õ";
            if (type == 12)
                learnText.GetComponent<Text>().text = "«¿œŒÃ»Õ¿…“≈ ›‘‘≈ “  ¿∆ƒŒ√Œ «≈À‹ﬂ";
            if (type == 13)
                learnText.GetComponent<Text>().text = "Õ≈ Œ“Œ–€≈ «≈À‹ﬂ ÃŒ√”“ ”¡»“‹ ¬¿—, ¡”ƒ‹“≈ Œ—“Œ–Œ∆Õ€!!!";
            if (type == 14)
                learnText.GetComponent<Text>().text = " ŒÀŒƒ≈÷ ¬Œ——“¿Õ¿¬À»¬¿≈“ ¬¿ÿ≈ «ƒŒ–Œ¬‹≈";

            GameManager.Instance.PauseGame();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Chest)
        {
            isMoving = false;
            stepPoint = transform.position;
            GameManager.Instance.Win();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Object)
        {
            for (int i = 0; i < Generator.Instance.containers.Length; i++)
            {
                if (Generator.Instance.containers[i].transform.position == stepPoint)
                {
                    Generator.Instance.InvtrContainers[i].SetActive(true);
                }
            }

            isMoving = false;
            stepPoint = transform.position;
            GameManager.Instance.PauseGame();
        }

        else if (Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Wall ||
            Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Object)
        {
            isMoving = false;
            stepPoint = transform.position;
        }

        else if (transform.position.x == (int)point.x && transform.position.y == (int)point.y)
        {
            isMoving = false;

            if (Generator.Instance.tiles[(int)point.x][(int)point.y] == Generator.TileType.Drop)
                OpenDrop();
        }

        else
        {
            if (!isAnimation && stepPoint != transform.position)
            {
                isAnimation = true;
                anim.Play("PlayerWalking", 0, 0.1f);
            }

            transform.position = Vector2.MoveTowards(transform.position, stepPoint, GameManager.Instance.gameSpeed * Time.deltaTime);
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
        }  
    }

    private void HitEnemy(int x, int y)
    {
        isInvisible = false;
        invisibleCount = 0;

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
            else if (isInvulnerable)
                l = 0;

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
                isInvulnerable = true;
                invulnerableCount = 30 + stamina;
                AudioManager.Instance.PlayEffects(PotionInvulnerableSound);
                this.GetComponent<SpriteRenderer>().material.color = Color.red;
                break;
            case "Potion_3":
                GetDamage(120);
                break;
            case "Potion_4":
                isInvisible = true;
                invisibleCount = 30 + stamina;
                AudioManager.Instance.PlayEffects(PotionInvulnerableSound);
                this.GetComponent<SpriteRenderer>().material.color = Color.green;
                break;
            case "Potion_5":
                AudioManager.Instance.PlayEffects(Vomiting);
                updateBars(0, -100);
                break;
            case "Potion_6":
                AudioManager.Instance.PlayEffects(PotionInvulnerableSound);
                getExp(500);
                break;
            case "Potion_7":
                AudioManager.Instance.PlayEffects(PotionInvulnerableSound);
                int randomIndex = Random.Range(0, Generator.Instance.enemyPositions.Count);
                Vector3 randomPosition = Generator.Instance.enemyPositions[randomIndex];
                transform.position = randomPosition;
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
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
        maxLifes = save.maxLifes;
        currentSatiety = save.currentSatiety;
        maxSatiety = save.maxSatiety;
        levelLimit = save.levelLimit;
        currentExp = save.currentExp;
        isMoving = save.isMoving;
        isInvulnerable = save.isInvulnerable;
        isInvisible = save.isInvisible;

        invulnerableCount = save.invulnerableCount;
        invisibleCount = save.invisibleCount;
        skillPoints = save.skillPoints;
        attack = save.attack;
        defense = save.defense;
        agility = save.agility;
        stamina = save.stamina;

        if(isInvulnerable)
            this.GetComponent<SpriteRenderer>().material.color = Color.red;

        Camera.main.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
        HealthBar.value = (float)currentLifes / (float)maxLifes;
        SatietyBar.value = (float)currentSatiety / (float)maxSatiety;
        GameManager.Instance.satietyCount.text = Player.Instance.currentSatiety.ToString() + "/" + Player.Instance.maxSatiety.ToString();
        GameManager.Instance.healthCount.text = Player.Instance.currentLifes.ToString() + "/" + Player.Instance.maxLifes.ToString();

        if (currentLifes <= 0)
        {
            isDeath = true;
            anim.Play("PlayerDeath", 0, 0.1f);
            GameManager.Instance.GameOver();
        }

        isAnimation = false;
    }
}
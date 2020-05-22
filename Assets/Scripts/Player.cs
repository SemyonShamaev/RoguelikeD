using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rogue;

public class Player : Singleton<Player>, IPointerDownHandler, IPointerUpHandler 
{
    public float speed;
    public int stepCount;
    public Vector3 point;
    public Slider HealthBar;
    public Vector3 stepPoint;
    public AudioClip getDamage;
    public AudioClip PunchSound;
    public AudioClip EatSound;
    public bool isMoving = false;
    public bool isAnimation = false;
    public int currentLifes;
    public int maxLifes;
    public int minDamage;
    public int maxDamage;

    private Camera cam;
    private bool isDeath = false;
    private Animator anim;
    private SpriteRenderer sprite;

    public void OnPointerDown(PointerEventData eventData) { } 
    public void OnPointerUp(PointerEventData eventData)
    {   
        if(!GameManager.Instance.onPause)
        {
            point = Camera.main.ScreenToWorldPoint(new Vector3((int)Mathf.Round(eventData.position.x), (int)Mathf.Round(eventData.position.y), 0)); 
            point = new Vector3((int)Mathf.Round(point.x), (int)Mathf.Round(point.y), 0);
            isMoving = true; 
            ChangeSprite();
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
            (stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y, (int)point.x, (int)point.y);  
            if(stepPoint != transform.position) 
                GiveStepEnemies();   
        }

        if(Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] == Generator.TileType.Enemy)
        {
            isMoving = false;
            HitEnemy((int)stepPoint.x, (int)stepPoint.y);
            stepPoint = transform.position;
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

            transform.position = Vector2.MoveTowards(transform.position, stepPoint, speed * Time.deltaTime);  
            Camera.main.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
        }  
    }

    private void HitEnemy(int x, int y)
    {
        for(int i = 0; i < Generator.Instance.enemies.Length; i++)
        {
            Enemy enemy = Generator.Instance.enemies[i].GetComponent<Enemy>(); // использовать наблюдателя
            enemy.isStep = true;
            if(x == Generator.Instance.enemies[i].transform.position.x && 
                y == Generator.Instance.enemies[i].transform.position.y)
            {
                enemy.GetDamage(Random.Range(minDamage, maxDamage));
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
        //anim.Play("PlayerHurt", 0, 0.5f);
        point = stepPoint;
        AudioManager.Instance.PlayEffects(getDamage);
        currentLifes -= l;
        HealthBar.value = (float)currentLifes / (float)maxLifes;
        GameManager.Instance.healthCount.text = Player.Instance.currentLifes.ToString() + "/" + Player.Instance.maxLifes.ToString();
        if(currentLifes <= 0)
        {
            isDeath = true;
            anim.Play("PlayerDeath", 0, 0.1f);
            GameManager.Instance.GameOver();
        }
    }

    public void HealPlayer()
    {
        AudioManager.Instance.PlayEffects(EatSound);
        if(currentLifes != maxLifes)
        {
            currentLifes++;
            HealthBar.value = currentLifes / maxLifes;
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

    private (int a, int b) FindWave(int startX, int startY, int targetX, int targetY) //Волновой алгоритм
    {
        int x, y,step=0;
        int stepX = 0, stepY = 0;
        int[,] cMap = new int[Generator.Instance.MapColumns, Generator.Instance.MapRows];

        for (x = 0; x < Generator.Instance.MapColumns; x++) //заполнение массива числами
            for (y = 0; y < Generator.Instance.MapRows; y++)
            {
                if (Generator.Instance.tiles[x][y] != Generator.TileType.Floor && 
                    Generator.Instance.tiles[x][y] != Generator.TileType.CorridorFloor && 
                    Generator.Instance.tiles[x][y] != Generator.TileType.End  && 
                    Generator.Instance.tiles[x][y] != Generator.TileType.Drop)
                    cMap[x, y] = -2; //есть препятствие
                else
                    cMap[x, y] = -1; //путь свободен
            }

        
        if(startX == targetX && startY == targetY)
        {
            isMoving = false;
            return (startX, startY);
        }

        cMap[targetX,targetY]=0; //отсчет начинается с конечной точки

        while (true) //поиск пути
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
            if (cMap[startX, startY] != -1) //удалось найти путь
                break;
            if (step > 20 * 20){ //если путь не удалось найти = возвращает стартовую точку
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
        isMoving = save.isMoving;
        Camera.main.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
        HealthBar.value = currentLifes / maxLifes;

        if(currentLifes <= 0)
        {
            isDeath = true;
            anim.Play("PlayerDeath", 0, 0.1f);
            GameManager.Instance.GameOver();
        }

        isAnimation = false;
    }
}

  
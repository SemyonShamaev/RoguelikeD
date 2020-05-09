using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rogue;

public class Player : Singleton<Player>, IPointerDownHandler, IPointerUpHandler 
{
    public float speed;
    public int stepCount;
    public Vector2 point;
    public Slider HealthBar;
    public Vector3 stepPoint;
    public AudioClip getDamage;
    public AudioClip PunchSound;
    public AudioClip EatSound;
    public bool isMoving = false;
    public bool isAnimation = false;
    public Sprite[] sprites = new Sprite[4];

    private Camera cam;
    private Animation anim;
    private float currentLifes;
    private float maxLifes = 10;
    private bool isDeath = false;

    public void OnPointerDown(PointerEventData eventData) { } 
    public void OnPointerUp(PointerEventData eventData)
    {   
        if(!GameManager.Instance.onPause)
        {
            point = Camera.main.ScreenToWorldPoint(new Vector3((int)Mathf.Round(eventData.position.x), (int)Mathf.Round(eventData.position.y), 0)); 
            point = new Vector2((int)Mathf.Round(point.x), (int)Mathf.Round(point.y));
            isMoving = true; 
        }    
    }

    private void Start()
    {
        currentLifes = maxLifes;
        stepPoint = transform.position;
        anim = gameObject.GetComponent<Animation>();      
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
            if(!isAnimation)
                (stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y, (int)point.x, (int)point.y); 

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
            ChangeSprite();
            transform.position = Vector2.MoveTowards(transform.position, stepPoint, speed * Time.deltaTime);  
            Camera.main.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);

            if(transform.position == stepPoint)
                GiveStepEnemies();
        }  
    }

    private void HitEnemy(int x, int y)
    {
        for(int i = 0; i < Generator.Instance.enemies.Length; i++)
        {
            if(x == Generator.Instance.enemies[i].transform.position.x && 
                y == Generator.Instance.enemies[i].transform.position.y)
            {
                Enemy enemy = Generator.Instance.enemies[i].GetComponent<Enemy>();
                enemy.GetDamage(1);

                ChangeSprite();
                AudioManager.Instance.PlayEffects(PunchSound);
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
        anim.Play("GetDamage");
        AudioManager.Instance.PlayEffects(getDamage);

        currentLifes -= l;
        HealthBar.value = currentLifes / maxLifes;

        if(currentLifes <= 0)
        {
            isDeath = true;
            anim.Play("Death");
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

    private void ChangeSprite()
    {
        if(stepPoint.x > (int)transform.position.x)
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        else if(stepPoint.x < (int)transform.position.x)
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        else if(stepPoint.y > (int)transform.position.y)
            GetComponent<SpriteRenderer>().sprite = sprites[2];
        else if(stepPoint.y < (int)transform.position.y)
            GetComponent<SpriteRenderer>().sprite = sprites[3];
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

    private void startAnimation()
    {
        isAnimation = true;
    }

    private void endAnimation()
    {
        isAnimation = false;
    }
}


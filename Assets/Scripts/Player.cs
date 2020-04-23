using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    public float speed;
    public int stepCount;
    int lifes = 50;

    public Vector3 stepPoint;
    Vector2 point;

    public bool isAnimation = false;
    public bool isMoving = false;

    public Sprite[] sprites = new Sprite[4];
    private Map Map;
    private Camera cam;
    Animation anim;
    

    public void OnPointerDown(PointerEventData eventData) //вызывается когда мышь нажата 
    { 

    } 
    public void OnPointerUp(PointerEventData eventData) //вызывается когда мышь отпущена
    {   
        point = cam.ScreenToWorldPoint(new Vector3((int)Mathf.Round(eventData.position.x), (int)Mathf.Round(eventData.position.y), 0)); //из локальных координат в мировые
        point = new Vector2((int)Mathf.Round(point.x), (int)Mathf.Round(point.y));
        isMoving = true; //запуск движения
    }

    void Start()
    {
        anim = gameObject.GetComponent<Animation>();
        cam = Camera.main;
        stepPoint = transform.position;
    }

    void Update()
    {   
        if (isMoving && !isAnimation)
        {
            Move();  
        }
    }

    void Awake()
    {
        Map = GameObject.Find("Map").GetComponent<Map>();
    }

    void Move()
    {
        float step = speed * Time.deltaTime;
        if(transform.position == stepPoint)
        {
            for(int i = 0; i < Map.enemies.Length; i++)
            {
                Enemy enemy = Map.enemies[i].GetComponent<Enemy>();
                enemy.isPunch = true;
            }
            (stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y, (int)Mathf.Round(point.x), (int)Mathf.Round(point.y)); 
        }
        if(Map.tiles[(int)stepPoint.x][(int)stepPoint.y] == Map.TileType.Enemy)
        {
            HitEnemy((int)stepPoint.x, (int)stepPoint.y);
            stepPoint = transform.position;
            isMoving = false;
        }
        if(Map.tiles[(int)stepPoint.x][(int)stepPoint.y] == Map.TileType.Wall || Map.tiles[(int)stepPoint.x][(int)stepPoint.y] == Map.TileType.Object)
            stepPoint = transform.position;
        else
        {
            ChangeSprite();
            transform.position = Vector2.MoveTowards(transform.position, stepPoint, step);  
            cam.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
            if (transform.position.x == (int)point.x && transform.position.y == (int)point.y)
            isMoving = false;    
        }  
    }

    void HitEnemy(int x, int y)
    {
        for(int i = 0; i < Map.enemies.Length; i++)
        {
            Enemy enemy = Map.enemies[i].GetComponent<Enemy>();
            enemy.isPunch = true;
            if(x == Map.enemies[i].transform.position.x && y == Map.enemies[i].transform.position.y)
            {
                enemy.getDamage(1);
            }
        }
    }

    public void GetDamage(int l)
    {
        anim.Play("GetDamage");
        lifes -= l;
    }

    (int a, int b) FindPlace(int x, int y, int sx, int sy)
    {
        if((sx == x - 1 && sy == y) || (sx == x + 1 && sy == y) || (sx == x && sy == y - 1) || (sx == x && sy == y + 1))
            return (x,y);
        if((Map.tiles[x+1][y]==Map.TileType.Floor || Map.tiles[x+1][y]==Map.TileType.CorridorFloor) && sx>x)
            return (x+1,y);
        if((Map.tiles[x-1][y]==Map.TileType.Floor || Map.tiles[x-1][y]==Map.TileType.CorridorFloor) && sx<x)
            return (x-1,y);
        if((Map.tiles[x][y+1]==Map.TileType.Floor || Map.tiles[x][y+1]==Map.TileType.CorridorFloor) && sy>y)
            return (x,y+1);
        if((Map.tiles[x][y-1]==Map.TileType.Floor || Map.tiles[x][y-1]==Map.TileType.CorridorFloor) && sy<y)
            return (x,y-1);
        if(Map.tiles[x+1][y]==Map.TileType.Floor || Map.tiles[x+1][y]==Map.TileType.CorridorFloor)
            return (x+1,y);
        if(Map.tiles[x-1][y]==Map.TileType.Floor || Map.tiles[x-1][y]==Map.TileType.CorridorFloor)
            return (x-1,y);
        if(Map.tiles[x][y+1]==Map.TileType.Floor || Map.tiles[x][y+1]==Map.TileType.CorridorFloor)
            return (x,y+1);
        if(Map.tiles[x][y-1]==Map.TileType.Floor || Map.tiles[x][y-1]==Map.TileType.CorridorFloor)
            return (x,y-1);
        return((int)transform.position.x, (int)transform.position.y);
    }

    void startAnimation()
    {
        isAnimation = true;
    }

    void endAnimation()
    {
        isAnimation = false;
    }

    void ChangeSprite()
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

    (int a, int b) FindWave(int startX, int startY, int targetX, int targetY) //Волновой алгоритм
    {
        int x, y,step=0;
        int stepX = 0, stepY = 0;
        int[,] cMap = new int[Map.MapColumns, Map.MapRows];

        for (x = 0; x < Map.MapColumns; x++) //заполнение массива числами
            for (y = 0; y < Map.MapRows; y++)
            {
                if (Map.tiles[x][y] != Map.TileType.Floor && 
                    Map.tiles[x][y] != Map.TileType.CorridorFloor && 
                    Map.tiles[x][y] != Map.TileType.End)
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
                        
                            if (x + 1 < Map.MapColumns)
                            if (cMap[x + 1, y] == -1)
                                cMap[x + 1, y] = step + 1;

                            if (y + 1 < Map.MapRows)
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
        if (x + 1 < Map.MapRows)
            if (cMap[x + 1, y] < step && cMap[x + 1, y] >= 0)
            {
                step = cMap[x + 1, y];
                stepX = x + 1;
                stepY = y;
                return (stepX,stepY);
            }                
        if (y + 1 < Map.MapColumns )
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
}


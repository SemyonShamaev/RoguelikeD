using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    public float speed;
    public int stepCount;

    public Vector3 stepPoint;
    Vector2 point;

    public bool isMoving = false;

    public Sprite[] sprites = new Sprite[4];
    private RoomGenerator RoomGenerator;
    private Camera cam;
    Animator anim;
    

    public void OnPointerDown(PointerEventData eventData) //вызывается когда мышь нажата 
    { 

    } 
    public void OnPointerUp(PointerEventData eventData) //вызывается когда мышь отпущена
    {   
        point = cam.ScreenToWorldPoint(new Vector3((int)Mathf.Round(eventData.position.x), (int)Mathf.Round(eventData.position.y), 0)); //из локальных координат в мировые
        point = new Vector2((int)Mathf.Round(point.x), (int)Mathf.Round(point.y));

        if(RoomGenerator.tiles[(int)point.x][(int)point.y] == RoomGenerator.TileType.Object && 
           RoomGenerator.tiles[(int)point.x][(int)point.y] == RoomGenerator.TileType.Enemy && 
           RoomGenerator.tiles[(int)point.x][(int)point.y] == RoomGenerator.TileType.Wall)
               (point.x, point.y) = FindPlace((int)point.x, (int)point.y, (int)transform.position.x, (int)transform.position.y); //если было нажато на препятствие то ищем ближайшее свободное место

        isMoving = true; //запуск движения
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
        stepPoint = transform.position;
    }

    void Update()
    {   
        if (isMoving)
        {
            float step = speed * Time.deltaTime;

            if(transform.position == stepPoint)
            {
                (stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y, (int)Mathf.Round(point.x), (int)Mathf.Round(point.y)); 
            }

            ChangeSprite();

            transform.position = Vector2.MoveTowards(transform.position, stepPoint, step);  
            cam.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
        
            if (transform.position.x == (int)point.x && transform.position.y == (int)point.y)
                isMoving = false;    
        }
    }

    void Awake()
    {
        RoomGenerator = GameObject.Find("Map").GetComponent<RoomGenerator>();
    }




    (int a, int b) FindWave(int startX, int startY, int targetX, int targetY) //Волновой алгоритм
    {
        int x, y,step=0;
        int stepX = 0, stepY = 0;
        int[,] cMap = new int[RoomGenerator.MapColumns, RoomGenerator.MapRows];

        for (x = 0; x < RoomGenerator.MapColumns; x++) //заполнение массива числами
            for (y = 0; y < RoomGenerator.MapRows; y++)
            {
                if (RoomGenerator.tiles[x][y] != RoomGenerator.TileType.Floor && 
                    RoomGenerator.tiles[x][y] != RoomGenerator.TileType.CorridorFloor && 
                    RoomGenerator.tiles[x][y] != RoomGenerator.TileType.End)
                    cMap[x, y] = -2; //есть препятствие
                else
                    cMap[x, y] = -1; //путь свободен
            }

        if(cMap[targetX, targetY] == -2) //если конечная точка это препятствие то возвращает стартовую точку
        {
            isMoving = false;
            return (startX, startY);
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
                        
                            if (x + 1 < RoomGenerator.MapColumns)
                            if (cMap[x + 1, y] == -1)
                                cMap[x + 1, y] = step + 1;

                            if (y + 1 < RoomGenerator.MapRows)
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
        if (x + 1 < RoomGenerator.MapRows)
            if (cMap[x + 1, y] < step && cMap[x + 1, y] >= 0)
            {
                step = cMap[x + 1, y];
                stepX = x + 1;
                stepY = y;
                return (stepX,stepY);
            }                
        if (y + 1 < RoomGenerator.MapColumns )
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

    void Punch(int x, int y)
    {
       
    }

    (int a, int b) FindPlace(int x, int y, int sx, int sy)
    {
        if((sx == x - 1 && sy == y) || (sx == x + 1 && sy == y) || (sx == x && sy == y - 1) || (sx == x && sy == y + 1))
            return (x,y);
        if((RoomGenerator.tiles[x+1][y]==RoomGenerator.TileType.Floor || RoomGenerator.tiles[x+1][y]==RoomGenerator.TileType.CorridorFloor) && sx>x)
            return (x+1,y);
        if((RoomGenerator.tiles[x-1][y]==RoomGenerator.TileType.Floor || RoomGenerator.tiles[x-1][y]==RoomGenerator.TileType.CorridorFloor) && sx<x)
            return (x-1,y);
        if((RoomGenerator.tiles[x][y+1]==RoomGenerator.TileType.Floor || RoomGenerator.tiles[x][y+1]==RoomGenerator.TileType.CorridorFloor) && sy>y)
            return (x,y+1);
        if((RoomGenerator.tiles[x][y-1]==RoomGenerator.TileType.Floor || RoomGenerator.tiles[x][y-1]==RoomGenerator.TileType.CorridorFloor) && sy<y)
            return (x,y-1);
        if(RoomGenerator.tiles[x+1][y]==RoomGenerator.TileType.Floor || RoomGenerator.tiles[x+1][y]==RoomGenerator.TileType.CorridorFloor)
            return (x+1,y);
        if(RoomGenerator.tiles[x-1][y]==RoomGenerator.TileType.Floor || RoomGenerator.tiles[x-1][y]==RoomGenerator.TileType.CorridorFloor)
            return (x-1,y);
        if(RoomGenerator.tiles[x][y+1]==RoomGenerator.TileType.Floor || RoomGenerator.tiles[x][y+1]==RoomGenerator.TileType.CorridorFloor)
            return (x,y+1);
        if(RoomGenerator.tiles[x][y-1]==RoomGenerator.TileType.Floor || RoomGenerator.tiles[x][y-1]==RoomGenerator.TileType.CorridorFloor)
            return (x,y-1);
        return((int)transform.position.x, (int)transform.position.y);
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

    public void getDamage()
    {
        anim.Play("GetDamage");
    }
}


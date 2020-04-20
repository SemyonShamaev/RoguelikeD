using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MotionController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
	public float speed;

    public Vector3 stepPoint;
	Vector2 point;

    public bool isMoving = false;

    public GameObject Player;
    public GameObject Map;
    public Sprite[] sprites = new Sprite[4];
    private RoomGenerator RoomGenerator;
    private Camera cam;
  	
    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData)
    {   
        point = cam.ScreenToWorldPoint(new Vector3((int)Mathf.Round(eventData.position.x), (int)Mathf.Round(eventData.position.y), 0));
        point = new Vector2((int)Mathf.Round(point.x), (int)Mathf.Round(point.y));

        if(RoomGenerator.tiles[(int)point.x][(int)point.y] != RoomGenerator.TileType.Floor && 
            RoomGenerator.tiles[(int)point.x][(int)point.y] != RoomGenerator.TileType.CorridorFloor && 
            RoomGenerator.tiles[(int)point.x][(int)point.y] != RoomGenerator.TileType.End)
            {
                (point.x, point.y) = CheckFloor((int)point.x, (int)point.y, (int)transform.position.x, (int)transform.position.y);
            }
        isMoving = true;
    }

	void Start()
	{
		cam = Camera.main;
		stepPoint = transform.position;
	}

    void Update()
    {	
        if (isMoving)
        {
            float step = speed * Time.deltaTime;

			if(transform.position == stepPoint)
                (stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y, (int)Mathf.Round(point.x), (int)Mathf.Round(point.y)); 

            ChangeSprite();
            transform.position = Vector2.MoveTowards(transform.position, stepPoint, step);  
            cam.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
        
            if (transform.position.x == (int)point.x && transform.position.y == (int)point.y)
                isMoving = false;    
        }
    }

    void Awake()
    {
        RoomGenerator = Map.GetComponent<RoomGenerator>();
    }




    (int a, int b) FindWave(int startX, int startY, int targetX, int targetY)
    {
        int x, y,step=0;
        int stepX = 0, stepY = 0;
        int[,] cMap = new int[RoomGenerator.MapColumns, RoomGenerator.MapRows];

        for (x = 0; x < RoomGenerator.MapColumns; x++)
            for (y = 0; y < RoomGenerator.MapRows; y++)
            {
                if (RoomGenerator.tiles[x][y] != RoomGenerator.TileType.Floor && RoomGenerator.tiles[x][y] != RoomGenerator.TileType.CorridorFloor && RoomGenerator.tiles[x][y] != RoomGenerator.TileType.End)
                    cMap[x, y] = -2;
                else
                    cMap[x, y] = -1;
            }

        if(cMap[targetX, targetY] == -2)
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
						
                            if (x + 1 < RoomGenerator.MapColumns)
							if (cMap[x + 1, y] == -1)
                                cMap[x + 1, y] = step + 1;

                            if (y + 1 < RoomGenerator.MapRows)
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


	
				if (x - 1 >= 0 )
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
        for(int i = 0; i < RoomGenerator.enemies.Length; i++)
        {
            if((int)RoomGenerator.enemies[i].enemy.transform.position.x == x && (int)RoomGenerator.enemies[i].enemy.transform.position.y == y)
            {
                RoomGenerator.enemies[i].getDamage(1);
                if(RoomGenerator.enemies[i].lifes <= 0)
                {
                    RoomGenerator.enemies[i].enemy.SetActive(false);
                    RoomGenerator.tiles[x][y] = RoomGenerator.TileType.Floor; 
                }
            }
        }
    }

    (int a, int b) CheckFloor(int x, int y, int sx, int sy)
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
            Player.GetComponent<SpriteRenderer>().sprite = sprites[0];
        else if(stepPoint.x < (int)transform.position.x)
            Player.GetComponent<SpriteRenderer>().sprite = sprites[1];
        else if(stepPoint.y > (int)transform.position.y)
            Player.GetComponent<SpriteRenderer>().sprite = sprites[2];
        else if(stepPoint.y < (int)transform.position.y)
            Player.GetComponent<SpriteRenderer>().sprite = sprites[3];

    }
}


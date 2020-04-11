using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MotionController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
	public float speed;

	Vector2 point;
	Vector3 stepPoint;
    bool isMoving = false;

    public GameObject Player;
    public GameObject Map;
    public Sprite[] sprites = new Sprite[4];

    private RoomGenerator RoomGenerator;
    private Camera cam;
  	
    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {    
        point.x = eventData.position.x;
        point.y = eventData.position.y;
        point = cam.ScreenToWorldPoint(new Vector3((int)point.x, (int)point.y, 0));
        isMoving = true;
    }

	void Start()
	{
		cam = Camera.main;
		stepPoint = transform.position;
	}

  	void Awake()
  	{
  		RoomGenerator = Map.GetComponent<RoomGenerator>();
  	}

    void Update()
    {
    	float step = speed * Time.deltaTime;
        if (isMoving)
        {
			if(transform.position == stepPoint) 
                (stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y, (int)Mathf.Round(point.x), (int)Mathf.Round(point.y)); 
            
            if(stepPoint.x > (int)transform.position.x)
                Player.GetComponent<SpriteRenderer>().sprite = sprites[0];
            else if(stepPoint.x < (int)transform.position.x)
                Player.GetComponent<SpriteRenderer>().sprite = sprites[1];
            else if(stepPoint.y > (int)transform.position.y)
                Player.GetComponent<SpriteRenderer>().sprite = sprites[2];
            else if(stepPoint.y < (int)transform.position.y)
                Player.GetComponent<SpriteRenderer>().sprite = sprites[3];

            transform.position = Vector2.MoveTowards(transform.position, stepPoint, step);  
            
            if(isMoving)
                cam.transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
            if (transform.position.x == (int)point.x && transform.position.y == (int)point.y) 
                isMoving = false;     
        }
    }

    public (int a, int b) FindWave(int startX, int startY, int targetX, int targetY)
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

        if(cMap[targetX, targetY] == -2){
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


		while (x != targetX || y != targetY)
		{
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
			}
		isMoving = false;
        return (startX,startY);
    }
}


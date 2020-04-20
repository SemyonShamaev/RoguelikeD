using UnityEngine;

public class Enemy : MonoBehaviour
{
	public GameObject enemy;
	public float speed;
	public int lifes;

	Vector3 stepPoint;
	Vector3 playerPosition;
	
	GameObject Map;
	RoomGenerator RoomGenerator;
	MotionController MotionController;

    void Start()
    {   
    	stepPoint = transform.position;
    	playerPosition = GameObject.Find("Player").transform.position;
    }

    void Update()
    {
    	float step = speed * Time.deltaTime;
 
    	if(checkPlayer() && MotionController.isMoving)
    	{

			if(transform.position == stepPoint)
			{
				playerPosition = MotionController.stepPoint;
       		    (stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y,  (int)Mathf.Round(playerPosition.x),  (int)Mathf.Round(playerPosition.y));   
       		    
       		    if(stepPoint == playerPosition)
       		    {
       		    	Punch();
       		    	stepPoint = transform.position;
       		    }
       		    else if(RoomGenerator.tiles[(int)stepPoint.x][(int)stepPoint.y] == RoomGenerator.TileType.Enemy)
       		    {
       		    	stepPoint = transform.position;
       		    }
       		    else
       		    {
       		    	RoomGenerator.tiles[(int)transform.position.x][(int)transform.position.y] = RoomGenerator.TileType.Floor;
       		    	RoomGenerator.tiles[(int)stepPoint.x][(int)stepPoint.y] = RoomGenerator.TileType.Enemy;	
       		    }	   
			}
    	}
    	transform.position = Vector2.MoveTowards(transform.position, stepPoint, step); 
    }

    void Awake()
    {
    	RoomGenerator = GameObject.Find("Map").GetComponent<RoomGenerator>();
    	MotionController = GameObject.Find("Player").GetComponent<MotionController>();
    }

    bool checkPlayer()
    {
    	if(GameObject.Find("Player").transform.position.x < transform.position.x + 6 && 
    	   GameObject.Find("Player").transform.position.x > transform.position.x - 6 &&
    	   GameObject.Find("Player").transform.position.y < transform.position.y + 6 &&
    	   GameObject.Find("Player").transform.position.y > transform.position.y - 6)
    		return true;
    	else 
    		return false;
    }

    void Punch()
    {

    }

    (int a, int b) FindWave(int startX, int startY, int targetX, int targetY)
    {
        int x, y,step=0;
        int stepX = 0, stepY = 0;
        int[,] cMap = new int[RoomGenerator.MapColumns, RoomGenerator.MapRows];

        for (x = 0; x < RoomGenerator.MapColumns; x++)
            for (y = 0; y < RoomGenerator.MapRows; y++)
            {
                if (RoomGenerator.tiles[x][y] != RoomGenerator.TileType.Floor && RoomGenerator.tiles[x][y] != RoomGenerator.TileType.CorridorFloor && RoomGenerator.tiles[x][y] != RoomGenerator.TileType.End  && RoomGenerator.tiles[x][y] != RoomGenerator.TileType.Enemy)
                    cMap[x, y] = -2;
                else
                    cMap[x, y] = -1;
            }

        if(cMap[targetX, targetY] == -2)
        {
        	return (startX, startY);
        }

        cMap[targetX,targetY]=0;

        while (true)
        {
            for (x = startX - 6; x < startX + 6; x++)
                for (y = startY - 6; y < startY + 6; y++)
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
            if (step > 20*20)
				return (startX, startY);
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
        return (startX,startY);
    }


    public void getDamage(int l)
    {
    	lifes -= l;
    }


}


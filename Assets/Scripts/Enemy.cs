using UnityEngine;

public class Enemy : MonoBehaviour
{
	public GameObject enemy;
	public float speed;
	public int lifes;
	public bool isPunch;
	Vector3 stepPoint;
	Vector3 playerPosition;

	Map Map;
	Player Player;
	Animation anim;

	void Start()
	{
		anim = gameObject.GetComponent<Animation>();
		anim["GetDamage"].layer = 123;
	}

    void Update()
    {
    	if(lifes <= 0)
    		Death();
 
 		else if(CheckPlayerNearby())
 			Punch();

    	else if(checkPlayerSees() && Player.isMoving)
			Move();

		float step = speed * Time.deltaTime;
    	transform.position = Vector2.MoveTowards(transform.position, stepPoint, step); 	
    }

    void Awake()
    {
    	Player = GameObject.Find("Player").GetComponent<Player>();
    	Map = GameObject.Find("Map").GetComponent<Map>();
    	stepPoint = transform.position;
    }

    void Move()
    {
    	if(transform.position == stepPoint)
		{
			playerPosition = Player.stepPoint;
       		(stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y,  (int)Mathf.Round(playerPosition.x),  (int)Mathf.Round(playerPosition.y));   
       		    
       		if(stepPoint == playerPosition)
       		    stepPoint = transform.position;
       		else
       		{
       		  	Map.tiles[(int)transform.position.x][(int)transform.position.y] = Map.TileType.Floor;
       		    Map.tiles[(int)stepPoint.x][(int)stepPoint.y] = Map.TileType.Enemy;	
       		}	   
		}

    }

    void Death()
    {
    	enemy.SetActive(false); 
    	Map.tiles[(int)enemy.transform.position.x][(int)enemy.transform.position.y] = Map.TileType.Floor;
    }

    bool checkPlayerSees()
    {
    	if(Player.transform.position.x < transform.position.x + 3 && 
    	   Player.transform.position.x > transform.position.x - 3 &&
    	   Player.transform.position.y < transform.position.y + 3 &&
    	   Player.transform.position.y > transform.position.y - 3)
    		return true;
    	else 
    		return false;
    }

    bool CheckPlayerNearby()
    {
    	if((Player.stepPoint.x == transform.position.x + 1 && Player.stepPoint.y == transform.position.y + 1) ||
    	   (Player.stepPoint.x == transform.position.x + 1 && Player.stepPoint.y == transform.position.y - 1) ||
    	   (Player.stepPoint.x == transform.position.x + 1 && Player.stepPoint.y == transform.position.y) ||
    	   (Player.stepPoint.x == transform.position.x - 1 && Player.stepPoint.y == transform.position.y + 1) ||
    	   (Player.stepPoint.x == transform.position.x - 1 && Player.stepPoint.y == transform.position.y - 1) ||
    	   (Player.stepPoint.x == transform.position.x - 1 && Player.stepPoint.y == transform.position.y) ||
    	   (Player.stepPoint.x == transform.position.x && Player.stepPoint.y == transform.position.y + 1) ||
    	   (Player.stepPoint.x == transform.position.x && Player.stepPoint.y == transform.position.y - 1))
    		return true;
    	else
    		return false;
    }

    void startAnimation()
    {

    }

    void endAnimation()
    {

    }

    void Punch()
    {
    	if(isPunch)
    	{
    		Player.GetDamage(1);
    		isPunch = false;;
    	}
    }

    (int a, int b) FindWave(int startX, int startY, int targetX, int targetY)
    {
        int x, y,step=0;
        int stepX = 0, stepY = 0;
        int[,] cMap = new int[Map.MapColumns, Map.MapRows];

        for (x = 0; x < Map.MapColumns; x++)
            for (y = 0; y < Map.MapRows; y++)
            {
                if (Map.tiles[x][y] != Map.TileType.Floor && Map.tiles[x][y] != Map.TileType.CorridorFloor && Map.tiles[x][y] != Map.TileType.End)
                    cMap[x, y] = -2;
                else
                    cMap[x, y] = -1;
            }

        cMap[startX, startY] = -1;
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
						
                        if (x + 1 < Map.MapColumns)
							if (cMap[x + 1, y] == -1)
                                cMap[x + 1, y] = step + 1;
						
                        if (y + 1 < Map.MapRows)
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
        return (startX,startY);
    }


    public void getDamage(int l)
    {
    	anim.Play("GetDamage");
    	lifes -= l;
    }
}


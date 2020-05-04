using UnityEngine;

public class Enemy : MonoBehaviour
{
	public int lifes;
	public float speed;

	public GameObject enemy;
    public GameObject drop;

	
	public bool isPunch;
	public bool isStep = false;
	
	private Vector3 stepPoint;
	private Vector3 playerPosition;

	private	Animation anim;

	void Start()
	{
		anim = gameObject.GetComponent<Animation>();
		stepPoint = transform.position;
	}

    void Update()
    {
 		if(CheckPlayerNearby())
 			Punch();

    	else if(checkPlayerSees() && (Player.Instance.isMoving || isStep))
			Move();

		float step = speed * Time.deltaTime;
    	transform.position = Vector2.MoveTowards(transform.position, stepPoint, step); 	
    }

    void Move()
    {
    	if(transform.position == stepPoint)
		{
			playerPosition = Player.Instance.stepPoint;
       		(stepPoint.x,stepPoint.y) = FindWave((int)transform.position.x, (int)transform.position.y,  (int)Mathf.Round(playerPosition.x),  (int)Mathf.Round(playerPosition.y));   
       		    
       		if(stepPoint == playerPosition)
       		    stepPoint = transform.position;
       		else
       		{
       		  	Generator.Instance.tiles[(int)transform.position.x][(int)transform.position.y] = Generator.TileType.Floor;
       		    Generator.Instance.tiles[(int)stepPoint.x][(int)stepPoint.y] = Generator.TileType.Enemy;	
       		}	   
		}

		isStep = false;
    }

    void Death()
    {
    	enemy.SetActive(false); 
        GameObject dropEnemy = Instantiate(drop, transform.position, Quaternion.identity) as GameObject;
       
        dropEnemy.transform.SetParent(GameManager.Instance.transform, false);
        dropEnemy.transform.position = transform.position;
       
    	Generator.Instance.tiles[(int)enemy.transform.position.x][(int)enemy.transform.position.y] = Generator.TileType.Drop;
    }

    bool checkPlayerSees()
    {
    	if(Player.Instance.transform.position.x < transform.position.x + 3 && 
    	   Player.Instance.transform.position.x > transform.position.x - 3 &&
    	   Player.Instance.transform.position.y < transform.position.y + 3 &&
    	   Player.Instance.transform.position.y > transform.position.y - 3)
    		return true;
    	else 
    		return false;
    }

    bool CheckPlayerNearby()
    {
    	if((Player.Instance.stepPoint.x == transform.position.x + 1 && Player.Instance.stepPoint.y == transform.position.y + 1) ||
    	   (Player.Instance.stepPoint.x == transform.position.x + 1 && Player.Instance.stepPoint.y == transform.position.y - 1) ||
    	   (Player.Instance.stepPoint.x == transform.position.x + 1 && Player.Instance.stepPoint.y == transform.position.y) ||
    	   (Player.Instance.stepPoint.x == transform.position.x - 1 && Player.Instance.stepPoint.y == transform.position.y + 1) ||
    	   (Player.Instance.stepPoint.x == transform.position.x - 1 && Player.Instance.stepPoint.y == transform.position.y - 1) ||
    	   (Player.Instance.stepPoint.x == transform.position.x - 1 && Player.Instance.stepPoint.y == transform.position.y) ||
    	   (Player.Instance.stepPoint.x == transform.position.x && Player.Instance.stepPoint.y == transform.position.y + 1) ||
    	   (Player.Instance.stepPoint.x == transform.position.x && Player.Instance.stepPoint.y == transform.position.y - 1))
    		return true;
    	else
    		return false;
    }

    void Punch()
    {
    	if(isPunch)
    	{
    		Player.Instance.GetDamage(1);
    		isPunch = false;;
    	}
    }

    (int a, int b) FindWave(int startX, int startY, int targetX, int targetY)
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
        return (startX,startY);
    }

    public void getDamage(int l)
    {
    	anim.Play("GetDamage");
    	lifes -= l;
        if(lifes <= 0)
        {
            Death();
        }
    }

    void startAnimation()
    {

    }

    void endAnimation()
    {

    }
}

	
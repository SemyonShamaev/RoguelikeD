using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{

    public enum TileType 
    {
        Empty, Wall, Floor, CorridorFloor, End, Start, Object, Enemy
    }

	public int MapColumns; 
    public int MapRows; 
    public int roomWidth; 
    public int roomHeight; 
    public int corridorLength; 

    public GameObject[] floorTiles; 
    public GameObject[] wallTiles; 
    public GameObject[] containersTiles;
    public GameObject[] endTiles;
    public GameObject[] enemyTiles;

    private Room[] rooms; 
    private Corridor[] corridors;
    private Room EndingRoom;
    private ButtonCreator Btn;

    public GameObject[] enemies;
    public TileType[][] tiles;

    private List<Vector3> gridPositions = new List<Vector3>();
    private List<Vector3> enemyPositions = new List<Vector3>();
    private Vector3 endPosition;

    void Update()
    {
        if(GameObject.Find("Player").transform.position == endPosition)
           SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Start()
    {
        addTilesMap();
        addRoomsAndCorridors();
        addInstance();
        addObjectsOnMap(containersTiles,20, 30);
        addEnemyOnMap(enemyTiles, 10, 20);
    }  

    void addTilesMap()
    {
        tiles = new TileType[MapColumns][];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[MapRows];
        }
    }

    void addRoomsAndCorridors()
    {
        rooms = new Room[Random.Range(8,10)]; 
        corridors = new Corridor[rooms.Length - 1];

        rooms[0] = new Room();
        corridors[0] = new Corridor();
        rooms[0].CreateRoom(roomWidth, roomHeight, MapColumns, MapRows);
        corridors[0].CreateCorridor(rooms[0], corridorLength, roomWidth, roomHeight, MapColumns, MapRows, true);

        for (int i = 1; i < rooms.Length; i++)
        {
            roomWidth = Random.Range(4,6);
            roomHeight = Random.Range(4,6);

            rooms[i] = new Room();
            rooms[i].CreateRoom(roomWidth, roomHeight, MapColumns, MapRows, corridors[i - 1]);

            if (i < corridors.Length) 
            {
                corridors[i] = new Corridor();
                corridorLength = Random.Range(8, 10);
                corridors[i].CreateCorridor(rooms[i], corridorLength, roomWidth, roomHeight, MapColumns, MapRows, false);
            }
            if(CheckCollision(i))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        for (int i = 0; i < corridors.Length; i++) 
        {
            Corridor currentCorridor = corridors[i];
            for (int j = 0; j < currentCorridor.corridorLength; j++) 
            {
                int xPos = currentCorridor.startXPos;
                int yPos = currentCorridor.startYPos;

                switch (currentCorridor.direction) 
                { 
                    case Direction.North:
                        yPos += j;
                        break;
                    case Direction.East:
                        xPos += j;
                        break;
                    case Direction.South:
                        yPos -= j;
                        break;
                    case Direction.West:
                        xPos -= j;
                        break;
                }
                tiles[xPos][yPos] = TileType.CorridorFloor;
                enemyPositions.Add(new Vector3(xPos, yPos));
            }
        }

        for (int i = 0; i < rooms.Length; i++) 
        {
            Room currentRoom = rooms[i];
 
            for (int j = 0; j < currentRoom.roomWidth; j++) 
            {
                int xPos = currentRoom.xPos + j;
                for (int k = 0; k < currentRoom.roomHeight; k++) 
                {
                    int yPos = currentRoom.yPos + k; 
                    tiles[xPos][yPos] = TileType.Floor;

                    if ((k == 0 || j == 0 || k == roomHeight || j == roomWidth) && CheckCorridorFloor(xPos, yPos))
                        gridPositions.Add(new Vector3(xPos, yPos));
                    else if (i > 0)
                        enemyPositions.Add(new Vector3(xPos, yPos));
                }
            } 

            if (i == rooms.Length - 1) 
                tiles[rooms[i].xPos + rooms[i].roomHeight/2][rooms[i].yPos + rooms[i].roomWidth/2] = TileType.End;
        }   


    }

    void addInstance()
    {
         for (int i = 1; i < tiles.Length - 1; i++)
        {
            for (int j = 1; j < tiles[i].Length - 1; j++)
            {
                if (tiles[i][j] != TileType.Floor && tiles[i][j] != TileType.CorridorFloor  && tiles[i][j] != TileType.End && CheckFloor(i,j))
                    tiles[i][j] = TileType.Wall;
            }
        }
        for (int i = 1; i < tiles.Length - 1; i++)
        {
            for (int j = 1; j < tiles[i].Length - 1; j++)
            {
                if (tiles[i][j] == TileType.End)
                {
                    endPosition = new Vector3(i, j, 0f);
                    GameObject endInstance = Instantiate(endTiles[0], endPosition, Quaternion.identity) as GameObject;
                    endInstance.transform.parent = transform;
                }
                else if (tiles[i][j] == TileType.Wall)
                {
                    Vector3 positionWall = new Vector3(i, j, 0f);
                    GameObject wallInstance = Instantiate(wallTiles[SelectTileWall(i, j)], positionWall, Quaternion.identity) as GameObject;
                    wallInstance.transform.parent = transform;
                }
                else if (tiles[i][j] == TileType.Floor || tiles[i][j] == TileType.CorridorFloor)
                {
                    Vector3 positionFloor = new Vector3(i, j, 0f);
                    GameObject floorInstance = Instantiate(floorTiles[0], positionFloor, Quaternion.identity) as GameObject;
                    floorInstance.transform.parent = transform;
                }    
            }
        }

    }

    void addObjectsOnMap(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            int randomIndex = Random.Range(0, gridPositions.Count);
            Vector3 randomPosition = gridPositions[randomIndex];
            gridPositions.RemoveAt(randomIndex);
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            GameObject tileCreated = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            tileCreated.transform.parent = transform;
            tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.Object; 
        }
    }

    void addEnemyOnMap(GameObject[] tileArray, int minimum, int maximum)
    {
        enemies = new GameObject[Random.Range(minimum, maximum + 1)]; 

        for (int i = 0; i <  enemies.Length; i++)
        {
            int randomIndex = Random.Range(0, enemyPositions.Count);
            Vector3 randomPosition = enemyPositions[randomIndex];
            enemyPositions.RemoveAt(randomIndex);
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            enemies[i] = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            enemies[i].transform.parent = transform;
            tiles[Mathf.RoundToInt(enemies[i].transform.position.x)][Mathf.RoundToInt(enemies[i].transform.position.y)] = TileType.Enemy;
        }
    }

    bool CheckFloor(int x, int y)
    {
        return tiles[x+1][y]==TileType.Floor || 
               tiles[x-1][y]==TileType.Floor || 
               tiles[x][y+1]==TileType.Floor || 
               tiles[x][y-1]==TileType.Floor || 
               tiles[x+1][y+1]==TileType.Floor || 
               tiles[x-1][y-1]==TileType.Floor || 
               tiles[x+1][y-1]==TileType.Floor || 
               tiles[x-1][y+1]==TileType.Floor || 
               tiles[x+1][y]==TileType.CorridorFloor || 
               tiles[x-1][y]==TileType.CorridorFloor || 
               tiles[x][y+1]==TileType.CorridorFloor || 
               tiles[x][y-1]==TileType.CorridorFloor || 
               tiles[x+1][y+1]==TileType.CorridorFloor || 
               tiles[x-1][y-1]==TileType.CorridorFloor || 
               tiles[x+1][y-1]==TileType.CorridorFloor || 
               tiles[x-1][y+1]==TileType.CorridorFloor;      
    }

    bool CheckCorridorFloor(int xPos, int yPos)
    {
        return tiles[xPos+1][yPos]!=TileType.CorridorFloor &&
               tiles[xPos][yPos-1]!=TileType.CorridorFloor &&
               tiles[xPos-1][yPos]!=TileType.CorridorFloor &&
               tiles[xPos][yPos+1]!=TileType.CorridorFloor &&
               tiles[xPos-2][yPos]!=TileType.CorridorFloor &&
               tiles[xPos+2][yPos]!=TileType.CorridorFloor &&
               tiles[xPos][yPos-2]!=TileType.CorridorFloor &&
               tiles[xPos][yPos+2]!=TileType.CorridorFloor &&
               tiles[xPos+1][yPos+1]!=TileType.CorridorFloor &&
               tiles[xPos-1][yPos-1]!=TileType.CorridorFloor;
    }

    bool CheckCollision(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if(rooms[i].xPos < rooms[n].xPos + rooms[n].roomWidth + 1 &&
               rooms[n].xPos < rooms[i].xPos + rooms[i].roomWidth + 1 &&
               rooms[i].yPos < rooms[n].yPos + rooms[n].roomHeight + 1 &&
               rooms[n].yPos < rooms[i].yPos + rooms[i].roomHeight + 1)
                    return true;
        }
        return false;
    }

    int SelectTileWall(int x, int y)
    {
        if(tiles[x - 1][y] == TileType.Wall && tiles[x + 1][y] == TileType.Wall && (tiles[x][y - 1] == TileType.Floor || tiles[x][y - 1] == TileType.CorridorFloor))
            return 0;
        if(tiles[x][y - 1] == TileType.Wall && tiles[x][y + 1] == TileType.Wall && (tiles[x - 1][y] == TileType.Floor || tiles[x - 1][y] == TileType.CorridorFloor))
            return 1;
        if(tiles[x][y - 1] == TileType.Wall && tiles[x][y + 1] == TileType.Wall && (tiles[x + 1][y] == TileType.Floor || tiles[x + 1][y] == TileType.CorridorFloor))
            return 2;
        if(tiles[x - 1][y] == TileType.Wall && tiles[x + 1][y] == TileType.Wall && (tiles[x][y + 1] == TileType.Floor || tiles[x][y + 1] == TileType.CorridorFloor))
            return 3;
        if(tiles[x + 1][y] == TileType.Wall && tiles[x][y - 1] == TileType.Wall && tiles[x - 1][y] != TileType.CorridorFloor && tiles[x][y - 1] != TileType.CorridorFloor && tiles[x][y + 1] != TileType.CorridorFloor)
            return 4;
        if(tiles[x - 1][y] == TileType.Wall && tiles[x][y - 1] == TileType.Wall && tiles[x + 1][y] != TileType.CorridorFloor && tiles[x][y + 1] != TileType.CorridorFloor)
            return 5;
        if(tiles[x + 1][y] == TileType.Wall && tiles[x][y + 1] == TileType.Wall && tiles[x][y + 1] != TileType.CorridorFloor && tiles[x - 1][y] != TileType.CorridorFloor && tiles[x][y - 1] != TileType.CorridorFloor)
            return 6;
        if(tiles[x - 1][y] == TileType.Wall && tiles[x][y + 1] == TileType.Wall && tiles[x][y - 1] != TileType.CorridorFloor && tiles[x + 1][y] != TileType.CorridorFloor)
            return 7;
        if(tiles[x + 1][y] == TileType.Wall && tiles[x][y - 1] == TileType.Wall)
            return 8;
        if(tiles[x - 1][y] == TileType.Wall && tiles[x][y - 1] == TileType.Wall)
            return 9;
        if(tiles[x + 1][y] == TileType.Wall && tiles[x][y + 1] == TileType.Wall)
            return 10;
        if(tiles[x - 1][y] == TileType.Wall && tiles[x][y + 1] == TileType.Wall)
            return 11;
        return 0;
    }
}

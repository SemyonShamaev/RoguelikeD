using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{

    public enum TileType 
    {
        Wall, Floor, CorridorFloor, End, Start, Object, Enemy
    }

	public int MapColumns; 
    public int MapRows; 

    public GameObject[] floorTiles; 
    public GameObject[] wallTiles; 
    public GameObject[] containersTiles;
    public GameObject[] endTiles;
    public GameObject[] enemyTiles;

    private Room[] rooms; 
    private Room EndingRoom;
    private Corridor[] corridors; 

    public int roomWidth; 
    public int roomHeight; 
    public int corridorLength; 

    public TileType[][] tiles;
    public GameObject Map; 
    public GameObject Player;
    private List<Vector3> gridPositions = new List<Vector3>();
    private List<Vector3> enemyPositions = new List<Vector3>();
    private Vector3 endPosition;

    int level = 1;

    void Update()
    {
        if(Player.transform.position == endPosition)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            ++level;
        }
    }

    private void Start()
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
        rooms = new Room[10]; 
        corridors = new Corridor[rooms.Length - 1];

        rooms[0] = new Room();
        corridors[0] = new Corridor();
        rooms[0].CreateRoom(roomWidth, roomHeight, MapColumns, MapRows);
        corridors[0].CreateCorridor(rooms[0], corridorLength, roomWidth, roomHeight,MapColumns, MapRows, true);

        for (int i = 1; i < rooms.Length; i++)
        {
            roomWidth = Random.Range(4,6);
            roomHeight = Random.Range(4,6);

            rooms[i] = new Room();
            rooms[i].CreateRoom(roomWidth, roomHeight, MapColumns, MapRows, corridors[i - 1]);

            if (i < corridors.Length) 
            {
                corridors[i] = new Corridor();
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

            if (i == rooms.Length - 1) 
                tiles[rooms[i].xPos + rooms[i].roomHeight/2][rooms[i].yPos + rooms[i].roomWidth/2] = TileType.End;

            for (int j = 0; j < currentRoom.roomWidth; j++) 
            {
                int xPos = currentRoom.xPos + j;
                for (int k = 0; k < currentRoom.roomHeight; k++) 
                {
                    int yPos = currentRoom.yPos + k; 
                    if(tiles[xPos][yPos]!= TileType.End)
                        tiles[xPos][yPos] = TileType.Floor;
                    if ((k == 0 || j == 0 || k == roomHeight || j == roomWidth) && CheckCorridorFloor(xPos, yPos))
                        gridPositions.Add(new Vector3(xPos, yPos));
                    else if (i > 0)
                        enemyPositions.Add(new Vector3(xPos, yPos));
                }
            }       
        }   
    }

    void addInstance()
    {
        for (int i = 1; i < tiles.Length - 1; i++)
        {
            for (int j = 1; j < tiles[i].Length - 1; j++)
            {
                if (tiles[i][j] == TileType.End)
                {
                    endPosition = new Vector3(i, j, 0f);
                    GameObject endInstance = Instantiate(endTiles[0], endPosition, Quaternion.identity) as GameObject;
                    endInstance.transform.parent = Map.transform;
                }
                else if (tiles[i][j] != TileType.Floor && tiles[i][j] != TileType.CorridorFloor  && tiles[i][j] != TileType.End && CheckFloor(i,j))
                {
                    Vector3 positionW = new Vector3(i, j, 0f);
                    GameObject wallInstance = Instantiate(wallTiles[0], positionW, Quaternion.identity) as GameObject;
                    wallInstance.transform.parent = Map.transform;
                }
                else if (tiles[i][j] == TileType.Floor || tiles[i][j] == TileType.CorridorFloor)
                {
                    Vector3 positionF = new Vector3(i, j, 0f);
                    GameObject floorInstance = Instantiate(floorTiles[0], positionF, Quaternion.identity) as GameObject;
                    floorInstance.transform.parent = Map.transform;
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
            tileCreated.transform.parent = Map.transform;
            tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.Object;
        }
    }

    void addEnemyOnMap(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            int randomIndex = Random.Range(0, enemyPositions.Count);
            Vector3 randomPosition = enemyPositions[randomIndex];
            enemyPositions.RemoveAt(randomIndex);
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            GameObject tileCreated = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            tileCreated.transform.parent = Map.transform;
            tiles[Mathf.RoundToInt(tileCreated.transform.position.x)][Mathf.RoundToInt(tileCreated.transform.position.y)] = TileType.Enemy;
        }
    }

    private bool CheckFloor(int x, int y)
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

    private bool CheckCorridorFloor(int xPos, int yPos)
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

    private bool CheckCollision(int n)
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
}

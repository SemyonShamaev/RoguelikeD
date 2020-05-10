using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rogue;

public class Generator : Singleton<Generator>
{
    public int MapRows; 
    public int MapColumns; 
    public int roomWidth; 
    public int roomHeight; 
    public int corridorLength; 
    public GameObject[] endTiles; 
    public GameObject[] wallTiles; 
    public GameObject[] enemyTiles;
    public GameObject[] floorTiles;
    public GameObject[] containersTiles;
    public GameObject[] enemies;
    public GameObject[] containers;
    public GameObject[] Invtr;
    public GameObject[] InvtrContainers;
    public GameObject Inventory;
    public TileType[][] tiles;
    public bool isEnd = true;

    private Room[] rooms; 
    private Corridor[] corridors;
    private Room EndingRoom;
    private List<Vector3> gridPositions = new List<Vector3>();
    private List<Vector3> enemyPositions = new List<Vector3>();
    private Vector3 endPosition;

    private void Update()
    {
        if(Player.Instance.transform.position == endPosition && isEnd)
        {
            GameManager.Instance.NewLevelMessage();
            isEnd = false;
        }

        if(Player.Instance.transform.position != endPosition && !isEnd)
            isEnd = true;
    }

    public void setupScene(int l)
    {
        gridPositions.Clear();
        enemyPositions.Clear();

        rooms = null;
        tiles = null;
        isEnd = true;
        enemies = null;
        Invtr = null;

        addTilesMap();
        addRoomsAndCorridors();
        addInstance();
        addPositions();
        addObjectsOnMap(containersTiles, 20, 30);
        addEnemyOnMap(enemyTiles, 2, 5);
    }

    private void addTilesMap()
    {
        tiles = new TileType[MapColumns][];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[MapRows];
        }

        tiles[MapColumns/2][MapRows/2] = TileType.Start;
    }

    private void addRoomsAndCorridors()
    {
        rooms = new Room[10]; 
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
                }
            } 

            if (i == rooms.Length - 1) 
                tiles[rooms[i].xPos + rooms[i].roomHeight/2][rooms[i].yPos + rooms[i].roomWidth/2] = TileType.End;
        }   


    }

    private void addInstance()
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

    private void addPositions()
    {
        for(int i = 0; i < MapColumns; i++)
        {
            for(int j =0; j < MapRows; j++)
            {
                if(tiles[i][j] == TileType.Floor && CheckWall(i, j) && CheckCorridorFloor(i, j))
                    gridPositions.Add(new Vector3(i, j));
                else if(tiles[i][j] == TileType.Floor || tiles[i][j] == TileType.CorridorFloor)
                    if(tiles[i][j] != TileType.End && tiles[i][j] != TileType.Start)
                        enemyPositions.Add(new Vector3(i,j));
            }
        }

    }

    private void addObjectsOnMap(GameObject[] tileArray, int minimum, int maximum)
    {
        containers = new GameObject[Random.Range(minimum, maximum + 1)];
        InvtrContainers = new GameObject[containers.Length];

        for (int i = 0; i < containers.Length; i++)
        {
            int randomIndex = Random.Range(0, gridPositions.Count);
            Vector3 randomPosition = gridPositions[randomIndex];
            gridPositions.RemoveAt(randomIndex);
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            containers[i] = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            containers[i].transform.parent = transform;
            tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.Object; 

            InvtrContainers[i] = Instantiate(Inventory, transform.position, Quaternion.identity) as GameObject;
            InvtrContainers[i].transform.SetParent(GameObject.Find("ContainersInventories").transform, false);
            InvtrContainers[i].transform.position = GameObject.Find("ContainersInventories").transform.position;
            
            InventoryEnemy InventoryContainers = InvtrContainers[i].GetComponentInChildren<InventoryEnemy>();
            
            int ItemId;
            for(int j = 0; j < InventoryContainers.maxCount; j++)
            {
                ItemId = Random.Range(0, 3);
                InventoryContainers.AddItem(j, DataBase.Instance.items[ItemId], Random.Range(1,5), DataBase.Instance.items[ItemId].type);
            } 
        }
    }

    private void addEnemyOnMap(GameObject[] tileArray, int minimum, int maximum)
    {
        enemies = new GameObject[Random.Range(minimum, maximum + 1)]; 
        Invtr = new GameObject[enemies.Length];
        for (int i = 0; i <  enemies.Length; i++)
        {
            int randomIndex = Random.Range(0, enemyPositions.Count);
            Vector3 randomPosition = enemyPositions[randomIndex];
            enemyPositions.RemoveAt(randomIndex);
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            enemies[i] = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            enemies[i].transform.parent = transform;
            tiles[Mathf.RoundToInt(enemies[i].transform.position.x)][Mathf.RoundToInt(enemies[i].transform.position.y)] = TileType.Enemy;

            Invtr[i] = Instantiate(Inventory, transform.position, Quaternion.identity) as GameObject;
            Invtr[i].transform.SetParent(GameObject.Find("EnemyInventories").transform, false);
            Invtr[i].transform.position = GameObject.Find("EnemyInventories").transform.position;
            
            InventoryEnemy invtrEnemy = Invtr[i].GetComponentInChildren<InventoryEnemy>();

            int ItemId;
            for(int j = 0; j < invtrEnemy.maxCount; j++)
            {
                ItemId = Random.Range(0, 3);
                invtrEnemy.AddItem(j, DataBase.Instance.items[ItemId], Random.Range(1,5), DataBase.Instance.items[ItemId].type);
            } 
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

    private bool CheckWall(int x, int y)
    {
        return tiles[x+1][y]==TileType.Wall || 
               tiles[x-1][y]==TileType.Wall || 
               tiles[x][y+1]==TileType.Wall || 
               tiles[x][y-1]==TileType.Wall || 
               tiles[x+1][y+1]==TileType.Wall || 
               tiles[x-1][y-1]==TileType.Wall || 
               tiles[x+1][y-1]==TileType.Wall || 
               tiles[x-1][y+1]==TileType.Wall;    
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
            if(rooms[i].xPos < rooms[n].xPos + rooms[n].roomWidth + 2 &&
               rooms[n].xPos < rooms[i].xPos + rooms[i].roomWidth + 2 &&
               rooms[i].yPos < rooms[n].yPos + rooms[n].roomHeight + 2 &&
               rooms[n].yPos < rooms[i].yPos + rooms[i].roomHeight + 2)
                    return true;
        }
        return false;
    }

    private int SelectTileWall(int x, int y)
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

    public enum TileType 
    {
        Empty, Wall, Floor, CorridorFloor, End, Start, Object, Enemy, Drop
    }
}
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using Rogue;

public class Generator : Singleton<Generator>
{
    public enum TileType 
    {
        Empty,
        Wall,
        Floor,
        CorridorFloor,
        End,
        Start,
        Object,
        Enemy,
        Drop,
        SellerPotion,
        SellerWeapon,
        SellerShield,
        SellerFood,
        SellerArmor,
        Chest,
        Sign,
        Well,
        Buyer
    };

    public TileType[][] tiles;

    public int MapRows; 
    public int MapColumns;
    public int roomsCount;
    public int roomWidth; 
    public int roomHeight; 
    public int corridorLength; 

    public GameObject[] endTiles; 
    public GameObject[] wallTiles; 
    public GameObject[] enemyTiles;
    public GameObject[] floorTiles;
    public GameObject[] containersTiles;
    public GameObject[] otherObjectsTiles;
    public GameObject[] enemies;
    public GameObject[] containers;
    public GameObject[] otherObjects;
    public GameObject[] Invtr;
    public GameObject[] InvtrContainers;
    public GameObject Inventory;
    public GameObject fillContainers;

    public int[] enemyTypes;
    public int[] otherObjectsType;

    private Room[] rooms; 
    private Corridor[] corridors;
    private Room EndingRoom;

    private List<Vector3> gridPositions = new List<Vector3>();
    public List<Vector3> enemyPositions = new List<Vector3>();
    private Vector3 endPosition;

    private int level;

    public void setupScene(int level)
    {
        this.level = level;
        roomsCount = level + 8;

        clearGeneratorData();
        addTilesMap();
        addRoomsAndCorridors();
        addWalls();
        addInstance();
        addPositions();

        addObjectsOnMap(containersTiles, roomsCount, roomsCount + 2);
        addEnemyOnMap(enemyTiles, roomsCount - 7, roomsCount - 5);
        addOtherObjectsOnMap(otherObjectsTiles);
    }

    private void addTilesMap()
    {
        tiles = new TileType[MapColumns][];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[MapRows];
        }
    }

    private void addRoomsAndCorridors()
    {
        rooms = new Room[roomsCount]; 
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
                tiles[rooms[i].xPos + rooms[i].roomHeight / 2][rooms[i].yPos + rooms[i].roomWidth / 2] = TileType.End;
        }

        tiles[0][0] = TileType.Start;
    }

    private void addWalls()
    {
        for (int i = 1; i < tiles.Length - 1; i++)
        {
            for (int j = 1; j < tiles[i].Length - 1; j++)
            {
                if (tiles[i][j] != TileType.Floor && tiles[i][j] != TileType.CorridorFloor && tiles[i][j] != TileType.End && CheckFloor(i, j))
                    tiles[i][j] = TileType.Wall;
            }
        }
    }

    private void addInstance()
    {
        int tileType = 0;

        if (level > 20)
            tileType = 1;

        for (int i = 1; i < tiles.Length - 1; i++)
        {
            for (int j = 1; j < tiles[i].Length - 1; j++)
            {
                if (tiles[i][j] == TileType.End && level != 25)
                {
                    endPosition = new Vector3(i, j, 0f);
                    GameObject endInstance = Instantiate(endTiles[0], endPosition, Quaternion.identity) as GameObject;
                    endInstance.transform.parent = transform;
                }
                else if (tiles[i][j] == TileType.Wall)
                {
                    Vector3 positionWall = new Vector3(i, j, 0f);
                    GameObject wallInstance = Instantiate(wallTiles[tileType], positionWall, Quaternion.identity) as GameObject;
                    wallInstance.transform.parent = transform;
                }
               
                else if(tiles[i][j] != TileType.Empty)
                {
                    Vector3 positionFloor = new Vector3(i, j, 0f);
                    GameObject floorInstance = Instantiate(floorTiles[tileType], positionFloor, Quaternion.identity) as GameObject;
                    floorInstance.transform.parent = transform;
                }

                if (level == 25 && tiles[i][j] == TileType.End)
                {
                    endPosition = new Vector3(i, j, 0f);
                    GameObject endInstance = Instantiate(endTiles[1], endPosition, Quaternion.identity) as GameObject;
                    endInstance.transform.parent = transform;
                    tiles[i][j] = TileType.Chest;
                }
            }
        }
    }

    private void addPositions()
    {
        for(int i = 0; i < MapColumns; i++)
        {
            for(int j = 0; j < MapRows; j++)
            {
                if (tiles[i][j] == TileType.Floor && CheckWall(i, j) && CheckCorridorFloor(i, j))
                    gridPositions.Add(new Vector3(i, j));

                else if (tiles[i][j] == TileType.Floor || tiles[i][j] == TileType.CorridorFloor)
                    if (tiles[i][j] != TileType.End && tiles[i][j] != TileType.Start)
                        enemyPositions.Add(new Vector3(i, j));
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
            InvtrContainers[i].transform.SetParent(fillContainers.transform, false);
            InvtrContainers[i].transform.position = fillContainers.transform.position;
            
            InventoryEnemy InventoryContainers = InvtrContainers[i].GetComponentInChildren<InventoryEnemy>();
            
            int ItemId;
            for(int j = 0; j < 3; j++)
            {
                ItemId = Random.Range(1, 4);
                InventoryContainers.AddItem(j, DataBase.Instance.items[ItemId], Random.Range(1,5), DataBase.Instance.items[ItemId].type);
            } 
        }
    }

    private void addOtherObjectsOnMap(GameObject[] tileArray)
    {
        otherObjects = new GameObject[roomsCount - 1];
        otherObjectsType = new int[otherObjects.Length];
        for (int i = 0; i < otherObjects.Length; i++)
        {
            int randomIndex = Random.Range(0, gridPositions.Count);
            Vector3 randomPosition = gridPositions[randomIndex];
            gridPositions.RemoveAt(randomIndex);

            otherObjectsType[i] = Random.Range(0, tileArray.Length);

            int random = Random.Range(0, 100);
            if(random > 30)
                otherObjectsType[i] = Random.Range(1, 3);


            if (otherObjectsType[i] == 0)
                tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.SellerPotion;
            else if(otherObjectsType[i] == 1)
                tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.Sign;
            else  if(otherObjectsType[i] == 2)
                tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.Well;
            else if (otherObjectsType[i] == 3)
                tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.Buyer;
            else if (otherObjectsType[i] == 4)
                tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.SellerWeapon;
            else  if (otherObjectsType[i] == 5)
                tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.SellerArmor;
            else if (otherObjectsType[i] == 6)
                tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.SellerShield;
            else if (otherObjectsType[i] == 7)
                tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.SellerFood;
            tiles[(int)randomPosition.x][(int)randomPosition.y] = TileType.Buyer;
            GameObject tileChoice = tileArray[otherObjectsType[i]];
            otherObjects[i] = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            otherObjects[i].transform.parent = transform;
        }
    }

    private void addEnemyOnMap(GameObject[] tileArray, int minimum, int maximum)
    {
        enemies = new GameObject[Random.Range(minimum, maximum + 1)]; 
        Invtr = new GameObject[enemies.Length];
        enemyTypes = new int[enemies.Length];

        for (int i = 0; i <  enemies.Length; i++)
        {
            int randomIndex = Random.Range(0, enemyPositions.Count);
            Vector3 randomPosition = enemyPositions[randomIndex];
            enemyPositions.RemoveAt(randomIndex);

            if (level < 5)
                enemyTypes[i] = Random.Range(0, 2);
            else if (level < 10)
                enemyTypes[i] = Random.Range(1, 3);
            else if(level < 15)
                enemyTypes[i] = Random.Range(2, 5);
            else if(level < 20)
                enemyTypes[i] = Random.Range(1, 7);
            else
                enemyTypes[i] = Random.Range(5, 7);

            GameObject tileChoice = tileArray[enemyTypes[i]];
            enemies[i] = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            enemies[i].transform.parent = transform;

            tiles[Mathf.RoundToInt(enemies[i].transform.position.x)][Mathf.RoundToInt(enemies[i].transform.position.y)] = TileType.Enemy;

            Invtr[i] = Instantiate(Inventory, transform.position, Quaternion.identity) as GameObject;
            Invtr[i].transform.SetParent(GameObject.Find("EnemyInventories").transform, false);
            Invtr[i].transform.position = GameObject.Find("EnemyInventories").transform.position;
            
            InventoryEnemy invtrEnemy = Invtr[i].GetComponentInChildren<InventoryEnemy>();

           
            if (enemyTypes[i] == 0)
            {
                invtrEnemy.AddItem(0, DataBase.Instance.items[4], 1, DataBase.Instance.items[4].type);
                invtrEnemy.AddItem(1, DataBase.Instance.items[9], 1, DataBase.Instance.items[9].type);
            }
            else if (enemyTypes[i] == 1)
            {
                invtrEnemy.AddItem(0, DataBase.Instance.items[4], 1, DataBase.Instance.items[4].type);
                invtrEnemy.AddItem(1, DataBase.Instance.items[9], 1, DataBase.Instance.items[9].type);
                invtrEnemy.AddItem(2, DataBase.Instance.items[18], 1, DataBase.Instance.items[18].type);
            }
            else if (enemyTypes[i] == 2)
            {
                invtrEnemy.AddItem(0, DataBase.Instance.items[4], 1, DataBase.Instance.items[4].type);
                invtrEnemy.AddItem(1, DataBase.Instance.items[9], 1, DataBase.Instance.items[9].type);
                invtrEnemy.AddItem(2, DataBase.Instance.items[19], 1, DataBase.Instance.items[19].type);
            }
            else if (enemyTypes[i] == 3)
            {
                invtrEnemy.AddItem(0, DataBase.Instance.items[Random.Range(14, 18)], 1, DataBase.Instance.items[14].type);
                invtrEnemy.AddItem(1, DataBase.Instance.items[Random.Range(14, 18)], 1, DataBase.Instance.items[14].type);
                invtrEnemy.AddItem(2, DataBase.Instance.items[Random.Range(14, 18)], 1, DataBase.Instance.items[14].type);
            }
            else if (enemyTypes[i] == 4)
            {
                invtrEnemy.AddItem(0, DataBase.Instance.items[14], 1, DataBase.Instance.items[14].type);
                invtrEnemy.AddItem(1, DataBase.Instance.items[11], 1, DataBase.Instance.items[11].type);
                invtrEnemy.AddItem(2, DataBase.Instance.items[1], 50, DataBase.Instance.items[1].type);
            }
            else if (enemyTypes[i] == 5)
            {
                invtrEnemy.AddItem(0, DataBase.Instance.items[Random.Range(14, 18)], 1, DataBase.Instance.items[14].type);
                invtrEnemy.AddItem(1, DataBase.Instance.items[Random.Range(14, 18)], 1, DataBase.Instance.items[14].type);
                invtrEnemy.AddItem(2, DataBase.Instance.items[Random.Range(14, 18)], 1, DataBase.Instance.items[14].type);
            }
            else if (enemyTypes[i] == 6)
            {
                invtrEnemy.AddItem(0, DataBase.Instance.items[7], 1, DataBase.Instance.items[7].type);
                invtrEnemy.AddItem(1, DataBase.Instance.items[11], 1, DataBase.Instance.items[11].type);
                invtrEnemy.AddItem(2, DataBase.Instance.items[21], 1, DataBase.Instance.items[21].type);
            }

            if (enemyTypes[i] != 1)
            {
                int ItemId;
                for (int j = 3; j < invtrEnemy.maxCount; j++)
                {
                    ItemId = Random.Range(0, 3);
                    invtrEnemy.AddItem(j, DataBase.Instance.items[ItemId], Random.Range(1, 5), DataBase.Instance.items[ItemId].type);
                }
            }
        }
    }

    private bool CheckFloor(int x, int y)
    {
        return tiles[x + 1][y] == TileType.Floor || 
               tiles[x - 1][y] == TileType.Floor || 
               tiles[x][y + 1] == TileType.Floor || 
               tiles[x][y - 1] == TileType.Floor || 
               tiles[x + 1][y + 1] == TileType.Floor || 
               tiles[x - 1][y - 1] == TileType.Floor || 
               tiles[x + 1][y - 1] == TileType.Floor || 
               tiles[x - 1][y + 1] == TileType.Floor || 
               tiles[x + 1][y] == TileType.CorridorFloor || 
               tiles[x - 1][y] == TileType.CorridorFloor || 
               tiles[x][y + 1] == TileType.CorridorFloor || 
               tiles[x][y - 1] == TileType.CorridorFloor || 
               tiles[x + 1][y + 1] == TileType.CorridorFloor || 
               tiles[x - 1][y - 1] == TileType.CorridorFloor || 
               tiles[x + 1][y - 1] == TileType.CorridorFloor || 
               tiles[x - 1][y + 1] == TileType.CorridorFloor;      
    }

    private bool CheckWall(int x, int y)
    {
        try
        {
            return tiles[x + 1][y] == TileType.Wall ||
                   tiles[x - 1][y] == TileType.Wall ||
                   tiles[x][y + 1] == TileType.Wall ||
                   tiles[x][y - 1] == TileType.Wall ||
                   tiles[x + 1][y + 1] == TileType.Wall ||
                   tiles[x - 1][y - 1] == TileType.Wall ||
                   tiles[x + 1][y - 1] == TileType.Wall ||
                   tiles[x - 1][y + 1] == TileType.Wall;
        }
        catch
        {
            return false;
        }
    }

    private bool CheckCorridorFloor(int x, int y)
    {
        try
        {
            return tiles[x + 1][y] != TileType.CorridorFloor &&
               tiles[x][y - 1] != TileType.CorridorFloor &&
               tiles[x - 1][y] != TileType.CorridorFloor &&
               tiles[x][y + 1] != TileType.CorridorFloor &&
               tiles[x - 2][y] != TileType.CorridorFloor &&
               tiles[x + 2][y] != TileType.CorridorFloor &&
               tiles[x][y - 2] != TileType.CorridorFloor &&
               tiles[x][y + 2] != TileType.CorridorFloor &&
               tiles[x + 1][y + 1] != TileType.CorridorFloor &&
               tiles[x - 1][y - 1] != TileType.CorridorFloor;
        }
        catch
        {
            return false;
        }
    }

    private void clearGeneratorData()
    {
        gridPositions.Clear();
        enemyPositions.Clear();

        enemyTypes = null;
        rooms = null;
        tiles = null;
        enemies = null;
        Invtr = null;
    }

    public void LoadData(List<Save.MapSaveData> save)
    {
        clearGeneratorData();
        GameManager.Instance.DestroyAllObjects();
        addTilesMap();
 
        int n = 0;
        for(int i = 0; i < MapColumns; i++)
        {
            for(int j = 0; j < MapRows; j++)
            {
                tiles[i][j] = (TileType)save[n].tiles;
                n++;
            }
        }

        addInstance();

        enemyTypes = save[0].enemyTypes;

        containers = new GameObject[save[0].containersCount];
        InvtrContainers = new GameObject[containers.Length];

        for (int i = 0; i < containers.Length; i++)
        {
            GameObject tileChoice = containersTiles[Random.Range(0, containersTiles.Length)];
            containers[i] = Instantiate(tileChoice, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            containers[i].transform.parent = transform;
            tiles[0][0] = TileType.Object; 

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

        enemies = new GameObject[save[0].enemyCount]; 
        Invtr = new GameObject[enemies.Length];
        for (int i = 0; i <  enemies.Length; i++)
        {
            GameObject tileChoice = enemyTiles[enemyTypes[i]];
            enemies[i] = Instantiate(tileChoice, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            enemies[i].transform.parent = transform;
            tiles[0][0] = TileType.Enemy;

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

        otherObjects = new GameObject[save[0].otherObjectsCount];
        otherObjectsType = save[0].otherObjectsType; 

        for (int i = 0; i < otherObjects.Length; i++)
        {
            GameObject tileChoice = otherObjectsTiles[otherObjectsType[i]];
            otherObjects[i] = Instantiate(tileChoice, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            otherObjects[i].transform.parent = transform;
        }
    }
}
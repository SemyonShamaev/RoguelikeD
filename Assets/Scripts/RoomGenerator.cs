using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public enum TileType 
    {
        Wall, Floor,
    }

	public int MapColumns; 
    public int MapRows; 

    public GameObject[] floorTiles; 
    public GameObject[] wallTiles; 
    public GameObject[] containersTiles;

    private Room[] rooms; 
    private Corridor[] corridors; 

    public int roomWidth; 
    public int roomHeight; 
    public int corridorLength; 

    public TileType[][] tiles;
    public GameObject Map; 
    private List<Vector3> gridPositions = new List<Vector3>();

    private void Start()
    {
        tiles = new TileType[MapColumns][];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[MapRows];
        }

        rooms = new Room[Random.Range(10,15)]; 
        corridors = new Corridor[rooms.Length - 1];
        rooms[0] = new Room();
        corridors[0] = new Corridor();
        rooms[0].CreateRoom(roomWidth, roomHeight, MapColumns, MapRows);
        corridors[0].CreateCorridor(rooms[0], corridorLength, roomWidth, roomHeight,MapColumns, MapRows, true);
       
        for (int i = 1; i < rooms.Length; i++)
        {
            roomWidth = Random.Range(6,10);
            roomHeight = Random.Range(6,10);
            rooms[i] = new Room();
            rooms[i].CreateRoom(roomWidth, roomHeight, MapColumns, MapRows, corridors[i - 1]);

            if (i < corridors.Length) 
            {
                corridors[i] = new Corridor();
                corridors[i].CreateCorridor(rooms[i], corridorLength, roomWidth, roomHeight, MapColumns, MapRows, false);
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
                tiles[xPos][yPos] = TileType.Floor;
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                if (tiles[i][j] != TileType.Floor && CheckFloor(i,j))
                {
                    Vector3 positionW = new Vector3(i, j, 0f);
                    GameObject wallInstance = Instantiate(wallTiles[0], positionW, Quaternion.identity) as GameObject;
                    wallInstance.transform.parent = Map.transform;
                }
                else if (tiles[i][j] == TileType.Floor)
                {
                    gridPositions.Add(new Vector3(i, j));
                    Vector3 positionF = new Vector3(i, j, 0f);
                    GameObject floorInstance = Instantiate(floorTiles[0], positionF, Quaternion.identity) as GameObject;
                    floorInstance.transform.parent = Map.transform;
                } 
            }
        }
       

    }  




    private bool CheckFloor(int x, int y)
    {
        try
        {
            if(tiles[x+1][y]==TileType.Floor || tiles[x-1][y]==TileType.Floor || tiles[x][y+1]==TileType.Floor || tiles[x][y-1]==TileType.Floor || tiles[x+1][y+1]==TileType.Floor || tiles[x-1][y-1]==TileType.Floor || tiles[x+1][y-1]==TileType.Floor || tiles[x-1][y+1]==TileType.Floor)
                return true;
            else
                return false;
        }
        catch
        {
            return false;
        }

    } 

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void AddObjectsOnMap(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            GameObject tileCreated = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            tileCreated.transform.parent = Map.transform;
        }
    }
}

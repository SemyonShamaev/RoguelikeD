using UnityEngine;

public class Room 
{
	public int xPos;
	public int yPos;
	public int roomWidth;
	public int roomHeight;
	public int CorridorPositionX;
	public int CorridorPositionY;
	public Direction enteringCorridor; 

    public void CreateRoom(int width, int height, int columns, int rows)
    { 
 		roomWidth = width;
        roomHeight = height;
 		xPos = Mathf.RoundToInt(columns / 2f - roomWidth / 2f);
 		yPos = Mathf.RoundToInt(rows / 2f - roomHeight / 2f);
 	}

	public void CreateRoom(int width, int height, int columns, int rows, Corridor corridor) 
 	{
 		enteringCorridor = corridor.direction; 
 		roomWidth = width; 
 		roomHeight = height;
 		switch (corridor.direction) 
 		{
 			case Direction.North:
 				roomHeight = Mathf.Clamp(roomHeight, 1, rows - corridor.EndPositionY);
 				yPos = corridor.EndPositionY;
 				xPos = Random.Range(corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);
 				xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
 				break;
 			case Direction.East:
 				roomWidth = Mathf.Clamp(roomWidth, 1, columns - corridor.EndPositionX);
 				xPos = corridor.EndPositionX;
 				yPos = Random.Range(corridor.EndPositionY - roomHeight + 1, corridor.EndPositionY);
 				yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
 				break;
 			case Direction.South:
 				roomHeight = Mathf.Clamp(roomHeight, 1, corridor.EndPositionY);
 				yPos = corridor.EndPositionY - roomHeight + 1;
 				xPos = Random.Range(corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);
 				xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
 				break;
 			case Direction.West:
 				roomWidth = Mathf.Clamp(roomWidth, 1, corridor.EndPositionX);
 				xPos = corridor.EndPositionX - roomWidth + 1;
 				yPos = Random.Range(corridor.EndPositionY - roomHeight + 1, corridor.EndPositionY);
 				yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
 				break;
 		}
 	}
}

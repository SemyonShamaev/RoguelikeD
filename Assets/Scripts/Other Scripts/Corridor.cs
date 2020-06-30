using UnityEngine;

public enum Direction 
{
 North, East, South, West,
}

public class Corridor 
{
   	public int startXPos; 
 	public int startYPos; 
 	public int corridorLength; 
 	public Direction direction;


 	public int EndPositionX 
 	{
 		get
 		{
 			if (direction == Direction.North || direction == Direction.South)
 				return startXPos;
 			if (direction == Direction.East)
 				return startXPos + corridorLength - 1;
 			return startXPos - corridorLength + 1;
 		}
 	}
 	public int EndPositionY 
 	{
 		get
 		{
 			if (direction == Direction.East || direction == Direction.West)
 				return startYPos;
 			if (direction == Direction.North)
 				return startYPos + corridorLength - 1;
 			return startYPos - corridorLength + 1;
 		}
 	}
 	public void CreateCorridor(Room room, int length, int roomWidth, int roomHeight, int columns, int rows, bool firstCorridor) 
 	{		
        bool isCollision = true;
        
        while (isCollision)
        {
            direction = (Direction)Random.Range(0, 4);
            switch (direction)
            {
                case Direction.North:
                    if (room.yPos + 10 < 200)
                        isCollision = false;
                    break;
                case Direction.East:
                    if (room.xPos - 10 > 0)
                        isCollision = false;
                    break;
                case Direction.South:
                    if (room.yPos - 10 > 0)
                        isCollision = false;
                    break;
                case Direction.West:
                    if (room.xPos + 10 < 200)
                        isCollision = false;
                    break;
            }
        }
 		Direction oppositeDirection = (Direction)(((int)room.enteringCorridor + 2) % 4);
		
		if (!firstCorridor && direction == oppositeDirection)
 		{ 
 			int directionInt = (int)direction;
 			directionInt++;
 			directionInt = directionInt % 4;
 			direction = (Direction)directionInt;
 		}

 		corridorLength = length;
 		int maxLength = 10;
 		switch (direction) 
 		{
 			case Direction.North:
 				startXPos = Random.Range(room.xPos, room.xPos + room.roomWidth - 1);
 				startYPos = room.yPos + room.roomHeight;
 				maxLength = rows - startYPos - 6;
 				break;
 			case Direction.East:
 				startXPos = room.xPos + room.roomWidth;
 				startYPos = Random.Range(room.yPos, room.yPos + room.roomHeight - 1);
 				maxLength = columns - startXPos - 6;
 				break;
 			case Direction.South:
 				startXPos = Random.Range(room.xPos, room.xPos + room.roomWidth);
 				startYPos = room.yPos;
 				maxLength = startYPos - 6;
 				break;
 			case Direction.West:
 				startXPos = room.xPos;
 				startYPos = Random.Range(room.yPos, room.yPos + room.roomHeight);
 				maxLength = startXPos - 6;
 				break;
 		}
 		corridorLength = Mathf.Clamp(corridorLength, 1, maxLength); 
 	} 
}

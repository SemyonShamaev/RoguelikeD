using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour
{
	public float speed;
    public float cellSize;
    bool isMoving = false;
    Vector3 direction;
    Vector3 destPos;
    public GameObject heroBody;
  	public Sprite[] sprites = new Sprite[4];

    void Update()
    {
        if (isMoving == true)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destPos, step);
            if (transform.position == destPos) isMoving = false;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                direction = Vector3.up;
                destPos = transform.position + direction * cellSize;
                heroBody.GetComponent<SpriteRenderer>().sprite = sprites[2];
                isMoving = true;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                direction = Vector3.left;
                destPos = transform.position + direction * cellSize;
                heroBody.GetComponent<SpriteRenderer>().sprite = sprites[1];
                isMoving = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                direction = Vector3.down;
                destPos = transform.position + direction * cellSize;
                heroBody.GetComponent<SpriteRenderer>().sprite = sprites[0];
                isMoving = true;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                direction = Vector3.right;
                destPos = transform.position + direction * cellSize;
                heroBody.GetComponent<SpriteRenderer>().sprite = sprites[3];
                isMoving = true;
            }
        }
    }
}
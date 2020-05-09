using UnityEngine;
using System.Collections;
using Rogue;

public class CameraMove : Singleton<CameraMove>
{
    private bool drag = false;
    private bool zoom = false;
    private float timer = 0;
    private float _timer = 0;
    private Vector3 initialTouchPosition;
    private Vector3 initialCameraPosition;
    private Vector3 initialTouch0Position;
    private Vector3 initialTouch1Position;
    private Vector3 initialMidPointScreen;
    private float initialOrthographicSize;
    private Camera cam;

    private void Start()
    {
    	cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.touchCount == 1 && !GameManager.Instance.onPause)
        {
        	timer = Time.deltaTime;
    		_timer = _timer + timer; 
        	if(_timer > 0.2)
        	{
            	zoom = false;
            	Touch touch0 = Input.GetTouch(0);
            	if (IsTouching(touch0))
            	{
                	if (!drag)
                	{
                    	initialTouchPosition = touch0.position;
                    	initialCameraPosition = this.transform.position;

                    	drag = true;
                	}
                	else
                	{
                    	Vector2 delta = cam.ScreenToWorldPoint(touch0.position) - 
                                    cam.ScreenToWorldPoint(initialTouchPosition);

                    	Vector3 newPos = initialCameraPosition;
                    	newPos.x -= delta.x;
                    	newPos.y -= delta.y;

                    	this.transform.position = newPos;
                	}
            	}
            if (!IsTouching(touch0))
                drag = false;
            }
        }
        else
        {
        	_timer = 0;
            drag = false;
        }

        if (Input.touchCount == 2 && !GameManager.Instance.onPause)
        {
            drag = false;
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            if (!zoom)
            {
                initialTouch0Position = touch0.position;
                initialTouch1Position = touch1.position;
                initialCameraPosition = this.transform.position;
                initialOrthographicSize = Camera.main.orthographicSize;
                initialMidPointScreen = (touch0.position + touch1.position) / 2;
                zoom = true;
            }
            else
            {
                this.transform.position = initialCameraPosition;
                cam.orthographicSize = initialOrthographicSize;

                float scaleFactor = GetScaleFactor(touch0.position, touch1.position, initialTouch0Position, initialTouch1Position);

                Vector2 currentMidPoint = (touch0.position + touch1.position) / 2;
                Vector3 initialPointWorldBeforeZoom = cam.ScreenToWorldPoint(initialMidPointScreen);

                Camera.main.orthographicSize = initialOrthographicSize / scaleFactor;

                Vector3 initialPointWorldAfterZoom = cam.ScreenToWorldPoint(initialMidPointScreen);
                Vector2 initialPointDelta = initialPointWorldBeforeZoom - initialPointWorldAfterZoom;

                Vector2 oldAndNewPointDelta = cam.ScreenToWorldPoint(currentMidPoint) - cam.ScreenToWorldPoint(initialMidPointScreen);

                Vector3 newPos = initialCameraPosition;
                newPos.x -= oldAndNewPointDelta.x - initialPointDelta.x;
                newPos.y -= oldAndNewPointDelta.y - initialPointDelta.y;

                this.transform.position = newPos;
            }
        }
    else
    {
    	zoom = false;
    }
	}

    static bool IsTouching(Touch touch)
    {
        return touch.phase == TouchPhase.Began ||
               touch.phase == TouchPhase.Moved ||
               touch.phase == TouchPhase.Stationary;
    }

    public static float GetScaleFactor(Vector2 position1, Vector2 position2, Vector2 oldPosition1, Vector2 oldPosition2)
    {
        float distance = Vector2.Distance(position1, position2);
        float oldDistance = Vector2.Distance(oldPosition1, oldPosition2);

        if (oldDistance == 0 || distance == 0)
            return 1.0f;

        return distance / oldDistance;
    }
}
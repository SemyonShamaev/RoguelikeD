using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
   public float runSpeed = 3f;
   public void Move(float ax, float ax1){
	Vector3 direction = transform.right * ax;
	transform.position = Vector3.Lerp(transform.position,transform.position+direction, runSpeed*Time.deltaTime);
	Vector3 direction1 = transform.up * ax1;
	transform.position = Vector3.Lerp(transform.position,transform.position+direction1, runSpeed*Time.deltaTime);
   }
   
}

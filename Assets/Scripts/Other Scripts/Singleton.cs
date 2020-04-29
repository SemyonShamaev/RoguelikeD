using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rogue
{
    public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
 {
     private static T _instance;

     public static T Instance
     {
         get
         {
             if (_instance == null)
             {
                 _instance = FindObjectOfType<T>();
 
                 if (_instance == null)
                 {
                     GameObject go = new GameObject("Singleton:Singleton");
                     _instance = go.AddComponent<T>();
                 }
             }
             return _instance;
         }
         set
         {
             _instance = value;
         }
     }
 
     void Awake ()
     {
         
     }
 
     void OnDisable ()
     {
         Instance = null;
     }
 }
}
 
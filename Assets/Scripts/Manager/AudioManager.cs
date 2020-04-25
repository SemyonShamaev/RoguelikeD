using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager: MonoBehaviour 
{
    public static AudioManager instance = null; 
    public static bool music = true; 
    public static bool sounds = true;

    void Start () 
    {
		if (instance == null) 
	    	instance = this; 
		else if(instance == this) 
	    	Destroy(gameObject); 
	
		DontDestroyOnLoad(gameObject);

		InitializeManager();
    }

    private void InitializeManager()
    {
        music = System.Convert.ToBoolean (PlayerPrefs.GetString ("music", "true"));
		sounds = System.Convert.ToBoolean (PlayerPrefs.GetString ("sounds", "true"));
    }

    public static void saveSettings(){
		PlayerPrefs.SetString ("music", music.ToString ());
		PlayerPrefs.SetString ("sounds", sounds.ToString ()); 
		PlayerPrefs.Save(); 
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SliderSave : MonoBehaviour
{
	public Slider slid;
    public AudioMixer masterMixer;

    void Start()
    {
    	if(slid.name == "MusicSlider")
    	{
        	slid.value = PlayerPrefs.GetFloat("VolumeMusic", 0);
            masterMixer.SetFloat("VolumeMusic",  Mathf.Lerp(-80, 0, slid.value));
    	}
    	else 
    	{
    		slid.value = PlayerPrefs.GetFloat("VolumeEffects", 0);
            masterMixer.SetFloat("VolumeEffects",  Mathf.Lerp(-80, 0, slid.value));
    	}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rogue; 

public class DataBase : Singleton<DataBase>
{
    public List<Item> items = new List<Item>();

	public enum ItemType 
	{
    	Empty, Gold, Food, Weapon
	}
}

[System.Serializable]

public class Item
{
	public int id;
	public string name;
	public Sprite image;
	public DataBase.ItemType type;
}

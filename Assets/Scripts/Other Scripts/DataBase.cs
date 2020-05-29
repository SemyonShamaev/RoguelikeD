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
    	Empty, Gold, Food, Weapon, Shield, Potion, Armor, Delete
	}
}

[System.Serializable]
public class Item
{
	public int id;
	public string name;
	public Sprite image;
	public DataBase.ItemType type;
    public int healthRecovery;
    public int satietyRecovery;
    public int damage;
    public int defense;
    public int agility;
    public int stamina;
    public int cost;
}
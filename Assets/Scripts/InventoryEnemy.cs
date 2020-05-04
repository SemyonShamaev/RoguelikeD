using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rogue; 

public class InventoryEnemy : Singleton<DataBase>
{
	public List<ItemInventory> items = new List<ItemInventory>();
   
	public GameObject gameObjShow;

	public GameObject InventoryMainObject;
	public int maxCount;

	public Camera cam;
	public EventSystem es;

	public int currentID;
	public ItemInventory currentItem;

	public RectTransform movingObject;
	public Vector3 offset;

	private bool isAdded = false;

	public void Start()
	{
		if(items.Count == 0)
		{
			addGraphics();
		}

		for(int i = 0; i < maxCount; i++)
		{
			AddItem(i, DataBase.Instance.items[Random.Range(0, DataBase.Instance.items.Count)], Random.Range(1,99));
		}
		UpdateInventory();
	}

	public void SearchForSameItem(Item item, int count)
	{
		for(int i = 0; i < maxCount; i++)
		{
			if(items[i].id == item.id)
			{
				if(items[0].count < 128)
				{
					items[i].count += count;
					if(items[i].count > 128)
					{
						count = items[i].count - 128;
						items[i].count = 64;
					}
					else
					{
						count = 0;
						i = maxCount;
					}
				}
			}
		}

		if(count > 0)
		{
			for(int i = 0; i < maxCount; i++)
			{
				if(items[i].id == 0)
				{
					AddItem(i, item, count);
					i = maxCount;
				}
			}
		}
	}

  	public void AddItem(int id, Item item, int count)
   	{
   		items[id].id = item.id;
   		items[id].count = count;
   		items[id].itemGameObj.GetComponent<Image>().sprite = item.image;

   		if(count > 1 && item.id != 0)
   		{
   			items[id].itemGameObj.GetComponentInChildren<Text>().text = count.ToString();
   		}
   		else
   		{
   			items[id].itemGameObj.GetComponentInChildren<Text>().text = "";
   		}
   	}

   	public void AddInventoryItem(int id, ItemInventory invItem)
   	{
   		items[id].id = invItem.id;
   		items[id].count = invItem.count;
   		items[id].itemGameObj.GetComponent<Image>().sprite = DataBase.Instance.items[invItem.id].image;
   		if(invItem.count > 1 && invItem.id != 0)
   		{
   			items[id].itemGameObj.GetComponentInChildren<Text>().text = invItem.count.ToString();
   		}
   		else
   		{
   			items[id].itemGameObj.GetComponentInChildren<Text>().text = "";
   		}
   	}

   	public void addGraphics()
   	{
   		for(int i = 0; i < maxCount; i++)
   		{
   			GameObject newItem = Instantiate(gameObjShow, InventoryMainObject.transform) as GameObject;

   			newItem.name = i.ToString();

   			ItemInventory ii = new ItemInventory();
   			ii.itemGameObj = newItem;

   			RectTransform rt = newItem.GetComponent<RectTransform>();

   			rt.localPosition = new Vector3(0,0,0);
   			rt.localScale = new Vector3(1,1,1);
   			newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1,1,1);

   			Button tempButton = newItem.GetComponent<Button>();

   			tempButton.onClick.AddListener(delegate { SelectObject(); });

   			items.Add(ii);
   		}
   	}

   	public void SelectObject()
   	{
   		if(currentID == -1)
   		{
   			currentID = int.Parse(es.currentSelectedGameObject.name);


   			for(int i = 0; i < Inventory.Instance.maxCount; i++)
   			{
   				if(Inventory.Instance.items[i].id == items[currentID].id)
   				{
   					if(Inventory.Instance.items[i].count + items[currentID].count <= 128)
   					{
   						Inventory.Instance.AddItem(i, DataBase.Instance.items[items[currentID].id], Inventory.Instance.items[i].count + items[currentID].count);
   						isAdded = true;
   						break;
   					}
   					else if(Inventory.Instance.items[i].count != 128)
   					{
   						items[currentID].count = items[currentID].count + Inventory.Instance.items[i].count - 128;
   						Inventory.Instance.AddItem(i, DataBase.Instance.items[items[currentID].id], 128);
   						break;
   					}
   				}
   			}

   			Debug.Log(isAdded);

   			if(!isAdded)
   			{
   				for(int i = 0; i < Inventory.Instance.maxCount; i++)
   				{
   					if(Inventory.Instance.items[i].id == 0)
   					{
   						Inventory.Instance.AddItem(i, DataBase.Instance.items[items[currentID].id], items[currentID].count);
   						break;
   					}
   				}
   			}

   			AddItem(currentID, DataBase.Instance.items[0], 0);
   			Inventory.Instance.UpdateInventory();
   			currentID = -1;
   			isAdded = false;
  		}

   	}

   	public void UpdateInventory()
   	{
   		for(int i = 0; i < maxCount; i++)
   		{
   			if(items[i].id != 0 && items[i].count > 1)
   			{
   				items[i].itemGameObj.GetComponentInChildren<Text>().text = items[i].count.ToString();
   			}
   			else
   			{
   				items[i].itemGameObj.GetComponentInChildren<Text>().text = "";
   			}

   			items[i].itemGameObj.GetComponent<Image>().sprite = DataBase.Instance.items[items[i].id].image;
   		}
   	}

   	

   	public ItemInventory CopyInventoryItem(ItemInventory old)
   	{
   		ItemInventory New = new ItemInventory();

   		New.id = old.id;
   		New.itemGameObj = old.itemGameObj;
   		New.count = old.count;
   		return New;
   	}
}

[System.Serializable]

public class ItemInventory
{
	public int id;
	public GameObject itemGameObj;
	public int count;
}

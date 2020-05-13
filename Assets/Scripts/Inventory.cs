using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rogue; 

public class Inventory : Singleton<Inventory>
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

	public void Start()
	{
		if(items.Count == 0)
			addGraphics();

		UpdateInventory();
	}

	public void Update()
	{
		if(currentID != -1 && !GameManager.Instance.onPause)
		{
			MoveObject();
		}
	}

  	public void AddItem(int id, Item item, int count, DataBase.ItemType type)
    {
   		items[id].id = item.id;
   		items[id].count = count;
   		items[id].type = type;

   		items[id].itemGameObj.GetComponent<Image>().sprite = item.image;

		if(count > 1 && item.id != 0)
			items[id].itemGameObj.GetComponentInChildren<Text>().text = count.ToString();
		else
			items[id].itemGameObj.GetComponentInChildren<Text>().text = "";
	}

   public void AddInventoryItem(int id, ItemInventory invItem)
	{
		items[id].id = invItem.id;
		items[id].count = invItem.count;
		items[id].itemGameObj.GetComponent<Image>().sprite = DataBase.Instance.items[invItem.id].image;
		if(invItem.count > 1 && invItem.id != 0)
			items[id].itemGameObj.GetComponentInChildren<Text>().text = invItem.count.ToString();
 		else
   		items[id].itemGameObj.GetComponentInChildren<Text>().text = "";
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

   			if(items[currentID].type == DataBase.ItemType.Food)
   			{
   				items[currentID].count--;
   				Player.Instance.HealPlayer();
   			}

   			currentID = -1;
   			UpdateInventory();
   		}
	}

	public void UpdateInventory()
	{
		for(int i = 0; i < maxCount; i++)
 		{
 			items[i].itemGameObj.GetComponent<Image>().sprite = DataBase.Instance.items[items[i].id].image;
			if(items[i].id != 0 && items[i].count > 0)
   				items[i].itemGameObj.GetComponentInChildren<Text>().text = items[i].count.ToString();
   			else
   			{
   				items[i].itemGameObj.GetComponentInChildren<Text>().text = "";
   				items[i].itemGameObj.GetComponent<Image>().sprite = DataBase.Instance.items[0].image;
   				items[i].type = DataBase.ItemType.Empty;
   				items[i].id = 0;
   			}		
   		}
	}

    public void MoveObject()
    {
   		Vector3 pos = Input.mousePosition + offset;
   		pos.z = InventoryMainObject.GetComponent<RectTransform>().position.z;
   		movingObject.position = cam.ScreenToWorldPoint(pos);
    }

    public void LoadData(List<Save.PlayerInventorySaveData> save)
    {
    	for(int i = 0; i < maxCount; i++)
    	{
    		AddItem(i, DataBase.Instance.items[save[i].id], save[i].count, DataBase.Instance.items[save[i].id].type);
    	}
    }
} 
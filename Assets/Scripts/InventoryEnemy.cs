using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Rogue; 

public class InventoryEnemy : Singleton<DataBase>
{
	public Camera cam;

    public int maxCount;
    public int currentID;

    public Vector3 offset;

    public EventSystem es;

	public GameObject gameObjShow;
    public GameObject InventoryMainObject;

    public ItemInventory currentItem;

	public RectTransform movingObject;
    
    public List<ItemInventory> items = new List<ItemInventory>();

    public AudioClip takeSound;
    public AudioClip cancelSound;

	private bool isAdded = false;

	public void Start()
	{
		UpdateInventory();
	}

  	public void AddItem(int id, Item item, int count, DataBase.ItemType type)
    {
        if(items.Count == 0)
            addGraphics();

   	    items[id].id = item.id;
   	    items[id].count = count;
        items[id].type = type;

   	    items[id].itemGameObj.GetComponent<Image>().sprite = item.image;

   	    if(count > 1 && item.id != 0)
   		    items[id].itemGameObj.GetComponentInChildren<Text>().text = count.ToString();
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
        if (currentID == -1)
        {
            currentID = int.Parse(es.currentSelectedGameObject.name);

            if (items[currentID].type == DataBase.ItemType.Gold)
            {
                Inventory.Instance.items[0].count += items[currentID].count;
                isAdded = true;
            }

            else if(items[currentID].type == DataBase.ItemType.Weapon || items[currentID].type == DataBase.ItemType.Shield)
            {
                for(int i = 4; i < Inventory.Instance.maxCount; i++)
                {
                    if (Inventory.Instance.items[i].id == 0)
                    {
                        Inventory.Instance.AddItem(i, DataBase.Instance.items[items[currentID].id], items[currentID].count, items[currentID].type);
                        isAdded = true;
                        break;
                    }
                }
            }

            else if (items[currentID].type != DataBase.ItemType.Empty)
            {
                for (int i = 4; i < Inventory.Instance.maxCount; i++)
                {
                    if (Inventory.Instance.items[i].id == items[currentID].id)
                    {
                        if (Inventory.Instance.items[i].count + items[currentID].count <= 128)
                        {
                            Inventory.Instance.AddItem(i, DataBase.Instance.items[items[currentID].id], Inventory.Instance.items[i].count + items[currentID].count, items[currentID].type);
                            isAdded = true;
                            break;
                        }
                        else if (Inventory.Instance.items[i].count != 128)
                        {
                            items[currentID].count = items[currentID].count + Inventory.Instance.items[i].count - 128;
                            Inventory.Instance.AddItem(i, DataBase.Instance.items[items[currentID].id], 128, items[currentID].type);
                            break;
                        }
                    }
                }
            }

            if (!isAdded)
            {
                for (int i = 4; i < Inventory.Instance.maxCount; i++)
                {
                    if (Inventory.Instance.items[i].id == 0)
                    {
                        Inventory.Instance.AddItem(i, DataBase.Instance.items[items[currentID].id], items[currentID].count, items[currentID].type);
                        isAdded = true;
                        break;
                    }
                }
            }

            Inventory.Instance.UpdateInventory();
            AudioManager.Instance.PlayEffects(takeSound);
        }

        if (isAdded)
        {
            AddItem(currentID, DataBase.Instance.items[0], 0, DataBase.Instance.items[0].type);
            Inventory.Instance.UpdateInventory();
            currentID = -1;
            isAdded = false;
        }
        else
            AudioManager.Instance.PlayEffects(cancelSound);
    }

    public void UpdateInventory()
    {
   	    for(int i = 0; i < maxCount; i++)
   	    {
   		    if(items[i].id != 0 && items[i].count > 1)
   			    items[i].itemGameObj.GetComponentInChildren<Text>().text = items[i].count.ToString();
   		    else
   			    items[i].itemGameObj.GetComponentInChildren<Text>().text = "";

  			items[i].itemGameObj.GetComponent<Image>().sprite = DataBase.Instance.items[items[i].id].image;
   	    }
    }

    public void LoadData(Save.EnemyInventorySaveData save, int k)
    {
        AddItem(k, DataBase.Instance.items[save.id], save.count, DataBase.Instance.items[save.id].type);
    }

    public void LoadDataContainers(Save.ContainersInventorySaveData save, int k)
    {
        AddItem(k, DataBase.Instance.items[save.id], save.count, DataBase.Instance.items[save.id].type);
    }
}

[System.Serializable]
public class ItemInventory
{
	public int id;
	public GameObject itemGameObj;
	public int count;
    public DataBase.ItemType type;
}



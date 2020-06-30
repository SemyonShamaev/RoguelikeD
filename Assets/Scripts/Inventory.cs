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

    public GameObject PanelInventory;
    public GameObject InventoryMainObject;
	public int maxCount;

    public AudioClip deleteSound;

    public Camera cam;
	public EventSystem es;

	public int currentID;
	public ItemInventory currentItem;

	public RectTransform movingObject;
	public Vector3 offset;

    private bool deleteMode;

	public void Start()
	{
		if(items.Count == 0)
			addGraphics();

        AddItem(0, DataBase.Instance.items[1], 100, DataBase.Instance.items[1].type);

        AddItem(1, DataBase.Instance.items[4], 1, DataBase.Instance.items[4].type);
        Player.Instance.attack += DataBase.Instance.items[4].damage;

        AddItem(2, DataBase.Instance.items[9], 1, DataBase.Instance.items[9].type);
        Player.Instance.defense += DataBase.Instance.items[9].defense;

        AddItem(3, DataBase.Instance.items[18], 1, DataBase.Instance.items[18].type);
        Player.Instance.agility += DataBase.Instance.items[18].agility;
        Player.Instance.stamina += DataBase.Instance.items[18].stamina;

        AddItem(4, DataBase.Instance.items[23], 1, DataBase.Instance.items[23].type);

        AddItem(5, DataBase.Instance.items[26], 1, DataBase.Instance.items[26].type);
        UpdateInventory();
	}

	public void Update()
	{
		if(currentID != -1 && !GameManager.Instance.onPause)
			MoveObject();
	}

  	public void AddItem(int id, Item item, int count, DataBase.ItemType type)
    {
   		items[id].id = item.id;
   		items[id].count = count;
   		items[id].type = type;

   		items[id].itemGameObj.GetComponent<Image>().sprite = item.image;

        if (count > 1)
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
        if(currentID == -1 && !deleteMode)
      	{
   			currentID = int.Parse(es.currentSelectedGameObject.name);

            if (items[currentID].type == DataBase.ItemType.Delete)
            {
                deleteMode = true;
                PanelInventory.GetComponent<Image>().color = Color.red;
            }

            if (items[currentID].type == DataBase.ItemType.Food)
            {
                items[currentID].count--;
                Player.Instance.EatPlayer(DataBase.Instance.items[items[currentID].id].healthRecovery, DataBase.Instance.items[items[currentID].id].satietyRecovery);
            }

            else if (items[currentID].type == DataBase.ItemType.Potion)
            {
                items[currentID].count--;
                Player.Instance.DrinkPotion(DataBase.Instance.items[items[currentID].id].name);
            }

            else if (items[currentID].type == DataBase.ItemType.Weapon)
            {
                int ID = items[currentID].id;
                Player.Instance.attack = Player.Instance.attack + DataBase.Instance.items[ID].damage - DataBase.Instance.items[items[1].id].damage;
                AddItem(currentID, DataBase.Instance.items[items[1].id], 1, DataBase.Instance.items[items[1].id].type);
                AddItem(1, DataBase.Instance.items[ID], 1, DataBase.Instance.items[ID].type);
            }

            else if (items[currentID].type == DataBase.ItemType.Shield)
            {
                int ID = items[currentID].id;
                Player.Instance.defense = Player.Instance.defense + DataBase.Instance.items[ID].defense - DataBase.Instance.items[items[2].id].defense;
                AddItem(currentID, DataBase.Instance.items[items[2].id], 1, DataBase.Instance.items[items[2].id].type);
                AddItem(2, DataBase.Instance.items[ID], 1, DataBase.Instance.items[ID].type);
            }

            else if (items[currentID].type == DataBase.ItemType.Armor)
            {
                int ID = items[currentID].id;
                Player.Instance.agility = Player.Instance.agility + DataBase.Instance.items[ID].agility - DataBase.Instance.items[items[3].id].agility;
                Player.Instance.stamina = Player.Instance.stamina + DataBase.Instance.items[ID].stamina - DataBase.Instance.items[items[3].id].stamina;
                Player.Instance.maxLifes = Player.Instance.maxLifes + DataBase.Instance.items[ID].stamina - DataBase.Instance.items[items[3].id].stamina;
                Player.Instance.maxSatiety = Player.Instance.maxSatiety + DataBase.Instance.items[ID].stamina - DataBase.Instance.items[items[3].id].stamina;

                if (Player.Instance.currentLifes > Player.Instance.maxLifes)
                    Player.Instance.currentLifes = Player.Instance.maxLifes;
                if (Player.Instance.currentSatiety > Player.Instance.maxSatiety)
                    Player.Instance.currentSatiety = Player.Instance.maxSatiety;

                Player.Instance.HealthBar.value = (float)Player.Instance.currentLifes / (float)Player.Instance.maxLifes;
                GameManager.Instance.healthCount.text = Player.Instance.currentLifes.ToString() + "/" + Player.Instance.maxLifes.ToString();

                Player.Instance.SatietyBar.value = (float)Player.Instance.currentSatiety / (float)Player.Instance.maxSatiety;
                GameManager.Instance.satietyCount.text = Player.Instance.currentSatiety.ToString() + "/" + Player.Instance.maxSatiety.ToString();

                AddItem(currentID, DataBase.Instance.items[items[3].id], 1, DataBase.Instance.items[items[3].id].type);
                AddItem(3, DataBase.Instance.items[ID], 1, DataBase.Instance.items[ID].type);
            }

            currentID = -1;
   			UpdateInventory();
   		}

        else if(currentID == -1 && deleteMode)
        {
            currentID = int.Parse(es.currentSelectedGameObject.name);

            if (items[currentID].type == DataBase.ItemType.Delete)
            {
                deleteMode = false;
                PanelInventory.GetComponent<Image>().color = Color.grey;
            }

            else if(currentID > 4)
            {
                AddItem(currentID, DataBase.Instance.items[0], 0, DataBase.Instance.items[0].type);
                AudioManager.Instance.PlayEffects(deleteSound);
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

            if(items[i].type == DataBase.ItemType.Gold && items[i].count == 0)
            {
                items[i].itemGameObj.GetComponentInChildren<Text>().text = "0";
            }

            else if (items[i].type == DataBase.ItemType.Weapon)
            {
                items[i].itemGameObj.GetComponentInChildren<Text>().text = DataBase.Instance.items[items[i].id].damage.ToString() + " НАП";
                items[i].itemGameObj.GetComponentInChildren<Text>().color = Color.red;
            }

            else if (items[i].type == DataBase.ItemType.Shield)
            {
                items[i].itemGameObj.GetComponentInChildren<Text>().text = DataBase.Instance.items[items[i].id].defense.ToString() + " ЗЩТ";
                items[i].itemGameObj.GetComponentInChildren<Text>().color = Color.green;
            }

            else if (items[i].type == DataBase.ItemType.Armor)
            {
                if (DataBase.Instance.items[items[i].id].agility > DataBase.Instance.items[items[i].id].stamina)
                {
                    items[i].itemGameObj.GetComponentInChildren<Text>().text = DataBase.Instance.items[items[i].id].agility.ToString() + " ЛОВ";
                    items[i].itemGameObj.GetComponentInChildren<Text>().color = Color.magenta;
                }
                else if(DataBase.Instance.items[items[i].id].agility < DataBase.Instance.items[items[i].id].stamina)
                {
                    items[i].itemGameObj.GetComponentInChildren<Text>().text = DataBase.Instance.items[items[i].id].stamina.ToString() + " ВЫН";
                    items[i].itemGameObj.GetComponentInChildren<Text>().color = Color.grey;
                }
            }

            else if (items[i].count > 1)
                items[i].itemGameObj.GetComponentInChildren<Text>().text = items[i].count.ToString();

            else if (items[i].count == 1)
                items[i].itemGameObj.GetComponentInChildren<Text>().text = "";

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

        UpdateInventory();
    }
} 
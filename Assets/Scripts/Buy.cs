using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Rogue;

public class Buy : Singleton<Buy>
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
    public AudioClip goldSound;
    public AudioClip cancelSound;

    private bool isAdded;

    public void Start()
    {

    }

    public void DeleteAllItems()
    {
        for (int i = 0; i < 25; i++)
        {
            AddItem(i, DataBase.Instance.items[0], 0, DataBase.Instance.items[0].type);
        }
    }

    public void AddItem(int id, Item item, int count, DataBase.ItemType type)
    {
        if (items.Count == 0)
            addGraphics();

        items[id].id = item.id;
        items[id].count = count;
        items[id].type = type;

        items[id].itemGameObj.GetComponent<Image>().sprite = item.image;

        if (count > 1 && item.id != 0)
            items[id].itemGameObj.GetComponentInChildren<Text>().text = count.ToString();
        else
            items[id].itemGameObj.GetComponentInChildren<Text>().text = "";

        UpdateInventory();
    }

    public void addGraphics()
    {
        for (int i = 0; i < maxCount; i++)
        {
            GameObject newItem = Instantiate(gameObjShow, InventoryMainObject.transform) as GameObject;

            newItem.name = i.ToString();

            ItemInventory ii = new ItemInventory();
            ii.itemGameObj = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();

            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);

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

         
            if (items[currentID].type == DataBase.ItemType.Empty)
            {
                AudioManager.Instance.PlayEffects(cancelSound);
            }

            else if(items[currentID].type == DataBase.ItemType.Armor || items[currentID].type == DataBase.ItemType.Weapon || items[currentID].type == DataBase.ItemType.Shield)
            {
                items[currentID].count = 0;
                Inventory.Instance.AddItem(currentID + 5, DataBase.Instance.items[0], 0, DataBase.Instance.items[0].type);

                Inventory.Instance.items[0].count += DataBase.Instance.items[items[currentID].id].cost;
                AudioManager.Instance.PlayEffects(goldSound);
            }

            else if (items[currentID].type != DataBase.ItemType.Empty)
            {
                items[currentID].count--;
                Inventory.Instance.items[currentID + 5].count--;

                Inventory.Instance.items[0].count += DataBase.Instance.items[items[currentID].id].cost;
                AudioManager.Instance.PlayEffects(goldSound);
            }


            if (items[currentID].count == 0)
            {
                Debug.Log("d");
                AddItem(currentID, DataBase.Instance.items[0], 0, DataBase.Instance.items[0].type);
            }

            currentID = -1;
            isAdded = false;
            UpdateInventory();
            Inventory.Instance.UpdateInventory();
        }
    }

    public void UpdateInventory()
    {
        GameManager.Instance.goldCount.text = Inventory.Instance.items[0].count.ToString();
        for (int i = 0; i < maxCount; i++)
        {
            items[i].itemGameObj.GetComponent<Image>().sprite = DataBase.Instance.items[items[i].id].image;

            Transform costText = items[i].itemGameObj.transform.GetChild(1);
            if (DataBase.Instance.items[items[i].id].cost != 0)
                costText.GetComponent<Text>().text = DataBase.Instance.items[items[i].id].cost.ToString();
            else
                costText.GetComponent<Text>().text = "";

            if (items[i].type == DataBase.ItemType.Weapon)
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
                else if (DataBase.Instance.items[items[i].id].agility < DataBase.Instance.items[items[i].id].stamina)
                {
                    items[i].itemGameObj.GetComponentInChildren<Text>().text = DataBase.Instance.items[items[i].id].stamina.ToString() + " ВЫН";
                    items[i].itemGameObj.GetComponentInChildren<Text>().color = Color.grey;
                }
            }

            else if (items[i].count > 1 && items[i].type != DataBase.ItemType.Empty)
                items[i].itemGameObj.GetComponentInChildren<Text>().text = items[i].count.ToString();

            else if (items[i].count == 1 && items[i].type != DataBase.ItemType.Empty)
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

    public void LoadData(Save.EnemyInventorySaveData save, int k)
    {
        AddItem(k, DataBase.Instance.items[save.id], save.count, DataBase.Instance.items[save.id].type);
    }

    public void LoadDataContainers(Save.ContainersInventorySaveData save, int k)
    {
        AddItem(k, DataBase.Instance.items[save.id], save.count, DataBase.Instance.items[save.id].type);
    }
}



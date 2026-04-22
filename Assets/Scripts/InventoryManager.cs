using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] itemPrefabs;
    public GameObject[] itemPrefab
    { get { return itemPrefabs; } set { itemPrefabs = value; } }

    [SerializeField]
    private ItemData[] itemData;
    public ItemData[] ItemData 
    { get { return itemData; } set { itemData = value; } }

    public const int MAXSLOT = 18;

    public static InventoryManager instance;

    private void Awake()
    {
        instance = this;
    }

    public bool AddItem(Character character, int id)
    {
        Item item = new Item(itemData[id]);

        for (int i = 0; i < character.InventoryItems.Length; i++)
        {
            if (character.InventoryItems[i] == null)
            {
                character.InventoryItems[i] = item;
                return true;
            }
        }

        Debug.Log("Inventory Full");
        return false;

    }

    public void SaveItemBag(int index, Item item)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
        {
            return;

        }

        PartyManager.instance.SelectChars[0].InventoryItems[index] = item;

        switch (index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].EquipShield(item); 
                break;
            case 17:
                PartyManager.instance.SelectChars[0].EquipWeapon(item); 
                break;
        }
    }

    public void RemoveItemInBag(int index)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = null;
        switch (index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].UnEquipShieid();
                break;
            case 17:
                PartyManager.instance.SelectChars[0].UnEquipWeapon();
                break;
        }
    }

    private void SpawnDropItem(Item item, Vector3 pos)
    {
        int id;
        
        switch(item.Type)
        {
            case ItemType.Consumable:
                id = 1;
                break;
                default:
                id = 0; 
                break;

        }

        GameObject itemObj = Instantiate(itemPrefab[id], pos, Quaternion.identity);
        itemObj.AddComponent<ItemPick>();

        ItemPick itemPick = itemObj.GetComponent<ItemPick>();
        itemPick.Init(item, instance, PartyManager.instance);
    }

    public void  SpawnDropInventory(Item[] items, Vector3 pos)
    {
        float radius = 1f;

        for (int i = 0; i < items.Length; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            Vector3 randomPos = new Vector3(pos.x + randomPoint.x, pos.y, pos.z + randomPoint.y);

            if (items[i] != null)
                SpawnDropItem((Item)items[i], randomPos);
        }
    }

    public void DrinkConsumbleItem(Item item, int slotId)
    {
        string s = string.Format("Drink: {0}", item.ItemName);
        Debug.Log(s);

        if (PartyManager.instance.SelectChars.Count > 0)
        {
            PartyManager.instance.SelectChars[0].Recover(item.power);
            RemoveItemInBag(slotId);
        }
    }

}

using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private int id;
    public int ID {  get { return id; } set { id = value; } }

    [SerializeField]
    private ItemType itemType;
    public ItemType ItemType 
    { get { return itemType; } set { itemType = value; } }

    [SerializeField]
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.instance;
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameObject objA = eventData.pointerDrag;
        ItemDrag itemDragA = objA.GetComponent<ItemDrag>();
        InventorySlot slotA = itemDragA.IconParent.GetComponent<InventorySlot>();

        if(itemType == ItemType.Shield)
        {
            if (itemDragA.Item.Type != itemType)
                return;
        }
        if(itemType == ItemType.Weapon)
        {
            if (itemDragA.Item.Type != itemType)
                return;
        }

       

        if (transform.childCount >  0)
        {
            GameObject objB = transform.GetChild(0).gameObject;
            ItemDrag itemDragB = objB.GetComponent<ItemDrag>();

            if (slotA.ItemType == ItemType.Shield)
            {
                if(itemDragB.Item.Type != slotA.ItemType)
                    return;
            }
            if(slotA.ItemType == ItemType.Weapon)
            {
                if(itemDragB.Item.Type != slotA.ItemType)
                    return;
            }

            inventoryManager.RemoveItemInBag(slotA.ID);

            itemDragB.transform.SetParent(itemDragA.IconParent);
            itemDragB.IconParent =itemDragA.IconParent;
            inventoryManager.SaveItemBag(slotA.ID, itemDragB.Item);

            inventoryManager.RemoveItemInBag(id);
        }
        else
        {
            inventoryManager.RemoveItemInBag(slotA.ID) ;
        }

        itemDragA.IconParent = transform;
        inventoryManager.SaveItemBag(id, itemDragA.Item);
    }
}

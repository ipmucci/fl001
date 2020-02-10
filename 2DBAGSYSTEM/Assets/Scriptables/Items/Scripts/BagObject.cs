using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New bag Object", menuName = "Bag System/Items/Bags")]
[ItemType(ItemType.Bag)]
public class BagObject : ItemObject
{
    public InventoryObject inventory;
}

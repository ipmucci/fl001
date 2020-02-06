using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLootScript : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var itemDrop = collision.GetComponent<ItemDropItem>();

        if(itemDrop)
        {
            var playerInventory = GetComponent<PlayerInventoryScript>();
            if(playerInventory?.AddItem(itemDrop.item, 1) ?? false)
            {
                Destroy(collision.gameObject);
            }
        }
    }

    public void Ondrag()
    {
        transform.position = Input.mousePosition;
    }
}

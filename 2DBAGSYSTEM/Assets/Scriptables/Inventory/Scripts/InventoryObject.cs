using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Inventory
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "New inventory Object", menuName = "Bag System/Inventory")]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver
{
    public List<InventorySlot> Slots = new List<InventorySlot>();

    public bool IsDirt { get; internal set; }

    public InventoryObject()
    {
    }

    /// <summary>
    /// Clear all slots in the inventory
    /// </summary>
    public void ClearInventory()
    {
        foreach (var slot in Slots)
        {
            slot.ClearSlot();
        }
    }

    /// <summary>
    /// Sets the inventory dirt, so it can be updated
    /// </summary>
    /// <param name="slot">the slot that changed</param>
    private void DirtTheInventory(InventorySlot slot)
    {
        IsDirt = true;
    }

    /// <summary>
    /// add an item to its stack or an empty slot
    /// </summary>
    /// <param name="item">item to add</param>
    /// <param name="amount">amount of the item</param>
    /// <returns>true if it was stored or false if not (inventory is full)</returns>
    public bool AddItem(ItemObject item, int amount)
    {
        var slot = GetSlotForItem(item, amount);
        if(slot != null)
        {
            slot.AddAmount(amount);
            IsDirt = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets a slot to add or a free slot or null when bag is full
    /// </summary>
    /// <param name="item">item to add values</param>
    /// <param name="amount">amount to add (if amount+value > maxstacksize, look for another slot)</param>
    /// <returns></returns>
    private InventorySlot GetSlotForItem(ItemObject item, int amount)
    {
        // stores the 1st empty slot in the inventory, if any.
        InventorySlot emptySlot = null;

        // search for the item in inventory, other wise, empty slot
        foreach (var slot in Slots)
        {
            // item found with space to get more items
            if (slot.Item == item)
                if (slot.Amount + amount <= item.maxItemStackSize)
                    return slot;

            // secure the 1st empty slot in the inventory (save us from another foreach)
            if (emptySlot == null && slot.Item == null)
                emptySlot = slot;
        }

        // if we found a empty slot, return it with the item and zero amount
        if(emptySlot != null)
        {
            emptySlot.Item = item;
            emptySlot.Amount = 0;
            return emptySlot;
        }

        // no item, no empty, cant doo anything.
        return null;
    }

    public void OnBeforeSerialize()
    {
    }

    /// <summary>
    /// setup the dirt inventory to the slots
    /// </summary>
    public void OnAfterDeserialize()
    {
        foreach (var slot in Slots)
        {
            slot.OnSlotChange -= DirtTheInventory;
            slot.OnSlotChange += DirtTheInventory;
        }
    }
}


/// <summary>
/// InventorySlot, holds information about item/qty for each slot
/// </summary>
[Serializable]
public class InventorySlot 
{
    public ItemObject Item;
    public int Amount;
    public ItemType[] ItemTypesAllowed;

    /// <summary>
    /// set this event to be invoked when this slot changes
    /// </summary>
    public event SlotChanged OnSlotChange;
    public delegate void SlotChanged(InventorySlot slot);
    public InventorySlot(ItemObject item, int amount)
    {
        this.Amount = amount;
        this.Item = item;
    }

    /// <summary>
    /// Adds a value to amount
    /// </summary>
    /// <param name="value">the value to sum</param>
    public void AddAmount(int value)
    {
        Amount += value;
        OnSlotChange?.Invoke(this);
    }

    /// <summary>
    /// Checks if the stot can store an item of a certain type (check for type only)
    /// </summary>
    /// <param name="item">Item to check if type is allowed</param>
    /// <returns>true, if the item type is allowed</returns>
    public bool CanStoreItemType(ItemObject item)
    {
        if (item == null) return false;
        // 0 or null, allow anything
        if (ItemTypesAllowed?.Length == 0 && true) return true;

        foreach (var itemType in ItemTypesAllowed)
            if (itemType == ItemType.Undefined || itemType == item.type) return true;

        return false;
    }

    /// <summary>
    /// swaps the values with other slot
    /// </summary>
    /// <param name="other">slot to swap with</param>
    /// <returns>true if ok.</returns>
    public bool SwapWith(InventorySlot other)
    {
        if (other == null) return false;
        if (CanStoreItemType(other.Item)) return false;

        var temp = new InventorySlot(Item, Amount);
        Item = other.Item;
        Amount = other.Amount;

        other.Item = temp.Item;
        other.Amount = temp.Amount;

        OnSlotChange?.Invoke(this);
        return true;

    }
    /// <summary>
    /// Reset the slot.
    /// </summary>
    public void ClearSlot()
    {
        if (Item != null)
        {
            Item = null;
            Amount = 0;
            OnSlotChange?.Invoke(this);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Static class to hold the slots and images under the cursor
/// </summary>
public static class MouseSlot
{
    public static GameObject MouseObject;
    public static InventorySlot SourceSlot;
    public static InventorySlot TargetSlot;
}


/// <summary>
/// Visual script attached to the InventoryPanelPrefab
/// </summary>
[RequireComponent(typeof(InventoryObject))]
public class InventoryPanelScript : MonoBehaviour
{
    public InventoryObject Inventory;

    public int StartX;
    public int StartY;
    public int SpaceX;
    public int SpaceY;
    public int SlotsPerRow;

    private Dictionary<GameObject, InventorySlot> slotsDisplayed = new Dictionary<GameObject, InventorySlot>();

    private GameObject SlotPrefab;
    private RectTransform rectTransform;
    private void Start()
    {
        SlotPrefab = Resources.Load<GameObject>("InventorySlotPrefab");
        CreateSlots();
    }


    /// <summary>
    /// fix the panel size and create and set all the graphic slots to the slots in the inventory
    /// </summary>
    private void CreateSlots()
    {
        rectTransform = GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(
            Math.Abs(StartX * 2) + Math.Abs(SpaceX * SlotsPerRow),
            Math.Abs(StartY * 2) + Math.Abs(SpaceY * (Inventory.Slots.Count / SlotsPerRow + (Inventory.Slots.Count % SlotsPerRow == 0 ? 0 : 1)))
            );

        slotsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < Inventory.Slots.Count; i++)
        {
            var obj = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            var slotScript = obj.GetComponent<SlotImageScript>();

            var slot = Inventory.Slots[i];
            slotScript.SetSlot(slot);
            slotScript.AddEvent(EventTriggerType.PointerEnter, (a) => OnEnter(slot));
            slotScript.AddEvent(EventTriggerType.PointerExit, (a) => OnExit(slot));
            slotScript.AddEvent(EventTriggerType.BeginDrag, (a) => OnBeginDrag(slot));
            slotScript.AddEvent(EventTriggerType.EndDrag, (a) => OnEndDrag(slot));
            slotScript.AddEvent(EventTriggerType.Drag, (a) => OnDrag(slot));

            slotsDisplayed.Add(obj, Inventory.Slots[i]);
        }
    }

    private void OnDrag(InventorySlot slot)
    {
        if(MouseSlot.MouseObject?.GetComponent<RectTransform>()?.position != null)
            MouseSlot.MouseObject.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    private void OnEndDrag(InventorySlot slot)
    {
        if(MouseSlot.TargetSlot != null)
        {
            MouseSlot.SourceSlot.SwapWith(MouseSlot.TargetSlot);
        } else
        {
            //TODO: DROP ITEM...
        }
        Destroy(MouseSlot.MouseObject);
        MouseSlot.MouseObject = null;
    }

    private void OnBeginDrag(InventorySlot slot)
    {
        MouseSlot.MouseObject = new GameObject();
        MouseSlot.MouseObject.AddComponent<RectTransform>().sizeDelta = new Vector2(SpaceX, SpaceY);
        MouseSlot.MouseObject.transform.SetParent(transform.parent);

        if (slot.Item?.icon != null)
        {
            var img = MouseSlot.MouseObject.AddComponent<Image>();
            img.sprite = slot.Item.icon;
            img.raycastTarget = false;
        }
        MouseSlot.SourceSlot = slot;
        MouseSlot.TargetSlot = slot;
    }

    private void OnExit(InventorySlot slot)
    {
        MouseSlot.TargetSlot = null;
    }

    private void OnEnter(InventorySlot slot)
    {
        MouseSlot.TargetSlot = slot;
    }

    public void LateUpdate()
    {
        // if inventory is dirt, update the graphic slots
        if (Inventory.IsDirt)
        {
            foreach (var slotKeyPair in slotsDisplayed)
                slotKeyPair.Key.GetComponent<SlotImageScript>().SetSlot(slotKeyPair.Value);
            Inventory.IsDirt = false;
        }
    }

    /// <summary>
    /// helper to get the position for each slot
    /// </summary>
    /// <param name="slotNum">the slot number (sequential)</param>
    /// <returns>A vector3 with x,y for the UIElement</returns>
    Vector3 GetPosition(int slotNum) => new Vector3(
        StartX + (SpaceX * (slotNum % SlotsPerRow)), 
        rectTransform.rect.height + StartY + (-SpaceY * (slotNum / SlotsPerRow)), 
        0);
}

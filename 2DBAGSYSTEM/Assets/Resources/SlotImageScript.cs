using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// script to the slot prefab
/// </summary>
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class SlotImageScript : MonoBehaviour
{
    private TextMeshProUGUI TMPText;
    private Image Image;

    /// <summary>
    /// caches everything we need
    /// </summary>
    private void Awake()
    {
        TMPText = GetComponentInChildren<TextMeshProUGUI>();
        Image = GetComponent<Image>();
    }

    /// <summary>
    /// sets the image to the slot, if null transparent gray background
    /// </summary>
    /// <param name="sprite">sprite to draw in the slot, or null for none</param>
    public void SetSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            Image.sprite = sprite;
            Image.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Image.color = new Color(0, 0, 0, 0.1f);
            Image.sprite = null;
        }
    }

    /// <summary>
    /// sets the number of stacks in the icon, if < 2, doesnt need to show.
    /// </summary>
    /// <param name="amount"></param>
    public void SetAmount(int amount)
    {
        TMPText.text = amount < 2 ? "" : amount.ToString();
    }

    /// <summary>
    /// Update both icon and amount based on the slot
    /// </summary>
    /// <param name="slot"></param>
    public void SetSlot(InventorySlot slot)
    {
        if(slot != null)
        {
            SetAmount(slot.Amount);
            SetSprite(slot.Item?.icon ?? null);
        }
        else
        {
            Clear();
        }
    }

    /// <summary>
    /// clears the icon and amount
    /// </summary>
    public void Clear()
    {
        SetSprite(null);
        SetAmount(0);
    }


    /// <summary>
    /// Adds events to the icon so it can respond to mouse
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public void AddEvent(EventTriggerType type, UnityAction<BaseEventData> action)
    {
        var trigger = GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
}

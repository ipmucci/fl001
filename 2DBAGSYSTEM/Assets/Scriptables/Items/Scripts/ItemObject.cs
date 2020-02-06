using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// enum with the item types
/// </summary>
public enum ItemType
{
    Undefined,
    Default,
    Bag,
    Equipment,
    Food
}

/// <summary>
/// attribute class to set up the item type.
/// so you dont need to set the type for each scriptableobject, just add the attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ItemTypeAttribute: System.Attribute {
    public ItemType Type;
    public ItemTypeAttribute(ItemType _type) => Type = _type;
}

/// <summary>
/// base class for items
/// </summary>
public abstract class ItemObject : ScriptableObject
{
    [TextArea(15, 20)]
    public string description;
    public ItemType type;
    public Sprite icon;
    [Space(10)]
    public uint maxItemStackSize = 1;

    /// <summary>
    /// gets the type attribute and assing to the type of the instanced class.
    /// </summary>
    public ItemObject()
    {
        var attrList = (this.GetType().GetCustomAttributes(typeof(ItemTypeAttribute), false) as ItemTypeAttribute[]);
        var attr = attrList?.Length > 0 ? attrList[0] : null;

        type = attr == null ? ItemType.Undefined : attr.Type;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// this is temporary, just as an example.
/// </summary>
[RequireComponent(typeof(InventoryObject))]
[RequireComponent(typeof(Canvas))]
public class PlayerInventoryScript : MonoBehaviour
{
    public InventoryObject Inventory;
    public Canvas Canvas;

    public Vector3 InventoryPosition;
    public int StartX;
    public int StartY;
    public int SpaceX;
    public int SpaceY;
    public int SlotsPerRow;

    private InventoryPanelScript Panel;

    /// <summary>
    /// instanciate the graphics part and assing the inventory to it....
    /// </summary>
    private void Awake()
    {
        var prefab = Resources.Load<InventoryPanelScript>("InventoryPanelPrefab");
        prefab.gameObject.SetActive(false);
        Panel = Instantiate(prefab, Vector3.zero, Quaternion.identity, Canvas.transform);
        Panel.GetComponent<RectTransform>().localPosition = InventoryPosition;
        Panel.Inventory = Inventory;
        Panel.StartX = StartX;
        Panel.StartY = StartY;
        Panel.SpaceX = SpaceX;
        Panel.SpaceY = SpaceY;
        Panel.SlotsPerRow = SlotsPerRow;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) 
            Panel.gameObject.SetActive(!Panel.gameObject.activeSelf);


        Panel.GetComponent<RectTransform>().localPosition = InventoryPosition;
    }


    public bool AddItem(ItemObject item, int amount)
    {
        if (Inventory?.AddItem(item, amount) ?? false) 
            return true;

        return false;
    }

    /// <summary>
    /// since we use scriptable objects, this prevents it to change every time we run the project.
    /// </summary>
    public void OnApplicationQuit()
    {
        Inventory?.ClearInventory();
    }
}

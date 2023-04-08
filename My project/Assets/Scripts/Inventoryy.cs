using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventoryy : MonoBehaviour
{
    public List<Item> playerItems = new List<Item>();
    public ItemDatabase itemDatabase;
    public UIInventory inventoryUI;

    private void Start(){
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.I)){
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
            itemDatabase.toString();
        } 
    }

    public void GiveItem(int id){
        Item itemToAdd = itemDatabase.GetItem(id);
        playerItems.Add(itemToAdd);
        inventoryUI.AddNewItem(itemToAdd);
        Debug.Log("Added item in the inventory : " + itemToAdd.name);

    }

    public void GiveItem(string name){
        Item itemToAdd = itemDatabase.GetItem(name);
        playerItems.Add(itemToAdd);
        inventoryUI.AddNewItem(itemToAdd);
        Debug.Log("Added item in the inventory : " + itemToAdd.name);

    }

    public Item CheckForItem(int id){
        return playerItems.Find(item => item.id == id);
    }

    public void RemoveItem(int id){
        Item itemToRemove = CheckForItem(id);
        if(itemToRemove != null){
            playerItems.Remove(itemToRemove);
            inventoryUI.RemoveItem(itemToRemove);
            Debug.Log("Item removed : " + itemToRemove.name);
        }
    }
}

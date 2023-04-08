using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    private void Awake(){
        buildDatabase();
    }

    public Item GetItem(int id){
        return items.Find(item => item.id == id);
    }

    public Item GetItem(string name){
        return items.Find(item => item.name == name);
    }

    void buildDatabase(){
        items = new List<Item>(){
            new Item(0,"pickaxe","Can break stone",
            new Dictionary<string,int>
            {
                {"mining",10},
                {"defence",0}

            }),
            new Item(1,"sword","Can kill enemies",
            new Dictionary<string,int>
            {
                {"power",10},
                {"defence",1}

            }),
            new Item(2,"chicken","Juicy. Really good.",
            new Dictionary<string,int>
            {
                {"health",10},

            })

        };

    }

    public void toString(){
        string name = GetItem(1).name;
        Debug.Log("name " + name);
    }
}

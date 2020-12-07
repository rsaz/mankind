using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    private void Awake()
    {
        BuildDatabase();
    }

    public Item GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }

    public Item GetItem(string itemName)
    {
        return items.Find(item => item.title == itemName);
    }

    void BuildDatabase()
    {
        items = new List<Item>() {
                new Item(0, "Diamond Sword", "A sword made with diamond.",
                    new Dictionary<string, int>
                    {
                        {"Power", 15},
                        {"Defence", 10},
                        {"Value", 600 }
                    }),
                new Item(1, "Diamond Ore", "A beautiful diamond.",
                    new Dictionary<string, int>
                    {
                        {"Value", 444}
                    }),
                new Item(2, "Silver Pick", "A very nice pick.",
                    new Dictionary<string, int>
                    {
                        {"Power", 15},
                        {"Mining", 333},
                        {"Value", 50 }
                    }),
            };
    }
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items;

    public Item GetItemById(int id)
    {
        return items.Find(item => item.id == id);
    }
}

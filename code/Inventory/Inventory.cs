using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox;



public partial class Inventory
{
    static List<Inventory> All = new();
    public Pawn Owner { get; set; }
    List<Item> ItemList = new();

    // Assigned ID
    public static int ID;

    public Inventory() 
    {
        All.Add(this);
    }
    
    ~Inventory()
    {
        All.Remove(this);
    }

    public void PickupItem(Item item)
    {
        Log.Info("Added item to inventory.");
        ItemList.Add(item);
    }

    public void DropItem(Item item)
    {
        Log.Info("Removed item from inventory.");
        ItemList.Remove(item);
    }

    public void EquipItem(Item item)
    {
        item.OnEquip(Owner);
    }

    public void SpawnRun()
    {
        foreach (Item item in ItemList) 
            item.OnSpawn(Owner);
    }

    public void DeathRun()
    {
        foreach (Item item in ItemList)
            item.OnDeath(Owner);
    }

    public List<Inventory> GetAllInventories()
    {
        return All;
    }

    public List<Item> GetItems()
    {
        return ItemList;
    }
}

public partial class Item {
    public string Name {get; set;}
    public string Description {get; set;}
    public string Model {get; set;}
    public ClothingResource ClothingResource {get; set;}

    public Item() {}

    // Intended use of this class, but can be manually filled with the use of the constructor above.
    public Item(ClothingResource resource)
    {
        Name = resource.Name;
        Description = resource.Description;
        Model = resource.Model;
        ClothingResource = resource;
    }

    public virtual void OnSpawn(Pawn pawn) 
    {
        // Assign values and update model.
        if (ClothingResource != null)
        {
            Log.Info("Equipped Clothing Item");
        }
    }
    public virtual void OnDeath(Pawn pawn) {}
    public virtual void OnEquip(Pawn pawn) {}
    public virtual void OnUnequip(Pawn pawn) {}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceElement : InteractiveElement
{
    public int LP = 10; //Loot points, por cada punto que resta es un loot 
    public List<LootRate> Loot = new List<LootRate>(); //Posible loot chances
    public ResourceElementEventChannelSO OnResourceElementInteracted;
    public bool LootFinished = false;
    public Sprite LootFinishedSprite;
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);

        if (OnResourceElementInteracted != null)
            OnResourceElementInteracted.RaiseEvent(this);
    }
    public (ItemSlot, ItemSlot, int) HitResource(int hitPoints, Inventory targetInventory)
    {
        if (LootFinished) return (null, null, 0);

        if (LP - hitPoints < 0)
        {
            hitPoints = LP;
        }
        LP -= hitPoints;
        ItemSlot newLoot = GetLoot(hitPoints);
        //Loot that is remaining because of space lack in inventory
        ItemSlot remainingLoot = new();
        if (newLoot != null)
            remainingLoot = AddLootToInventory(targetInventory, newLoot);

        if (LP <= 0)
        {
            //Finish resource loot
            LootFinished = true;
            GetComponent<SpriteRenderer>().sprite = LootFinishedSprite;
        }
        return (newLoot, remainingLoot, hitPoints);
    }
    private ItemSlot GetLoot(int lp)
    {
        int maxNumber = 0;
        foreach (LootRate rate in Loot)
        {
            maxNumber += rate.ChanceRate;
        }
        int lootNumber = Random.Range(0, maxNumber);
        for (int i = 0; i < Loot.Count; i++)
        {
            if (lootNumber < Loot[i].ChanceRate)
            {
                //lp = loot point done
                return new ItemSlot(Loot[i].ItemType, Loot[i].Amount * lp);
            }
            else
            {
                lootNumber -= Loot[i].ChanceRate;
                maxNumber -= Loot[i].ChanceRate;
            }
        }
        return null;
    }
    private ItemSlot AddLootToInventory(Inventory inventoryTarget, ItemSlot item)
    {
        if (inventoryTarget == null) return null;
        int remaining = inventoryTarget.AddNewItem(item);
        if (remaining != item.Amount)
        {
            Debug.Log($"{inventoryTarget.Inv_Name} has loted {item.Amount - remaining} {item.ItemInfo.i_Name}");
        }
        if (remaining > 0)
        {
            if (remaining == item.Amount)
            {
                Debug.Log($"No slot free in {inventoryTarget.Inv_Name} inventory");
                return item;
            }
            else
            {
                Debug.Log($"Not enough space in {inventoryTarget.Inv_Name} inventory, only {item.Amount - remaining} added");
                return new ItemSlot(item.ItemInfo, remaining);
            }
        }
        return null;
    }
    [System.Serializable]
    public class LootRate
    {
        public ItemsSO ItemType;
        public int Amount;
        public int ChanceRate = 10;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceElement : InteractiveElement
{
    public int LP; //Loot points
    public List<LootRate> Loot = new List<LootRate>(); //Posible loot chances

    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        ItemSlot newLoot = GetLoot();
        if (newLoot != null)
            AddLootToPlayerInventory(character, GetLoot());

    }

    private ItemSlot GetLoot()
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
                return new ItemSlot(Loot[i].ItemType, Loot[i].Amount);
            }
            else
            {
                lootNumber -= Loot[i].ChanceRate;
                maxNumber -= Loot[i].ChanceRate;
            }
        }
        return null;
    }
    private void AddLootToPlayerInventory(CharacterInfo character, ItemSlot item)
    {
        if (character == null) return;
        int remaining = character.PlayerInventory.AddNewItem(item);
        if (remaining != item.Amount)
        {
            Debug.Log($"Player has loted {item.Amount - remaining} {item.ItemInfo.i_Name}");
        }
        if (remaining > 0)
        {
            if (remaining == item.Amount)
            {
                Debug.Log($"No slot free in player inventory");
            }
            else
            {
                Debug.Log($"Not enough space in player inventory, only {item.Amount - remaining} added");
            }
        }
    }
    [System.Serializable]
    public class LootRate
    {
        public ItemsSO ItemType;
        public int Amount;
        public int ChanceRate = 10;
    }
}

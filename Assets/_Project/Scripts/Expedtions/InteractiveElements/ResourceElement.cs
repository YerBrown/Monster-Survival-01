using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
[Serializable]
public class ResourceElement : InteractiveElement
{
    [SerializeField]
    public int LootPoints = 10;
    public List<LootRate> Loot = new(); // Posible loot chances.
    public ResourceSO ResourceInfo; // Resource information scriptableObject.
    public List<ResourceElement> ResourcePack = new(); // If the resource is part of a bigger one, this list contains all the resources that make up one.
    public Sprite LootFinishedSprite; // The sprite for empty resource.
    public SpriteRenderer ResourceMainRenderer; // The sprite renderer of the resource.
    public ResourceElementEventChannelSO ResourceElementInteracted; //Event triggered when the player interacts with the resource.

    [System.Serializable]
    public class LootRate
    {
        public ItemsSO ItemType;
        public int Amount;
        public int ChanceRate = 10;
    }

    private void Start()
    {
        // Update cursor color for resource elements.
        ChangeCursorColor("#FF9600");
        // Check with the resource info the empty state sprite.
        if (ResourceInfo != null && ResourceInfo.R_EmptySprite != null)
            LootFinishedSprite = ResourceInfo.R_EmptySprite;
    }
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        // Check if the lp is more than 0 to display the ui elemnts of looting.
        if (LootPoints > 0)
        {
            if (ResourceElementInteracted != null)
                ResourceElementInteracted.RaiseEvent(this);
        }
    }
    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        // Check if the instance is of the same type.
        if (data is ExpeditionData.ResourceData)
        {
            base.UpdateElement(data);
            LootPoints = ((ExpeditionData.ResourceData)data).LP;
            CheckLootPoints(false); //Check the llootpoints 
        }
        else
        {
            Debug.LogWarning("Can not be updated from another type of element.");
        }
    }
    // Hit the resource and returns the loot obtained.
    public (ItemSlot, ItemSlot, int) HitResource(int hitPoints, Inventory targetInventory, Transform targetTransform)
    {
        if (LootPoints == 0) { return (null, null, 0); };
        // TODO: Play hit animation
        // Check if this resource is part from a biger one
        if (ResourcePack.Count > 0)
        {
            foreach (var resource in ResourcePack)
            {
                resource.ChangeLP(hitPoints);
            }
        }
        else
        {
            ChangeLP(hitPoints);
        }
        ItemSlot newLoot = GetLoot(hitPoints);
        // Loot that is remaining because of space lack in inventory.
        ItemSlot remainingLoot = new();
        if (newLoot != null)
        {
            remainingLoot = AddLootToInventory(targetInventory, newLoot);
        }
        // Remaining loot add to droped container.
        if (remainingLoot != null && remainingLoot.Amount > 0)
        {
            SpawnDropItemsContainer(targetTransform.position, remainingLoot);
        }
        ShowLootText(newLoot);
        CheckLootPoints(true);
        return (newLoot, remainingLoot, hitPoints);
    }
    // Change the lp of the resource.
    public void ChangeLP(int hitPoints)
    {
        if (LootPoints - hitPoints < 0)
        {
            hitPoints = LootPoints;
        }
        LootPoints -= hitPoints;
    }
    // Check the loop points to set the empty sprite 
    private void CheckLootPoints(bool duringLoot)
    {
        // If loot points is 0, disable resource 
        if (LootPoints <= 0)
        {
            IsBlockingMovement = false;
            if (LootFinishedSprite != null)
            {
                ResourceMainRenderer.sprite = LootFinishedSprite;
            }
            else
            {
                gameObject.SetActive(false);
            }
            // Disable ui if player was looting the resource
            if (duringLoot)
            {
                if (ResourceElementInteracted != null)
                    ResourceElementInteracted.RaiseEvent(this);
            }
            this.enabled = false;
        }
    }
    // Returns the loot by chance of the loots posible list.
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
    // Adds the loot obtained in the inventory target.
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

    private RaycastHit2D? DetectOverlayTile(Vector3 checkPosition)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(checkPosition, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }
    private void SpawnDropItemsContainer(Vector3 targetPos, ItemSlot dropedSlot)
    {
        var overlayDetected = DetectOverlayTile(targetPos);
        if (overlayDetected.HasValue)
        {
            OverlayTile targetPositionOverlay = overlayDetected.Value.collider.gameObject.GetComponent<OverlayTile>();
            DropedItemsContainerElement dropedContainer = null;
            if (targetPositionOverlay.I_Element != null && targetPositionOverlay.I_Element is DropedItemsContainerElement)
            {
                dropedContainer = targetPositionOverlay.I_Element.GetComponent<DropedItemsContainerElement>();
            }
            else if (targetPositionOverlay.I_Element == null)
            {
                dropedContainer = MapManager.Instance.AddLootBag(targetPos);
            }

            if (dropedContainer == null) return;
            dropedContainer.AddNewItem(dropedSlot);
        }
    }
    private void ShowLootText(ItemSlot loot)
    {
        Notify(transform.position, $"+ {loot.Amount} {loot.ItemInfo.i_Name}");
    }
}

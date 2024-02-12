using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerElement : InteractiveElement
{
    public Inventory ContainerInventory;
    public SpriteRenderer Renderer;

    public Sprite FullContainer;
    public Sprite EmptyContainer;
    public override void Interact(CharacterInfo characterInfo = null)
    {
        base.Interact();
        if (characterInfo == null) return;
        characterInfo.OpenContainer(ContainerInventory);
        Renderer.sprite = EmptyContainer;
    }
    private void CheckInventoryItems()
    {
        if (ContainerInventory == null) return;
        if (ContainerInventory.Slots.Count > 0)
        {
            Renderer.sprite = FullContainer;
        }
        else
        {
            Renderer.sprite = EmptyContainer;
        }
    }
}

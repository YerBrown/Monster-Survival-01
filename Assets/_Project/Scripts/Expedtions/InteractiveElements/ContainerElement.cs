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
        if (EmptyContainer != null) 
            Renderer.sprite = EmptyContainer;
    }
}

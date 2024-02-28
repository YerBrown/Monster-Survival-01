using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class BlockerElement : InteractiveElement
{
    public SpriteRenderer BlockerRenderer;
    public ItemsSO Unblocker;
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        if (character != null)
        {
            if (BlockMovement == true)
            {
                if (character.PlayerInventory.GetAmountOfType(Unblocker) > 0)
                {
                    DisableBlocker();
                    character.PlayerInventory.RemoveItemOfType(Unblocker, 1);
                    Debug.Log($"The Player has used the object \"{Unblocker.i_Name}\" to unlock the path");
                }
                else
                {
                    Debug.Log($"The Player needs \"{Unblocker.i_Name}\" to unlock the path");
                }
            }
        }
    }
    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        //Check if the instance is of the same type
        if (data is ExpeditionData.BlockerData)
        {
            base.UpdateElement(data);
            SetActive(((ExpeditionData.BlockerData)data).Blocks);
        }
        else
        {
            Console.WriteLine("cannot update from a different type element.");
        }
    }
    private void Start()
    {
        ChangeCursorColor("#2D2D2D");
    }
    public void SetActive(bool active)
    {
        if (active)
        {
            EnableBlocker();

        }
        else
        {
            DisableBlocker();
        }
    }
    private void DisableBlocker()
    {
        BlockMovement = false;
        if (BlockerRenderer != null)
        {
            BlockerRenderer.enabled = false;
        }
    }
    private void EnableBlocker()
    {
        BlockMovement = true;
        if (BlockerRenderer != null)
        {
            BlockerRenderer.enabled = true;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BlockerElement : InteractiveElement
{
    [SerializeField] SpriteRenderer _blockerRenderer;
    [SerializeField] ItemsSO _unblockerItem;

    [SerializeField] List<BlockerElement> _blockerSet = new List<BlockerElement>();
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        if (character != null)
        {
            if (IsBlockingMovement == true && _unblockerItem != null)
            {
                if (character.PlayerInventory.GetAmountOfType(_unblockerItem) > 0)
                {
                    // Check if this blocker is part from another one biger
                    if (_blockerSet.Count > 0)
                    {
                        foreach (var blocker in _blockerSet)
                        {
                            blocker.DisableBlocker();
                        }
                    }
                    else
                    {
                        DisableBlocker();
                    }
                    character.PlayerInventory.RemoveItemOfType(_unblockerItem, 1);
                    Debug.Log($"The Player has used the object \"{_unblockerItem.i_Name}\" to unlock the path");
                    Notify(transform.position, $"\"{_unblockerItem.i_Name}\" used to unlock");
                }
                else
                {
                    Debug.Log($"The Player needs \"{_unblockerItem.i_Name}\" to unlock the path");
                    Notify(transform.position, $"\"{_unblockerItem.i_Name}\" needed to unlock");
                }
            }
        }
    }
    /// <summary>
    /// Update de blocker variables with the data provided
    /// </summary>
    /// <param name="data">Data provided with blocker info</param>
    public override void UpdateElement(ExpeditionData.ParentData data)
    {
        //Check if the instance is of the same type
        if (data is ExpeditionData.BlockerData)
        {
            base.UpdateElement(data);
            SetActive(((ExpeditionData.BlockerData)data).Blocks);
            EnableElement(true);
        }
        else
        {
            Console.WriteLine("cannot update from a different type element.");
        }
    }
    private void Start()
    {
        // Sets the cursor color for blocker elements
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
        IsBlockingMovement = false;
        if (_blockerRenderer != null)
        {
            _blockerRenderer.enabled = false;
        }
    }
    private void EnableBlocker()
    {
        IsBlockingMovement = true;
        if (_blockerRenderer != null)
        {
            _blockerRenderer.enabled = true;
        }
    }
}

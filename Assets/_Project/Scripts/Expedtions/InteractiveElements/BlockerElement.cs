using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockerElement : InteractiveElement
{
    public SpriteRenderer BlockerRenderer;
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
        if(BlockerRenderer != null)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSpawnController : MonoBehaviour
{
    public SpriteRenderer FighterSpriteRenderer;
    public void EnableSpriteRenderer()
    {
        FighterSpriteRenderer.enabled = true;
    }
    public void DisableSpriteRenderer()
    {
        FighterSpriteRenderer.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchElement : InteractiveElement
{
    public bool SwitchOn = true;
    public List<InteractiveElement> SwitchTargets = new();
    public SpriteRenderer Renderer;
    public Sprite SwitchOnSprite;
    public Sprite SwitchOffSprite;
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        SwitchOn = !SwitchOn;
        Renderer.sprite = SwitchOn ? SwitchOnSprite : SwitchOffSprite;
        if (SwitchTargets.Count == 0) return;
        foreach (var target in SwitchTargets)
        {
            if (target == null) continue;
            if (target is BlockerElement)
            {
                BlockerElement blockTarget = (BlockerElement)target;
                blockTarget.SetActive(SwitchOn);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SwitchElement : InteractiveElement
{
    public bool SwitchOn = true;
    public List<InteractiveElement> SwitchTargets = new();
    public SpriteRenderer Renderer;
    public Sprite SwitchOnSprite;
    public Sprite SwitchOffSprite;
    private void Start()
    {
        ChangeCursorColor("#C400FF");
    }
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
    //public override void UpdateElement(InteractiveElement element)
    //{
    //    //Check if the instance is of the same type
    //    if (element is SwitchElement)
    //    {
    //        base.UpdateElement(element);
    //        SwitchOn = ((SwitchElement)element).SwitchOn;
    //    }
    //    else
    //    {
    //        Console.WriteLine("cannot update from a different type element.");
    //    }
    //}
}

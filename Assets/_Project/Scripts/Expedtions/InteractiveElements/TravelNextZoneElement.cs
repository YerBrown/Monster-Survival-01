using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelNextZoneElement : InteractiveElement
{
    public Vector2Int TravelDistance = Vector2Int.zero;
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        //Travel to next zone
        MapManager.Instance.ChangeField(TravelDistance);
    }
}

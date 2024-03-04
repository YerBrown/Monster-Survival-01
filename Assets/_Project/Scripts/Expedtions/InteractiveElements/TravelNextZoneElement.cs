using UnityEngine;
public class TravelNextZoneElement : InteractiveElement
{
    public Vector2Int TravelDirection = Vector2Int.zero;
    public int StartPointSide = 0; // Number in list of the startpoints from an speific side, 0 = left side, 1 = right side 
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        // Travel to next zone
        MapManager.Instance.ChangeField(TravelDirection, StartPointSide);
    }
}
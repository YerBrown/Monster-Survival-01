using UnityEngine;
/// <summary>
/// A Scriptable Object-based event that passes an item and amount as a payload.
/// </summary>
[CreateAssetMenu(fileName = "ItemSlotEventChannel", menuName = "Events/ItemSlotChannelSO")]
public class ItemSlotEventChannelSO : GenericEventChannelSO<ItemSlot>
{

}

using UnityEngine;
/// <summary>
/// A Scriptable Object-based event that passes a pair of inventories as a payload.
/// </summary>
[CreateAssetMenu(fileName = "PairInventoriesEventChannel", menuName = "Events/PairInventoriesChannelSO")]
public class PairInventoriesEventChannelSO : GenericEventChannelSO<PairInventories>
{
}

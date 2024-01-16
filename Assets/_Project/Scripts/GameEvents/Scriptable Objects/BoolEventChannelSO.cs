using UnityEngine;

/// <summary>
/// A Scriptable Object-based event that passes a bool as a payload.
/// </summary>
[CreateAssetMenu(fileName = "BoolEventChannel", menuName = "Events/BoolEventChannelSO")]
public class BoolEventChannelSO : GenericEventChannelSO<bool>
{
}
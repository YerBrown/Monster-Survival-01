using UnityEngine;
/// <summary>
/// A Scriptable Object-based event that passes a fighter data.
/// </summary>

[CreateAssetMenu(fileName = "FighterDataEventChannel", menuName = "Events/FighterDataChannelSO")]
public class FighterDataEventChannelSO : GenericEventChannelSO<FighterData>
{
}

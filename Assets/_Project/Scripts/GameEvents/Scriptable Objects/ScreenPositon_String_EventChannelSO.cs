using UnityEngine;
/// <summary>
/// A Scriptable Object-based event that passes an item and amount as a payload.
/// </summary>
[CreateAssetMenu(fileName = "ScreenPosition+StringEventChannel", menuName = "Events/Screen+StringChannelSO")]
public class ScreenPositon_String_EventChannelSO : GenericEventChannelSO<(Vector3, string)>
{

}

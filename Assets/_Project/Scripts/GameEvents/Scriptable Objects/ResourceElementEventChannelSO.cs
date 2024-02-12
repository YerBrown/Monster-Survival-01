using UnityEngine;
/// <summary>
/// A Scriptable Object-based event that passes a Resource Element as a payload.
/// </summary>
[CreateAssetMenu(fileName = "ResourceElementEventChannel", menuName = "Events/ResourceElementChannelSO")]
public class ResourceElementEventChannelSO : GenericEventChannelSO<ResourceElement>
{
}

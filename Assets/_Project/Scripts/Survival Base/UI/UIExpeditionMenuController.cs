using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIExpeditionMenuController : MonoBehaviour
{
    public BoolEventChannelSO OnOpenMenuPopup;
    public List<ExpeditionMapSO> Maps = new();
    public ExpeditionMapSO SelectedMap;
    public GameObject MenuParent;
    public GameObject PoppupParent;
    public void OpenMenu()
    {
        MenuParent.SetActive(true);
        OnOpenMenuPopup.RaiseEvent(true);
    }
    public void CloseMenu()
    {
        MenuParent.SetActive(false);
        OnOpenMenuPopup.RaiseEvent(false);
    }
    public void OpenConfirmPopup(int mapSelected)
    {
        SelectedMap = Maps[mapSelected];
        PoppupParent.SetActive(true);
    }
    public void CloseConfirmButton()
    {
        PoppupParent.SetActive(false);
    }

    public void ConfirmExpeditionDestination()
    {
        SceneLoadManager.Instance.LoadExpeditionFromSurvivalBase(SelectedMap);
    }
}

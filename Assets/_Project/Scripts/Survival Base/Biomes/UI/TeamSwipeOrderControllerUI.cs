using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSwipeOrderControllerUI : BiomeTeamPanelControllerUI
{
    // Swipe order 
    public int FirstIndex = 0;
    public int SecondIndex = 0;
    public GameObject BackgroundTrigger;
    protected override void OnEnable()
    {
        base.OnEnable();
        DisableAllFrames();
        BackgroundTrigger.SetActive(true);
        FirstIndex = 0;
        SecondIndex = 0;
    }
    private void OnDisable()
    {
        BackgroundTrigger.SetActive(false);
    }
    public void SelectCreatureToSwipe(int index)
    {
        if (PlayerManager.Instance.Team[index] != null && !string.IsNullOrEmpty(PlayerManager.Instance.Team[index].ID))
        {
            if (FirstIndex > 0)
            {
                SecondIndex = index;
            }
            else
            {
                FirstIndex = index;
                EnableFrameByIndex(FirstIndex);
            }
            if (FirstIndex > 0 && SecondIndex > 0)
            {
                if (FirstIndex != SecondIndex)
                {
                    PlayerManager.Instance.SwipeTeamOrder(FirstIndex, SecondIndex);
                }
                FirstIndex = 0;
                SecondIndex = 0;
                DisableAllFrames();
                UpdateUI();
            }
        }
    }
}

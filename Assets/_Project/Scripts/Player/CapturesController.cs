using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class CapturesController
{
    public List<FighterData> CaptureSlots = new();
    public int CurrentMaxIntensity = 1;
    public bool TryAddNewCreature(FighterData newFighter)
    {
        for (int i = 0; i < CaptureSlots.Count; i++)
        {
            if (CaptureSlots[i] == null || string.IsNullOrEmpty(CaptureSlots[i].ID))
            {
                CaptureSlots[i] = newFighter;
                return true;
            }
        }
        return false;
    }
    public int GetRemainingCapsules()
    {
        int remaining = 0;
        for (int i = 0; i < CaptureSlots.Count; i++)
        {
            if (CaptureSlots[i] == null || string.IsNullOrEmpty(CaptureSlots[i].ID))
            {
                remaining++;
            }
        }
        return remaining;
    }
}

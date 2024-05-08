using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public BuildingSO BuildingInfo;
    public int Level;
    public bool InProgress;
    public DateTime StartBuildingDateTime;
    public DateTime FinishBuildingDateTime;
    private Coroutine _UpdateCoroutine;
    public void StartUpdate(DateTime finishDateTime)
    {
        InProgress = true;
        StartBuildingDateTime = RealDateTimeManager.Instance.GetCurrentDateTime();
        FinishBuildingDateTime = finishDateTime;
        StartUpdateCoroutine();
        // Save data 
    }

    private void StartUpdateCoroutine()
    {
        _UpdateCoroutine = StartCoroutine(UpdateBuilding());
    }

    IEnumerator UpdateBuilding()
    {
        while (InProgress)
        {
            DateTime intermediate = RealDateTimeManager.Instance.GetCurrentDateTime();
            // Update ui
            TimeSpan totalTimeSpan = FinishBuildingDateTime - StartBuildingDateTime;
            TimeSpan intermediateTimeSPan = intermediate - StartBuildingDateTime;

            double percentage = (intermediateTimeSPan.TotalSeconds / totalTimeSpan.TotalSeconds);
            Debug.Log("Update time progress: " + percentage.ToString("00.00") + " Start Date Time: " + StartBuildingDateTime.ToString() + " Finish Date Time: " + FinishBuildingDateTime.ToString() + " Current Date Time: " + intermediate.ToString());

            if (intermediate > FinishBuildingDateTime)
            {
                // Update finished
                Level++;
                InProgress = false;
                yield break;
            }
            yield return new WaitForSeconds(1f);
            yield return null;
        }
    }
}

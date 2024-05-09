using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    public BuildingSO BuildingInfo;
    public BuildAreaController ParentArea;
    public int Level;
    public bool InProgress;
    public DateTime StartBuildingDateTime;
    public DateTime FinishBuildingDateTime;
    private Coroutine _UpdateCoroutine;

    [Header("UI")]
    // Upgrade/ Build Time Remaining UI
    public CanvasGroup RemainingTimeUI;
    public Slider RemainingTimeSlider;
    public TMP_Text RemainingTimeText;
    // Identification
    public CanvasGroup BuildingSelectedUI;
    public TMP_Text BuildingNameText;
    public TMP_Text BuildingLevelText;
    private Sequence SelectBuildingSequence;
    public void CheckCurrentBuildingState(SurvivalBaseData.BuildingData buildingData)
    {
        BuildingInfo = BuildingInfoWiki.Instance.GetBuildingByID(buildingData.Building_ID);
        Level = buildingData.Level;
        InProgress = buildingData.InProgress;
        StartBuildingDateTime = DateTime.ParseExact(buildingData.StartBuildingDateTime, "o", CultureInfo.InvariantCulture);
        FinishBuildingDateTime = DateTime.ParseExact(buildingData.FinishBuildingDateTime, "o", CultureInfo.InvariantCulture);
        if (InProgress)
        {
            DateTime currentDateTime = RealDateTimeManager.Instance.GetCurrentDateTime();
            if (currentDateTime > FinishBuildingDateTime)
            {
                Level++;
                InProgress = false;
                CampManager.Instance.SaveBuildAreaData(ParentArea);
            }
            else
            {
                StartUpdateCoroutine();
            }
        }
    }
    public void StartUpdate(DateTime finishDateTime)
    {
        InProgress = true;
        StartBuildingDateTime = RealDateTimeManager.Instance.GetCurrentDateTime();
        FinishBuildingDateTime = finishDateTime;
        StartUpdateCoroutine();
        // Save data 
        CampManager.Instance.SaveBuildAreaData(ParentArea);
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
            TimeSpan totalTimeSpan = FinishBuildingDateTime - StartBuildingDateTime;
            TimeSpan intermediateTimeSpan = intermediate - StartBuildingDateTime;
            UpdateRemainingTimeUI(totalTimeSpan, intermediateTimeSpan);
            //Debug.Log($"Total: {(float)totalTimeSpan.TotalSeconds}, Progress: {(float)intermediateTimeSpan.TotalSeconds} ");
            //Debug.Log("Update time progress: " + percentage.ToString("00.00") + " Start Date Time: " + StartBuildingDateTime.ToString() + " Finish Date Time: " + FinishBuildingDateTime.ToString() + " Current Date Time: " + intermediate.ToString());

            if (intermediate > FinishBuildingDateTime)
            {
                // Update finished
                Level++;
                BuildingLevelText.text = $"Level {Level}";
                InProgress = false;
                CampManager.Instance.SaveBuildAreaData(ParentArea);
                RemainingTimeUI.DOFade(0, 1f).OnComplete(() =>
                {
                    RemainingTimeUI.gameObject.SetActive(false);
                });

                yield break;
            }
            yield return new WaitForSeconds(1f);
            yield return null;
        }
    }

    private void UpdateRemainingTimeUI(TimeSpan totalTimeSpan, TimeSpan currentTimeSpan)
    {
        TimeSpan currentTimeSpanMiliseconds = new TimeSpan(0, 0, 0, 0, currentTimeSpan.Milliseconds);
        currentTimeSpan -= currentTimeSpanMiliseconds;

        RemainingTimeSlider.maxValue = (int)totalTimeSpan.TotalSeconds;
        RemainingTimeSlider.value = (int)currentTimeSpan.TotalSeconds;

        TimeSpan remainingTimeSpan = totalTimeSpan - currentTimeSpan;
        StringBuilder remainingTimeStringBuilder = new StringBuilder();
        if (remainingTimeSpan.Days > 0)
        {
            remainingTimeStringBuilder.Append($"{remainingTimeSpan.Days}d ");
        }
        if (remainingTimeSpan.Hours > 0)
        {
            remainingTimeStringBuilder.Append($"{remainingTimeSpan.Hours}h ");
        }
        if (remainingTimeSpan.Minutes > 0)
        {
            remainingTimeStringBuilder.Append($"{remainingTimeSpan.Minutes}min ");
        }
        if (remainingTimeSpan.Seconds > 0 || (RemainingTimeSlider.maxValue <= RemainingTimeSlider.value))
        {
            remainingTimeStringBuilder.Append($"{remainingTimeSpan.Seconds}s");
        }
        RemainingTimeText.text = remainingTimeStringBuilder.ToString();
        if (RemainingTimeUI.alpha == 0)
        {
            RemainingTimeUI.gameObject.SetActive(true);
            RemainingTimeUI.DOFade(1, 1f);
        }
    }
    public void TriggerSelectBuilding()
    {
        CampManager.Instance.SelectBuilding(this);
    }
    public void SelectBuilding()
    {
        BuildingNameText.text = BuildingInfo.Name;
        BuildingLevelText.text = $"Level {Level}";

        if (SelectBuildingSequence != null)
        {
            SelectBuildingSequence.Kill();
        }
        SelectBuildingSequence = DOTween.Sequence();
        SelectBuildingSequence.Append(BuildingSelectedUI.DOFade(1f, 0.25f));
        ParentArea.SelectArea();
    }
    public void UnselectBuilding()
    {
        if (SelectBuildingSequence != null)
        {
            SelectBuildingSequence.Kill();
        }
        SelectBuildingSequence = DOTween.Sequence();
        SelectBuildingSequence.Append(BuildingSelectedUI.DOFade(0f, 0.25f));
        ParentArea.UnselectArea();
    }
}

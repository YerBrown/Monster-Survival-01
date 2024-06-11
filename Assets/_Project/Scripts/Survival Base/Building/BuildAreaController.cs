using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildAreaController : MonoBehaviour
{
    public string ID;
    public BuildingSO.BuildingSize Size;
    public SpriteRenderer HighlightRenderer;
    public BuildingController ChildBuildingController;
    private Tween HiglightSequence;
    public void TriggerSelectArea()
    {
        if (ChildBuildingController != null)
        {
            CampManager.Instance.SelectBuilding(ChildBuildingController);
        }
        else
        {
            CampManager.Instance.SelectBuildArea(this);
        }
    }
    public void SelectArea()
    {
        if (HiglightSequence != null)
        {
            HiglightSequence.Kill();
        }
        HighlightRenderer.color = new Color(HighlightRenderer.color.r, HighlightRenderer.color.g, HighlightRenderer.color.b, 0);
        HiglightSequence = HighlightRenderer.DOFade(0.2f, 0.25f).SetLoops(-1, LoopType.Yoyo);
    }
    public void UnselectArea()
    {
        if (HiglightSequence != null)
        {
            HiglightSequence.Kill();
        }
        HiglightSequence = HighlightRenderer.DOFade(0f, 0.5f);
    }
    private void OnDestroy()
    {
        if (HiglightSequence != null)
        {
            HiglightSequence.Kill();
        }
    }
}

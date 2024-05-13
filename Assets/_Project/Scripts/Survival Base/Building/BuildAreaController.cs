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
    private Sequence HiglightSequence;
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
        HiglightSequence = DOTween.Sequence();
        HiglightSequence.Append(HighlightRenderer.DOFade(0f, 0.1f));
        HiglightSequence.Append(HighlightRenderer.DOFade(0.2f, 0.25f).SetLoops(-1, LoopType.Yoyo));
    }
    public void UnselectArea()
    {
        if (HiglightSequence != null)
        {
            HiglightSequence.Kill();
        }
        HiglightSequence = DOTween.Sequence();
        HiglightSequence.Append(HighlightRenderer.DOFade(0f, 0.5f));
    }
}

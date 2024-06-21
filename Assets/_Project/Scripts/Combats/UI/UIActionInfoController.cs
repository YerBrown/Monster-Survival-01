using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIActionInfoController : MonoBehaviour
{
    [Header("UI")]
    public GameObject InfoPopup;
    public Image ActionImage;
    public Image ActionElement;
    public TMP_Text ActionNameText;
    public TMP_Text ActionTypeText;
    public TMP_Text ActionDescriptionText;
    public void OpenInfoPanel(Sprite imageSprite, string actionName, string actionType, ElementType element, string actionDescription)
    {
        ActionImage.sprite = imageSprite;
        if (MainWikiManager.Instance.GetElementSprite(element, out Sprite elementSprite))
        {
            ActionElement.sprite = elementSprite;
        }
        else
        {
            ActionElement.sprite = MainWikiManager.Instance.MissingSprite;
        }
        ActionNameText.text = actionName;
        ActionTypeText.text = actionType;
        ActionDescriptionText.text = actionDescription;
        InfoPopup.SetActive(true);
    }
    public void CloseInfoPanel()
    {
        InfoPopup.SetActive(false);
    }
}

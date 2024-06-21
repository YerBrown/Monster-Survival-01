using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamCreatureSlotControllerUI : MonoBehaviour
{
    public Image CreatureAvatar;
    public Image OneElementBackground;
    public Image OneElementIcon;
    public Image FirstElementBackground;
    public Image FirstElementIcon;
    public Image SecondElementBackground;
    public Image SecondElementIcon;
    public Image FrameImage;
    public TMP_Text CreatureText;

    public void UpdateSlot(Creature creature, ElementType oneElement)
    {
        EnableSlot(true);
        CreatureAvatar.sprite = creature.CreatureInfo.c_AvatarSprite;
        CreatureText.text = creature.Nickname;
        if (MainWikiManager.Instance.GetElementInfo(oneElement, out FightersInfoWiki.ElementInfoUI elementInfo))
        {
            OneElementBackground.color = elementInfo.BackgroundColor;
            OneElementIcon.color = elementInfo.ElementColor;
            OneElementIcon.sprite = elementInfo.ElementSprite;
        }
        else
        {
            OneElementBackground.color = Color.gray;
            OneElementIcon.color = Color.white;
            OneElementIcon.sprite = MainWikiManager.Instance.MissingSprite;
        }
        OneElementBackground.gameObject.SetActive(true);
        FirstElementBackground.gameObject.SetActive(false);
        SecondElementBackground.gameObject.SetActive(false);
    }
    public void UpdateSlot(Creature creature, ElementType firstElement, ElementType secondElement)
    {
        EnableSlot(true);
        CreatureAvatar.sprite = creature.CreatureInfo.c_AvatarSprite;
        CreatureText.text = creature.Nickname;
        if (MainWikiManager.Instance.GetElementInfo(firstElement, out FightersInfoWiki.ElementInfoUI firstElementInfo))
        {
            FirstElementBackground.color = firstElementInfo.BackgroundColor;
            FirstElementIcon.color = firstElementInfo.ElementColor;
            FirstElementIcon.sprite = firstElementInfo.ElementSprite;
        }
        else
        {
            FirstElementBackground.color = Color.gray;
            FirstElementIcon.color = Color.white;
            FirstElementIcon.sprite = MainWikiManager.Instance.MissingSprite;
        }
        if (MainWikiManager.Instance.GetElementInfo(secondElement, out FightersInfoWiki.ElementInfoUI secondElementInfo))
        {
            SecondElementBackground.color = secondElementInfo.BackgroundColor;
            SecondElementIcon.color = secondElementInfo.ElementColor;
            SecondElementIcon.sprite = secondElementInfo.ElementSprite;
        }
        else
        {
            SecondElementBackground.color = Color.gray;
            SecondElementIcon.color = Color.white;
            SecondElementIcon.sprite = MainWikiManager.Instance.MissingSprite;
        }
        OneElementBackground.gameObject.SetActive(false);
        FirstElementBackground.gameObject.SetActive(true);
        SecondElementBackground.gameObject.SetActive(true);
    }
    public void EnableSlot(bool enable)
    {
        if (enable)
        {
            CreatureAvatar.gameObject.SetActive(true);
            CreatureText.gameObject.SetActive(true);
        }
        else
        {
            OneElementBackground.gameObject.SetActive(false);
            FirstElementBackground.gameObject.SetActive(false);
            SecondElementBackground.gameObject.SetActive(false);
            CreatureAvatar.gameObject.SetActive(false);
            CreatureText.gameObject.SetActive(false);
        }
    }
    public void EnableFrame(bool enable)
    {
        FrameImage.gameObject.SetActive(enable);
    }
}

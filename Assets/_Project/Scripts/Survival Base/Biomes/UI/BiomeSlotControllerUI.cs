using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomeSlotControllerUI : MonoBehaviour
{
    [SerializeField] private BiomePanelControllerUI UIBiomeController;
    public Creature CreatureSlot;
    public Image CreatureAvatarImage;
    public Button SlotButton;
    private void Awake()
    {
        SlotButton.onClick.AddListener(SelectThisSlot);
    }

    public void SelectThisSlot()
    {
        UIBiomeController.OnBiomeSlotSelected.Invoke(this);
    }
    public void UpdateSlot(Creature addedCreature)
    {
        CreatureSlot = addedCreature;
        if (CreatureSlot != null)
        {
            if (MainWikiManager.Instance.GetCreatureInfo(CreatureSlot.CreatureInfo.c_Name, out CreatureSO creature))
            {
                CreatureAvatarImage.sprite = creature.c_AvatarSprite;
            }
            else
            {
                CreatureAvatarImage.sprite = MainWikiManager.Instance.MissingSprite;
            }
            CreatureAvatarImage.gameObject.SetActive(true);
            SlotButton.interactable = true;
        }
        else
        {
            CreatureAvatarImage.gameObject.SetActive(false);
            SlotButton.interactable = false;
        }
    }
    public void UnlockSlot()
    {
        SlotButton.targetGraphic.color = Color.white;
        SlotButton.interactable = true;
    }
    public void LockSlot()
    {
        SlotButton.targetGraphic.color = Color.gray;
        CreatureAvatarImage.gameObject.SetActive(false);
        SlotButton.interactable = false;
    }

}

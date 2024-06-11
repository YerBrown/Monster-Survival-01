using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBiomesMenuController : MonoBehaviour
{
    public GameObject MenuParent;
    private Creature _CurrentCreatureSelected;
    [Header("UI")]
    [Header("Player Team Panel")]
    [Header("Biomes Panel")]
    public BiomePanelControllerUI BiomeUIController;
    [Header("Selected Creature Panel")]
    public Image CreatureImage;
    public TMP_Text CreatureInfoText;
    public TMP_Text CreatureRaceText;
    public TMP_Text CreatureElementText;
    public TMP_Text CreatureStatsText;
    public void OpenMenu()
    {
        MenuParent.SetActive(true);
    }
    public void ClosePopup()
    {
        MenuParent.SetActive(false);
    }
    public void SelectCreature(Creature selectedCreature)
    {
        _CurrentCreatureSelected = selectedCreature;
        ShowSelectedCreatureInfo();
    }
    public void ShowSelectedCreatureInfo()
    {
        if (_CurrentCreatureSelected != null)
        {
            if (_CurrentCreatureSelected.CreatureInfo.c_Sprite != null)
            {
                CreatureImage.sprite = _CurrentCreatureSelected.CreatureInfo.c_Sprite;
            }
            else
            {
                CreatureImage.sprite = MainWikiManager.Instance.MissingSprite;
            }
            CreatureImage.enabled = true;
            CreatureInfoText.text = $"{_CurrentCreatureSelected.Nickname}\n{_CurrentCreatureSelected.CreatureInfo.c_Name}";
            CreatureStatsText.text = $"Health Point {_CurrentCreatureSelected.MaxHealthPoints} - Energy Points {_CurrentCreatureSelected.MaxEnergyPoints} \n Fisical Attack {_CurrentCreatureSelected.FisicalPower} - Range Attack {_CurrentCreatureSelected.RangePower}\n Defense {_CurrentCreatureSelected.Defense} - Speed {_CurrentCreatureSelected.Speed}";
        }
        else
        {
            CreatureImage.enabled = false;
            CreatureInfoText.text = "";
            CreatureStatsText.text = "";
        }
    }
}

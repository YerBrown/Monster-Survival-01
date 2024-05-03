using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UIResourceElementInteractionController : MonoBehaviour
{
    public ResourceElement ResourceTarget;
    public CharacterInfo PlayerTarget;
    public ResourceElementEventChannelSO OnResourceInteracted;
    public VoidEventChannelSO OnMovementGridPointed;
    public StringEventChannelSO OnCreatureFarm;

    private FighterData[] _CreaturesInTeam = new FighterData[0];
    [Header("UI")]
    public GameObject UIParent;
    public List<Button> TeamButtons = new();
    public Button LootButton;
    public Image PlayerHitButtonFillImage;
    public TMP_Text ResourceNameText;
    public List<LootRateSlot> LootRateSlots = new();
    public Slider ResourceHealthPointsSlider;
    public float HitCooldownTime = 2f;
    [Serializable]
    public class LootRateSlot
    {
        public GameObject SlotParent;
        public Image LootImage;
        public TMP_Text LootText;
    }
    void OnEnable()
    {
        OnResourceInteracted.OnEventRaised += OpenResourceMenu;
        OnMovementGridPointed.OnEventRaised += ClosePopup;
        LootButton.onClick.AddListener(HitResourceByPlayer);
    }

    void OnDisable()
    {
        OnResourceInteracted.OnEventRaised -= OpenResourceMenu;
        OnMovementGridPointed.OnEventRaised -= ClosePopup;
        LootButton.onClick.RemoveListener(HitResourceByPlayer);
    }
    private void OpenResourceMenu(ResourceElement newResource)
    {
        if (newResource == null) return;
        if (newResource.LootPoints > 0)
        {
            ResourceTarget = newResource;
            PlayerTarget = MapManager.Instance.Character;
            //TODO: Checkear el equipo del jugador
            UpdateTeamMembersButtons();
            UpdateLootRateTable();
            UIParent.SetActive(true);
        }
        else
        {
            ClosePopup();
        }
    }

    public void HitResourceByPlayer()
    {
        if (ResourceTarget == null || PlayerTarget == null) return;
        //PlayerTarget.PlayFisicalAttackAnim();
        (ItemSlot, ItemSlot, int) hitResourceAnswer = ResourceTarget.HitResource(PlayerTarget.ResourcesHitPower, PlayerTarget.PlayerInventory, PlayerTarget.transform);
        LootButton.interactable = false;
        CheckResourceHealthPoints();
        StartCoroutine(UpdateHitCooldown(LootButton, PlayerHitButtonFillImage));
    }
    public void ClosePopup()
    {
        ResourceTarget = null;
        UIParent.SetActive(false);
    }

    public void UpdateTeamMembersButtons()
    {
        _CreaturesInTeam = PlayerManager.Instance.GetTeamCreatures();
        for (int i = 0; i < TeamButtons.Count; i++)
        {
            if (i < _CreaturesInTeam.Length)
            {
                if (FightersInfoWiki.Instance.GetCreatureInfo(_CreaturesInTeam[i].TypeID, out CreatureSO creatureInfo))
                {
                    bool isFarmingPosible = creatureInfo.c_Skills.Contains(ResourceTarget.ResourceInfo.R_SkillNeeded);
                    TeamButtons[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = creatureInfo.c_AvatarSprite; ;
                    TeamButtons[i].transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = creatureInfo.c_AvatarSprite; ;
                    TeamButtons[i].interactable = isFarmingPosible;

                    TeamButtons[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(isFarmingPosible ? 1 : 0.25f, 0.25f);

                }
                TeamButtons[i].gameObject.SetActive(true);
            }
            else
            {
                TeamButtons[i].gameObject.SetActive(false);
            }
        }
    }
    public void FarmWithCreature(int index)
    {
        if (ResourceTarget == null || PlayerTarget == null) return;
        OnCreatureFarm.RaiseEvent(_CreaturesInTeam[index].ID);
        //PlayerTarget.PlayFisicalAttackAnim();
        (ItemSlot, ItemSlot, int) hitResourceAnswer = ResourceTarget.HitResource(PlayerTarget.ResourcesHitPower, PlayerTarget.PlayerInventory, PlayerTarget.transform);
        if (ResourceTarget != null)
        {
            CheckResourceHealthPoints();
            for (int i = 0; i < TeamButtons.Count; i++)
            {
                if (FightersInfoWiki.Instance.GetCreatureInfo(_CreaturesInTeam[i].TypeID, out CreatureSO creatureInfo))
                {
                    bool isFarmingPosible = creatureInfo.c_Skills.Contains(ResourceTarget.ResourceInfo.R_SkillNeeded);
                    if (isFarmingPosible)
                    {
                        StartCoroutine(UpdateHitCooldown(TeamButtons[i], TeamButtons[i].transform.GetChild(0).GetChild(1).GetComponent<Image>()));
                    }
                }
            }
        }
    }
    IEnumerator UpdateHitCooldown(Button button, Image icon)
    {
        float currentTime = 0f;
        icon.fillAmount = 0;
        icon.enabled = true;
        button.interactable = false;
        // Iterar hasta que haya pasado el tiempo de duración de la transición
        while (currentTime < HitCooldownTime)
        {
            // Calcular el valor actual en función del tiempo pasado
            float currentValue = Mathf.Lerp(1f, 0f, currentTime / HitCooldownTime);
            //Update image
            icon.fillAmount = currentValue;

            // Incrementar el tiempo pasado
            currentTime += Time.deltaTime;

            // Esperar un frame antes de continuar
            yield return null;
        }
        //Cooldown finished
        icon.fillAmount = 1;
        button.interactable = true;
        icon.enabled = false;
    }
    private void UpdateLootRateTable()
    {
        ResourceNameText.text = ResourceTarget.ResourceInfo.R_Name;
        CheckResourceHealthPoints();
        int sumOfRate = 0;
        foreach (var loot in ResourceTarget.Loot)
        {
            sumOfRate += loot.ChanceRate;
        }
        for (int i = 0; i < LootRateSlots.Count; i++)
        {
            if (i < ResourceTarget.Loot.Count)
            {
                ResourceElement.LootRate loot = ResourceTarget.Loot[i];
                float percentage = ((float)loot.ChanceRate / sumOfRate) * 100;
                int percentageRounded = (int)Mathf.Round(percentage);
                LootRateSlots[i].LootText.text = $"x{loot.Amount}\n%{percentageRounded}";
                LootRateSlots[i].LootImage.sprite = loot.ItemType.i_Sprite;
                LootRateSlots[i].SlotParent.SetActive(true);
            }
            else
            {
                LootRateSlots[i].SlotParent.SetActive(false);
            }
        }
    }
    private void CheckResourceHealthPoints()
    {

        if (ResourceTarget != null)
        {
            ResourceHealthPointsSlider.value = (float)ResourceTarget.LootPoints / ResourceTarget.MaxLootPoints;
        }
    }
}

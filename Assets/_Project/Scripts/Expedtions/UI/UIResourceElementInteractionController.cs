using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIResourceElementInteractionController : MonoBehaviour
{
    public ResourceElement ResourceTarget;
    public CharacterInfo PlayerTarget;
    public ResourceElementEventChannelSO OnResourceInteracted;

    [Header("UI")]
    public GameObject UIParent;
    public TMP_Text ResourceNameText;
    public Image ResourceImage;
    public Image CreatureImage;
    public List<Button> TeamButtons = new();
    public Button LootButton;
    public Button CloseButton;
    public TMP_Text PlayerHitResourceText;
    public RectTransform CreaturePos, PlayerPos;

    public Vector3 PunchAnimScale = Vector3.one;
    public float PunchAnimDuration = 1f;
    public Transform PlayerLootInfoParent;
    private List<LootInfo> PlayerLootInfos = new();


    void OnEnable()
    {
        OnResourceInteracted.OnEventRaised += OpenResourceMenu;
        LootButton.onClick.AddListener(HitResourceByPlayer);
        CloseButton.onClick.AddListener(ClosePopup);
    }

    void OnDisable()
    {
        OnResourceInteracted.OnEventRaised -= OpenResourceMenu;
        LootButton.onClick.RemoveListener(HitResourceByPlayer);
        CloseButton.onClick.RemoveListener(ClosePopup);
    }

    void Start()
    {
        //Set player loot info refs
        foreach (Transform child in PlayerLootInfoParent)
        {
            LootInfo newLootInfo = new LootInfo
            {
                LI_ItemText = child.GetComponentInChildren<TMP_Text>(),
                LI_ItemImage = child.GetComponentInChildren<Image>()
            };
            PlayerLootInfos.Add(newLootInfo);
        }
    }
    private void OpenResourceMenu(ResourceElement newResource)
    {
        if (newResource == null) return;
        ResourceTarget = newResource;
        if (ResourceTarget.LP == 0)
        {
            //Actualizar imagen para dar a entender que se han terminado los recurso de esa fuente
            ResourceImage.color = Color.red;
        }
        ResourceNameText.text = ResourceTarget.name;
        PlayerTarget = MapManager.Instance.Character;
        ResourceImage.sprite = ResourceTarget.GetComponent<SpriteRenderer>().sprite;
        //Checkear el equipo del jugador
        UIParent.SetActive(true);
        GeneralUIController.Instance.OpenMenu(true);
    }

    public void HitResourceByPlayer()
    {
        if (ResourceTarget == null || PlayerTarget == null) return;
        (ItemSlot, ItemSlot, int) hitResourceAnswer = ResourceTarget.HitResource(PlayerTarget.ResourcesHitPower, PlayerTarget.PlayerInventory);
        //AddPlayerHitText(hitResourceAnswer.Item3,hitResourceAnswer );
        if (ResourceTarget.LP == 0)
        {
            //Actualizar imagen para dar a entender que se han terminado los recurso de esa fuente
            ResourceImage.color = Color.red;
        }
    }

    private void AddPlayerHitText(int lp, ItemSlot loot)
    {
        if (loot == null) return;
        PlayerHitResourceText.text = $"-{lp} HP";
        PlayerHitResourceText.gameObject.SetActive(true);
        PlayerHitResourceText.gameObject.GetComponent<RectTransform>().DOPunchScale(PunchAnimScale, PunchAnimDuration);

        foreach (var lootInfo in PlayerLootInfos)
        {
            if (lootInfo.LI_ItemSlot != null && lootInfo.LI_ItemSlot.ItemInfo != null)
            {
                if (lootInfo.LI_ItemSlot.ItemInfo == loot.ItemInfo)
                {
                    lootInfo.LI_ItemSlot.Amount += loot.Amount;
                    lootInfo.LI_ItemText.text = $"+{lootInfo.LI_ItemSlot.Amount} {lootInfo.LI_ItemSlot.ItemInfo.i_Name}";
                    return;
                }
                else { continue; }
            }
            else
            {
                lootInfo.LI_ItemSlot = loot;
                lootInfo.LI_ItemText.text = $"+{lootInfo.LI_ItemSlot.Amount} {lootInfo.LI_ItemSlot.ItemInfo.i_Name}";
                lootInfo.LI_ItemImage.sprite = lootInfo.LI_ItemSlot.ItemInfo.i_Sprite;
                lootInfo.LI_ItemText.transform.parent.gameObject.SetActive(true);
                return;
            }

        }
        Debug.LogWarning("No loot info free");
    }
    public void ClosePopup()
    {
        ResourceTarget = null;
        UIParent.SetActive(false);
        ResourceImage.color = Color.white;
        ResetLootInfo();
        GeneralUIController.Instance.OpenMenu(false);
    }
    private void ResetLootInfo()
    {
        //Reset player loot info item slot
        foreach (var lootInfo in PlayerLootInfos)
        {
            lootInfo.LI_ItemSlot = null;
            lootInfo.LI_ItemText.transform.parent.gameObject.SetActive(false);
        }
    }
    [Serializable]
    private class LootInfo
    {
        public ItemSlot LI_ItemSlot;
        public TMP_Text LI_ItemText;
        public Image LI_ItemImage;
    }
}

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
    public VoidEventChannelSO OnMovementGridPointed;

    [Header("UI")]
    public GameObject UIParent;
    public List<Button> TeamButtons = new();
    public Button LootButton;
    public Image PlayerHitButtonFillImage;

    public float HitCooldownTime = 2f;


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
            //Checkear el equipo del jugador
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
        StartCoroutine(UpdateHitCooldown());
    }
    public void ClosePopup()
    {
        ResourceTarget = null;
        UIParent.SetActive(false);
    }


    IEnumerator UpdateHitCooldown()
    {
        float currentTime = 0f;
        PlayerHitButtonFillImage.fillAmount = 0;
        PlayerHitButtonFillImage.enabled = true;
        // Iterar hasta que haya pasado el tiempo de duración de la transición
        while (currentTime < HitCooldownTime)
        {
            // Calcular el valor actual en función del tiempo pasado
            float currentValue = Mathf.Lerp(0f, 1f, currentTime / HitCooldownTime);
            //Update image
            PlayerHitButtonFillImage.fillAmount = currentValue;

            // Incrementar el tiempo pasado
            currentTime += Time.deltaTime;

            // Esperar un frame antes de continuar
            yield return null;
        }
        //Cooldown finished
        PlayerHitButtonFillImage.fillAmount = 1;
        LootButton.interactable = true;
        PlayerHitButtonFillImage.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingCreatureController : MonoBehaviour
{
    public FighterData CurrentCreature;
    public Vector2Int FieldCoordinates;
    public Vector3 CreaturePosition;

    public SpriteRenderer CreatureRenderer;
    public Animator CreatureAnimator;

    public StringEventChannelSO OnTakeOutCreature;
    public VoidEventChannelSO OnStoreCreature;
    private void OnEnable()
    {
        OnTakeOutCreature.OnEventRaised += TakeOutCreature;
        OnStoreCreature.OnEventRaised += StoreCreature;
    }
    private void OnDisable()
    {
        OnTakeOutCreature.OnEventRaised -= TakeOutCreature;
        OnStoreCreature.OnEventRaised -= StoreCreature;
    }
    private void TakeOutCreature(string creatureID)
    {
        if (!string.IsNullOrEmpty(creatureID) && (!CreatureRenderer.gameObject.activeSelf || CurrentCreature.ID != creatureID))
        {
            CurrentCreature = PlayerManager.Instance.GetCreature(creatureID);
            FieldCoordinates = MapManager.Instance.CurrentCoordinates;
            CharacterInfo player = MapManager.Instance.Character;
            Vector2 playerLookDirection = new Vector2(player.Animator.GetFloat("Horizontal"), player.Animator.GetFloat("Vertical"));
            Debug.Log(playerLookDirection);
            CreaturePosition = PlayerManager.Instance.transform.position;
            CreatureSO fighterInfo = CurrentCreature.GetCreatureInfo();
            CreatureRenderer.gameObject.SetActive(true);
            CreatureAnimator.runtimeAnimatorController = fighterInfo.c_Animator;
            CreatureAnimator.SetFloat("Horizontal", playerLookDirection.x);
            CreatureAnimator.SetFloat("Vertical", playerLookDirection.y);
            //CreatureAnimator.SetTrigger("Fisical Attack");
        }
    }
    private void StoreCreature()
    {
        CreatureRenderer.gameObject.SetActive(false);
    }
}

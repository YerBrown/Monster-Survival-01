using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public OverlayTile ActiveTile;
    public Vector2 Movement;
    public Animator Animator;
    public Animator FollowerAnimator;
    public Inventory PlayerInventory;
    public PairInventoriesEventChannelSO OnOpenInventoriesMenu;
    public Creature[] Team = new Creature[5];
    public int ResourcesHitPower = 1;
    public void SetMovement(Vector2 newMovement)
    {
        Movement = newMovement;
        UpdateAnimator(Animator);
        if (FollowerAnimator != null)
            UpdateAnimator(FollowerAnimator);

    }

    private void UpdateAnimator(Animator animator)
    {
        if (Movement.x > 0 || Movement.y > 0 || Movement.x < 0 || Movement.y < 0)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }


        if (Movement.x > 0)
        {
            animator.SetFloat("Horizontal", 1);

        }
        else if (Movement.x < 0)
        {
            animator.SetFloat("Horizontal", -1);
        }
        if (Movement.y > 0)
        {
            animator.SetFloat("Vertical", 1);
        }
        else if (Movement.y < 0)
        {
            animator.SetFloat("Vertical", -1);
        }
    }

    public void OpenContainer(Inventory containerInventory)
    {
        if (containerInventory == null) return;
        PairInventories containerPlayerInventories = new PairInventories(PlayerInventory, containerInventory);
        OnOpenInventoriesMenu.RaiseEvent(containerPlayerInventories);
    }
}

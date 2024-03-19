using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAnimationController : MonoBehaviour
{
    public Animator Anim;
    public void StartDelayAnim(bool animDelay)
    {
        if (animDelay)
        {
            Anim.enabled = false;
            StartCoroutine(EnableAnimWithRandomDelay());
        }
    }
    IEnumerator EnableAnimWithRandomDelay()
    {
        float delay = Random.Range(0f, 0.2f);
        yield return new WaitForSeconds(delay);
        Anim.enabled = true;
    }
    public void SetMovement(Vector2 newMovement)
    {
        UpdateAnimator(newMovement);
    }
    private void UpdateAnimator(Vector2 newMovement)
    {
        if (newMovement.x > 0 || newMovement.y > 0 || newMovement.x < 0 || newMovement.y < 0)
        {
            Anim.SetBool("Moving", true);
        }
        else
        {
            Anim.SetBool("Moving", false);
        }
        UpdateDirection(newMovement);
    }
    public void SetMovementIdle(Vector2 newMovement)
    {
        UpdateDirection(newMovement);
    }
    private void UpdateDirection(Vector2 newMovement)
    {
        if (newMovement.x > 0)
        {
            Anim.SetFloat("Horizontal", 1);

        }
        else if (newMovement.x < 0)
        {
            Anim.SetFloat("Horizontal", -1);
        }
        if (newMovement.y > 0)
        {
            Anim.SetFloat("Vertical", 1);
        }
        else if (newMovement.y < 0)
        {
            Anim.SetFloat("Vertical", -1);
        }
    }

    public void PlayDefenseMode()
    {
        Anim.SetTrigger("Enable Shield");
    }
    public void PlayRemoveDefense()
    {
        Anim.SetTrigger("Disable Shield");
    }
    public void PlayFisicalAttack()
    {
        Anim.SetTrigger("Fisical Attack");
    }
    public void PlayRangeAttack()
    {
        Anim.SetTrigger("Range Attack");
    }
    public void PlayReceiveHit()
    {
        Anim.SetTrigger("Get Hit");
    }
    public void PlayReceiveHeal()
    {
        Anim.SetTrigger("Get Heal");
    }
    public void PlayDieAnimation()
    {
        Anim.SetTrigger("Die");
    }
}

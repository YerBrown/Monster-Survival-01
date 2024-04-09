using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureAnimationController : MonoBehaviour
{
    public Animator Anim;
    [SerializeField] private Transform _CaptureSpriteRenderer;
    private Fighter _TargetCapture;
    public void PlayCaptureRay(Fighter player, Fighter targetCapture, bool sucess)
    {
        _CaptureSpriteRenderer.gameObject.SetActive(true);
        _CaptureSpriteRenderer.position = player.transform.position;
        // Calculate rotation to target
        Vector2 direction = targetCapture.transform.position - _CaptureSpriteRenderer.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        _CaptureSpriteRenderer.rotation = rotation;


        _TargetCapture = targetCapture;
        Anim.SetTrigger("Start Anim");
        _CaptureSpriteRenderer.DOMove(targetCapture.transform.position, 0.5f).OnComplete(() =>
        {
            PlayCaptureSphere(sucess);
        });
    }

    private void PlayCaptureSphere(bool sucess)
    {
        if (sucess)
        {
            Anim.SetTrigger("Capture Sucess");
        }
        else
        {
            Anim.SetTrigger("Capture Fail");

        }
    }
    public void DisableCreatureSpriteRenderer()
    {
        _TargetCapture.GetComponent<SpriteRenderer>().enabled = false;
        _TargetCapture.GetComponentInChildren<UIFighterController>().gameObject.SetActive(false);
    }
    public void StopAnim()
    {
        _CaptureSpriteRenderer.gameObject.SetActive(false);
        CombatManager.Instance.TriggerTurnFlowInput();
    }
}

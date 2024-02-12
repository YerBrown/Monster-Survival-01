using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemUpdateController : MonoBehaviour
{
    public Image ItemIcon;
    public Image ItemAmountFrame;
    public TMP_Text ItemAmountText;

    private Coroutine _currentCoroutine;
    public void StartDisableCount(int delay)
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _currentCoroutine = StartCoroutine(DisableCoroutine(delay));
    }

    IEnumerator DisableCoroutine(int delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Playables;
public class NotificationTextController : MonoBehaviour
{
    public TMP_Text NotificationText; //Tmp_text reference
    public float AnimY = 25; //Movement in vertical 
    public float AnimDuration = 1f; //Animation duration
    public ScreenPositon_String_EventChannelSO NotificateEvent;
    private Sequence secuencia; // Referencia a la secuencia de Dotween
    private void OnEnable()
    {
        NotificateEvent.OnEventRaised += Notify;
    }


    private void OnDisable()
    {
        NotificateEvent.OnEventRaised -= Notify;
    }
    private void Notify((Vector3, string) notifyData)
    {
        if (secuencia != null)
        {
            secuencia.Kill();
        }
        //Set start values to text
        NotificationText.transform.position = Camera.main.WorldToScreenPoint(notifyData.Item1);
        NotificationText.text = notifyData.Item2;
        NotificationText.color = Color.white;
        NotificationText.gameObject.SetActive(true);
        // Crear una secuencia de tweens encadenados
        secuencia = DOTween.Sequence();
        // Agregar tweens para mover el texto y cambiar su color
        secuencia.Append(NotificationText.rectTransform.DOMoveY(NotificationText.transform.position.y + AnimY, AnimDuration).SetEase(Ease.InOutQuad)); // Mover durante 2 segundos

        secuencia.Join(NotificationText.DOColor(Color.clear, AnimDuration).SetEase(Ease.InOutQuint)); // Cambiar el color durante 2 segundos

        // Agregar una función OnComplete para desactivar el GameObject cuando la secuencia termine
        secuencia.OnComplete(() =>
        {
            // Desactivar el GameObject del texto
            NotificationText.gameObject.SetActive(false);
        });

        // Reproducir la secuencia
        secuencia.Play();
    }
}

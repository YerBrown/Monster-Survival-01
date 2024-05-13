using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    public TMP_Text CurrentTimeText;
    public RectTransform HoursClockHand;
    public RectTransform MinutesClockHand;
    public DateTime CurrentTime;
    // Start is called before the first frame update
    void Start()
    {
        DateTime now = RealDateTimeManager.Instance.GetCurrentDateTime();
        InvokeRepeating(nameof(UpdateClock), 1, 1);
    }

    private void UpdateClock()
    {
        DateTime currentDateTime = RealDateTimeManager.Instance.GetCurrentDateTime();
        if (currentDateTime.Hour != CurrentTime.Hour || currentDateTime.Minute != CurrentTime.Minute)
        {
            CurrentTimeText.text = RealDateTimeManager.Instance.GetCurrentDateTime().ToString("HH:mm");
            // Calcular el ángulo de rotación para la manecilla de las horas y minutos
            if (currentDateTime.Hour != CurrentTime.Hour)
            {
                float anguloHoras = (currentDateTime.Hour % 12 + currentDateTime.Minute / 60f) * 360f / 12f;
                HoursClockHand.localRotation = Quaternion.Euler(0, 0, -anguloHoras);
            }
            if (currentDateTime.Minute != CurrentTime.Minute)
            {
                float anguloMinutos = currentDateTime.Minute * 360f / 60f;
                MinutesClockHand.localRotation = Quaternion.Euler(0, 0, -anguloMinutos);
            }
            CurrentTime = currentDateTime;
        }

    }
}

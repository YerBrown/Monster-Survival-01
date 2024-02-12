using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneralUIController : MonoBehaviour
{
    public static GeneralUIController _instance;
    public static GeneralUIController Instance { get { return _instance; } }

    public bool MenuOpened = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void OpenMenu(bool enable)
    {
        MenuOpened = enable;
    }
}

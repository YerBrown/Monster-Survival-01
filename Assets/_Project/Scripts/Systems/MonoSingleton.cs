using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance { get { return _instance; } }
    [Header("Singleton")]
    [SerializeField] private bool _DontDestroyOnLoad = true;
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this as T;
            if (_DontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}

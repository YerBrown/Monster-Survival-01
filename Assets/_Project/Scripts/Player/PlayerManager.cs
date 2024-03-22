using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    public string Nickname;
    public HumanFighter P_Fighter;
    public Inventory P_Inventory;
    public FighterData[] Team = new FighterData[6];
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

    public void LoadCombatScene()
    {
        SceneManager.LoadScene("Combat Field");
    }
    public void LoadExpeditionScene()
    {
        SceneManager.LoadScene("Expedition");
    }
}
public class HumanFighter : Fighter
{

}
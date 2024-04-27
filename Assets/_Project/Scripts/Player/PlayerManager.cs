using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    public string Nickname;
    public int PlayerLevel;
    public FighterData P_Fighter;
    public Inventory P_Inventory;
    public FighterData[] Team = new FighterData[6];
    public CapturesController Captures;
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
            LoadPlayerInfo();
        }

    }

    private void LoadPlayerInfo()
    {
        for (int i = 0; i < Team.Length; i++)
        {
            if (Team[i].ID == P_Fighter.ID)
            {
                Team[i] = P_Fighter;

            }
            Team[i].Lvl = PlayerLevel;
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
    public FighterData[] GetTeamCreatures()
    {
        return Team.Where(fighter => fighter.ID != P_Fighter.ID).ToArray();
    }
    public FighterData GetCreature(string id)
    {
        foreach (FighterData creature in Team)
        {
            if (creature.ID == id)
            {
                return creature;
            }
        }
        return null;
    }
}
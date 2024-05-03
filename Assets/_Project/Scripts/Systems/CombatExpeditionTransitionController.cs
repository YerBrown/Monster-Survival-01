using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatExpeditionTransitionController : MonoBehaviour
{
    private static CombatExpeditionTransitionController _instance;
    public static CombatExpeditionTransitionController Instance { get { return _instance; } }

    public ExpeditionMapSO CurrentMap;
    public Vector2Int MapFieldCoordinates;
    public Vector2 PlayerLastPosition;
    public string TeamID;
    public FighterData[] EnemyTeam = new FighterData[6];
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    public void LoadCombatScene(string teamID,FighterData[] team)
    {
        CurrentMap = MapManager.Instance.FullMap;
        MapFieldCoordinates = MapManager.Instance.CurrentCoordinates;
        PlayerLastPosition = MapManager.Instance.Character.transform.position;
        TeamID = teamID;
        EnemyTeam = team;

        StartCoroutine(LoadCombatSceneCoroutine());
    }
    IEnumerator LoadCombatSceneCoroutine()
    {
        GeneralUIController.Instance.EnableBlackBackground(true);
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("Combat Field");
        yield return new WaitForSeconds(0.25f);
        GeneralUIController.Instance.EnableBlackBackground(false);
    }
    
    public void LoadExpeditionScene()
    {
        
        StartCoroutine(LoadExpeditionSceneCoroutine());
    }
    IEnumerator LoadExpeditionSceneCoroutine()
    {
        GeneralUIController.Instance.EnableBlackBackground(true);
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("Expedition");
    }
    public (string, FighterData[]) LoadCombatResult()
    {
        var combatResult = (TeamID, EnemyTeam);
        TeamID = null;
        EnemyTeam = null;
        return (combatResult);
    }
}

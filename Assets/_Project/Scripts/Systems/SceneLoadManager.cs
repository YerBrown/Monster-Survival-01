using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    [Header("Survival Base Data")]
    [Header("Expedition Data")]
    public ExpeditionMapSO CurrentMap;
    public Vector2Int MapFieldCoordinates;
    public Vector2 PlayerLastPosition;
    [Header("Combat Data")]
    public string TeamID;
    public FighterData[] EnemyTeam = new FighterData[6];
    IEnumerator LoadCombatScene()
    {
        GeneralUIController.Instance.EnableBlackBackground(true);
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("Combat Field");
        yield return new WaitForSeconds(0.25f);
        GeneralUIController.Instance.EnableBlackBackground(false);
    }
    IEnumerator LoadExpeditionScene()
    {
        GeneralUIController.Instance.EnableBlackBackground(true);
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("Expedition");
    }
    IEnumerator LoadSurvivalBaseSceneCoroutine()
    {
        GeneralUIController.Instance.EnableBlackBackground(true);
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("Survival Base");
        yield return new WaitForSeconds(0.25f);
        GeneralUIController.Instance.EnableBlackBackground(false);
    }
    public void LoadCombatFromExpeditionEnemyEncounter(string teamID, FighterData[] team)
    {
        CurrentMap = MapManager.Instance.FullMap;
        MapFieldCoordinates = MapManager.Instance.CurrentCoordinates;
        PlayerLastPosition = MapManager.Instance.Character.transform.position;
        TeamID = teamID;
        EnemyTeam = team;

        StartCoroutine(LoadCombatScene());
    }
    public void LoadExpeditionFromCombat()
    {
        StartCoroutine(LoadExpeditionScene());
    }
    public (string, FighterData[]) LoadCombatResult()
    {
        var combatResult = (TeamID, EnemyTeam);
        TeamID = null;
        EnemyTeam = null;
        return (combatResult);
    }
}

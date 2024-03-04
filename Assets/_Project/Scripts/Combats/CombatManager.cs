using System;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public CombatTeam PlayerTeam;
    public CombatTeam EnemyTeam;
    [Serializable]
    public class CombatTeam
    {
        public FighterData[] Fighters = new FighterData[6];
        public Fighter[] FightersInField = new Fighter[3];
        public Transform[] FightersPos = new Transform[3];
    }
    private void Start()
    {
        SpawnFighters();
    }
    public void SpawnFighters()
    {
        SpawnFightersByTeam(PlayerTeam);
        SpawnFightersByTeam(EnemyTeam);
    }
    public void ChangeFighter(Fighter fighterChanged, FighterData newFighterData, CombatTeam team)
    {
        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            if (team.FightersInField[i] == fighterChanged)
            {
                Destroy(team.FightersInField[i].gameObject);
                team.FightersInField[i] = null;
                SpawnFighterInTeam(i, newFighterData, team);
            }
        }
    }
    private void SpawnFightersByTeam(CombatTeam team)
    {
        if (FightersInfoWiki.Instance == null) { return; }
        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            FighterData nextFighterData = GetNextFighter(team);
            if (team.FightersInField[i] == null && nextFighterData != null)
            {
                SpawnFighterInTeam(i, nextFighterData, team);
            }
        }
    }
    private void SpawnFighterInTeam(int fighterNum, FighterData fighterData, CombatTeam team)
    {
        GameObject fighterPrefab = FightersInfoWiki.Instance.GetFighterPrefab(fighterData.TypeID);
        if (fighterPrefab != null)
        {
            Fighter newFighter = Instantiate(fighterPrefab, team.FightersPos[fighterNum]).GetComponent<Fighter>();

            newFighter.transform.position = team.FightersPos[fighterNum].position + Vector3.forward;
            newFighter.UpdateFighter(fighterData);
            team.FightersInField[fighterNum] = newFighter;
        }
        else
        {
            return;
        }
    }
    private FighterData GetNextFighter(CombatTeam team)
    {
        foreach (FighterData fighterData in team.Fighters)
        {
            if (GetFighterByID(fighterData.ID, team) == null)
            {
                return fighterData;
            }
            else
            {
                continue;
            }
        }
        return null;
    }

    private Fighter GetFighterByID(string id, CombatTeam team)
    {
        foreach (Fighter fighter in team.FightersInField)
        {
            if (fighter != null && fighter.ID == id)
            {
                return fighter;
            }
        }
        return null;
    }
}

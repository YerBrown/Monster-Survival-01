using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CombatTeamsController : MonoBehaviour
{
    [Header("Teams")]
    public CombatTeam PlayerTeam;
    public CombatTeam EnemyTeam;
    #region StartingCombat
    // Spawns the starting fighters of both teams
    public void SpawnStartingFighters()
    {
        SpawnFightersByTeam(PlayerTeam);
        SpawnFightersByTeam(EnemyTeam);
    }
    // Spawn the staring fighters of and specific team
    private void SpawnFightersByTeam(CombatTeam team)
    {
        if (FightersInfoWiki.Instance == null) { return; }
        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            FighterData nextFighterData = GetNextFighter(team);
            if (team.FightersInField[i] == null && nextFighterData != null)
            {
                SpawnFighterInTeam(i, nextFighterData, team, true);
            }
        }
    }
    // Spawn a fighter in the field of the provided team and using the fighter data
    public void SpawnFighterInTeam(int fighterNum, FighterData fighterData, CombatTeam team, bool animDelay = false)
    {
        GameObject fighterPrefab = null;
        if (FightersInfoWiki.Instance != null && FightersInfoWiki.Instance.GetCreatureInfo(fighterData.TypeID, out CreatureSO fighter))
        {
            fighterPrefab = fighter.CombatFighterPrefab;
        }
        else
        {
            Debug.Log($"Fighter with ID:{fighterData.TypeID} not found in wiki");
        }
        if (fighterPrefab != null)
        {
            Fighter newFighter = Instantiate(fighterPrefab, team.FightersPos[fighterNum]).GetComponent<Fighter>();
            newFighter.gameObject.name = fighterData.Nickname;
            newFighter.transform.position = team.FightersPos[fighterNum].position + Vector3.forward;
            newFighter.UpdateFighter(fighterData, animDelay);
            newFighter.UIController.MoveStatsInfoToCorrectPosition(team == PlayerTeam);
            newFighter.PositionCharacterOnTile(CombatManager.Instance.GetTile(newFighter.transform.position));
            team.FightersInField[fighterNum] = newFighter;
            Vector2Int fighterForward;
            if (IsPlayerTeamFighter(newFighter))
            {
                fighterForward = new Vector2Int(1, 1);
            }
            else
            {
                fighterForward = new Vector2Int(-1, -1);
            }
            newFighter.ForwardDirection = fighterForward;
            newFighter.LookForward();
        }
        else
        {
            return;
        }
    }
    #endregion
    #region Get Fighter Info
    // Returns the fighter data associated with the provided fighter.
    public FighterData GetFighterDataByFighter(Fighter fighter)
    {
        if (IsPlayerTeamFighter(fighter))
        {
            foreach (var fighterData in PlayerTeam.Fighters)
            {
                if (fighterData.ID == fighter.ID)
                {
                    return fighterData;
                }
            }
            return null;
        }
        else
        {
            foreach (var fighterData in EnemyTeam.Fighters)
            {
                if (fighterData.ID == fighter.ID)
                {
                    return fighterData;
                }
            }
            return null;
        }
    }
    // Returns the team to which this fighter belongs.
    public CombatTeam GetCombatTeamOfFighter(Fighter fighter)
    {
        if (IsPlayerTeamFighter(fighter))
        {
            return PlayerTeam;
        }
        else
        {
            return EnemyTeam;
        }
    }
    public CombatTeam GetCombatTeamOfFighter(string fighterId)
    {
        foreach (var fighterData in PlayerTeam.Fighters)
        {
            if (fighterData.ID == fighterId)
            {
                return PlayerTeam;
            }
        }
        foreach (var fighterData in EnemyTeam.Fighters)
        {
            if (fighterData.ID == fighterId)
            {
                return EnemyTeam;
            }
        }
        return null;
    }
    // Returns the number of fighters on the field.
    public int GetAmountOfFighterInField()
    {
        int playerTeamFighter = PlayerTeam.FightersInField.Count(fighter => fighter != null); ;
        int enemyTeamFighter = EnemyTeam.FightersInField.Count(fighter => fighter != null); ;
        return playerTeamFighter + enemyTeamFighter;
    }
    // Returns the fighter in the field by passing an ID and her team.
    public Fighter GetFighterInFieldByID(string fighterId)
    {
        foreach (Fighter fighter in GetCombatTeamOfFighter(fighterId).FightersInField)
        {
            if (fighter != null && fighter.ID == fighterId)
            {
                return fighter;
            }
        }
        return null;
    }

    // Return the index of a fighter in the list of fighters on the field.
    public int GetFighterInFieldNum(Fighter fighter)
    {
        CombatTeam team = GetCombatTeamOfFighter(fighter);
        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            if (fighter == team.FightersInField[i])
            {
                return i;
            }
        }
        return -1;
    }
    // This function tells you if the fighter is on the player's team. 
    public bool IsPlayerTeamFighter(Fighter fighter)
    {
        foreach (var fighterInField in PlayerTeam.FightersInField)
        {
            if (fighterInField == fighter)
            {
                return true;
            }
        }
        return false;
    }
    // This function tells you if the team has living fighters remaining that are not on the field.
    public bool IsTeamWithMoreFightersAliveThanInTheField(CombatTeam team)
    {
        foreach (var fighterData in team.Fighters)
        {
            if (fighterData.HealthPoints > 0)
            {
                bool currentlyInField = false;
                // Check all fighters in field
                for (int i = 0; i < team.FightersInField.Length; i++)
                {
                    if (team.FightersInField[i] != null && team.FightersInField[i].ID == fighterData.ID)
                    {
                        currentlyInField = true;
                    }
                }
                if (!currentlyInField)
                {
                    return true;
                }
            }
        }
        return false;
    }
    // Returns the next alive fighter data posible that isnt already in the field
    private FighterData GetNextFighter(CombatTeam team)
    {
        foreach (FighterData fighterData in team.Fighters)
        {
            if (GetFighterInFieldByID(fighterData.ID) == null && fighterData.HealthPoints > 0)
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
    // Returns the fighters index in range of ha selectedfighter
    public List<int> GetFightersNumInRange(Fighter selectedFighter)
    {
        int currentFighterNum = GetFighterInFieldNum(selectedFighter);

        List<int> fightersInRangeIndex = new()
        {
            currentFighterNum
        };
        if (currentFighterNum - 1 >= 0)
        {
            fightersInRangeIndex.Add(currentFighterNum - 1);
        }
        if (currentFighterNum + 1 <= 2)
        {
            fightersInRangeIndex.Add(currentFighterNum + 1);
        }

        return fightersInRangeIndex;
    }
    // Returns a random fighter on the field by passing the team.
    public Fighter GetRandonFighterOfTeam(CombatTeam team)
    {
        List<Fighter> activeFighters = team.FightersInField.Where(fighter => fighter != null).ToList();
        return activeFighters[UnityEngine.Random.Range(0, activeFighters.Count)];
    }
    // Returns position of attack pos of a fighter
    public Vector2 GetTargetAttackPosition(Fighter fighter, bool rangeAttack)
    {
        Vector2 offset;
        if (IsPlayerTeamFighter(fighter))
        {
            offset = (rangeAttack) ? new Vector2(1f, 0.5f) : new Vector2(0.5f, 0.25f);
        }
        else
        {
            offset = (rangeAttack) ? new Vector2(-1f, -0.5f) : new Vector2(-0.5f, -0.25f);
        }
        return (Vector2)fighter.transform.position + offset;
    }
    #endregion
    #region Utilities
    // Updates the fighter data with the health and energy points from the provided fighter.
    public void UpdateFighterData(Fighter fighter)
    {
        FighterData fighterToUpdate = GetFighterDataByFighter(fighter);
        fighterToUpdate.HealthPoints = fighter.HealthPoints;
        fighterToUpdate.EnergyPoints = fighter.EnergyPoints;
    }
    private bool HasAnyoneDiedThisTurn(out List<Fighter> diedFighters)
    {
        diedFighters = new();
        foreach (var fighter in PlayerTeam.FightersInField)
        {
            if (fighter.ID != "" && fighter.HealthPoints <= 0)
            {
                diedFighters.Add(fighter);
            }
        }
        foreach (var fighter in EnemyTeam.FightersInField)
        {
            if (fighter.ID != "" && fighter.HealthPoints <= 0)
            {
                diedFighters.Add(fighter);
            }
        }
        return diedFighters.Count > 0;
    }
    #endregion

    public void ChangeFighter(Fighter fighterChanged, FighterData newFighterData, CombatTeam team)
    {
        int fighterNum = GetFighterInFieldNum(fighterChanged);


        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            if (team.FightersInField[i] == fighterChanged)
            {
                RemoveFighterFromField(fighterChanged, team);
                SpawnFighterInTeam(i, newFighterData, team);
                // Calculate the order for the next turn
                CombatManager.Instance.CalculateTurnOrderOnFighterChanged();
            }
        }


    }
    public bool IsSomeOneDeadInField()
    {
        for (int i = 0; i < EnemyTeam.FightersInField.Length; i++)
        {
            if (EnemyTeam.FightersInField[i] != null && EnemyTeam.FightersInField[i].HealthPoints <= 0)
            {
                return true;
            }
        }
        for (int i = 0; i < PlayerTeam.FightersInField.Length; i++)
        {
            if (PlayerTeam.FightersInField[i] != null && PlayerTeam.FightersInField[i].HealthPoints <= 0)
            {
                return true;
            }
        }
        return false;
    }
    public (List<Fighter>, List<Fighter>) GetDiedFightersInField()
    {
        List<Fighter> diedPlayerFighters = new();
        List<Fighter> diedEnemyFighters = new();
        for (int i = 0; i < EnemyTeam.FightersInField.Length; i++)
        {
            if (EnemyTeam.FightersInField[i] != null && EnemyTeam.FightersInField[i].HealthPoints <= 0)
            {

                diedEnemyFighters.Add(EnemyTeam.FightersInField[i]);

            }
        }
        for (int i = 0; i < PlayerTeam.FightersInField.Length; i++)
        {
            if (PlayerTeam.FightersInField[i] != null && PlayerTeam.FightersInField[i].HealthPoints <= 0)
            {
                diedPlayerFighters.Add(PlayerTeam.FightersInField[i]);
            }
        }

        return (diedPlayerFighters, diedEnemyFighters);
    }
    public void RemoveFighterFromField(Fighter fighter, CombatTeam team)
    {
        for (int i = 0; i < team.FightersInField.Length; i++)
        {
            if (team.FightersInField[i] == fighter)
            {
                Destroy(team.FightersInField[i].gameObject);
                team.FightersInField[i] = null;
            }
        }
    }
}
[Serializable]
public class CombatTeam
{
    public FighterData[] Fighters = new FighterData[6];
    public Fighter[] FightersInField = new Fighter[3];
    public Transform[] FightersPos = new Transform[3];
    public void SwipeFightersOrder(int index1, int index2)
    {
        FighterData fighter1 = Fighters[index1];
        FighterData fighter2 = Fighters[index2];
        Fighters[index1] = fighter2;
        Fighters[index2] = fighter1;
    }
    public int GetFighterDataIndex(string fighterId)
    {
        for (int i = 0; i < Fighters.Length; i++)
        {
            if (Fighters[i].ID == fighterId)
            {
                return i;
            }
        }
        return -1;
    }
    public List<FighterData> GetFightersNotInField(int amountNeeded)
    {
        List<FighterData> fightersNotInField = new();

        foreach (var fighterData in Fighters)
        {
            if (fighterData.HealthPoints > 0 && fightersNotInField.Count < amountNeeded)
            {
                bool currentlyInField = false;
                // Check all fighters in field
                for (int i = 0; i < FightersInField.Length; i++)
                {
                    if (FightersInField[i] != null && FightersInField[i].ID == fighterData.ID)
                    {
                        currentlyInField = true;
                    }
                }
                if (!currentlyInField)
                {
                    fightersNotInField.Add(fighterData);
                    if (fightersNotInField.Count == amountNeeded)
                    {
                        return fightersNotInField;
                    }
                }
            }
        }
        return fightersNotInField;
    }
    public bool IsAllTeamDefeated()
    {
        for (int i = 0; i < FightersInField.Length; i++)
        {
            if (FightersInField[i] != null)
            {
                return false;
            }
        }
        return true;
    }
}

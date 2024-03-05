using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIFighterOrderController : MonoBehaviour
{
    public List<GameObject> FighterAvatars;
    public List<Fighter> CompleteOrderLine = new();

    [SerializeField] private Color PlayerTeamColor;
    [SerializeField] private Color EnemyTeamColor;

    public VoidEventChannelSO FinishCurrentFighterAction;
    private void OnEnable()
    {
        FinishCurrentFighterAction.OnEventRaised += CheckOrder;
    }
    private void OnDisable()
    {
        FinishCurrentFighterAction.OnEventRaised -= CheckOrder;
    }

    public void CheckOrder()
    {
        CompleteOrderLine = GetCompleteOrder();
        UpdateAvatarsLine();
    }
    private List<Fighter> GetCompleteOrder()
    {
        if (CombatManager.Instance == null) { return null; }
        //int fightersCount = CombatManager.Instance.GetAmountOfFighterInField(); 
        int fightersCount = 6; 
        List<Fighter> currentAndNextTurn = new();
        currentAndNextTurn = CombatManager.Instance.GetCurrentAndNextTurn();
        if (currentAndNextTurn.Count > fightersCount)
        {
            currentAndNextTurn.RemoveRange(fightersCount, currentAndNextTurn.Count - fightersCount);
        }
        return currentAndNextTurn;
    }
    private void UpdateAvatarsLine()
    {
        for (int i = 0; i < FighterAvatars.Count; i++)
        {
            if (CompleteOrderLine.Count > i)
            {
                FighterAvatars[i].GetComponent<Image>().color = (CombatManager.Instance.IsPlayerTeamFighter(CompleteOrderLine[i])) ? PlayerTeamColor : EnemyTeamColor;
                //FighterAvatars[i].GetComponent<Image>().sprite = CompleteOrderLine[i].AvatarSprite;
                FighterAvatars[i].transform.GetChild(0).GetComponent<Image>().sprite = CompleteOrderLine[i].AvatarSprite;
                FighterAvatars[i].SetActive(true);
            }
            else
            {
                FighterAvatars[i].SetActive(false);
            }
        }
    }
}

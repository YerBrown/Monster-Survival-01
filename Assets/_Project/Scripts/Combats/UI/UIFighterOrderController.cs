using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UIFighterOrderController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private List<GameObject> _FighterAvatars;
    [SerializeField] private GameObject _TurnFinishImage;
    [SerializeField] private Color _PlayerTeamColor;
    [SerializeField] private Color _EnemyTeamColor;

    private List<Fighter> _CompleteOrderLine = new();
    [Header("Events")]
    [SerializeField] private VoidEventChannelSO _FinishCurrentFighterAction;
    private void OnEnable()
    {
        _FinishCurrentFighterAction.OnEventRaised += CheckOrder;
    }
    private void OnDisable()
    {
        _FinishCurrentFighterAction.OnEventRaised -= CheckOrder;
    }
    // Gets order and updates UI
    public void CheckOrder()
    {
        _CompleteOrderLine = GetCompleteOrder();
        UpdateAvatarsLine();
    }
    // Get required order
    private List<Fighter> GetCompleteOrder()
    {
        if (CombatManager.Instance == null) { return null; }
        //int fightersCount = CombatManager.Instance.GetAmountOfFighterInField(); 
        int fightersCount = _FighterAvatars.Count; 
        List<Fighter> currentAndNextTurn = new();
        currentAndNextTurn = CombatManager.Instance.GetCurrentAndNextTurn();
        if (currentAndNextTurn.Count > fightersCount)
        {
            currentAndNextTurn.RemoveRange(fightersCount, currentAndNextTurn.Count - fightersCount);
        }
        return currentAndNextTurn;
    }
    // Update yhe turn order line UI 
    private void UpdateAvatarsLine()
    {
        for (int i = 0; i < _FighterAvatars.Count; i++)
        {
            if (_CompleteOrderLine.Count > i)
            {
                _FighterAvatars[i].transform.GetChild(1).GetComponent<Image>().color = (CombatManager.Instance.TeamsController.IsPlayerTeamFighter(_CompleteOrderLine[i])) ? _PlayerTeamColor : _EnemyTeamColor;
                _FighterAvatars[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = _CompleteOrderLine[i].AvatarSprite;
                _FighterAvatars[i].SetActive(true);
            }
            else
            {
                _FighterAvatars[i].SetActive(false);
            }
        }
        _TurnFinishImage.transform.SetSiblingIndex(CombatManager.Instance.CurrentTurnOrder.Count);
    }
}

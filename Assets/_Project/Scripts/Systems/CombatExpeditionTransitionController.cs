using DG.Tweening;
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

    public FighterData[] EnemyTeam = new FighterData[6];


    [Header("UI")]
    public Image BackgroundImage;
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

    public void LoadCombatScene(FighterData[] team)
    {
        CurrentMap = MapManager.Instance.FullMap;
        MapFieldCoordinates = MapManager.Instance.CurrentCoordinates;
        PlayerLastPosition = MapManager.Instance.Character.transform.position;

        EnemyTeam = team;
        BackgroundImage.DOFade(1f, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene("Combat Field");
            BackgroundImage.DOFade(0, 1f);
        });

    }
    public void LoadExpeditionScene()
    {
        // TODO: set up saved map
        // TODO: Spawn player in last position
        BackgroundImage.DOFade(1f, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene("Expedition");
            BackgroundImage.DOFade(0, 1f);
        });
    }
}

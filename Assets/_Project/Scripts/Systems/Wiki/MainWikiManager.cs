using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainWikiManager : MonoSingleton<MainWikiManager>
{
    [Header("Main Wiki Manager")]
    [SerializeField] private ItemInfoWiki _ItemsWiki;
    [SerializeField] private FightersInfoWiki _FightersWiki;
    [SerializeField] private BuildingInfoWiki _BuildingsWiki;
    [SerializeField] private AttacksInfoWiki _AttacksWiki;
    [SerializeField] private InteractiveElementsWiki _InteractiveElementsWiki;  
    [SerializeField] private SkillsInfoWiki _SkillsWiki;  
    [SerializeField] private BiomesWiki _BiomesWiki;  
    public Sprite MissingSprite;
    #region Items Wiki
    public ItemsSO GetItemByID(string id)
    {
        if (_ItemsWiki != null)
        {
            return _ItemsWiki.GetItemByID(id);
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region Fighters Wiki
    public bool GetCreatureInfo(string id, out CreatureSO creature)
    {
        if (_FightersWiki.GetCreatureInfo(id, out creature))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GetElementSprite(ElementType element, out Sprite elementSprite)
    {
        if (_FightersWiki.GetElementSprite(element, out elementSprite))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GetElementInfo(ElementType element, out FightersInfoWiki.ElementInfoUI elementInfo)
    {
        if (_FightersWiki.GetElementInfo(element, out elementInfo))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #region Buildings Wiki
    public BuildingSO GetBuildingByID(string id)
    {
        return _BuildingsWiki.GetBuildingByID(id);
    }
    public Dictionary<string, BuildingSO> GetBuidlingsDictionary()
    {
        return _BuildingsWiki.GetBuidlingsDictionary();
    }
    #endregion
    #region Attacks Wiki
    public AnimationClip GetAttackAnimationClipByID(string id)
    {
        return _AttacksWiki.GetAttackAnimClip(id);
    }
    #endregion
    #region Interactive Elements Wiki
    public GameObject GetInteractiveElementByID(string id)
    {
        return _InteractiveElementsWiki.GetPrefab(id);
    }
    #endregion
    #region
    public (Color, Color, Sprite) GetSkillInfo(Skills skill)
    {
        return _SkillsWiki.GetSkillInfo(skill);
    }
    #endregion
    #region Biomes Wiki
    public BiomeSO GetBiomeByID(string id)
    {
        return _BiomesWiki.GetBiomeByID(id);
    }
    #endregion
}

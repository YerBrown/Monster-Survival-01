using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemsRelatedUtilities
{
    private static string _defaultSprite = "Sprites/PictoIcon_Mark_Question";
    public static Sprite CheckItemIcon(ItemsSO itemInfo)
    {
        if (itemInfo != null)
        {
            if(itemInfo.i_Sprite == null)
            {
                return Resources.Load<Sprite>(_defaultSprite);
            }
            else
            {
                return itemInfo.i_Sprite;
            }
        }
        else
        {
            return Resources.Load<Sprite>(_defaultSprite);
        }
    }
}

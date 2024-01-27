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
            if (itemInfo.i_Sprite == null)
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

    public static Sprite DefaultIcon()
    {
        return Resources.Load<Sprite>(_defaultSprite);
    }
    public class CompareItemsByName : IComparer<ItemSlot>
    {
        public int Compare(ItemSlot x, ItemSlot y)
        {
            // Primero, compara por nombre
            int comparacionNombre = x.ItemInfo.i_Name.CompareTo(y.ItemInfo.i_Name);

            // Si los nombres son iguales, compara por cantidad de manera descendente
            if (comparacionNombre == 0)
            {
                return y.Amount.CompareTo(x.Amount);
            }

            return comparacionNombre;
        }
    }
}

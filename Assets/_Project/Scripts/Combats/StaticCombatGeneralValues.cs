using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeneralValues
{
    public static class StaticCombatGeneralValues
    {
        public static float Fighter_MoveSpeed = 2;
        public static float Fighter_ThrowAnimDuration = 0.75f;
        public static int Fighter_DefenseSplitter_WithoutDefenseMode = 2;
        public static int Fighter_DefenseSplitter_InDefenseMode = 1;
        public static int Fighter_EnergySplitter_OnReceiveDamage = 3;
        public static int Fighter_EnergyNeededFor_SpecialMovement = 5;
        public static int Fighter_StatusProblem_BurnedMaxTurns = 3;
        public static int Fighter_StatusProblem_ParalizedNotMoveRate = 40;
        public static int Fighter_Energy_ObtainedOnTurn = 2;
        public static int Fighter_Energy_ObtainedOnEnableDefense = 2;
        public static int[] Capture_CaptureIntensity_EnergyCosts = new int[] { 8, 16, 32, 64, 128 };
        public static int[] Capture_CaptureIntensity_CaptureRate = new int[] { 300, 250, 200, 150, 100 };
        public static float Capture_Modifier_HealthPoints = 20;
        public static float Capture_Modifier_FriendshipPoints = 10;
        public static float Capture_Modifier_Paralized = 2.5f;
        public static float Capture_Modifier_Frozen = 5;
        public static Effectiveness GetDamageMultiplier(ElementType attackType, ElementType defenseType)
        {
            Effectiveness effectiveness = Effectiveness.NORMAL;
            switch (attackType)
            {
                case ElementType.WOOD:
                    if (defenseType == ElementType.EARTH)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.FIRE)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                case ElementType.FIRE:
                    if (defenseType == ElementType.ORE)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.EARTH)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                case ElementType.EARTH:
                    if (defenseType == ElementType.WATER)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.ORE)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                case ElementType.ORE:
                    if (defenseType == ElementType.EARTH)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.WATER)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                case ElementType.WATER:
                    if (defenseType == ElementType.FIRE)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.WOOD)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                default:
                    break;
            }
            return effectiveness;
        }
        public static Effectiveness GetHealMultiplier(ElementType healType, ElementType defenseType)
        {
            Effectiveness effectiveness = Effectiveness.NORMAL;
            switch (healType)
            {
                case ElementType.WOOD:
                    if (defenseType == ElementType.FIRE)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.EARTH)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                case ElementType.FIRE:
                    if (defenseType == ElementType.EARTH)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.ORE)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                case ElementType.EARTH:
                    if (defenseType == ElementType.ORE)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.WATER)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                case ElementType.ORE:
                    if (defenseType == ElementType.WATER)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.EARTH)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                case ElementType.WATER:
                    if (defenseType == ElementType.WOOD)
                    {
                        effectiveness = Effectiveness.VERY_EFFECTIVE;
                    }
                    if (defenseType == ElementType.ORE)
                    {
                        effectiveness = Effectiveness.LESS_EFFECTIVE;
                    }
                    break;
                default:
                    break;
            }
            return effectiveness;
        }
    }
}

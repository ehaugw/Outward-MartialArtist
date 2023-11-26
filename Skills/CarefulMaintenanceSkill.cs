using HarmonyLib;
using InstanceIDs;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;
using UnityEngine;

namespace MartialArtist
{
    public class CarefulMaintenanceSkill
    {
        public const string NAME = "Careful Maintenance";
        public const int DURATION = 2400;
        public static Skill Init()
        {
            var myitem = new SL_Skill()
            {
                Name = NAME,
                EffectBehaviour = EditBehaviours.Destroy,
                Target_ItemID = IDs.arbitraryPassiveSkillID,
                New_ItemID = IDs.carefulMaintenanceSkillID,
                SLPackName = MartialArtist.ModFolderName,
                SubfolderName = "CarefulMaintenance",
                Description = "Applies Honed Edge to sharp weapons when you repair them.",
                IsUsable = false,
                CastType = Character.SpellCastType.NONE,
                CastModifier = Character.SpellCastModifier.Immobilized,
                CastLocomotionEnabled = false,
                MobileCastMovementMult = -1f,
            };
            myitem.ApplyTemplate();
            Skill skill = ResourcesPrefabManager.Instance.GetItemPrefab(myitem.New_ItemID) as Skill;
            return skill;
        }
    }

    [HarmonyPatch(typeof(Item), nameof(Item.SetDurabilityRatio))]
    public class Item_SetDurabilityRatio
    {
        [HarmonyPrefix]
        public static void HarmonyPrefix(Item __instance, float _maxDurabilityRatio, float ___m_currentDurability)
        {
            if (_maxDurabilityRatio > ___m_currentDurability / __instance.MaxDurability)
            {
                if (SkillRequirements.SafeHasSkillKnowledge(__instance.OwnerCharacter, IDs.carefulMaintenanceSkillID) && __instance is Weapon weapon)
                {
                    TinyHelper.TinyHelperRPCManager.Instance.photonView.RPC("ApplyAddImbueEffectRPC", PhotonTargets.All, new object[] { weapon.UID, IDs.honedBladeImbueID, (float)CarefulMaintenanceSkill.DURATION });
                }

            }
        }
    }

    [HarmonyPatch(typeof(Item), nameof(Item.ReduceDurability))]
    public class Item_ReduceDurability
    {
        [HarmonyPrefix]
        public static void HarmonyPrefix(Item __instance)
        {
            if (__instance is Weapon weapon && weapon.HasImbuePreset(IDs.honedBladeImbueID))
            {
                weapon.FirstImbue.RemainingLifespan -= 20;
            }
        }
    }
}


        
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
    public class ForagerSkill
    {
        public const string NAME = "Forager";
        public static Skill Init()
        {
            var myitem = new SL_Skill()
            {
                Name = NAME,
                EffectBehaviour = EditBehaviours.Destroy,
                Target_ItemID = IDs.arbitraryPassiveSkillID,
                New_ItemID = IDs.foragerSkillID,
                SLPackName = MartialArtist.ModFolderName,
                SubfolderName = "Forager",
                Description = "Eating fresh fruits and berries restores some burnt stamina and health.",
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

    [HarmonyPatch(typeof(Item), "Use")]
    public class Item_Use
    {
        [HarmonyPostfix]
        public static void HarmonyPostfix(Item __instance)
        {
            if (__instance.DurabilityRatio > 0.99f && __instance.HasTag(MartialArtist.Instance.ForagerTag) && SkillRequirements.SafeHasSkillKnowledge(__instance.OwnerCharacter, IDs.foragerSkillID))
            {
                __instance.OwnerCharacter.Stats.RestoreBurntHealth(5, false);
                __instance.OwnerCharacter.Stats.RestoreBurntStamina(5, false);
            }
        }
    }
}

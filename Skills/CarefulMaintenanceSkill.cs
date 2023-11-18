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
                New_ItemID = IDs.carefulMaintenanceID,
                SLPackName = MartialArtist.ModFolderName,
                SubfolderName = "Footwork",
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

    [HarmonyPatch(typeof(Item), nameof(Item.SetDurabilityRatio) )]
    public class Item_SetDurabilityRatio
    {
        [HarmonyPrefix]
        public static void HarmonyPrefix(Item __instance, float _maxDurabilityRatio, float ___m_currentDurability)
        {
            Debug.Log("hooked");

            if (_maxDurabilityRatio > __instance.MaxDurability / ___m_currentDurability)
            {
                Debug.Log("repairing");
                if (SkillRequirements.SafeHasSkillKnowledge(__instance.OwnerCharacter, IDs.carefulMaintenanceID) && __instance is Weapon weapon)
                {
                    Debug.Log("has skill");
                    TinyHelper.TinyHelperRPCManager.Instance.photonView.RPC("ApplyAddImbueEffectRPC", PhotonTargets.All, new object[] { weapon.UID, IDs.honedBladeImbueID, (float) CarefulMaintenanceSkill.DURATION});
                }

            }
        }
    }
}


        
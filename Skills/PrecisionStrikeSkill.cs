﻿using InstanceIDs;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;
using UnityEngine;
using HarmonyLib;


namespace MartialArtist
{
    public class PrecisionStrikeSkill
    {
        public const string NAME = "Precision Strike";
        public static Skill Init()
        {
            var myitem = new SL_Skill()
            {
                Name = NAME,
                EffectBehaviour = EditBehaviours.Destroy,
                Target_ItemID = IDs.arbitraryPassiveSkillID,
                New_ItemID = IDs.precisionStrikeID,
                SLPackName = MartialArtist.ModFolderName,
                SubfolderName = "Parry",
                Description = "Weapon attacks ignores half of the targets resistances when the target is prone or hit from behind.",
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
    [HarmonyPatch(typeof(Character), nameof(Character.ReceiveHit), new Type[] { typeof(UnityEngine.Object), typeof(DamageList), typeof(Vector3), typeof(Vector3), typeof(float), typeof(float), typeof(Character), typeof(float), typeof(bool) })]
    //[HarmonyPatch(typeof(Character), nameof(Character.ReceiveHit), new Type[] { typeof(UnityEngine.Object), typeof(DamageList), typeof(Vector3), typeof(Vector3), typeof(float), typeof(float), typeof(Character), typeof(float) })]
    public class Character_ReceiveHit
    {
        [HarmonyPrefix]
        public static void HarmonyPrefix(Character __instance, UnityEngine.Object _damageSource, DamageList _damage, Vector3 _hitDir, float _angle)
        {
            var hitFromBack = Vector3.Dot(__instance.transform.forward, -_hitDir) < 0f && _angle >= 120f;
            if (_damageSource is Weapon _weapon && SkillRequirements.SafeHasSkillKnowledge(_weapon?.OwnerCharacter, IDs.precisionStrikeID) && (hitFromBack || __instance.CharHurtType == Character.HurtType.Knockdown))
            {
                var attackType = ((int)SideLoader.At.GetField<Character>(_weapon.OwnerCharacter, "m_attackID"));
                if (attackType == 0 || attackType == 1)
                {
                    _damage.IgnoreHalfResistances = true;
                }
            }
        }
    }
}
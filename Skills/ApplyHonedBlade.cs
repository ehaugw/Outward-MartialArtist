using InstanceIDs;
using JetBrains.Annotations;
using SideLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MartialArtist
{
    public class ApplyHonedBlade
    {
        public static Skill Init()
        {
            var myitem = new SL_AttackSkill()
            {
                Name = "Hone Blade",
                EffectBehaviour = EditBehaviours.Override,
                Target_ItemID = IDs.infuseLightID, //Infuse Light
                New_ItemID = IDs.honeBladeID,
                Description = "Hone weapon",
                IsUsable = true,
                CastType = 
                    //Character.SpellCastType.SpellBindLight,
                    Character.SpellCastType.Bubble,
                CastModifier = Character.SpellCastModifier.Immobilized,
                CastLocomotionEnabled = false,
                MobileCastMovementMult = -1f,

                RequiredWeaponTypes = new Weapon.WeaponType[] {
                    Weapon.WeaponType.Axe_1H,
                    Weapon.WeaponType.Axe_2H,
                    Weapon.WeaponType.Sword_1H,
                    Weapon.WeaponType.Sword_2H,
                    Weapon.WeaponType.Halberd_2H,
                    Weapon.WeaponType.Spear_2H,
                },

                EffectTransforms = new SL_EffectTransform[] {
                    new SL_EffectTransform() {
                        TransformName = "Effects",
                        Effects = new SL_Effect[] {
                            new SL_ImbueWeapon() {
                                Lifespan = CarefulMaintenanceSkill.DURATION,
                                ImbueEffect_Preset_ID = IDs.honedBladeImbueID,
                                Imbue_Slot = Weapon.WeaponSlot.MainHand
                            },
                            //new SL_AddStatusEffect()
                            //{
                            //    StatusEffect = "Last Stand",
                            //    ChanceToContract = 100
                            //}
                        }
                    },
                },

                Cooldown = 100,
                StaminaCost = 0,
                HealthCost = 0,
                ManaCost = 0,
            };
            myitem.ApplyTemplate();
            Skill skill = ResourcesPrefabManager.Instance.GetItemPrefab(myitem.New_ItemID) as Skill;
            GameObject.Destroy(skill.gameObject.GetComponentInChildren<HasStatusEffectEffectCondition>());
            foreach (var playSound in skill.gameObject.GetComponentsInChildren<PlaySoundEffect>())
            {
                Object.Destroy(playSound);
            }

            //new SL_PlayVFX().
            new SL_PlaySoundEffect()
            {
                Follow = true,
                OverrideCategory = EffectSynchronizer.EffectCategories.None,
                Delay = 0,
                MinPitch = 1,
                MaxPitch = 1,
                SyncType = Effect.SyncTypes.OwnerSync,
                Sounds = new List<GlobalAudioManager.Sounds>() { GlobalAudioManager.Sounds.SFX_SKILL_PreciseStrike_WhooshImpact }
            }.ApplyToTransform(skill.transform.Find("ActivationEffects"));

            return skill;
        }
    }
}

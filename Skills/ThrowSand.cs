using InstanceIDs;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyHelper;

namespace MartialArtist
{
    public class ThrowSandSKill
    {
        public static Skill Init()
        {
            var myitem = new SL_AttackSkill()
            {
                Name = "Throw Sand",
                EffectBehaviour = EditBehaviours.Destroy,
                Target_ItemID = IDs.sparkID,
                New_ItemID = IDs.throwSandSkillID,
                SLPackName = MartialArtist.ModFolderName,
                SubfolderName = "ThrowSand",
                Description = "Causes Rage and Confusion",
                CastType = Character.SpellCastType.LanternThrowLeft,
                CastModifier = Character.SpellCastModifier.Mobile,
                CastLocomotionEnabled = true,
                MobileCastMovementMult = 0.3f,
                CastSheatheRequired = 0,
                
                RequiredItems = new SL_Skill.SkillItemReq[]
                {
                    new SL_Skill.SkillItemReq()
                    {
                        Consume = true,
                        ItemID = IDs.saltID,
                        Quantity = 1,
                    }
                },
                Cooldown = 15,
                StaminaCost = 0,
                HealthCost = 0,
                ManaCost = 0,
            };

            myitem.ApplyTemplate();
            Skill skill = ResourcesPrefabManager.Instance.GetItemPrefab(myitem.New_ItemID) as Skill;

            //Set the correct animation
            new SL_PlaySoundEffect()
            {
                Follow = true,
                OverrideCategory = EffectSynchronizer.EffectCategories.None,
                Delay = 0.05f,
                MinPitch = 1,
                MaxPitch = 1,
                SyncType = Effect.SyncTypes.OwnerSync,
                Sounds = new List<GlobalAudioManager.Sounds>() { GlobalAudioManager.Sounds.FT_Gravel1 }
            }.ApplyToTransform(TinyGameObjectManager.GetOrMake(skill.transform, "ActivationEffects", true, true));


            var effects = TinyGameObjectManager.MakeFreshObject("Effects", true, true, skill.transform).transform;

            var damageBlast = new SL_ShootBlast()
            {
                CastPosition = Shooter.CastPositionType.Local,
                LocalPositionAdd = new Vector3(0, 1.25f, .7f),

                TargetType = Shooter.TargetTypes.Enemies,

                BaseBlast = SL_ShootBlast.BlastPrefabs.ForcePush,
                Radius = 1.2f,
                BlastLifespan = 1,
                RefreshTime = -1,
                InstantiatedAmount = 5,
                Interruptible = false,
                HitOnShoot = true,
                IgnoreShooter = true,
                ParentToShootTransform = false,
                ImpactSoundMaterial = EquipmentSoundMaterials.Leather,
                DontPlayHitSound = false,
                EffectBehaviour = EditBehaviours.DestroyEffects,
                Delay = 0,
                BlastEffects = new SL_EffectTransform[] {
                    new SL_EffectTransform() {
                        TransformName = "Effects",
                        Effects = new SL_Effect[] {
                            new SL_AddStatusEffect()
                            {
                                StatusEffect = IDs.rageNameID,
                                ChanceToContract = 100,
                            },
                            new SL_AddStatusEffect()
                            {
                                StatusEffect = IDs.confusionNameID,
                                ChanceToContract = 100,
                            },
                            new SL_PunctualDamage()
                            {
                                Knockback = 30,
                            }
                        }
                    }
                }
            }.ApplyToTransform(effects) as ShootBlast;

            return skill;
        }
    }
}

using InstanceIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SideLoader;

namespace MartialArtist
{
    public class MartialArtistSkillTree
    {
        public static void SetupSkillTree(ref SkillSchool skillTreeInstance)
        {
            SL_SkillTree myskilltree;

            myskilltree = GetSkillTree();

            skillTreeInstance = myskilltree.CreateBaseSchool();
            myskilltree.ApplyRows();
        }

        public static SL_SkillTree GetSkillTree()
        {
            return new SL_SkillTree()
            {
                Name = "Martial Artist",

                SkillRows = new List<SL_SkillRow>() {
                    new SL_SkillRow() { RowIndex = 1, Slots = new List<SL_BaseSkillSlot>() {
                            new SL_SkillSlot() { ColumnIndex = 1, SilverCost = 50, SkillID = IDs.parrySkillID,      Breakthrough = false,   RequiredSkillSlot = Vector2.zero, },
                            new SL_SkillSlot() { ColumnIndex = 3, SilverCost = 50, SkillID = IDs.blockSkillID,      Breakthrough = false,   RequiredSkillSlot = Vector2.zero  },
                    } },

                    new SL_SkillRow() { RowIndex = 2, Slots = new List<SL_BaseSkillSlot>() {
                        new SL_SkillSlotFork()
                        {
                            ColumnIndex = 2,
                            RequiredSkillSlot = new Vector2(3,2),
                            Choice1 = new SL_SkillSlot()
                            {
                                ColumnIndex = 2,
                                SilverCost = 100,
                                SkillID = IDs.finesseSkillID,
                                RequiredSkillSlot = new Vector2(3,2),
                                Breakthrough = false
                            },
                            Choice2 = new SL_SkillSlot()
                            {
                                ColumnIndex = 2,
                                SilverCost = 100,
                                SkillID = IDs.bastardSkillID,
                                RequiredSkillSlot = new Vector2(3,2),
                                Breakthrough = false
                            }
                        },
                    } },

                    new SL_SkillRow() { RowIndex = 3, Slots = new List<SL_BaseSkillSlot>() {
                    } },

                    new SL_SkillRow() { RowIndex = 4, Slots = new List<SL_BaseSkillSlot>() {
                    } },

                    new SL_SkillRow() { RowIndex = 5, Slots = new List<SL_BaseSkillSlot>() {
                    } },
                }
            };
        }
    }
}

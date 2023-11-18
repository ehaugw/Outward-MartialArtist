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
    public class KenseiOutsideTracker
    {
        public const string NAME = "Saving Kensei";
        public static Quest Init()
        {
            var myitem = new SL_Quest()
            {
                Name = NAME,
                EffectBehaviour = EditBehaviours.Destroy,
                Target_ItemID = 7011003,
                New_ItemID = IDs.kenseiOutsideTrackerID,
                Description = "This skill should never be visible. Please contact the mod developer.",
                IsUsable = false,
                IsSideQuest = true,
                Tags = TinyTagManager.GetOrMakeTags(new string[]
                {
                    IDs.InvisitibleItemTag
                }),
            };
            myitem.ApplyTemplate();
            Quest quest = ResourcesPrefabManager.Instance.GetItemPrefab(myitem.New_ItemID) as Quest;
            //skill.IgnoreLearnNotification = true;

            return quest;
        }
    }
}

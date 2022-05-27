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
        public const string NAME = "KenseiOutsideTracker";
        //public static Skill Init()
        //{
        //    var myitem = new SL_Skill()
        //    {
        //        Name = NAME,
        //        EffectBehaviour = EditBehaviours.Destroy,
        //        Target_ItemID = IDs.arbitraryPassiveSkillID,
        //        New_ItemID = IDs.bastardSkillID,
        //        Description = "This skill should never be visible. Please contact the mod developer.",
        //        IsUsable = false,
        //        Tags = TinyTagManager.GetOrMakeTags( new string[]
        //        {
        //            IDs.InvisitibleItemTag
        //        })
        //    };
        //    myitem.ApplyTemplate();
        //    Skill skill = ResourcesPrefabManager.Instance.GetItemPrefab(myitem.New_ItemID) as Skill;
        //    skill.IgnoreLearnNotification = true;
            
        //    return skill;
        //}
    }
}

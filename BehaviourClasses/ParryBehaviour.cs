using InstanceIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;
using UnityEngine;

namespace MartialArtist
{
    public class ParryBehaviourSkillRequired : CustomWeaponBehaviour.ParryBehaviour
    {
        public override bool Eligible(Weapon weapon)
        {
            return SkillRequirements.SafeHasSkillKnowledge(weapon.OwnerCharacter, IDs.parrySkillID) || weapon.OwnerCharacter.IsAI;
        }
    }
}

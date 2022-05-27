using InstanceIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MartialArtist
{
    using TinyHelper;
    
    public class FinesseBehaviour : CustomWeaponBehaviour.FinesseBehaviour
    {
        public override bool Eligible(Weapon weapon)
        {
            return (SkillRequirements.SafeHasSkillKnowledge(weapon.OwnerCharacter, IDs.finesseSkillID) || weapon.OwnerCharacter.IsAI) && base.Eligible(weapon);
        }
    }
}

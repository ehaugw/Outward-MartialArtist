using InstanceIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MartialArtist
{
    using TinyHelper;

    public class AttackCancelByBlockBehaviour : CustomWeaponBehaviour.AttackCancelByBlockBehaviour
    {
        public override bool Eligible(Character character)
        {
            return SkillRequirements.SafeHasSkillKnowledge(character, IDs.blockSkillID) || character.IsAI;
        }
    }
}

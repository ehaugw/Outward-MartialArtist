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
            return new[] { Weapon.WeaponType.Sword_1H, Weapon.WeaponType.Sword_2H, Weapon.WeaponType.Spear_2H}.Contains(character?.CurrentWeapon?.Type ?? Weapon.WeaponType.Shield) && SkillRequirements.SafeHasSkillKnowledge(character, IDs.blockSkillID) || character.IsAI;
        }
    }
}

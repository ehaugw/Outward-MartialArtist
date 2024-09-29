using InstanceIDs;

namespace MartialArtist
{
    using TinyHelper;
    public class MaulShoveBehaviour : CustomWeaponBehaviour.MaulShoveBehaviour
    {
        public override bool Eligible(Weapon weapon)
        {
            return (SkillRequirements.SafeHasSkillKnowledge(weapon.OwnerCharacter, IDs.brawlerSkillID) || weapon.OwnerCharacter.IsAI) && base.Eligible(weapon);
        }
    }
}

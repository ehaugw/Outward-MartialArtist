using InstanceIDs;

namespace MartialArtist
{
    using TinyHelper;
    public class BastardBehaviour : CustomWeaponBehaviour.BastardBehaviour
    {
        public override bool Eligible(Weapon weapon)
        {
            return (SkillRequirements.SafeHasSkillKnowledge(weapon.OwnerCharacter, IDs.bastardSkillID) || weapon.OwnerCharacter.IsAI) && base.Eligible(weapon);
        }
    }
}

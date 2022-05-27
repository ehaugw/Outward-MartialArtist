using InstanceIDs;

namespace MartialArtist
{
    using TinyHelper;
    public class AttackCancelBySkillBehaviour : CustomWeaponBehaviour.AttackCancelBySkillBehaviour
    {
        public override bool Eligible(Character character)
        {
            return SkillRequirements.SafeHasSkillKnowledge(character, IDs.blockSkillID) || character.IsAI;
        }
    }
}

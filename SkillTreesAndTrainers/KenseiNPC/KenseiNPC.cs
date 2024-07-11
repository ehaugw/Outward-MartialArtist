using InstanceIDs;
using NodeCanvas.Tasks.Actions;
using UnityEngine;

namespace MartialArtist
{
    using NodeCanvas.Framework;
    using SideLoader;
    using SynchronizedWorldObjects;
    using TinyHelper;

    public class KenseiNPC : SynchronizedNPC
    {
        public const string Name = "Kensei";
        public static void Init()
        {
            var syncedNPC = new KenseiNPC(
                identifierName:     Name,
                rpcListenerID:      IDs.NPCID_Kensei,
                defaultEquipment: new int[] { IDs.tatteredAttireID, IDs.shadowKaziteLightBootsID, IDs.tatteredHood1 },
                visualData: new SL_Character.VisualData()
                {
                    Gender = Character.Gender.Male,
                    SkinIndex = (int)SL_Character.Ethnicities.Asian,
                    HeadVariationIndex = 1,
                    HairStyleIndex = (int)HairStyles.PonyTailBraids,
                    HairColorIndex = (int)HairColors.BrownDark
                }
            );

            syncedNPC.AddToScene(new SynchronizedNPCScene(
                scene:              "ChersoneseDungeonsSmall", 
                position:           new Vector3(299.7f, 0.1f, 32.0f),
                rotation:           new Vector3(0, 144.0f, 0),
                rpcMeta:            "prison",
                shouldSpawnInScene: delegate () { return !ShouldSpawnOutside(); }
            ));

            syncedNPC.AddToScene(new SynchronizedNPCScene(
                scene: "Emercar",
                position: new Vector3(1076.278f, -28.6992f, 1191.85f),
                rotation: new Vector3(0, 175f, 0),
                rpcMeta: "emercar",
                shouldSpawnInScene: delegate () { return ShouldSpawnOutside(); }
            ));
        }

        public static bool ShouldSpawnOutside()
        {
            return
                QuestRequirements.HasQuestKnowledge(CharacterManager.Instance.GetWorldHostCharacter(), new int[] { IDs.whiteFangOutsideTrackerID }, LogicType.All, requireCompleted: true) ||
                QuestRequirements.HasQuestEvent("ehaugw.questie.saving_white_fang.move_order_to_emercar");
        }

        public KenseiNPC(string identifierName, int rpcListenerID, int[] defaultEquipment = null, int[] moddedEquipment = null, Vector3? scale = null, Character.Factions? faction = null, SL_Character.VisualData visualData = null) :
            base(identifierName, rpcListenerID, defaultEquipment: defaultEquipment, moddedEquipment: moddedEquipment, scale: scale, faction: faction, visualData: visualData)
        { }

        override public object SetupClientSide(int rpcListenerID, string instanceUID, int sceneViewID, int recursionCount, string rpcMeta)
        {
            Character instanceCharacter = base.SetupClientSide(rpcListenerID, instanceUID, sceneViewID, recursionCount, rpcMeta) as Character;
            if (instanceCharacter == null) return null;

            GameObject instanceGameObject = instanceCharacter.gameObject;
            var trainerTemplate = TinyDialogueManager.AssignTrainerTemplate(instanceGameObject.transform);
            var actor = TinyDialogueManager.SetDialogueActorName(trainerTemplate, IdentifierName);
            var trainerComp = TinyDialogueManager.SetTrainerSkillTree(trainerTemplate, MartialArtist.Instance.martialArtistTreeInstance.UID);
            var graph = TinyDialogueManager.GetDialogueGraph(trainerTemplate);
            TinyDialogueManager.SetActorReference(graph, actor);
            graph.allNodes.Clear();

            //Actions
            var openTrainer = TinyDialogueManager.MakeTrainDialogueAction(graph, trainerComp);
            graph.primeNode = openTrainer;

            var obj = instanceGameObject.transform.parent.gameObject;
            obj.SetActive(true);

            return instanceCharacter;
        }
    }
}
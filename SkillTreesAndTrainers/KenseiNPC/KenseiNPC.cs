using InstanceIDs;
using NodeCanvas.Tasks.Actions;
using UnityEngine;

namespace MartialArtist
{
    using SynchronizedWorldObjects;
    using TinyHelper;

    public class KenseiNPC : SynchronizedNPC
    {
        public static void Init()
        {
            var syncedNPC = new KenseiNPC(
                identifierName:     "Kensei",
                rpcListenerID:      IDs.NPCID_Kensei,
                defaultEquipment:   new int[] { IDs.tatteredHood1, IDs.tatteredAttireID, IDs.shadowKaziteLightBootsID, IDs.kaziteBladeID }
            );

            syncedNPC.AddToScene(new SynchronizedNPCScene(
                scene:              "ChersoneseDungeonsSmall", 
                position:           new Vector3(298.7f, 0.1f, 32.0f),
                rotation:           new Vector3(0, 144.0f, 0),
                rpcMeta:            "inside",
                shouldSpawnInScene: delegate () { return !ShouldSpawnOutside(); }
            ));

            syncedNPC.AddToScene(new SynchronizedNPCScene(
                scene: "Emercar",
                position: new Vector3(1075.278f, -28.6992f, 1191.85f),
                rotation: new Vector3(0, 175f, 0),
                rpcMeta: "outside",
                shouldSpawnInScene: delegate () { return ShouldSpawnOutside(); }
            ));
        }

        public static bool ShouldSpawnOutside()
        {
            //return false;
            return CharacterManager.Instance.GetWorldHostCharacter().Inventory.SkillKnowledge.IsItemLearned(IDs.carefulMaintenanceID);
        }

        public KenseiNPC(string identifierName, int rpcListenerID, int[] defaultEquipment = null, int[] moddedEquipment = null, Vector3? scale = null, Character.Factions? faction = null) :
            base(identifierName, rpcListenerID, defaultEquipment: defaultEquipment, moddedEquipment: moddedEquipment, scale: scale, faction: faction)
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

            var openTrainer = TinyDialogueManager.MakeTrainDialogueAction(graph, trainerComp);
            //var actionNode = TinyDialogueManager.MakeGiveItemReward(graph, IDs.kenseiOutsideTrackerID, GiveReward.Receiver.Host);
            var rootStatement = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I am Kensei. I was captured by bandits. Thank you for saving me!");
            var wantToLeavePrisonStatement = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I would like to leave this prison as soon as the bandits outside are gone.");

            var introMultipleChoice = TinyDialogueManager.MakeMultipleChoiceNode(graph, new string[] {
                "Oh! Don't think about it. What are you planning to do now?",
                "Can you teach me something useful?"
            });

            graph.allNodes.Clear();
            graph.allNodes.Add(rootStatement);
            graph.allNodes.Add(introMultipleChoice);
            graph.allNodes.Add(wantToLeavePrisonStatement);
            //graph.allNodes.Add(actionNode);
            graph.allNodes.Add(openTrainer);

            graph.primeNode = rootStatement;
            //graph.ConnectNodes(actionNode, rootStatement);
            graph.ConnectNodes(rootStatement, introMultipleChoice);
            graph.ConnectNodes(introMultipleChoice, wantToLeavePrisonStatement, 0);
            graph.ConnectNodes(introMultipleChoice, openTrainer, 1);
            graph.ConnectNodes(wantToLeavePrisonStatement, rootStatement);

            var obj = instanceGameObject.transform.parent.gameObject;
            obj.SetActive(true);

            return instanceCharacter;
        }
    }
}
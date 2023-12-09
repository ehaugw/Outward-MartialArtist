using InstanceIDs;
using NodeCanvas.Tasks.Actions;
using UnityEngine;

namespace MartialArtist
{
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
                defaultEquipment:   new int[] { IDs.tatteredHood1, IDs.tatteredAttireID, IDs.shadowKaziteLightBootsID, IDs.kaziteBladeID }
            );

            syncedNPC.AddToScene(new SynchronizedNPCScene(
                scene:              "ChersoneseDungeonsSmall", 
                position:           new Vector3(298.7f, 0.1f, 32.0f),
                rotation:           new Vector3(0, 144.0f, 0),
                rpcMeta:            "prison",
                shouldSpawnInScene: delegate () { return !ShouldSpawnOutside(); }
            ));

            syncedNPC.AddToScene(new SynchronizedNPCScene(
                scene: "Emercar",
                position: new Vector3(1075.278f, -28.6992f, 1191.85f),
                rotation: new Vector3(0, 175f, 0),
                rpcMeta: "emercar",
                shouldSpawnInScene: delegate () { return ShouldSpawnOutside(); }
            ));
        }

        public static bool ShouldSpawnOutside()
        {
            return
                QuestRequirements.HasQuestKnowledge(CharacterManager.Instance.GetWorldHostCharacter(), new int[] { IDs.kenseiOutsideTrackerID }, LogicType.All, requireCompleted: true) ||
                QuestRequirements.HasQuestEvent(KenseiOutsideTracker.QE_MoveToEmercar.EventUID);
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
            
            switch (rpcMeta)
            {
                case "prison":
                    var npcIntro = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I am " + Name + ". I was captured by bandits. Thank you for saving me!");
                    var giveMoveTracker = TinyDialogueManager.MakeQuestEvent(graph, KenseiOutsideTracker.QE_MoveToEmercar.EventUID);
                    var wantToLeavePrisonStatement = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I would like to leave this prison as soon as the bandits outside are gone.");
                    var goesToEmercar = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I trust you in this. See you there!");
                    var openTrainer = TinyDialogueManager.MakeTrainDialogueAction(graph, trainerComp);

                    var introMultipleChoice = TinyDialogueManager.MakeMultipleChoiceNode(graph, new string[] {
                        "Oh! Don't think about it. What are you planning to do now?",
                        "Can you teach me something useful?"
                    });

                    var leaveResponse = TinyDialogueManager.MakeMultipleChoiceNode(graph, new string[] {
                        "Right! That makes perfect sense.",
                        "Why don't you go to the docks in Emercar?"
                    });

                    graph.allNodes.Clear();
                    graph.allNodes.Add(npcIntro);
                    graph.allNodes.Add(introMultipleChoice);
                    graph.allNodes.Add(wantToLeavePrisonStatement);
                    graph.allNodes.Add(giveMoveTracker);
                    graph.allNodes.Add(leaveResponse);
                    graph.allNodes.Add(goesToEmercar);
                    graph.allNodes.Add(openTrainer);

                    graph.primeNode = npcIntro;
                    graph.ConnectNodes(npcIntro, introMultipleChoice);
                    graph.ConnectNodes(introMultipleChoice, wantToLeavePrisonStatement, 0);
                    graph.ConnectNodes(introMultipleChoice, openTrainer, 1);
                    graph.ConnectNodes(wantToLeavePrisonStatement, leaveResponse);
                    graph.ConnectNodes(leaveResponse, npcIntro, 0);
                    graph.ConnectNodes(leaveResponse, goesToEmercar, 1);
                    graph.ConnectNodes(goesToEmercar, giveMoveTracker);

                    break;
                case "emercar":
                    var npcIntro2 = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "Hello! What are you up to?");
                    var enjoysEmercar = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "Thank you for checking on me. I love this place!");
                    var giveReward = TinyDialogueManager.MakeGiveItemReward(graph, IDs.ironCoinID, GiveReward.Receiver.Host);
                    var isComplete = TinyDialogueManager.MakeEventOccuredCondition(graph, KenseiOutsideTracker.QE_FoundInEmercar.EventUID, 1);
                    var makeComplete = TinyDialogueManager.MakeQuestEvent(graph, KenseiOutsideTracker.QE_FoundInEmercar.EventUID);
                    var openTrainer2 = TinyDialogueManager.MakeTrainDialogueAction(graph, trainerComp);

                    var introMultipleChoice1 = TinyDialogueManager.MakeMultipleChoiceNode(graph, new string[] {
                        "I wanted to see that you got here safely!",
                        "Can you teach me something useful?"
                    });

                    var introMultipleChoice2 = TinyDialogueManager.MakeMultipleChoiceNode(graph, new string[] {
                        "I am just stopping by.",
                        "Can you teach me something useful?"
                    });

                    graph.allNodes.Clear();
                    graph.allNodes.Add(npcIntro2);
                    graph.allNodes.Add(introMultipleChoice1);
                    graph.allNodes.Add(introMultipleChoice2);
                    graph.allNodes.Add(enjoysEmercar);
                    graph.allNodes.Add(openTrainer2);
                    graph.allNodes.Add(giveReward);
                    graph.allNodes.Add(isComplete);
                    graph.allNodes.Add(makeComplete);

                    graph.primeNode = npcIntro2;
                    graph.ConnectNodes(npcIntro2, isComplete);
                    graph.ConnectNodes(isComplete, introMultipleChoice2, 0);
                    
                    graph.ConnectNodes(introMultipleChoice2, enjoysEmercar, 0);
                    graph.ConnectNodes(introMultipleChoice2, openTrainer2, 1);

                    graph.ConnectNodes(isComplete, introMultipleChoice1, 1);
                    graph.ConnectNodes(introMultipleChoice1, giveReward, 0);
                    graph.ConnectNodes(introMultipleChoice1, openTrainer2, 1);
                    graph.ConnectNodes(giveReward, makeComplete);
                    graph.ConnectNodes(makeComplete, enjoysEmercar);
                    graph.ConnectNodes(enjoysEmercar, npcIntro2);

                    break;
                default:
                    break;
            }
            
            var obj = instanceGameObject.transform.parent.gameObject;
            obj.SetActive(true);

            return instanceCharacter;
        }
    }
}
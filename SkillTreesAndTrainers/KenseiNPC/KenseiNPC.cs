using InstanceIDs;
using NodeCanvas.Tasks.Actions;
using UnityEngine;

namespace MartialArtist
{
    using NodeCanvas.Framework;
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
                defaultEquipment: new int[] { IDs.ironSwordID },
                shouldSpawnInScene: delegate () { return ShouldSpawnOutside(); }
            ));
        }

        public static bool ShouldSpawnOutside()
        {
            return
                QuestRequirements.HasQuestKnowledge(CharacterManager.Instance.GetWorldHostCharacter(), new int[] { IDs.kenseiOutsideTrackerID }, LogicType.All, requireCompleted: true) ||
                QuestRequirements.HasQuestEvent(KenseiOutsideTracker.QE_MoveOrderToEmercar.EventUID);
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
            graph.allNodes.Clear();

            //Actions
            var openTrainer = TinyDialogueManager.MakeTrainDialogueAction(graph, trainerComp);
            var markInitialTalkDone = TinyDialogueManager.MakeQuestEvent(graph, KenseiOutsideTracker.QE_InitialTalk.EventUID);
            var giveSwordEvent = TinyDialogueManager.MakeQuestEvent(graph, KenseiOutsideTracker.QE_GivenSword.EventUID);
            var giveSwordEventEnchanted = TinyDialogueManager.MakeQuestEvent(graph, KenseiOutsideTracker.QE_GivenSwordEnchanted.EventUID);
            var giveMoveCommandEvent = TinyDialogueManager.MakeQuestEvent(graph, KenseiOutsideTracker.QE_MoveOrderToEmercar.EventUID);
            var giveIronCoin = TinyDialogueManager.MakeGiveItemReward(graph, IDs.ironCoinID, GiveReward.Receiver.Host);
            var setComplete = TinyDialogueManager.MakeQuestEvent(graph, KenseiOutsideTracker.QE_FoundInEmercar.EventUID);
            var giveUpSword = TinyDialogueManager.MakeResignItem(graph, IDs.ironSwordID, GiveReward.Receiver.Instigator);
            var giveUpSwordEnchanted = TinyDialogueManager.MakeResignItem(graph, IDs.tsarSwordID, GiveReward.Receiver.Instigator, enchantment: IDs.unsuspectedStrengthID);

            //Trainer Statements
            var tookDownAccursed = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "You there, you took down that occursed thing? Most impressive.");
            var couldUseWeapon = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "Thank you for freeing me from this cage. I could use a weapon though, the plains outside are quite dangerous still, no? An Iron Sword will suffice.");
            var givenRegularSword = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "You have quite the eye, you could have given me a stick and it would have proven enough, but I am grateful that you did not, that handicap would surely end poorly.");
            var givenEnchantedSword = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "This blade is nothing so simple. You've handed me an enchanted weapon, I can only hope to repay you one day.");
            var presentation = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I am a fighter, gladiator, monk, ranger; in short, I am a master of the martial arts, a student of diaspora disciplines, and teacher of my own school of thought.");
            var journeyDescription = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "After your effort to see me freed, I made short work of the bandits and hyenas in my way to the canyon passage. It was the forest that I saw the most trouble, those giants, the exiled ones, are quite the tough training companions, but not enough to sway my resolve.");
            var aboutClaimingDocks = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "Strong beasts roam those docks, and reliably attempt to claim it as their territory, it is a proving ground for any who truly wish to test their worth. A worthy domain for a warrior's rest in the making. I like the way you think my pupil.");
            var responseToTrain = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "As would I from you.");
            var staySafe = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "Stay safe comrade. I look forward to training with you again.");
            var isImpressed = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "You have done well, you continue to impress. I must admit, your drive is inspiring, even to a master like myself.");
            var soughtPower = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I sought power, but never cared for the sorceries of the conflux leylines and the hexes of Sorobor. I find the balance of a blade more reliable than a sleep deprived wizard any day. Not that magic is not worthwhile, I appreciate healers and alchemists, but they are at so much more risk than a swordmaster.");
            var facedImmaculate = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "The immaculate? Yes, there was one, they were... strange, they had no drive to kill like the others, and they spoke with a wisdom I could never have expected. I asked them then for a spar, and was handedly defeated. But it was an experience I will not take for granted. Should I see them again I would like to test my mettle once more.");
            var previousFights = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I've sparred with the monk in Monsoon, The hunter of Berg, The rogues of Levant, and even the spellblade that takes up residence in Cierzo. Eto put up the most fight, striking me with a shieldburst of fire caught me ablaze, and off-guard.");
            var welcomeBack = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "Welcome back my pupil. I look forward to how you have improved.");
            var readyToLeave = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "I will be in Emercar shortly.");
            var thankForEffort = TinyDialogueManager.MakeStatementNode(graph, IdentifierName, "Thank you! I will remember this.");

            //Player Statements
            var whoAreYou = "Who are you exactly?";
            var howWasJourney = "How was your journey eastward?";
            var suggestTheDocs = "I have a proposition, let us meet again in the abandonned river docks of Enmerkar forest.";
            var ellatProtect = "Elatt protect you.";
            var whyThisPath = "What led you down this path?";
            var askFacedImmaculate = "Have you ever faced an Immacuate?";
            var giveSimpleWeapon = "Here, a simple weapon, but you seem most capable.";
            var requestTraining = "If you are willing, I would like to learn from you.";
            var didYouChallenge = "Have you ever challenged the masters of each trade around Aurai?";
            var tryFindWeapon = "I do not have an Iron Sword, but I will try to find one for you.";

            //Conditions

            var talkedBefore = TinyDialogueManager.MakeEventOccuredCondition(graph, KenseiOutsideTracker.QE_InitialTalk.EventUID, 1);
            var givenSword = TinyDialogueManager.MakeEventOccuredCondition(graph, KenseiOutsideTracker.QE_GivenSword.EventUID, 1);
            var givenSwordEnchanted = TinyDialogueManager.MakeEventOccuredCondition(graph, KenseiOutsideTracker.QE_GivenSwordEnchanted.EventUID, 1);
            var receivedCoin = TinyDialogueManager.MakeEventOccuredCondition(graph, KenseiOutsideTracker.QE_FoundInEmercar.EventUID, 1);
            var hasSword = TinyDialogueManager.MakeHasItemCondition(graph, IDs.ironSwordID, 1);
            var hasSwordEnchanted = TinyDialogueManager.MakeHasItemConditionSimple(graph, IDs.tsarSwordID, 1, enchantment: IDs.unsuspectedStrengthID);
            var hasMoveOrder = TinyDialogueManager.MakeEventOccuredCondition(graph, KenseiOutsideTracker.QE_MoveOrderToEmercar.EventUID, 1);


            //Player Choices
            var giveWeaponOrPresentationOrTrain = TinyDialogueManager.MakeMultipleChoiceNode(graph, new string[] { giveSimpleWeapon, whoAreYou, requestTraining, });
            var tryFindOrPresentationOrTrain = TinyDialogueManager.MakeMultipleChoiceNode(graph, new string[] { tryFindWeapon, whoAreYou, requestTraining, });
            var presentationOrTrain = TinyDialogueManager.MakeMultipleChoiceNode(graph, new string[] { whoAreYou, requestTraining, });

            if (rpcMeta == "prison")
            {

                graph.primeNode = hasMoveOrder;

                TinyDialogueManager.ConnectMultipleChoices(graph, hasMoveOrder, new Node[] { readyToLeave, talkedBefore });

                //If already instructed to leave
                graph.ConnectNodes(readyToLeave, presentationOrTrain);
                graph.ConnectNodes(presentationOrTrain, presentation, 0);
                graph.ConnectNodes(presentation, hasSword);
                graph.ConnectNodes(presentationOrTrain, responseToTrain, 1);

                //inject compliment about killing wendigo if first time talking
                //graph.ConnectNodes(hasMoveOrder, talkedBefore, 1);
                graph.ConnectNodes(talkedBefore, couldUseWeapon, 0);
                graph.ConnectNodes(talkedBefore, tookDownAccursed, 1);
                TinyDialogueManager.ChainNodes(graph, new Node[] { tookDownAccursed, markInitialTalkDone, couldUseWeapon});
                graph.ConnectNodes(couldUseWeapon, hasSword);

                //enable player to give sword if owned
                graph.ConnectNodes(hasSword, giveWeaponOrPresentationOrTrain, 0);
                graph.ConnectNodes(giveWeaponOrPresentationOrTrain, hasSwordEnchanted, 0); // See given sword
                graph.ConnectNodes(giveWeaponOrPresentationOrTrain, presentation, 1);
                graph.ConnectNodes(giveWeaponOrPresentationOrTrain, responseToTrain, 2);
                graph.ConnectNodes(hasSwordEnchanted, givenEnchantedSword, 0);
                graph.ConnectNodes(hasSwordEnchanted, givenRegularSword, 1);

                //enable player to tell that he will be looking for ther weapon if not owned
                graph.ConnectNodes(hasSword, tryFindOrPresentationOrTrain, 1);
                graph.ConnectNodes(tryFindOrPresentationOrTrain, thankForEffort, 0);
                graph.ConnectNodes(tryFindOrPresentationOrTrain, presentation, 1);
                graph.ConnectNodes(tryFindOrPresentationOrTrain, responseToTrain, 2);

                //Open trainer
                graph.ConnectNodes(responseToTrain, openTrainer);

                //Give sword branch
                TinyDialogueManager.ChainNodes(graph, new Node[] { givenRegularSword, giveUpSword, giveSwordEvent, giveMoveCommandEvent });
                TinyDialogueManager.ChainNodes(graph, new Node[] { givenEnchantedSword, giveUpSwordEnchanted, giveSwordEventEnchanted, giveMoveCommandEvent});
                TinyDialogueManager.ChainNodes(graph, new Node[] { giveMoveCommandEvent, hasMoveOrder });
            }
            else if (rpcMeta == "emercar")
            {

            }

            var obj = instanceGameObject.transform.parent.gameObject;
            obj.SetActive(true);

            return instanceCharacter;
        }
    }
}
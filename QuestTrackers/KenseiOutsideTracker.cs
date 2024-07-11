using InstanceIDs;
using SideLoader;
using SideLoader.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace MartialArtist
{
    using TinyQuests;

    public class MoveToEmercarListener : IQuestEventAddedListener
    {
        public void OnQuestEventAdded(QuestEventData _eventData)
        {
            Character host = CharacterManager.Instance.GetWorldHostCharacter();
            var quest = host.Inventory.QuestKnowledge.GetItemFromItemID(KenseiOutsideTracker.QuestID) as Quest;
            KenseiOutsideTracker.UpdateQuestProgress(quest);
        }
    }

    public class TalkInEmercarListener : IQuestEventAddedListener
    {
        public void OnQuestEventAdded(QuestEventData _eventData)
        {
            Character host = CharacterManager.Instance.GetWorldHostCharacter();
            var quest = host.Inventory.QuestKnowledge.GetItemFromItemID(KenseiOutsideTracker.QuestID) as Quest;
            var progress = quest.GetComponent<QuestProgress>();

            quest.SetIsCompleted();
            progress.SetIsCompleted(true);

            KenseiOutsideTracker.UpdateQuestProgress(quest);
        }
    }

    public static class KenseiOutsideTracker
    {
        const string QuestName = "Saving " + KenseiNPC.Name;
        public static int QuestID = IDs.kenseiOutsideTrackerID;
        public const string QE_Scenario_UID = "ehaugw.questie.saving_kensei";
        public const string QUEST_EVENT_FAMILY_NAME = "Ehaugw_Questie_Saving_Kensei";

        static void PrepareTinyQuest()
        {
            var QuestTemplate = new SL_Quest()
            {
                Target_ItemID = IDs.arbitraryQuestID,
                New_ItemID = QuestID,
                Name = QuestName,
                IsSideQuest = false,
                ItemExtensions = new SL_ItemExtension[] { new SL_QuestProgress() },
            };

            Dictionary<string, string> QuestLogSignatures = new Dictionary<string, string>()
            {
                { GetLogSignature("find"),                  "Find " + KenseiNPC.Name + " in the bandit camp." },
                { GetLogSignature("provide"),               "Provide " + KenseiNPC.Name + " with an Iron Sword." },
                { GetLogSignature("send"),                  "Send " + KenseiNPC.Name + " to the Docks in Emercar." },
                { GetLogSignature("freed"),                 "Meet " + KenseiNPC.Name + " at the Docks in Emercar." },
                { GetLogSignature("rewarded"),              "You saved " + KenseiNPC.Name + "." },
            };

            TinyQuests.PrepareSLQuest(QuestTemplate, QuestLogSignatures, UpdateQuestProgress);

            QuestEventManager.Instance.RegisterOnQEAddedListener(QE_MoveOrderToEmercar.EventUID, new MoveToEmercarListener());
            QuestEventManager.Instance.RegisterOnQEAddedListener(QE_FoundInEmercar.EventUID, new TalkInEmercarListener());
        }

        public static string GetLogSignature(string letter) => QE_Scenario_UID + ".log_signature." + letter;

        public static QuestEventSignature QE_NotFound;
        public static QuestEventSignature QE_InitialTalk;
        public static QuestEventSignature QE_GivenSword;
        public static QuestEventSignature QE_GivenSwordEnchanted;
        public static QuestEventSignature QE_MoveOrderToEmercar;
        public static QuestEventSignature QE_FoundInEmercar;

        public static void Init()
        {
            QE_NotFound = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".not_found", false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_InitialTalk = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".initial_talk", false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_GivenSword = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".given_sword", false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_GivenSwordEnchanted = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".given_sword_enchanted", false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_MoveOrderToEmercar = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".move_order_to_emercar", false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_FoundInEmercar = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".found_in_emercar", false, true, true, QUEST_EVENT_FAMILY_NAME);
            
            SL.OnPacksLoaded += PrepareTinyQuest;
            SL.OnSceneLoaded += OnSceneLoaded;
            SL.OnGameplayResumedAfterLoading += OnGamePlayResumed;
        }

        static void OnSceneLoaded()
        {
            if (PhotonNetwork.isNonMasterClientInRoom)
                return;
            
            Character host = CharacterManager.Instance.GetWorldHostCharacter();
            if (SceneManagerHelper.ActiveSceneName == "ChersoneseDungeonsSmall" && (host.transform.position - new Vector3(300, 0, 1)).magnitude < 3)
            {
                var quest = TinyQuests.GetOrGiveQuestToHost(QuestID);
                UpdateQuestProgress(quest);
            }
        }

        static void OnGamePlayResumed()
        {
            if (PhotonNetwork.isNonMasterClientInRoom)
                return;
            
            Character host = CharacterManager.Instance.GetWorldHostCharacter();
            if (host.Inventory.QuestKnowledge.GetItemFromItemID(QuestID) is Quest quest)
                UpdateQuestProgress(quest);
        }
        
        public static void UpdateQuestProgress(Quest quest)
        {
            // Do nothing if we are not the host.
            if (PhotonNetwork.isNonMasterClientInRoom)
                return;

            QuestProgress progress = quest.GetComponent<QuestProgress>();

            int talked = QuestEventManager.Instance.GetEventCurrentStack(QE_InitialTalk.EventUID);
            int provided = QuestEventManager.Instance.GetEventCurrentStack(QE_GivenSword.EventUID);
            int move = QuestEventManager.Instance.GetEventCurrentStack(QE_MoveOrderToEmercar.EventUID);
            int found = QuestEventManager.Instance.GetEventCurrentStack(QE_FoundInEmercar.EventUID);

            if (quest.IsCompleted)
            {
                talked = 1;
                move = 1;
                found = 1;
                provided = 1;
            }
            
            progress.UpdateLogEntry(QE_Scenario_UID, false, progress.GetLogSignature(GetLogSignature("find")), talked >= 1);

            if (talked >= 1)
                progress.UpdateLogEntry(QE_Scenario_UID, false, progress.GetLogSignature(GetLogSignature("provide")), provided >= 1);
            
            if (provided >= 1)
                progress.UpdateLogEntry(QE_Scenario_UID, false, progress.GetLogSignature(GetLogSignature("send")), move >= 1);

            if (move >= 1)
                progress.UpdateLogEntry(QE_Scenario_UID, false, progress.GetLogSignature(GetLogSignature("freed")), found >= 1);

            if (found >= 1)
                progress.UpdateLogEntry(QE_Scenario_UID, false, progress.GetLogSignature(GetLogSignature("rewarded")), found >= 1);
        }
    }
}
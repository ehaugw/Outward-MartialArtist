using HarmonyLib;
using InstanceIDs;
using SideLoader;
using SideLoader.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace MartialArtist
{
    public class TinyQuest
    {
        public string QuestName;
        public int QuestId;
        public string ScenarioUID;
        public string QuestEventFamilyName;
    }
    
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

        public const string LogSignature_A = QE_Scenario_UID + ".log_signature.a";
        public const string LogSignature_B = QE_Scenario_UID + ".log_signature.b";
        public const string LogSignature_C = QE_Scenario_UID + ".log_signature.c";
        
        public static Dictionary<string, string> QuestLogSignatures => new Dictionary<string, string>()
        {
            { LogSignature_A, "Free " + KenseiNPC.Name + " from the bandit camp." },
            { LogSignature_B, "Meet " + KenseiNPC.Name + " at the Docks in Emercar." },
            { LogSignature_C, "You saved " + KenseiNPC.Name + "." },
        };

        public static QuestEventSignature QE_NotFound;
        public static QuestEventSignature QE_MoveToEmercar;
        public static QuestEventSignature QE_FoundInEmercar;

        public static void Init()
        {
            QE_NotFound = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".not_found", false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_MoveToEmercar = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".move_to_emercar", false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_FoundInEmercar = CustomQuests.CreateQuestEvent(QE_Scenario_UID + ".found_in_emercar", false, true, true, QUEST_EVENT_FAMILY_NAME);
            
            SL.OnPacksLoaded += PrepareSLQuest;
            SL.OnSceneLoaded += OnSceneLoaded;
            SL.OnGameplayResumedAfterLoading += OnGamePlayResumed;
        }

        static void OnSceneLoaded()
        {
            if (PhotonNetwork.isNonMasterClientInRoom)
                return;
            
            Character host = CharacterManager.Instance.GetWorldHostCharacter();
            if (SceneManagerHelper.ActiveSceneName == "ChersoneseDungeonsSmall" && (host.transform.position - new Vector3(300, 0, 1)).magnitude < 3)
                GetOrGiveQuestToHost();
        }

        static void OnGamePlayResumed()
        {
            if (PhotonNetwork.isNonMasterClientInRoom)
                return;
            
            Character host = CharacterManager.Instance.GetWorldHostCharacter();
            if (host.Inventory.QuestKnowledge.GetItemFromItemID(QuestID) is Quest quest)
                UpdateQuestProgress(quest);
        }

        public static Quest GetOrGiveQuestToHost()
        {
            Character character = CharacterManager.Instance.GetWorldHostCharacter();

            if (character.Inventory.QuestKnowledge.IsItemLearned(QuestID))
                return character.Inventory.QuestKnowledge.GetItemFromItemID(QuestID) as Quest;

            Quest quest = ItemManager.Instance.GenerateItemNetwork(QuestID) as Quest;
            quest.transform.SetParent(character.Inventory.QuestKnowledge.transform);
            character.Inventory.QuestKnowledge.AddItem(quest);
            SideLoader.At.SetField<QuestProgress>("m_progressState", QuestProgress.ProgressState.InProgress);

            QuestEventManager.Instance.AddEvent(QE_NotFound, 1);
            UpdateQuestProgress(quest);

            return quest;
        }

        static void PrepareSLQuest()
        {
            var QuestTemplate = new SL_Quest()
            {
                Target_ItemID = IDs.arbitraryQuestID,
                New_ItemID = QuestID,
                Name = QuestName,
                IsSideQuest = false,
                ItemExtensions = new SL_ItemExtension[] { new SL_QuestProgress() },
            };

            List<SL_QuestLogEntrySignature> list = new List<SL_QuestLogEntrySignature>();
            foreach (KeyValuePair<string, string> sig in QuestLogSignatures)
            {
                list.Add(new SL_QuestLogEntrySignature()
                {
                    UID = sig.Key,
                    Text = sig.Value,
                    Type = QuestLogEntrySignatureType.Static,
                });
            }

            SL_QuestProgress progress = QuestTemplate.ItemExtensions[0] as SL_QuestProgress;
            progress.LogSignatures = list.ToArray();

            QuestTemplate.ApplyTemplate();

            QuestTemplate.OnQuestLoaded += UpdateQuestProgress;

            QuestEventManager.Instance.RegisterOnQEAddedListener(QE_MoveToEmercar.EventUID, new MoveToEmercarListener());
            QuestEventManager.Instance.RegisterOnQEAddedListener(QE_FoundInEmercar.EventUID, new TalkInEmercarListener());
        }

        public static void UpdateQuestProgress(Quest quest)
        {
            // Do nothing if we are not the host.
            if (PhotonNetwork.isNonMasterClientInRoom)
                return;

            QuestProgress progress = quest.GetComponent<QuestProgress>();

            int found_in_cell = QuestEventManager.Instance.GetEventCurrentStack(QE_MoveToEmercar.EventUID);
            int found_in_emercar = QuestEventManager.Instance.GetEventCurrentStack(QE_FoundInEmercar.EventUID);
            
            if (quest.IsCompleted)
            {
                found_in_cell = 1;
                found_in_emercar = 1;
            }
            
            progress.UpdateLogEntry(QE_Scenario_UID, false, progress.GetLogSignature(LogSignature_A), found_in_cell >= 1);

            if (found_in_cell >= 1)
            {
                progress.UpdateLogEntry(QE_Scenario_UID, false, progress.GetLogSignature(LogSignature_B), found_in_emercar >= 1);
            }
            if (found_in_emercar >= 1)
            {
                progress.UpdateLogEntry(QE_Scenario_UID, false, progress.GetLogSignature(LogSignature_C), false);
            }
        }
    }
}
using HarmonyLib;
using InstanceIDs;
using SideLoader;
using SideLoader.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;
using UnityEngine;

namespace MartialArtist
{
    public static class KenseiOutsideTracker
    {
        const string QuestName = "Saving " + KenseiNPC.Name;
        public static int QuestID = IDs.kenseiOutsideTrackerID;
        public static string QE_Scenario_UID => "ehaugw.questie.saving_kensei";
        public const string QUEST_EVENT_FAMILY_NAME = "Ehaugw_Questie_Saving_Kensei";
        public static SL_Quest QuestTemplate { get; private set; }
        public static QuestEventSignature QE_NotFound;
        public static QuestEventSignature QE_FoundInCell;
        public static QuestEventSignature QE_FoundInEmercar;

        public const string LogSignature_A = "questie.saving_kensei.objective.a";
        public const string LogSignature_B = "questie.saving_kensei.objective.b";
        public const string LogSignature_C = "questie.saving_kensei.objective.c";

        public const string FirstQuestEventUID = "questie.saving_kensei.not_found";
        public const string SecondQuestEventUID = "questie.saving_kensei.found_in_cell";
        public const string ThirdQuestEventUID = "questie.saving_kensei.found_in_emercar";

        public static Dictionary<string, string> QuestLogSignatures => new Dictionary<string, string>()
        {
            {
                LogSignature_A,
                "Free " + KenseiNPC.Name + " from the bandit camp."
            },
            {
                LogSignature_B,
                "Meet " + KenseiNPC.Name + " at the Docks in Emercar."
            },
            {
                LogSignature_C,
                "You saved " + KenseiNPC.Name + "."
            },
        };

        public static void Init()
        {
            SL.OnPacksLoaded += PrepareSLQuest;
            SL.OnGameplayResumedAfterLoading += AfterLoad;
            QE_NotFound = CustomQuests.CreateQuestEvent(FirstQuestEventUID, false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_FoundInCell = CustomQuests.CreateQuestEvent(SecondQuestEventUID, false, true, true, QUEST_EVENT_FAMILY_NAME);
            QE_FoundInEmercar = CustomQuests.CreateQuestEvent(ThirdQuestEventUID, false, true, true, QUEST_EVENT_FAMILY_NAME);
        }

        static void AfterLoad()
        {
            if (PhotonNetwork.isNonMasterClientInRoom)
                return;
            
            Character host = CharacterManager.Instance.GetWorldHostCharacter();

            if (SceneManagerHelper.ActiveSceneName == "ChersoneseDungeonsSmall" && (host.transform.position - new Vector3(300, 0, 1)).magnitude < 3)
            {
                Quest quest = GetOrGiveQuestToHost();
                UpdateQuestProgress(quest);
            }
            else
            {
                Quest quest = host.Inventory.QuestKnowledge.GetItemFromItemID(QuestID) as Quest;
                UpdateQuestProgress(quest);
            }
        }

        public static Quest GetOrGiveQuestToHost() //Ienumerator
        {
            Debug.Log("Getting Kensei Quest...");

            Character character = CharacterManager.Instance.GetWorldHostCharacter();

            if (character.Inventory.QuestKnowledge.IsItemLearned(QuestID))
                return character.Inventory.QuestKnowledge.GetItemFromItemID(QuestID) as Quest;

            Quest quest = ItemManager.Instance.GenerateItemNetwork(QuestID) as Quest;
            quest.transform.SetParent(character.Inventory.QuestKnowledge.transform);
            character.Inventory.QuestKnowledge.AddItem(quest);

            QuestProgress progress = quest.GetComponent<QuestProgress>();
            SideLoader.At.SetField<QuestProgress>("m_progressState", QuestProgress.ProgressState.InProgress);

            QuestEventManager.Instance.AddEvent(QE_NotFound, 1);
            UpdateQuestProgress(quest);

            return quest;
        }

        static void PrepareSLQuest()
        {
            QuestTemplate = new SL_Quest()
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
        }

        // FULL CONTROL

        public static void UpdateQuestProgress(Quest quest)
        {
            // Do nothing if we are not the host.
            if (PhotonNetwork.isNonMasterClientInRoom)
                return;

            QuestProgress progress = quest.GetComponent<QuestProgress>();

            int found_in_cell = QuestEventManager.Instance.GetEventCurrentStack(QE_FoundInCell.EventUID);
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
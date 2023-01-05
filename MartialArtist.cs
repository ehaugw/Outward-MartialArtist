namespace MartialArtist
{
    using System.Collections.Generic;
    using UnityEngine;
    using SideLoader;
    using BepInEx;
    using InstanceIDs;
    using TinyHelper;

    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(SL.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(SynchronizedWorldObjects.SynchronizedWorldObjects.GUID, SynchronizedWorldObjects.SynchronizedWorldObjects.VERSION)]

    public class MartialArtist : BaseUnityPlugin
    {
        public const string GUID = "com.ehaugw.martialartist";
        public const string VERSION = "2.0.1";
        public const string NAME = "Martial Artist";

        public Skill BastardSkillInstance;
        public Skill FinesseSkillInstance;
        public Skill ParrySkillInstance;
        public Skill BlockSkillInstance;

        //public Item kenseiOutsideTrackerInstance;

        public SkillSchool martialArtistTreeInstance;

        public static MartialArtist Instance;
        internal void Awake()
        {
            Instance = this;

            KenseiNPC.Init();
            //KenseiNPC_OutsideCell.Init();

            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.parryBehaviour = new ParryBehaviourSkillRequired();
            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.bastardBehaviour = new BastardBehaviour();
            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.finesseBehaviour = new FinesseBehaviour();
            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.attackCancelByBlockBehaviour = new AttackCancelByBlockBehaviour();
            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.attackCancelBySkillBehaviour = new AttackCancelBySkillBehaviour();

            SL.OnPacksLoaded += OnPackLoaded;
            SL.OnSceneLoaded += OnSceneLoaded;

        }

        private void OnSceneLoaded()
        {
        }

        private void OnPackLoaded()
        {
            ParrySkillInstance = ParrySkill.Init();
            BastardSkillInstance = BastardSkill.Init();
            FinesseSkillInstance = FinesseSkill.Init();
            BlockSkillInstance = BlockSkill.Init();

            //kenseiOutsideTrackerInstance = KenseiOutsideTracker.Init();


            //kenseiOutsideTrackerInstance = TinyItemManager.MakeSkill(newID: IDs.kenseiOutsideTrackerID, targetID: IDs.arbitraryPassiveSkillID, identifierName: "kenseiOutsideTracker", description: "Kensei will wait for you outside of the bandit prison.", ignoreLearnNotification: false);

            MartialArtistSkillTree.SetupSkillTree(ref martialArtistTreeInstance);
        }
    }
}
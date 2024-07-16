﻿namespace MartialArtist
{
    using System.Collections.Generic;
    using UnityEngine;
    using SideLoader;
    using BepInEx;
    using InstanceIDs;
    using TinyHelper;
    using System.IO;
    using HarmonyLib;
    using System;

    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(SL.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(SynchronizedWorldObjects.SynchronizedWorldObjects.GUID, SynchronizedWorldObjects.SynchronizedWorldObjects.VERSION)]

    public class MartialArtist : BaseUnityPlugin
    {
        public const string GUID = "com.ehaugw.martialartist";
        public const string VERSION = "3.0.2";
        public const string NAME = "Martial Artist";
        public static string ModFolderName = Directory.GetParent(typeof(MartialArtist).Assembly.Location).Name.ToString();

        public SkillSchool martialArtistTreeInstance;

        public static MartialArtist Instance;
        internal void Awake()
        {
            Instance = this;

            KenseiNPC.Init();

            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.parryBehaviour = new ParryBehaviourSkillRequired();
            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.bastardBehaviour = new BastardBehaviour();
            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.finesseBehaviour = new FinesseBehaviour();
            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.attackCancelByBlockBehaviour = new AttackCancelByBlockBehaviour();
            CustomWeaponBehaviour.CustomWeaponBehaviour.Instance.attackCancelBySkillBehaviour = new AttackCancelBySkillBehaviour();

            SL.OnPacksLoaded += OnPackLoaded;
            SL.OnSceneLoaded += OnSceneLoaded;
            SL.BeforePacksLoaded += BeforePacksLoaded;

            var harmony = new Harmony(GUID);
            harmony.PatchAll();
        }
        private void BeforePacksLoaded()
        {
        }
        private void OnSceneLoaded()
        {
        }

        private void OnPackLoaded()
        {
            ParrySkill.Init();
            BastardSkill.Init();
            FinesseSkill.Init();
            BlockSkill.Init();
            MartialArtistSkillTree.SetupSkillTree(ref martialArtistTreeInstance);
        }
    }
}

﻿using System;
using BepInEx;
using UnityEngine;
using ImprovedInput;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
//using DressMySlugcat;
using System.Linq;
using System.Collections.Generic;
using System.Security.Permissions;
using SlugBase;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;
using System.Runtime.CompilerServices;

//Private members can't be accesed?, NOT ANYMORE    
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Deadlands
{
    [BepInPlugin("DeadLands", "DeadLands", "0.1.2")]
    class NomadPlugin : BaseUnityPlugin
    {
        private DeadlandsOptions options;
        bool _initialized;

        private void LogInfo(object ex) => Logger.LogInfo(ex);

        //Slugbase features, HELL I WANT TO DELETE THEM ALL!!!!!!!
        public static readonly PlayerFeature<int> slideStamina = PlayerInt("Nomad/SlideStamina");
        public static readonly PlayerFeature<float> SlideRecovery = PlayerFloat("Nomad/SlideRecovery");
        public static readonly PlayerFeature<float> SlideSpeed = PlayerFloat("Nomad/SlideSpeed");
        public static readonly PlayerFeature<bool> Nomad = PlayerBool("Nomad/Nomad");



        public NomadPlugin()
        {
            try
            {
                options = new DeadlandsOptions();
            } 
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }


        public void OnEnable()
        {
            LogInfo("DeadLans is working or skill issue?, wortking");
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.Menu.SleepAndDeathScreen.GetDataFromGame += SleepAndDeathScreen_GetDataFromGame;
        }


        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            try
            {
                //Security check
                if (_initialized) return;
                _initialized = true;

                //Sounds enums
                NomadEnums.RegisterValues();
                //Fly Logic
                var SlideData = NomadFly.OnInit();

                NomadGraphics.OnInit(SlideData);

                //DMS compatibility
                if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
                {
                    NomadDMScompatibility.OnInit();
                }
                
                //Remix Menu
                MachineConnector.SetRegisteredOI("DeadLands", options);
            }
            catch (Exception ex)
            {
                Debug.Log($"Remix Menu: Hook_OnModsInit options failed init error {options}{ex}");
                Logger.LogError(ex);
                Logger.LogMessage("WHOOPS something go wrong");
            }
            finally
            {
                orig.Invoke(self);
            }
        }

        private void SleepAndDeathScreen_GetDataFromGame(On.Menu.SleepAndDeathScreen.orig_GetDataFromGame orig, Menu.SleepAndDeathScreen self, Menu.KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            orig(self, package);
            if (package.mapData.regionName == "DL") //Will need to be updated or replaced with enum
            {
                self.mySoundLoopID = SoundID.None;
            }
        }
    }
}

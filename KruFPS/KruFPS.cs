using MSCLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using System.IO;
using HutongGames.PlayMaker;

namespace KruFPS
{
    public class KruFPS : Mod
    {
        public override string ID => "KruFPS"; //Your mod ID (unique)
        public override string Name => "KruFPS"; //You mod name
        public override string Author => "Krutonium"; //Your Username
        public override string Version => "1.0"; //Version

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => false;

        private GameObject PLAYER;
        private GameObject YARD;
        private GameObject SATSUMA;
        private List<GameObject> gameObjects;
        private List<GameObject> awayFromHouse;
        private List<GameObject> Cars;
        private Dictionary<string, Transform> objectCoords;

        private static float DrawDistance;
        //private KeyValuePair<GameObject, Vector3> internalcars = new KeyValuePair<GameObject, Vector3>();
        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            gameObjects = new List<GameObject>();
            objectCoords = new Dictionary<string, Transform>();
            Cars = new List<GameObject>();
            
            //Player Vehicles
            Cars.Add(GameObject.Find("FERNDALE(1630kg)"));
            Cars.Add(GameObject.Find("FLATBED"));
            Cars.Add(GameObject.Find("GIFU(750/450psi)"));
            Cars.Add(GameObject.Find("HAYOSIKO(1500kg, 250)"));
            Cars.Add(GameObject.Find("JONNEZ ES(Clone)"));
            Cars.Add(GameObject.Find("KEKMET(350-400psi)"));
            Cars.Add(GameObject.Find("RCO_RUSCKO12(270)"));

            //Locations and objects that can be enabled and disabled easily on proximity
            gameObjects.Add(GameObject.Find("BOAT")); //Boat is not a Car, oddly enough.
            gameObjects.Add(GameObject.Find("CABIN"));
            gameObjects.Add(GameObject.Find("COTTAGE"));
            gameObjects.Add(GameObject.Find("DANCEHALL"));
            gameObjects.Add(GameObject.Find("DRAGRACE"));
            gameObjects.Add(GameObject.Find("INSPECTION"));
            gameObjects.Add(GameObject.Find("LANDFILL"));
            gameObjects.Add(GameObject.Find("PERAJARVI"));
            //gameObjects.Add(GameObject.Find("REPAIRSHOP")); //Has to be loaded for repairs and such - Maybe fixable
            gameObjects.Add(GameObject.Find("RYKIPOHJA"));
            gameObjects.Add(GameObject.Find("SOCCER"));
            //gameObjects.Add(GameObject.Find("STORE")); //Has to be loaded on Thursdays to restock
            gameObjects.Add(GameObject.Find("WATERFACILITY"));
            gameObjects.Add(GameObject.Find("KILJUGUY"));
            gameObjects.Add(GameObject.Find("CHURCHWALL"));
            gameObjects.Add(GameObject.Find("TREES1_COLL"));
            gameObjects.Add(GameObject.Find("TREES2_COLL"));
            gameObjects.Add(GameObject.Find("TREES3_COLL"));

            //Things that should be enabled when out of proximity of the house
            awayFromHouse = new List<GameObject>();
            awayFromHouse.Add(GameObject.Find("NPC_CARS"));
            awayFromHouse.Add(GameObject.Find("RALLY"));
            awayFromHouse.Add(GameObject.Find("TRAFFIC"));
            awayFromHouse.Add(GameObject.Find("TRAIN"));
            awayFromHouse.Add(GameObject.Find("Buildings"));
            awayFromHouse.Add(GameObject.Find("TrafficSigns"));
            awayFromHouse.Add(GameObject.Find("ELEC_POLES"));

            //TODO: Solve Bugs from Unloading/Reloading Satsuma
            // Bugs: 
            // Can't open doors
            // May randomly fall through floor

            //TODO: 
            // Figure out how to make repairs works at Fleetari's without loading it
            // Figure out how to trigger a restock at Tiemos on Thursdays without loading it.

            //Camera.main.farClipPlane = (int)RenderDistance.Value; //Helps with lower end GPU's. This specific value. Any others are wrong.
            PLAYER = GameObject.Find("PLAYER");
            YARD = GameObject.Find("YARD");                     //Used to find out how far the player is from the Object
            SATSUMA = GameObject.Find("SATSUMA(557kg, 248)");
            ModConsole.Print("[KruFPS] Found all objects");
        }
        Settings Satsuma = new Settings("Satsuma", "Enable/Disable Satsuma", false);
        Settings OtherCars = new Settings("OtherCars", "Enable/Disable Other Player Vehicles", false);
        static Settings RenderDistance = new Settings("slider", "Render Distance", 420, UpdateDrawDistance);

        public static void UpdateDrawDistance()
        {
            DrawDistance = (float)RenderDistance.GetValue();
        }

        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings.
            Settings.AddHeader(this, "Warning: Enabling these removes more lag but can break the game until you save and reload.");
            Settings.AddCheckBox(this, Satsuma);
            Settings.AddCheckBox(this, OtherCars);
            Settings.AddSlider(this, RenderDistance, 0, 1000);
        }

        public override void OnSave()
        {
            // Called once, when save and quit
            // Serialize your save file here.
        }

        public override void OnGUI()
        {
            // Draw unity OnGUI() here
        }

        int Frame = 0;
        public override void Update()
        {
            if (Frame == 60)
            {

                //Code to run once every second assuming 60 FPS
                Camera.main.farClipPlane = DrawDistance;
                //ModConsole.Print(RenderDistance.GetValue());
                foreach (var item in gameObjects)
                {
                    EnableDisable(item, ShouldEnable(PLAYER.transform, item.transform));
                }
                if ((bool)OtherCars.GetValue() == true) //OtherCars
                {
                    foreach(var car in Cars)
                    {
                        EnableDisable(car, ShouldEnable(PLAYER.transform, car.transform));
                    }
                }
                if ((bool)Satsuma.GetValue() == true) //Satsuma
                {
                    EnableDisable(SATSUMA, ShouldEnable(PLAYER.transform, SATSUMA.transform));
                }
    
                if (Distance(PLAYER.transform, YARD.transform) > 100) {
                    foreach(var item in awayFromHouse)
                    {
                        EnableDisable(item, true);
                    }
                } else
                {
                    foreach(var item in awayFromHouse)
                    {
                        EnableDisable(item, false);
                    }
                }
            }
            Frame++;
            if (Frame > 60)
            {
                Frame = 0;
            }
        }

        private bool ShouldEnable(Transform player, Transform target, int distanceTarget = 200)
        {

            //This determines if somthing should be enabled or not - Returning FALSE means that the object should be Disabled, and inversely
            // if it returns TRUE the object should be Enabled.

            float distance = Vector3.Distance(player.position, target.position);
            //ModConsole.Print(distance);
            try
            {
                if (distance > distanceTarget) //100 to 200 Seems ideal
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }
        private float Distance(Transform player, Transform target)
        {
            //Gets Distance.
            return Vector3.Distance(player.position, target.position);
        }
        private void EnableDisable(GameObject thing, bool enabled)
        {
            try
            {
                thing.SetActive(enabled);
            }
            catch { }
        }
    }
}

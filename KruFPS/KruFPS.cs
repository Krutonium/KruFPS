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
        public override string Version => "3.0"; //Version

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => false;

        private GameObject PLAYER;
        private GameObject YARD;
        private Vehicle SATSUMA;
        private GameObject SATSUMA_2;
        private Vehicle FLATBED;
        private Vehicle GIFU;
        private Vehicle HAYOSIKO;
        private Vehicle JONNEZ;
        private Vehicle KEKMET;
        private Vehicle RUSKO;
        private Vehicle FERNDALE;
        private GameObject CABIN;
        private Rigidbody KINEMATIC;
        private CarDynamics CAR_DYNAMICS;
        private Axles AXLES;
        private List<GameObject> gameObjects;
        private List<GameObject> awayFromHouse;
        //private List<GameObject> Cars;

        List<GameObject> minorObjects = new List<GameObject>(); 
        // List of all whitelisted objects that can appear on the minorObjects list
        // Note: batteries aren't included
        string[] listOfMinorObjects = {"ax", "beer case", "booze", "brake fluid", "cigarettes", "coffee pan", "coffee cup", "coolant", 
        "diesel", "empty plastic can", "fire extinguisher", "gasoline", "grill charcoal", "ground coffee", "juice", "kilju", "lamp", "macaronbox", "milk", 
        "moosemeat", "mosquito spray", "motor oil", "oilfilter", "pike", "pizza", "potato chips", "sausages", "shopping bag", "sugar", "spray can", 
            "two stroke fuel", "wood carrier", "yeast" };

        private static float DrawDistance = 420;
        //private KeyValuePair<GameObject, Vector3> internalcars = new KeyValuePair<GameObject, Vector3>();
        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            gameObjects = new List<GameObject>();
            //Cars = new List<GameObject>();

            //Player Vehicles
            //Cars.Add(GameObject.Find("FERNDALE(1630kg)"));
            //Cars.Add(GameObject.Find("FLATBED"));
            //Cars.Add(GameObject.Find("GIFU(750/450psi)"));
            //Cars.Add(GameObject.Find("HAYOSIKO(1500kg, 250)"));
            //Cars.Add(GameObject.Find("JONNEZ ES(Clone)"));
            //Cars.Add(GameObject.Find("KEKMET(350-400psi)"));
            //Cars.Add(GameObject.Find("RCO_RUSCKO12(270)"));
            //SATSUMA = GameObject.Find("SATSUMA(557kg, 248)");
            SATSUMA = new Vehicle("SATSUMA(557kg, 248)");
            SATSUMA_2 = GameObject.Find("SATSUMA(557kg, 248)");
            FLATBED = new Vehicle("FLATBED");
            GIFU = new Vehicle("GIFU(750/450psi)");
            HAYOSIKO = new Vehicle("HAYOSIKO(1500kg, 250)");
            JONNEZ = new Vehicle("JONNEZ ES(Clone)");
            KEKMET = new Vehicle("KEKMET(350-400psi)");
            RUSKO = new Vehicle("RCO_RUSCKO12(270)");
            FERNDALE = new Vehicle("FERNDALE(1630kg)");
            CABIN = GameObject.Find("CABIN");
            AXLES = SATSUMA_2.GetComponent<Axles>();
            CAR_DYNAMICS = SATSUMA_2.GetComponent<CarDynamics>();
            ModConsole.Print("Cars Done");

            //Locations and objects that can be enabled and disabled easily on proximity
            gameObjects.Add(GameObject.Find("BOAT")); //Boat is not a Car, oddly enough.
            //gameObjects.Add(GameObject.Find("CABIN"));
            gameObjects.Add(GameObject.Find("COTTAGE"));
            gameObjects.Add(GameObject.Find("DANCEHALL"));
            //gameObjects.Add(GameObject.Find("DRAGRACE")); //Is broken when disabled, so leave enabled
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

            ModConsole.Print("GameObjects Done");

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
            KINEMATIC = SATSUMA.Object.GetComponent<Rigidbody>();

            // Get all minor objects from the game world (like beer cases, sausages)
            // Only items that are in the listOfMinorObjects list, and also contain "(itemx)" in their name will be loaded
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in allObjects)
                foreach (string itemName in listOfMinorObjects)
                    if (gameObject.name.Contains(itemName) && gameObject.name.Contains("(itemx)"))
                        minorObjects.Add(gameObject);

            ModConsole.Print("[KruFPS] Found all objects");
            //DrawDistance = (float)RenderDistance.GetValue();
        }

        public static void UpdateDrawDistance()
        {
            DrawDistance = (float)RenderDistance.GetValue();
        }
        Settings Satsuma = new Settings("Satsuma", "Enable/Disable Satsuma", false);
        static Settings RenderDistance = new Settings("slider", "Render Distance", 420, UpdateDrawDistance);
        Settings ferndale = new Settings("ferndale", "Ferndale", false);
        Settings flatbed = new Settings("flatbed", "Flatbed", false);
        Settings gifu = new Settings("gifu", "Gifu", false);
        Settings hayosiko = new Settings("hayosiko", "Hayosiko", false);
        Settings jonnez = new Settings("jonnez", "Jonnez", false);
        Settings kekmet = new Settings("kekmet", "Kekmet", false);
        Settings rusko = new Settings("rusko", "Rusko", false);
        Settings cabin = new Settings("cabin", "Unload the Cabin", false);
        Settings minorobjects = new Settings("minorObjects", "Minor objects (ex. beer cases, sausages...)", false);
        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings.
            Settings.AddHeader(this, "Warning: Enabling these removes more lag but can break the game until you save and reload.");
            Settings.AddText(this, "The Satsuma has parts disabled, the others are completely disabled.");
            Settings.AddCheckBox(this, Satsuma);
            Settings.AddCheckBox(this, ferndale);
            Settings.AddCheckBox(this, flatbed);
            Settings.AddCheckBox(this, gifu);
            Settings.AddCheckBox(this, hayosiko);
            Settings.AddCheckBox(this, jonnez);
            Settings.AddCheckBox(this, kekmet);
            Settings.AddCheckBox(this, rusko);
            Settings.AddCheckBox(this, minorobjects);
            Settings.AddText(this, "Check this if you're not using the cabin.");
            Settings.AddCheckBox(this, cabin);
            Settings.AddText(this, "Turn this down if you have a weak GPU.");
            Settings.AddSlider(this, RenderDistance, 1, 6000);

        }

        public override void OnSave()
        {
            // Called once, when save and quit
            // Serialize your save file here.
            foreach (var item in awayFromHouse)
            {
                EnableDisable(item, true); //ENABLE
            }
            foreach(var item in gameObjects)
            {
                EnableDisable(item, true); //ENABLE
            }
            FERNDALE.EnableDisable(true);
            FLATBED.EnableDisable(true);
            GIFU.EnableDisable(true);
            HAYOSIKO.EnableDisable(true);
            JONNEZ.EnableDisable(true);
            KEKMET.EnableDisable(true);
            RUSKO.EnableDisable(true);
            CABIN.SetActive(true);
            KINEMATIC.isKinematic = true;

            foreach (var item in minorObjects)
            {
                EnableDisable(item, true);
            }
        }

        public override void OnGUI()
        {
            // Draw unity OnGUI() here
        }

        int Frame = 0;
        int ResetPeriod = 300;
        public override void Update()
        {
            if (Frame == ResetPeriod)
            {
                //Code to run once every second assuming 60 FPS
                Camera.main.farClipPlane = DrawDistance;
                //ModConsole.Print(RenderDistance.GetValue());
                foreach (var item in gameObjects)
                {
                    EnableDisable(item, ShouldEnable(PLAYER.transform, item.transform));
                }
                //CARS
                if ((bool)Satsuma.GetValue() == true) //Satsuma
                {
                    if (Distance(PLAYER.transform, SATSUMA.transform) > 5)
                    {
                        KINEMATIC.isKinematic = true;
                        AXLES.enabled = true;
                        CAR_DYNAMICS.enabled = true;
                    }
                    else
                    {
                        KINEMATIC.isKinematic = false;
                        AXLES.enabled = false;
                        CAR_DYNAMICS.enabled = false;
                    }
                    // ^ Mild Performance Win
                    //EnableDisable(SATSUMA, ShouldEnable(PLAYER.transform, SATSUMA.transform));
                }
                if ((bool)flatbed.GetValue() == true)
                {
                    FLATBED.EnableDisable(ShouldEnable(PLAYER.transform, FLATBED.transform));
                }
                if ((bool)gifu.GetValue() == true)
                {
                    GIFU.EnableDisable(ShouldEnable(PLAYER.transform, GIFU.transform));
                }
                if ((bool)hayosiko.GetValue() == true)
                {
                    HAYOSIKO.EnableDisable(ShouldEnable(PLAYER.transform, HAYOSIKO.transform));
                }
                if ((bool)jonnez.GetValue() == true)
                {
                    JONNEZ.EnableDisable(ShouldEnable(PLAYER.transform, JONNEZ.transform));
                }
                if ((bool)rusko.GetValue() == true)
                {
                    RUSKO.EnableDisable(ShouldEnable(PLAYER.transform, RUSKO.transform));
                }
                if ((bool)ferndale.GetValue() == true)
                {
                    FERNDALE.EnableDisable(ShouldEnable(PLAYER.transform, FERNDALE.transform));
                }
                if ((bool)kekmet.GetValue() == true)
                {
                    KEKMET.EnableDisable(ShouldEnable(PLAYER.transform, KEKMET.transform));
                }
                if((bool)cabin.GetValue() == true)
                {
                    EnableDisable(CABIN, ShouldEnable(PLAYER.transform, CABIN.transform));
                }
                if ((bool)minorobjects.GetValue() == true)
                {
                    foreach (GameObject obj in minorObjects)
                        EnableDisable(obj, ShouldEnable(PLAYER.transform, obj.transform));
                }

                //Away from house
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
            if (Frame > ResetPeriod)
            {
                if(ResetPeriod > 60)
                {
                    ResetPeriod = 60;
                }
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
                return distance < distanceTarget;
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

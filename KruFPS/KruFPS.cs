using MSCLoader;
using System.Collections.Generic;
using UnityEngine;

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
        private Gifu GIFU;
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

        string[] listOfMinorObjects = {"ax", "beer case", "booze", "brake fluid", "cigarettes", "coffee pan", "coffee cup", "coolant", "diesel",
        "empty plastic can", "fire extinguisher", "gasoline", "grill", "grill charcoal", "ground coffee", "juice", "kilju", "lamp", "macaronbox", "milk",
        "moosemeat", "mosquito spray", "motor oil", "oilfilter", "pike", "pizza", "ratchet set", "potato chips", "sausages", "sugar", "spanner set",
        "spray can", "two stroke fuel", "wiring mess", "wood carrier", "yeast" };

        private Store STORE;
        private RepairShop REPAIRSHOP;

        // Objects that are further than others and needto be rendered earlier
        List<GameObject> farGameObjects = new List<GameObject>();

        private static float DrawDistance = 420;
        //private KeyValuePair<GameObject, Vector3> internalcars = new KeyValuePair<GameObject, Vector3>();
        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            gameObjects = new List<GameObject>();

            //Player Vehicles
            // For each vehicle in the game, a new instance of Vehicle class is initialized.
            SATSUMA = new Vehicle("SATSUMA(557kg, 248)");
            SATSUMA_2 = GameObject.Find("SATSUMA(557kg, 248)");
            FLATBED = new Vehicle("FLATBED");
            GIFU = new Gifu("GIFU(750/450psi)");
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
            gameObjects.Add(GameObject.Find("WATERFACILITY"));
            gameObjects.Add(GameObject.Find("KILJUGUY"));
            gameObjects.Add(GameObject.Find("CHURCHWALL"));
            gameObjects.Add(GameObject.Find("TREES1_COLL"));
            gameObjects.Add(GameObject.Find("TREES2_COLL"));
            gameObjects.Add(GameObject.Find("TREES3_COLL"));

            // Initialize Store class
            STORE = new Store();
            REPAIRSHOP = new RepairShop();

            // Find house of Teimo and detach it from Perajarvi, so it can be loaded and unloaded separately
            // It shouldn't cause any issues, but that needs testing.
            GameObject perajarvi = GameObject.Find("PERAJARVI");
            GameObject TEIMO_HOUSE = perajarvi.transform.Find("HouseRintama4").gameObject;
            TEIMO_HOUSE.transform.parent = null;
            // Same for chicken house
            GameObject CHICKEN_HOUSE = perajarvi.transform.Find("ChickenHouse").gameObject;
            CHICKEN_HOUSE.transform.parent = null;

            // Now that Teimo's house and chicken house is separated from Perajarvi, we can manage them separately. We're throwing them to gameObjects.
            // Fixes the bug with both dissapearing when leaving Perajarvi, even tho logically they should still load when approached.
            gameObjects.Add(TEIMO_HOUSE);
            gameObjects.Add(CHICKEN_HOUSE);

            // Fix for disappearing grain processing plant
            // https://my-summer-car.fandom.com/wiki/Grain_processing_plant
            //
            // It also puts them to farGameObjects - objects that are larger and need to be rendered from further distance
            foreach (Transform trans in perajarvi.GetComponentsInChildren<Transform>())
            {
                if (trans.gameObject.name.Contains("silo"))
                {
                    trans.parent = null;
                    farGameObjects.Add(trans.gameObject);
                }
            }

            // Chicken house (barn) close to player's house
            Transform playerChickenHouse = GameObject.Find("Buildings").transform.Find("ChickenHouse");
            playerChickenHouse.parent = null;
            gameObjects.Add(playerChickenHouse.gameObject);

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
            // (*1) Figure out how to trigger a restock at Tiemos on Thursdays without loading it.

            //NOTES:
            // (*1) Partially addressed the Teimo's issue, by unloading part of the shop

            //Camera.main.farClipPlane = (int)RenderDistance.Value; //Helps with lower end GPU's. This specific value. Any others are wrong.
            PLAYER = GameObject.Find("PLAYER");
            YARD = GameObject.Find("YARD");                     //Used to find out how far the player is from the Object
            KINEMATIC = SATSUMA.Object.GetComponent<Rigidbody>();

            // Get all minor objects from the game world (like beer cases, sausages)
            // Only items that are in the listOfMinorObjects list, and also contain "(itemx)" in their name will be loaded
            // UPDATED: added support for (Clone) items
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in allObjects)
                foreach (string itemName in listOfMinorObjects)
                    if (gameObject.name.Contains(itemName) && gameObject.name.ContainsAny("(itemx)", "(Clone)"))
                        minorObjects.Add(gameObject);

            ModConsole.Print("[KruFPS] Found all objects");
            DrawDistance = float.Parse(RenderDistance.GetValue().ToString()); //Update saved draw distance variable
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
        Settings teimoshop = new Settings("teimoshop", "Teimo's Shop", false);
        Settings fleetarirepairshop = new Settings("fleetarirepairshop", "Fleetari Repair Shop", false);
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
            Settings.AddCheckBox(this, teimoshop);
            Settings.AddCheckBox(this, fleetarirepairshop);
            Settings.AddText(this, "Check this if you're not using it.");
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
            foreach (var item in gameObjects)
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

            STORE.EnableDisable(true);
            REPAIRSHOP.EnableDisable(true);
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

                // Objects that are larger and need to enabled earlier
                foreach (var item in farGameObjects)
                {
                    EnableDisable(item, ShouldEnable(PLAYER.transform, item.transform, 400));
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
                if ((bool)cabin.GetValue() == true)
                {
                    EnableDisable(CABIN, ShouldEnable(PLAYER.transform, CABIN.transform));
                }
                if ((bool)minorobjects.GetValue() == true)
                {
                    for (int i = 0; i < minorObjects.Count; i++)
                    {
                        EnableDisable(minorObjects[i], ShouldEnable(PLAYER.transform, minorObjects[i].transform));
                    }
                }
                if ((bool)teimoshop.GetValue() == true)
                {
                    STORE.EnableDisable(ShouldEnable(PLAYER.transform, STORE.transform));
                }
                if ((bool)fleetarirepairshop.GetValue() == true)
                {
                    REPAIRSHOP.EnableDisable(ShouldEnable(PLAYER.transform, REPAIRSHOP.transform));
                }

                //Away from house
                if (Distance(PLAYER.transform, YARD.transform) > 100)
                {
                    for (int i = 0; i < awayFromHouse.Count; i++)
                    {
                        EnableDisable(awayFromHouse[i], true);
                    }
                }
                else
                {
                    for (int i = 0; i < awayFromHouse.Count; i++)
                    {
                        EnableDisable(awayFromHouse[i], false);
                    }
                }
            }
            Frame++;
            if (Frame > ResetPeriod)
            {
                if (ResetPeriod > 60)
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
            return distance < distanceTarget;
        }
        private float Distance(Transform player, Transform target)
        {
            //Gets Distance.
            return Vector3.Distance(player.position, target.position);
        }
        private void EnableDisable(GameObject thing, bool enabled)
        {
            if (thing != null && thing.activeSelf != enabled)
                thing.SetActive(enabled);
        }
    }
}
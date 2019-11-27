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
        private GameObject DRAGRACE;
        //private List<GameObject> Cars;

        List<GameObject> minorObjects = new List<GameObject>();
        // List of all whitelisted objects that can appear on the minorObjects list
        // Note: batteries aren't included

        string[] listOfMinorObjects = {"ax", "beer case", "booze", "brake fluid", "cigarettes", "coffee pan", "coffee cup", "coolant", "diesel",
        "empty plastic can", "fire extinguisher", "gasoline", "grill", "grill charcoal", "ground coffee", "juice", "kilju", "lamp", "macaronbox", "milk",
        "moosemeat", "mosquito spray", "motor oil", "oilfilter", "pike", "pizza", "ratchet set", "potato chips", "sausages", "sugar", "spanner set",
        "spray can", "two stroke fuel", "wiring mess", "wood carrier", "yeast", "shopping bag" };

        private Store STORE;
        private RepairShop REPAIRSHOP;
        //private DragRace DRAGRACE;

        // Objects that should be loaded from further distance, because it looks terrbile when they load at default value...
        List<GameObject> farGameObjects = new List<GameObject>();

        private static float DrawDistance = 420;


        public override void OnLoad()
        {
            gameObjects = new List<GameObject>();

            //Player Vehicles
            // For each vehicle in the game, a new instance of Vehicle class is initialized.
            SATSUMA = new Vehicle("SATSUMA(557kg, 248)");
            FLATBED = new Vehicle("FLATBED");
            GIFU = new Gifu("GIFU(750/450psi)");
            HAYOSIKO = new Vehicle("HAYOSIKO(1500kg, 250)");
            JONNEZ = new Vehicle("JONNEZ ES(Clone)");
            KEKMET = new Vehicle("KEKMET(350-400psi)");
            RUSKO = new Vehicle("RCO_RUSCKO12(270)");
            FERNDALE = new Vehicle("FERNDALE(1630kg)");
            CABIN = GameObject.Find("CABIN");
            AXLES = SATSUMA.Object.GetComponent<Axles>();
            CAR_DYNAMICS = SATSUMA.Object.GetComponent<CarDynamics>();
            ModConsole.Print("Cars Done");

            //Locations and objects that can be enabled and disabled easily on proximity
            gameObjects.Add(GameObject.Find("BOAT")); //Boat is not a Car, oddly enough.
            gameObjects.Add(GameObject.Find("COTTAGE"));
            gameObjects.Add(GameObject.Find("DANCEHALL"));
            gameObjects.Add(GameObject.Find("INSPECTION"));
            gameObjects.Add(GameObject.Find("LANDFILL"));
            gameObjects.Add(GameObject.Find("PERAJARVI"));
            gameObjects.Add(GameObject.Find("RYKIPOHJA"));
            gameObjects.Add(GameObject.Find("SOCCER"));
            gameObjects.Add(GameObject.Find("WATERFACILITY"));
            gameObjects.Add(GameObject.Find("KILJUGUY"));
            gameObjects.Add(GameObject.Find("TREES1_COLL"));
            gameObjects.Add(GameObject.Find("TREES2_COLL"));
            gameObjects.Add(GameObject.Find("TREES3_COLL"));

            // Initialize Store class
            STORE = new Store();
            REPAIRSHOP = new RepairShop();

            DRAGRACE = GameObject.Find("DRAGRACE");

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

            // Chicken house (barn) close to player's house
            Transform playerChickenHouse = GameObject.Find("Buildings").transform.Find("ChickenHouse");
            playerChickenHouse.parent = null;
            gameObjects.Add(playerChickenHouse.gameObject);

            // Fix for church wall. Changing it's parent to CHURCH, so it will loaded with all Perajarvi
            GameObject.Find("CHURCHWALL").transform.parent = GameObject.Find("CHURCH").transform;

            // Fix for old house on the way from Perajarvi to Ventti's house (HouseOld5)
            Transform houseOld5 = perajarvi.transform.Find("HouseOld5");
            houseOld5.parent = null;
            gameObjects.Add(houseOld5.gameObject);

            // Perajarvi fixes for multiple objects with the same name
            foreach (Transform trans in perajarvi.GetComponentsInChildren<Transform>())
            {
                // Fix for disappearing grain processing plant
                // https://my-summer-car.fandom.com/wiki/Grain_processing_plant
                // It also puts them to farGameObjects - objects that are larger and need to be rendered from further distance
                if (trans.gameObject.name.Contains("silo"))
                {
                    trans.parent = null;
                    farGameObjects.Add(trans.gameObject);
                    continue;
                }

                // Fix for Ventti's and Teimo's mailboxes (and pretty much all mailboxes that are inside of Perajarvi)
                if (trans.gameObject.name == "MailBox")
                {
                    trans.parent = null;
                    gameObjects.Add(trans.gameObject);
                    continue;
                }

                // Fix for greenhouses on the road from Perajarvi to Ventti's house
                if (trans.name == "Greenhouse")
                {
                    trans.parent = null;
                    gameObjects.Add(trans.gameObject);
                    continue;
                }
            }

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
            // (*2) Figure out how to make repairs works at Fleetari's without loading it
            // (*1) Figure out how to trigger a restock at Tiemos on Thursdays without loading it.

            //NOTES:
            // (*1) Partially addressed the Teimo's issue, by unloading part of the shop
            // (*2) Partially addressed the same way as for Teimo's shop

            //Camera.main.farClipPlane = (int)RenderDistance.Value; //Helps with lower end GPU's. This specific value. Any others are wrong.
            PLAYER = GameObject.Find("PLAYER");
            YARD = GameObject.Find("YARD");                     //Used to find out how far the player is from the Object
            KINEMATIC = SATSUMA.Object.GetComponent<Rigidbody>();

            // Get all minor objects from the game world (like beer cases, sausages)
            // Only items that are in the listOfMinorObjects list, and also contain "(itemx)" in their name will be loaded
            // UPDATED: added support for (Clone) items
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in allObjects)
                foreach (string itemName in listOfMinorObjects)
                    if (gameObject.name.Contains(itemName) && gameObject.name.ContainsAny("(itemx)", "(Clone)") && gameObject.activeSelf)
                        minorObjects.Add(gameObject);

            ModConsole.Print("[KruFPS] Found all objects");
            DrawDistance = float.Parse(RenderDistance.GetValue().ToString()); //Update saved draw distance variable
            HookAllSavePoints(); //Hook all save points (it's before first pass of Update)
        }


        public static void UpdateDrawDistance()
        {
            DrawDistance = (float)RenderDistance.GetValue();
        }
        Settings Satsuma = new Settings("Satsuma", "Satsuma (only parts disabled)", false);
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
        Settings dragstrip = new Settings("dragstrip", "Dragstrip", false);
        Settings minorobjects = new Settings("minorObjects", "Minor objects (ex. beer cases, sausages...)", false);
        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings.
            Settings.AddHeader(this, "Performance settings", new Color32(0, 128, 0, 255));
            Settings.AddText(this, "<color=orange><b>Warning: Enabling these removes more lag but can break the game until you save and reload.</b></color>");
            //Settings.AddText(this, "The Satsuma has parts disabled, the others are completely disabled.");
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
            Settings.AddCheckBox(this, dragstrip);
            Settings.AddText(this, "Check this if you're not using the cabin.");
            Settings.AddCheckBox(this, cabin);
            Settings.AddHeader(this, "Graphics settings", Color.green);
            Settings.AddText(this, "Turn this down if you have a weak GPU.");
            Settings.AddSlider(this, RenderDistance, 1, 6000);

        }
        private void HookAllSavePoints()
        {
            //Here we hook into all save points
            //So we can do stuff Before SaveGame event is fired,
            //In PrepareForSaveGame() function.
            FsmHook.FsmInject(GameObject.Find("REPAIRSHOP").transform.Find("LOD/SHITHOUSE/SavePivot").GetChild(0).gameObject, "Mute audio", PrepareForSaveGame);
            if (!GameObject.Find("STORE").transform.Find("LOD/SHITHOUSE/SavePivot").GetChild(0).gameObject.activeSelf)
                GameObject.Find("STORE").transform.Find("LOD/SHITHOUSE/SavePivot").GetChild(0).gameObject.SetActive(true);
            FsmHook.FsmInject(GameObject.Find("STORE").transform.Find("LOD/SHITHOUSE/SavePivot").GetChild(0).gameObject, "Mute audio", PrepareForSaveGame);
            FsmHook.FsmInject(GameObject.Find("COTTAGE").transform.Find("SHITHOUSE/SavePivot").GetChild(0).gameObject, "Mute audio", PrepareForSaveGame);
            if (!GameObject.Find("CABIN").transform.Find("SHITHOUSE/SavePivot").GetChild(0).gameObject.activeSelf)
                GameObject.Find("CABIN").transform.Find("SHITHOUSE/SavePivot").GetChild(0).gameObject.SetActive(true);
            FsmHook.FsmInject(GameObject.Find("CABIN").transform.Find("SHITHOUSE/SavePivot").GetChild(0).gameObject, "Mute audio", PrepareForSaveGame);
            FsmHook.FsmInject(GameObject.Find("LANDFILL").transform.Find("SHITHOUSE/SavePivot").GetChild(0).gameObject, "Mute audio", PrepareForSaveGame);
            FsmHook.FsmInject(GameObject.Find("YARD").transform.Find("Building/LIVINGROOM/LOD_livingroom/SAVEGAME").gameObject, "Mute audio", PrepareForSaveGame);
            if(GameObject.Find("JAIL") != null)
                FsmHook.FsmInject(GameObject.Find("JAIL/SAVEGAME"), "Mute audio", PrepareForSaveGame);

        }
        private void PrepareForSaveGame()
        {
            Debug.Log("[KruFPS] Prepare for save started!");
            //Prepare everything here for SAVEGAME event
            try
            {
                foreach (GameObject item in awayFromHouse)
                {
                    EnableDisable(item, true); //ENABLE
                }
                foreach (GameObject item in gameObjects)
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
                AXLES.enabled = true;
                CAR_DYNAMICS.enabled = true;
                foreach (GameObject item in minorObjects)
                {
                    EnableDisable(item, true);
                }

                STORE.EnableDisable(true);
                REPAIRSHOP.EnableDisable(true);
            }
            catch (System.Exception e)
            {
                //Don't break playmaker state, catch potential error in output_log.txt
                Debug.Log("[KruFPS] Prepare for save failed with Exception:");
                Debug.Log(e);
            }
            Debug.Log("[KruFPS] Prepare for save finished!");
        }

        public override void OnGUI()
        {
            // Draw unity OnGUI() here
        }
        float timer = 0.0f;
        public override void Update()
        {
            timer += Time.deltaTime;
            float seconds = (timer % 60);
            if (seconds <= 1f) return; 
            //return if one second didn't pass. 

            //if we are here, one second passed so reset timer and do rest of the code.
            timer = 0f;


            Camera.main.farClipPlane = DrawDistance;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                EnableDisable(gameObjects[i], ShouldEnable(PLAYER.transform, gameObjects[i].transform));
            }

            // Objects that are larger and need to enabled earlier
            for (int i = 0; i < farGameObjects.Count; i++)
            {
                EnableDisable(farGameObjects[i], ShouldEnable(PLAYER.transform, farGameObjects[i].transform, 400));
            }

            //CARS
            if ((bool)Satsuma.GetValue() == true) //Satsuma
            {
                if (Distance(PLAYER.transform, SATSUMA.transform) > 5)
                {
                    KINEMATIC.isKinematic = true;
                    AXLES.enabled = false;
                    CAR_DYNAMICS.enabled = false;
                }
                else
                {
                    KINEMATIC.isKinematic = false;
                    AXLES.enabled = true;
                    CAR_DYNAMICS.enabled = true;
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
            if ((bool)dragstrip.GetValue() == true)
            {
                // Large distance ammount for Dragstrip, to let the script run, even on the second end of drag strip
                EnableDisable(DRAGRACE, ShouldEnable(PLAYER.transform, DRAGRACE.transform, 1100));
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

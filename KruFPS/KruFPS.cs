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
        public override string Version => "4.0"; //Version

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

        private Store STORE;
        private RepairShop REPAIRSHOP;

        private List<Objects> gameObjects;
        // Objects that should be loaded from further distance, because it looks terrbile when they load at default value...
        //List<GameObject> farGameObjects = new List<GameObject>();

        private static float DrawDistance = 420;

        public override void OnLoad()
        {
            gameObjects = new List<Objects>();

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
            gameObjects.Add(new Objects(GameObject.Find("BOAT"))); //Boat is not a Car, oddly enough.
            gameObjects.Add(new Objects(GameObject.Find("COTTAGE")));
            gameObjects.Add(new Objects(GameObject.Find("DANCEHALL")));
            gameObjects.Add(new Objects(GameObject.Find("INSPECTION")));
            gameObjects.Add(new Objects(GameObject.Find("LANDFILL")));
            gameObjects.Add(new Objects(GameObject.Find("PERAJARVI")));
            gameObjects.Add(new Objects(GameObject.Find("RYKIPOHJA")));
            gameObjects.Add(new Objects(GameObject.Find("SOCCER")));
            gameObjects.Add(new Objects(GameObject.Find("WATERFACILITY")));
            gameObjects.Add(new Objects(GameObject.Find("TREES1_COLL")));
            gameObjects.Add(new Objects(GameObject.Find("TREES2_COLL")));
            gameObjects.Add(new Objects(GameObject.Find("TREES3_COLL")));

            // Initialize Store class
            STORE = new Store();
            REPAIRSHOP = new RepairShop();
            gameObjects.Add(new Objects(GameObject.Find("DRAGRACE"), 1100));

            Transform buildings = GameObject.Find("Buildings").transform;

            // Find house of Teimo and detach it from Perajarvi, so it can be loaded and unloaded separately
            // It shouldn't cause any issues, but that needs testing.
            GameObject perajarvi = GameObject.Find("PERAJARVI");
            perajarvi.transform.Find("HouseRintama4").parent = buildings;
            // Same for chicken house
            perajarvi.transform.Find("ChickenHouse").parent = buildings;

            // Chicken house (barn) close to player's house
            Transform playerChickenHouse = GameObject.Find("Buildings").transform.Find("ChickenHouse");
            playerChickenHouse.parent = null;

            // Fix for church wall. Changing it's parent ot NULL, so it will not be loaded or unloaded.
            // It used to be changed to CHURCH gameobject, 
            // but the Amis cars (yellow and grey cars) used to end up in the graveyard area.
            GameObject.Find("CHURCHWALL").transform.parent = null;

            // Fix for old house on the way from Perajarvi to Ventti's house (HouseOld5)
            perajarvi.transform.Find("HouseOld5").parent = buildings;

            // Perajarvi fixes for multiple objects with the same name.
            // Instead of being the part of Perajarvi, we're changing it to be the part of Buildings.
            foreach (Transform trans in perajarvi.GetComponentsInChildren<Transform>())
            {
                // Fix for disappearing grain processing plant
                // https://my-summer-car.fandom.com/wiki/Grain_processing_plant
                if (trans.gameObject.name.Contains("silo"))
                {
                    trans.parent = buildings;
                    continue;
                }

                // Fix for Ventti's and Teimo's mailboxes (and pretty much all mailboxes that are inside of Perajarvi)
                if (trans.gameObject.name == "MailBox")
                {
                    trans.parent = buildings;
                    continue;
                }

                // Fix for greenhouses on the road from Perajarvi to Ventti's house
                if (trans.name == "Greenhouse")
                {
                    trans.parent = buildings;
                    continue;
                }
            }

            // Possible fix for Jokke.
            // Needs testing
            foreach (Transform trans in GameObject.Find("KILJUGUY").transform.GetComponentsInChildren<Transform>())
            {
                gameObjects.Add(new Objects(trans.gameObject));
            }

            // Removes the mansion from the Buildings, so the tires will not land under the mansion.
            GameObject.Find("autiotalo").transform.parent = null;

            ModConsole.Print("GameObjects Done");

            //Things that should be enabled when out of proximity of the house
            gameObjects.Add(new Objects(GameObject.Find("NPC_CARS"), awayFromHouse: true));
            gameObjects.Add(new Objects(GameObject.Find("RALLY"), awayFromHouse: true));
            gameObjects.Add(new Objects(GameObject.Find("TRAFFIC"), awayFromHouse: true));
            gameObjects.Add(new Objects(GameObject.Find("TRAIN"), awayFromHouse: true));
            gameObjects.Add(new Objects(GameObject.Find("Buildings"), awayFromHouse: true));
            gameObjects.Add(new Objects(GameObject.Find("TrafficSigns"), awayFromHouse: true));
            gameObjects.Add(new Objects(GameObject.Find("ELEC_POLES"), awayFromHouse: true));
            gameObjects.Add(new Objects(GameObject.Find("StreetLights"), awayFromHouse: true));

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

            // Initialize MinorObjects
            new MinorObjects();

            ModConsole.Print("[KruFPS] Found all objects");
            DrawDistance = float.Parse(RenderDistance.GetValue().ToString()); //Update saved draw distance variable
            HookAllSavePoints(); //Hook all save points (it's before first pass of Update)

            //Camera.main.gameObject.AddComponent<CameraHook>();
        }

        public static void UpdateDrawDistance()
        {
            DrawDistance = (float)RenderDistance.GetValue();
        }

        Settings Satsuma = new Settings("Satsuma", "Satsuma (only parts disabled)", false);
        static Settings RenderDistance = new Settings("slider", "Render Distance", 3000, UpdateDrawDistance);
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
        public static Settings FogDensity = new Settings("fogDensity", "Fog density", 100, UpdateDrawDistance);

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
            Settings.AddText(this, "Check this if you're not using the cabin.");
            Settings.AddCheckBox(this, cabin);
            Settings.AddHeader(this, "Graphics settings", Color.green);
            Settings.AddText(this, "Turn this down if you have a weak GPU.");
            Settings.AddSlider(this, RenderDistance, 1, 6000);
            //Settings.AddSlider(this, FogDensity, 0, 100);

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
                foreach (Objects item in gameObjects)
                {
                    item.EnableDisable(true); //ENABLE
                }
                FERNDALE.EnableDisable(true);
                FLATBED.EnableDisable(true);
                GIFU.EnableDisable(true);
                HAYOSIKO.EnableDisable(true);
                JONNEZ.EnableDisable(true);
                KEKMET.EnableDisable(true);
                RUSKO.EnableDisable(true);
                CABIN.SetActive(true);
                KINEMATIC.isKinematic = false;
                AXLES.enabled = true;
                CAR_DYNAMICS.enabled = true;
                foreach (ObjectHook item in MinorObjects.instance.ObjectHooks)
                {
                    EnableDisable(item.gm, true);
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

            // Go through the list gameObjects list
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].AwayFromHouse)
                {
                    // Should the object be disabled when the player leaves the house?
                    gameObjects[i].EnableDisable(Distance(PLAYER.transform, YARD.transform) > 100);
                }
                else
                {
                    // The object will be disables, if the player is in the range of that object.
                    gameObjects[i].EnableDisable(ShouldEnable(PLAYER.transform, gameObjects[i].transform, gameObjects[i].Distance));
                }
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
                for (int i = 0; i < MinorObjects.instance.ObjectHooks.Count; i++)
                {
                    MinorObjects.instance.ObjectHooks[i].EnableDisable(
                        ShouldEnable(PLAYER.transform, MinorObjects.instance.ObjectHooks[i].gm.transform));
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


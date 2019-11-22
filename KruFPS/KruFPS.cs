using MSCLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using System.IO;
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
        private List<GameObject> gameObjects;

        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            gameObjects = new List<GameObject>();
            gameObjects.Add(GameObject.Find("BOAT"));
            gameObjects.Add(GameObject.Find("FERNDALE(1630kg)"));
            gameObjects.Add(GameObject.Find("FLATBED"));
            gameObjects.Add(GameObject.Find("GIFU(750/450psi)"));
            gameObjects.Add(GameObject.Find("HAYOSIKO(1500kg, 250)"));
            gameObjects.Add(GameObject.Find("JONNEZ ES(Clone)"));
            gameObjects.Add(GameObject.Find("KEKMET(350-400psi)"));
            gameObjects.Add(GameObject.Find("RCO_RUSCKO12(270)"));
            //gameObjects.Add(GameObject.Find("SATSUMA(557kg, 248)"));
            gameObjects.Add(GameObject.Find("CABIN"));
            gameObjects.Add(GameObject.Find("COTTAGE"));
            gameObjects.Add(GameObject.Find("DANCEHALL"));
            gameObjects.Add(GameObject.Find("DRAGRACE"));
            gameObjects.Add(GameObject.Find("INSPECTION"));
            gameObjects.Add(GameObject.Find("LANDFILL"));
            gameObjects.Add(GameObject.Find("PERAJARVI"));
            gameObjects.Add(GameObject.Find("REPAIRSHOP"));
            gameObjects.Add(GameObject.Find("RYKIPOHJA"));
            gameObjects.Add(GameObject.Find("SOCCER"));
            gameObjects.Add(GameObject.Find("STORE"));
            gameObjects.Add(GameObject.Find("WATERFACILITY"));
            gameObjects.Add(GameObject.Find("KILJUGUY"));
            gameObjects.Add(GameObject.Find("CHURCHWALL"));
            gameObjects.Add(GameObject.Find("TREES1_COLL"));
            gameObjects.Add(GameObject.Find("TREES2_COLL"));
            gameObjects.Add(GameObject.Find("TREES3_COLL"));
            
           

            //TODO: When not at house:
            
            // NPC_CARS 
            // RALLY
            // TRAFFIC
            // TRAIN
            // Buildings
            // TrafficSigns
            // StreetLights
            // ELEC_POLES
            // 

            //TODO: Solve Bugs from Unloading/Reloading Satsuma
            // Bugs: 
            // Can't open doors
            // May randomly fall through floor
            
            Camera.main.farClipPlane = 420; //Helps with lower end GPU's. This specific value. Any others are wrong.
            PLAYER = GameObject.Find("PLAYER");
            ModConsole.Print("[KruFPS] Found all objects");
        }
        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings.
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

                //Code to run once every second
                //ShouldEnable(PLAYER.transform, HAYOSIKOG.transform));
                foreach (var item in gameObjects)
                {
                    EnableDisable(item, ShouldEnable(PLAYER.transform, item.transform));
                }

            }
            Frame++;
            if (Frame > 60)
            {
                Frame = 0;
            }
        }

        private bool ShouldEnable(Transform player, Transform target)
        {
            float distance = Vector3.Distance(player.position, target.position);
            //ModConsole.Print(distance);
            try
            {
                if (distance > 10) //200 is probably optimal
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

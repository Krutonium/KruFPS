﻿using UnityEngine;

namespace KruFPS
{
    class Store : Place
    {
        // Store Class - made by Konrad "Athlon" Figura
        //
        // Extends Place.cs
        //
        // It is responsible for loading and unloading parts of the store, that are safe to be unloaded or loaded again.
        // It gives some performance bennefit, while still letting the shop and Teimo routines running without any issues.
        // Some objects on the WhiteList can be removed, but that needs testing.
        //
        // NOTE: That script DOES NOT disable the store itself, rather some of its childrens.

        // Objects from that whitelist will not be disabled
        // It is so to prevent from restock script and Teimo's bike routine not working
        string[] whiteList = { "STORE", "SpawnToStore", "BikeStore", "BikeHome", "Inventory", "Collider", "TeimoInShop", "Bicycle",
                                        "bicycle_pedals", "Pedal", "Teimo", "bodymesh", "skeleton", "pelvs", "spine", "collar", "shoulder",
                                        "hand", "ItemPivot", "finger", "collar", "arm", "fingers", "HeadPivot", "head", "eye_glasses_regular",
                                        "teimo_hat", "thig", "knee", "ankle", "TeimoCollider", "OriginalPos", "TeimoInBike", "Pivot", "pelvis", 
                                        "bicycle" };

        string[] whiteListClose = { "STORE", "SpawnToStore", "BikeStore", "BikeHome", "Inventory", "Collider", "TeimoInShop", "Bicycle",
                                        "bicycle_pedals", "Pedal", "Teimo", "bodymesh", "skeleton", "pelvs", "spine", "collar", "shoulder",
                                        "hand", "ItemPivot", "finger", "collar", "arm", "fingers", "HeadPivot", "head", "eye_glasses_regular",
                                        "teimo_hat", "thig", "knee", "ankle", "TeimoCollider", "OriginalPos", "TeimoInBike", "Pivot", "pelvis",
                                        "bicycle", "Collider", "collider", "StoreCashRegister", "cash_register", "Register", "store_", "MESH",
                                        "bikebar", "LOD", "tire", "rim", "MailBox", };

        /// <summary>
        /// Initialize the Store class
        /// </summary>
        public Store() : base("STORE")
        {
            // Two different whitelist, when player is in Perajarvi, and different if the player is outside of Perajarvi
            // whiteListClose for some reaosn doesn't work, if the player loaded for instance in house.
            // And whiteList disables too many objects, if the player loads at Teimo's shop (such as hitboxes, so Amis cars can drive inside of shop area).

            float playerToStoreDistance = Vector3.Distance(GameObject.Find("PLAYER").transform.position, GameObject.Find("STORE").transform.position);
            GameObjectWhiteList =  playerToStoreDistance > 1000 ? whiteList : whiteListClose;
            Childs = GetAllChilds();
        }
    }
}

using UnityEngine;

namespace KruFPS
{
    class Store
    {
        // Store Class - made by Konrad "Athlon" Figura
        //
        // It is responsible for loading and unloading parts of the store, that are safe to be unloaded or loaded again.
        // It gives some performance bennefit, while still letting the shop and Teimo routines running without any issues.
        // Some objects on the WhiteList can be removed, but that needs testing.
        //
        // NOTE: That script DOES NOT disable the store itself, rather some of its childrens.

        public GameObject StoreObject { get; set; }

        // Objects from that whitelist will not be disabled
        // It is so to prevent from restock script and Teimo's bike routine not working
        string[] GameObjectWhiteList = { "STORE", "SpawnToStore", "BikeStore", "BikeHome", "Inventory", "Collider", "TeimoInShop", "Bicycle",
                                        "bicycle_pedals", "Pedal", "Teimo", "bodymesh", "skeleton", "pelvs", "spine", "collar", "shoulder",
                                        "hand", "ItemPivot", "finger", "collar", "arm", "fingers", "HeadPivot", "head", "eye_glasses_regular",
                                        "teimo_hat", "thig", "knee", "ankle", "TeimoCollider", "OriginalPos", "TeimoInBike", "StoreCashRegister",
                                        "cash_register", "Register", "Pivot", "pelvis", "bicycle" };

        /// <summary>
        /// Here are all childs of Store gameobject
        /// </summary>
        Transform[] Childs;

        public Transform transform => StoreObject.transform;

        /// <summary>
        /// Saves what value has been last used, to prevent unnescesary launch of loop.
        /// </summary>
        bool lastValue = true;

        /// <summary>
        /// Initialize the Store class
        /// </summary>
        public Store()
        {
            StoreObject = GameObject.Find("STORE");
            Childs = GetAllChilds();
        }

        /// <summary>
        /// Enable or disable the car
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableDisable(bool enabled)
        {
            if (lastValue == enabled) return;
            lastValue = enabled;

            // Load and unload only the objects that aren't on the whitelist.
            for (int i = 0; i < Childs.Length; i++)
            {
                if (!Childs[i].gameObject.name.ContainsAny(GameObjectWhiteList))
                    Childs[i].gameObject.SetActive(enabled);
            }
        }

        /// <summary>
        /// Returns all childs found under Store gameobject.
        /// </summary>
        /// <returns></returns>
        Transform[] GetAllChilds()
        {
            return StoreObject.transform.GetComponentsInChildren<Transform>();
        }
    }
}

﻿using System.Linq;
using UnityEngine;

namespace KruFPS
{
    class Place
    {
        // Place Class - made by Konrad "Athlon" Figura
        //
        // It is responsible for loading and unloading important to the game places, it is extended by other classes.
        //
        // NOTE: That script DOES NOT disable the store itself, rather some of its childrens.

        // Place class by Konrad "Athlon" Figura

        public GameObject Object { get; set; }

        // Objects from that whitelist will not be disabled
        // It is so to prevent from restock script and Teimo's bike routine not working
        internal string[] GameObjectBlackList;

        /// <summary>
        /// Here are all childs of Store gameobject
        /// </summary>
        internal Transform[] Childs;

        internal Transform[] DisableableChilds;

        public Transform transform => Object.transform;

        /// <summary>
        /// Saves what value has been last used, to prevent unnescesary launch of loop.
        /// </summary>
        internal bool lastValue = true;

        /// <summary>
        /// Initialize the Store class
        /// </summary>
        public Place(string placeName)
        {
            Object = GameObject.Find(placeName);
        }

        /// <summary>
        /// Enable or disable the place
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableDisable(bool enabled)
        {
            if (lastValue == enabled) return;
            lastValue = enabled;

            // Load and unload only the objects that aren't on the whitelist.
            for (int i = 0; i < DisableableChilds.Length; i++)
            {
                DisableableChilds[i].gameObject.SetActive(enabled);
            }
        }

        /// <summary>
        /// Returns all childs found under Store gameobject.
        /// </summary>
        /// <returns></returns>
        internal Transform[] GetAllChilds()
        {
            return Object.transform.GetComponentsInChildren<Transform>(true);
        }

        internal Transform[] GetDisableableChilds()
        {
            return Childs.Where(trans => !trans.gameObject.name.ContainsAny(GameObjectBlackList)).ToArray();
        }
    }
}

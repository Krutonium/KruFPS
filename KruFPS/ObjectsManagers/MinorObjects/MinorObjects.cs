﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KruFPS
{
    class MinorObjects
    {
        // MinorObjects class by Konrad "Athlon" Figura

        // List of all whitelisted objects that can appear on the minorObjects list
        // Note: batteries aren't included
        public string[] listOfMinorObjects = { "ax", "booze", "brake fluid", "cigarettes", "coffee pan", "coffee cup", "coolant", "diesel",
        "empty plastic can", "fire extinguisher", "gasoline", "grill", "grill charcoal", "ground coffee", "juice", "kilju", "lamp", "macaronbox", "milk",
        "moosemeat", "mosquito spray", "motor oil", "oilfilter", "pike", "pizza", "ratchet set", "potato chips", "sausages", "sugar", "spanner set",
        "spray can", "two stroke fuel", "wiring mess", "wood carrier", "yeast", "shopping bag", "flashlight", "beer case" };

        // List of ObjectHooks attached to minor objects
        public List<ObjectHook> ObjectHooks = new List<ObjectHook>();

        public static MinorObjects instance;

        /// <summary>
        /// Initialize MinorObjects class
        /// </summary>
        public MinorObjects()
        {
            instance = this;
            InitializeList();
            HookCashRegister();
        }

        /// <summary>
        /// Add object hook to the list
        /// </summary>
        /// <param name="newHook"></param>
        public void Add(ObjectHook newHook)
        {
            ObjectHooks.Add(newHook);
        }

        /// <summary>
        /// Remove object hook from the list
        /// </summary>
        /// <param name="objectHook"></param>
        public void Remove(ObjectHook objectHook)
        {
            if (ObjectHooks.Contains(objectHook))
            {
                ObjectHooks.Remove(objectHook);
            }
        }

        /// <summary>
        /// Lists all the game objects in the game's world and that are on the whitelist,
        /// then it adds ObjectHook to them
        /// </summary>
        void InitializeList()
        {
            // Get all minor objects from the game world (like beer cases, sausages)
            // Only items that are in the listOfMinorObjects list, and also contain "(itemx)" in their name will be loaded

            GameObject[] minorObjects = Object.FindObjectsOfType<GameObject>()
                .Where(gm => gm.name.ContainsAny(listOfMinorObjects) && gm.name.ContainsAny("(itemx)", "(Clone)") && gm.activeSelf).ToArray();

            for (int i = 0; i < minorObjects.Length; i++)
            {
                minorObjects[i].AddComponent<ObjectHook>();
            }
        }

        /// <summary>
        /// Hooks CashRegisterHook to Register GameObject
        /// </summary>
        private void HookCashRegister()
        {
            GameObject.Find("STORE/StoreCashRegister/Register").AddComponent<CashRegisterHook>();
        }
    }
}

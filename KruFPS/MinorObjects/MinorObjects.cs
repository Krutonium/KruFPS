using System.Collections.Generic;
using UnityEngine;

namespace KruFPS
{
    class MinorObjects
    {
        public List<GameObject> minorObjects = new List<GameObject>();
        // List of all whitelisted objects that can appear on the minorObjects list
        // Note: batteries aren't included

        public string[] listOfMinorObjects = {"ax", "beer case", "booze", "brake fluid", "cigarettes", "coffee pan", "coffee cup", "coolant", "diesel",
        "empty plastic can", "fire extinguisher", "gasoline", "grill", "grill charcoal", "ground coffee", "juice", "kilju", "lamp", "macaronbox", "milk",
        "moosemeat", "mosquito spray", "motor oil", "oilfilter", "pike", "pizza", "ratchet set", "potato chips", "sausages", "sugar", "spanner set",
        "spray can", "two stroke fuel", "wiring mess", "wood carrier", "yeast", "shopping bag", "flashlight" };

        public static MinorObjects instance;

        public MinorObjects()
        {
            instance = this;
            RefreshMinorObjectsList();
            HookMinorObjectsListRefresh();
        }

        private void RefreshMinorObjectsList()
        {
            // Get all minor objects from the game world (like beer cases, sausages)
            // Only items that are in the listOfMinorObjects list, and also contain "(itemx)" in their name will be loaded
            // UPDATED: added support for (Clone) items

            minorObjects.Clear();

            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            for (int i = 0; i < allObjects.Length; i++)
            {
                for (int j = 0; j < listOfMinorObjects.Length; j++)
                {
                    if (allObjects[i].name.Contains(listOfMinorObjects[j])
                        && allObjects[i].name.ContainsAny("(itemx)", "(Clone)")
                        && allObjects[i].activeSelf)
                    {
                        minorObjects.Add(allObjects[i]);
                    }
                }
            }
        }

        private void HookMinorObjectsListRefresh()
        {
            // Hook refresh class to cash register
            GameObject.Find("STORE/StoreCashRegister/Register").AddComponent<MinorObjectsHook>();
        }
    }
}

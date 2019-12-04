using UnityEngine;

namespace KruFPS
{
    class Objects
    {
        // Objects class by Konrad "Athlon" Figura

        /// <summary>
        /// Game object that this instance of the class controls.
        /// </summary>
        public GameObject GameObject { get; set; }

        /// <summary>
        /// How close player has to be to the object, in order to be enabled.
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// If true, the RenderDistance is ignored. Instead of enabling GameObject by distance,
        /// it will be enabled when player is not in the house area.
        /// </summary>
        public bool AwayFromHouse { get; set; }

        public Transform transform => GameObject.transform;

        /// <summary>
        /// Initializes the Objects instance.
        /// </summary>
        /// <param name="gameObject">Game object that this instance controls.</param>
        /// <param name="renderDistance">From how far should that object be enabled (default 200).</param>
        public Objects(GameObject gameObject, int distance = 200)
        {
            this.GameObject = gameObject;
            this.Distance = distance;
        }

        /// <summary>
        /// Initializes the Objects instance.
        /// </summary>
        /// <param name="gameObject">Game object that this instance controls.</param>
        /// <param name="awayFromHouse">If true, the object will be enabled, when the player leaves the house area.</param>
        public Objects(GameObject gameObject, bool awayFromHouse)
        {
            this.GameObject = gameObject;
            this.AwayFromHouse = awayFromHouse;
        }
        
        /// <summary>
        /// Enable or disable the object.
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableDisable(bool enabled)
        {
            if (this.GameObject != null && this.GameObject.activeSelf != enabled)
                this.GameObject.SetActive(enabled);
        }
    }
}

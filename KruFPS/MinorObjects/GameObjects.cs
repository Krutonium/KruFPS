using UnityEngine;
using MSCLoader;

namespace KruFPS
{
    class GameObjects
    {
        public GameObject GameObject { get; set; }
        public float RenderDistance { get; set; }

        public GameObjects(GameObject gameObject, float renderDistance = 200)
        {
            this.GameObject = gameObject;
            this.RenderDistance = renderDistance;
        }

        public void EnableDisable(bool enabled)
        {
            if (this.GameObject != null && this.GameObject.activeSelf != enabled)
                this.GameObject.SetActive(enabled);
        }
    }
}

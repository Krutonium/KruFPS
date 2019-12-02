using UnityEngine;
using MSCLoader;

namespace KruFPS
{
    class ObjectHook : MonoBehaviour
    {
        // This MonoBehaviour hooks to all minor objects.

        public GameObject gm => this.gameObject;
        PlayMakerFSM playMakerFSM;

        Vector3 position;
        Quaternion rotation;

        /// <summary>
        /// Rigidbody of this object.
        /// </summary>
        Rigidbody rb;

        /// <summary>
        /// Stores the current state of an object.
        /// </summary>
        bool currentState;

        public ObjectHook()
        {
            // Add self to the MinorObjects.objectHooks list
            MinorObjects.instance.Add(this);

            // Get the current rotation and position.
            position = gm.transform.position;
            rotation = gm.transform.rotation;

            // Get PlayMakerFSM
            playMakerFSM = gm.GetComponent<PlayMakerFSM>();

            // Get rigidbody
            rb = GetComponent<Rigidbody>();

            // From PlayMakerFSM, find states that contain one of the names that relate to destroying object,
            // and inject RemoveSelf void.
            foreach (var st in playMakerFSM.FsmStates)
            {
                switch (st.Name)
                {
                    case "Destroy self":
                        FsmHook.FsmInject(this.gameObject, "Destroy self", RemoveSelf);
                        break;
                    case "Destroy":
                        FsmHook.FsmInject(this.gameObject, "Destroy", RemoveSelf);
                        break;
                    case "Destroy 2":
                        FsmHook.FsmInject(this.gameObject, "Destroy 2", RemoveSelf);
                        break;
                }
            }
        }

        // Triggered before the object is destroyed.
        // Removes self from MinorObjects.instance.objectHooks.
        public void RemoveSelf()
        {
            MinorObjects.instance.Remove(this);
        }

        public void EnableDisable(bool enabled)
        {
            if (gm == null || this.currentState == enabled)
                return;

            currentState = enabled;

            // Beer cases are treated differently.
            // Instead of disabling entire object, change it's rigibdody values, to prevent them from landing under vehicles.
            // It fixes the problems with bottles in beer cases disappearing.
            if (gm.name.Contains("beer case"))
            {
                rb.detectCollisions = enabled;
                rb.isKinematic = !enabled;
                return;
            }

            // Fix for Carry More mod
            if (rb.isKinematic)
                return;

            // Uppon disabling, save position and rotation
            if (!enabled)
            {
                position = gm.transform.position;
                rotation = gm.transform.rotation;
            }
            
            gm.SetActive(enabled);

            // Uppon reenabling, load position from saved value
            if (enabled)
            {
                // Only teleport object to the last known position, 
                // if the differenc between current and last known position is larger than 2 units.
                if (Vector3.Distance(gm.transform.position, position) < 2)
                    return;

                gm.transform.position = position;
                gm.transform.rotation = rotation;
            }
        }
    }
}

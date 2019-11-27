using System.Linq;
using UnityEngine;

namespace KruFPS
{
    class Vehicle
    {
        // Vehicle class - made by Konrad "Athlon" Figura
        // 
        // It fixes known issue with missing vehicles engine sound by (admittedly hacky way) creating new GameObject,
        // that stores GameObjects responsible for playing sounds in vehicle (named "audio", or "SoundSrc").
        // Whenever the vehicle is disabled, all audio objects are changing parent to that temporary object.
        //
        // It also fixes the issue of vehicles going back to the original spawn position, instead of staying in the same place - as they should,
        // by simply saving the Transform.position and Transform.rotation parameters just before disabling the object, and then loading these values,
        // just after loading them.

        public GameObject Object { get; set; }

        // Values that are being saved or loaded
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        internal Transform[] AudioObjects;
        internal GameObject TemporaryAudioParent;

        // Overwrites the "Component.transform", to prevent eventual mod crashes caused by missuse of Vehicle.transform.
        // Technically, you should use Vehicle.Object.transform (ex. GIFU.Object.Transform), this here just lets you use Vehicle.transform
        // (ex. GIFU.transform).
        public Transform transform => Object.transform;

        /// <summary>
        /// Initialize class
        /// </summary>
        /// <param name="gameObject"></param>
        public Vehicle(string gameObject)
        {
            // Find the object by name
            Object = GameObject.Find(gameObject);

            // Dump the object position and rotation
            Position = Object.transform.localPosition;
            Rotation = Object.transform.localRotation;

            // Get the object's child which are responsible for audio
            AudioObjects = GetAudioObjects();
            // Creates a new gameobject that is names after the original file + '_TEMPAUDIO' (ex. SATSUMA(557kg, 248)_TEMPAUDIO)
            TemporaryAudioParent = new GameObject(Object.name + "_TEMPAUDIO");
        }

        /// <summary>
        /// Retrieves the child audio objects from parent object.
        /// Basically looks for files with "audio" and "SoundSrc" name in it
        /// </summary>
        /// <returns></returns>
        internal Transform[] GetAudioObjects()
        {
            Transform[] childs = Object.transform.GetComponentsInChildren<Transform>();
            return childs.Where(obj => obj.gameObject.name.Contains("audio") || obj.gameObject.name.Contains("SoundSrc")).ToArray();
        }

        /// <summary>
        /// Enable or disable car
        /// </summary>
        public void EnableDisable(bool enabled)
        {
            if (Object == null) return;
            // Don't run the code, if the value is the same
            if (Object.activeSelf == enabled) return;

            // If we're disabling a car, set the audio child parent to TemporaryAudioParent, and save the position and rotation.
            // We're doing that BEFORE we disable the object.
            if (!enabled)
            {
                SetParentForChilds(AudioObjects, TemporaryAudioParent);
                Position = Object.transform.localPosition;
                Rotation = Object.transform.localRotation;
            }

            Object.SetActive(enabled);

            // Uppon enabling the file, set the localPosition and localRotation to the object's transform, and change audio source parents to Object
            // We're doing that AFTER we enable the object.
            if (enabled)
            {
                Object.transform.localPosition = Position;
                Object.transform.localRotation = Rotation;
                SetParentForChilds(AudioObjects, Object);
            }
        }


        /// <summary>
        /// Changes audio childs parent
        /// </summary>
        /// <param name="childs"></param>
        /// <param name="newParent"></param>
        internal void SetParentForChilds(Transform[] childs, GameObject newParent)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].parent = newParent.transform;
            }
        }

        /// <summary>
        /// Changes parent for single child
        /// </summary>
        /// <param name="child"></param>
        /// <param name="newParent"></param>
        internal void SetParentForChild(Transform child, GameObject newParent)
        {
            child.transform.parent = newParent.transform;
        }
    }
}

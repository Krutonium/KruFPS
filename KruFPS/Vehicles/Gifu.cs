using UnityEngine;

namespace KruFPS
{
    class Gifu : Vehicle
    {
        // Gifu class - made by Konrad "Athlon" Figura
        // 
        // This class extends the functionality of Vehicle class, which is tailored for Gifu.
        // It fixes the issue with Gifu's beams being turned on after respawn.

        Transform BeaconSwitchParent;
        Transform BeaconSwitch;

        Transform BeaconsParent;
        Transform Beacons;

        GameObject TemporaryBeaconParent;

        /// <summary>
        /// Initialize class
        /// </summary>
        /// <param name="gameObject"></param>
        public Gifu(string gameObject) : base (gameObject)
        {
            TemporaryBeaconParent = new GameObject(Object.name + "_TEMPBEACON");

            BeaconSwitchParent = Object.transform.Find("Dashboard").Find("Knobs");
            BeaconsParent = Object.transform.Find("LOD");

            BeaconSwitch = BeaconSwitchParent.transform.Find("KnobBeacon");
            Beacons = BeaconsParent.transform.Find("Beacon");
        }

        /// <summary>
        /// Enable or disable car
        /// </summary>
        public new void EnableDisable(bool enabled)
        {
            try
            {
                // Don't run the code, if the value is the same
                if (Object.activeSelf == enabled) return;

                // If we're disabling a car, set the audio child parent to TemporaryAudioParent, and save the position and rotation.
                // We're doing that BEFORE we disable the object.
                if (!enabled)
                {
                    SetParentForChilds(AudioObjects, TemporaryAudioParent);
                    SetParentForChild(BeaconSwitch, TemporaryBeaconParent);
                    SetParentForChild(Beacons, TemporaryBeaconParent);
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
                    SetParentForChild(BeaconSwitch, BeaconSwitchParent.gameObject);
                    SetParentForChild(Beacons, BeaconsParent.gameObject);
                }
            }
            catch { }
        }
    }
}

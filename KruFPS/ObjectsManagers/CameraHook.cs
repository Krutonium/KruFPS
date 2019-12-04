using UnityEngine;

namespace KruFPS
{
    class CameraHook : MonoBehaviour
    {
        // Currently not used
        //
        // Hooks to Camera.main
        // Adds the fog that is supposed to mask the render distance border,
        // by changing RenderSettings every 1 minute that OnPreRender() is toggled by Unity engine.

        float timer = 0.0f;
        
        /// <summary>
        /// Every 10 seconds, start UpdateFog() void.
        /// </summary>
        void OnPreRender()
        {
            timer += Time.deltaTime;
            float seconds = (timer % 60);
            if (seconds <= 10f) return;
            timer = 0f;
            UpdateFog();
        }

        /// <summary>
        /// Retrieves values from skybox material data.
        /// </summary>
        Color GetFogColor()
        {
            return RenderSettings.skybox.GetColor("_SkyTint") + RenderSettings.skybox.GetColor("_GroundColor") * .5f;
        }

        /// <summary>
        /// Updates the RenderSettings.fog values.
        /// </summary>
        void UpdateFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.005f * ((float)KruFPS.FogDensity.GetValue() * 0.01f);
            Color newColor = GetFogColor();
            newColor.a = 1;
            RenderSettings.fogColor = newColor;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light DirectinalLight;
    [SerializeField] private LighPreset Preset;
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField, Range(0, 1)] private float speed = 0.006f;

    public static bool night = false;

    float nightLight = 0.48f;

    private void Update()
    {

        if (Preset == null)
            return;
        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime * speed;
            TimeOfDay %= 24;
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }

        if (TimeOfDay <= 5.3f || TimeOfDay >= 18.3f)
        {
            night = true;
        }
        else
        {
            night = false;
        }
        if (night == false && RenderSettings.ambientIntensity != 1)
        {
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 1, 1 * Time.deltaTime);
            DirectinalLight.intensity = Mathf.Lerp(DirectinalLight.intensity, 1, 1 * Time.deltaTime);
        }
        else if (night == true && RenderSettings.ambientIntensity != nightLight)
        {
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 0.48f, 1 * Time.deltaTime);
            DirectinalLight.intensity = Mathf.Lerp(DirectinalLight.intensity, 0.2f, 1 * Time.deltaTime);
        }

        if (night == true && RenderSettings.reflectionIntensity != 0.14f)
        {
            RenderSettings.reflectionIntensity = Mathf.Lerp(RenderSettings.reflectionIntensity, 0.14f, 1 * Time.deltaTime);
        }
        else if (night == false && RenderSettings.reflectionIntensity != 1)
        {
            RenderSettings.reflectionIntensity = Mathf.Lerp(RenderSettings.reflectionIntensity, 1, 1 * Time.deltaTime);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        Camera[] cams = FindObjectsOfType<Camera>();

        foreach (Camera cam in cams)
        {
            cam.backgroundColor = Preset.SkyColor.Evaluate(timePercent);
        }

        if (night == true && RenderSettings.fogDensity != 0.03f)
        {
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.03f, speed * Time.deltaTime);
            //FindObjectOfType<Enviroment>().DayNight(night);
        }
        else if (night == false && RenderSettings.fogDensity != 0.01f)
        {
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.01f, speed * Time.deltaTime);
            //FindObjectOfType<Enviroment>().DayNight(night);
        }

        if (DirectinalLight != null)
        {
            DirectinalLight.color = Preset.DirectinalColor.Evaluate(timePercent);
            DirectinalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            if (night == true && DirectinalLight.intensity != 0)
            {
                DirectinalLight.intensity = Mathf.Lerp(DirectinalLight.intensity, 0, speed * Time.deltaTime);
            }
            else if (night == false && DirectinalLight.intensity != 1)
            {
                DirectinalLight.intensity = Mathf.Lerp(DirectinalLight.intensity, 1, speed * Time.deltaTime);
            }
        }
    }

    private void OnValidate()
    {
        if (DirectinalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            DirectinalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectinalLight = light;
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptable/Light Preset", order = 1)]
public class LighPreset : ScriptableObject
{
    public Gradient ambientColor;
    public Gradient DirectinalColor;
    public Gradient FogColor;
    public Gradient SkyColor;
}
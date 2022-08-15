using GLTF.Schema;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WebaAmbientLightExtension: Extension
{
    public float intensity = 0;
    public Color color;

    public JProperty Serialize()
    {
        return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaAmbientLightExtensionFactory)), new JObject(
            new JProperty("intensity", intensity),
            new JProperty("color", new JArray{ color.r, color.g, color.b })
        ));
    }
}
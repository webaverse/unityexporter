using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class WebaAudioListenerExtension : Extension
{
    public bool rotatable;

    public JProperty Serialize()
    {
        var value = new JObject(
            new JProperty("rotatable", rotatable)
        );

        return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaAudioListenerExtensionFactory)), value);
    }
}
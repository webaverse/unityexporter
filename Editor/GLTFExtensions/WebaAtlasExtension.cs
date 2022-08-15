using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class WebaAtlasExtension : Extension
{
    public struct Atlas
    {
        public JObject json; 
    }

    public List<Atlas> atlases = new List<Atlas>();

    public JProperty Serialize()
    {
        var value = new JArray();

        foreach (var atlas in atlases)
        {
            value.Add(atlas.json);
        }

        return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaAtlasExtensionFactory)), new JObject(
            new JProperty("atlases", value)
        ));
    }
}
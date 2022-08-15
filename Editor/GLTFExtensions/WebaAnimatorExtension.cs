using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class WebaAnimatorExtension : Extension
{
    public string[] modelAnimations;
    public string defaultAnimation;
    public string prefix = null;
    public string[] prefixes = null;
    public string name;

    public JProperty Serialize()
    {
        var value = new JObject(
            new JProperty("name", name),
            new JProperty("defaultAnimation", defaultAnimation),
            new JProperty("modelAnimations", new JArray(modelAnimations)),
            new JProperty("prefixes", new JArray(prefixes))
        );

        if (prefix != null && prefix != "")
        {
            value.Add("prefix", prefix);
        }

        return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaAnimatorExtensionFactory)), value);
    }
}
using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class WebaSpriteExtension : Extension
{
    public struct Sprite
    {
        public float width;
        public float height;
        public int atlasId;
        public string frameName;
        public bool isBillboard;
        public bool frustumTest;
        public MaterialId materialId;
    }

    public bool isGlobal = false;

    // global
    public List<Sprite> sprites = new List<Sprite>();

    // node
    public int index = -1;

    public JProperty Serialize()
    {
        var value = new JObject();

        if (!isGlobal)
        {
            value.Add("index", index);
        }
        else
        {
            var sps = new JArray();
            value.Add("sprites", sps);
            foreach (var sprite in sprites)
            {
                var s = new JObject(
                    new JProperty("width", sprite.width),
                    new JProperty("height", sprite.height),
                    new JProperty("atlas", new JObject(
                        new JProperty("index", sprite.atlasId),
                        new JProperty("frameName", sprite.frameName)
                    )),
                    new JProperty("isBillboard", sprite.isBillboard),
                    new JProperty("frustumTest", sprite.frustumTest)
                );

                if (sprite.materialId != null)
                {
                    s.Add("material", new JObject(new JProperty("index", sprite.materialId.Id)));
                }

                sps.Add(s);
            }
        }

        return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaSpriteExtensionFactory)), value);
    }
}
using GLTF.Schema;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WebaTextureImproveExtension : Extension
{
    public bool isImageCanRelease = false;
    public int anisotropic = 1;
    // NORMAL: 5121
    // 4444: 32819
    public int textureType = 5121;
    public bool useMipmaps = true;

    public JProperty Serialize()
    {
        var res = new JObject(
            new JProperty("isImageCanRelease", isImageCanRelease),
            new JProperty("anisotropic", anisotropic),
            new JProperty("textureType", textureType)
        );

        if (!useMipmaps)
        {
            res.Add("useMapmaps", false);
        }

        return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaTextureImproveExtensionFactory)), res);
    }
}
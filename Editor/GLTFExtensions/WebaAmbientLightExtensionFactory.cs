using System;
using System.Collections.Generic;
using GLTF.Schema;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WebaAmbientLightExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaAmbientLight"; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        var extension = new WebaAmbientLightExtension();

        var hdrColor = RenderSettings.ambientLight;
        var r = hdrColor.r;
        var g = hdrColor.r;
        var b = hdrColor.b;
        var d = Math.Max(r, Math.Max(g, b));

        if (d >= 0.01)
        {
            r /= d;
            g /= d;
            b /= d;
        }

        extension.intensity = d;
        extension.color = new Color(r, g, b);

        AddExtension(extensions, extension);
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaAmbientLightExtension();

        if (extensionToken == null)
        {
            return null;
        }

        extension.intensity = (float)extensionToken.Value["intensity"];
        var c = (JArray)extensionToken.Value["color"];
        extension.color = new Color((float)c[0], (float)c[1], (float)c[2]);

        return extension;
    }
}
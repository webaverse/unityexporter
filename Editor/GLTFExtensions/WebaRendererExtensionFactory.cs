using System;
using System.Collections.Generic;
using GLTF.Schema;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class WebaRendererExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaRenderer"; }
    public override List<Type> GetBindedComponents() { return new List<Type> { typeof(MeshRenderer), typeof(SkinnedMeshRenderer) }; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaRendererExtension();

        if (extensionToken.Value["castShadows"] == null)
        {
            extension.gammaCorrection = (bool)extensionToken.Value["gammaCorrection"];
            extension.useHDR = (bool)extensionToken.Value["useHDR"];
            extension.exposure = (float)extensionToken.Value["exposure"];
        }
        else
        {
            extension.castShadows = (bool)extensionToken.Value["castShadows"];
            extension.receiveShadows = (bool)extensionToken.Value["receiveShadows"];
        }

        return new WebaRendererExtension();
    }
}
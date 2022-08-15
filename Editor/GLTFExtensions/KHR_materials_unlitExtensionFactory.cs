using System;
using System.Collections.Generic;
using GLTF.Schema;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class KHR_materials_unlitExtensionFactory: WebaExtensionFactory
{
    public override string GetExtensionName() { return "KHR_materials_unlit"; }
    public override List<Type> GetBindedComponents() { return new List<Type>(); }
    public override List<EExtensionType> GetExtensionTypes() { return new List<EExtensionType> { EExtensionType.Material }; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        var extension = new KHR_materials_unlitExtension();

        AddExtension(extensions, extension);
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        return new KHR_materials_unlitExtension();
    }
}

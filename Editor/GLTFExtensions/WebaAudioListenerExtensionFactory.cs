using System;
using Newtonsoft.Json.Linq;
using GLTF.Math;
using Newtonsoft.Json;
using GLTF.Extensions;
using System.Collections.Generic;
using GLTF.Schema;
using UnityEngine;

public class WebaAudioListenerExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaAudioListener"; }
    public override List<Type> GetBindedComponents() { return new List<Type> { typeof(WebaAudioListener) }; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        var extension = new WebaAudioListenerExtension();
        var listener = component as WebaAudioListener;

        extension.rotatable = listener.rotatable;

        AddExtension(extensions, extension);
    }
    
    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaAudioListenerExtension();

        if (extensionToken != null)
        {
            extension.rotatable = (bool)extensionToken.Value["rotatable"];
        }

        return extension;
    }
}

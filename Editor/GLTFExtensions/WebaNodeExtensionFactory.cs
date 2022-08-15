using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using GLTF.Schema;
using UnityEngine;

public class WebaNodeExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaNode"; }
    public override List<Type> GetBindedComponents() { return new List<Type> { typeof(WebaNode) }; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        var extension = new WebaNodeExtension();
        var node = component as WebaNode;

        extension.selfType = node.selfType;
        extension.childrenType = node.childrenType;
        extension.className = node.className;
        extension.tag = node.tag;
        extension.layer = node.layer;
        extension.persistent = node.persistent;
        extension.emitComponentsDestroy = node.emitComponentsDestroy;
        extension.updateOnEverTick = node.updateOnEverTick;
        extension.isStatic = node.isStatic;
        extension.skipThisNode = node.skipThisNode;

        var com = node.GetComponent<WebaNodeClass>();
        if (com != null)
        {
            //extension.initOptions = com.Serialize(entry, extension);
        }

        AddExtension(extensions, extension);
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaNodeExtension();

        if (extensionToken != null)
        {
            extension.selfType = (int)extensionToken.Value["selfType"] == 1 ? EWebaNodeType.Component : EWebaNodeType.Actor;
            extension.childrenType = (int)extensionToken.Value["childrenType"] == 1 ? EWebaNodeType.Component : EWebaNodeType.Actor;
            extension.className = (string)extensionToken.Value["className"];
            extension.tag = (string)extensionToken.Value["tag"];
            if (extensionToken.Value["layer"] != null)
            {
                extension.layer = (int)extensionToken.Value["layer"];
            }
            extension.persistent = (bool)extensionToken.Value["persistent"];
            extension.updateOnEverTick = (bool)extensionToken.Value["updateOnEverTick"];
            if (extensionToken.Value["isStatic"] != null)
            {
                extension.isStatic = (bool)extensionToken.Value["isStatic"];
            }
            extension.skipThisNode = (bool)extensionToken.Value["skipThisNode"];
        }

        return extension;
    }
    /*
    public override void Import(EditorImporter importer, GameObject gameObject, Node gltfNode, Extension extension)
    {
        var n = (WebaNodeExtension)extension;
        var WebaNode = gameObject.AddComponent<WebaNode>();
        WebaNode.selfType = n.selfType;
        WebaNode.className = n.className;
        WebaNode.tag = n.tag;
        WebaNode.layer = n.layer;
        WebaNode.persistent = n.persistent;
        WebaNode.emitComponentsDestroy = n.emitComponentsDestroy;
        WebaNode.updateOnEverTick = n.updateOnEverTick;
        WebaNode.isStatic = n.isStatic;
        WebaNode.skipThisNode = n.skipThisNode;
    }*/
}
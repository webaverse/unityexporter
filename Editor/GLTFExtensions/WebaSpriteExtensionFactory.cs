using System;
using Newtonsoft.Json.Linq;
using GLTF.Math;
using Newtonsoft.Json;
using GLTF.Extensions;
using System.Collections.Generic;
using GLTF.Schema;
using UnityEngine;

public class WebaSpriteExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaSprite"; }
    public override List<EExtensionType> GetExtensionTypes() { return new List<EExtensionType> { EExtensionType.Mesh }; }
    public override List<Type> GetBindedComponents() { return new List<Type> { typeof(WebaSprite) }; }

    private static Dictionary<ExporterEntry, Dictionary<string, int>> _CAHCE = new Dictionary<ExporterEntry, Dictionary<string, int>>();

    public override void BeforeExport()
    {
        _CAHCE.Clear();
    }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        if (entry.root.Extensions == null)
        {
            entry.root.Extensions = new Dictionary<string, Extension>();
        }

        WebaSpriteExtension globalExtension;
        if (!entry.root.Extensions.ContainsKey(ExtensionName))
        {
            globalExtension = new WebaSpriteExtension { isGlobal = true };
            entry.root.Extensions.Add(ExtensionName, globalExtension);
        }
        else
        {
            globalExtension = (WebaSpriteExtension)entry.root.Extensions[ExtensionName];
        }

        var sprite = component as WebaSprite;
        var extension = new WebaSpriteExtension { isGlobal = false };
        var sp = sprite;
        var customMaterial = sprite.GetComponent<WebaCustomMaterial>();
        var cacheId = $"w{sp.width}-h{sp.height}-at{sp.atlas.GetInstanceID()}-fn{sp.frameName}-bb{sp.isBillboard}-ft{sp.frustumTest}";
        if (customMaterial != null)
        {
            cacheId += $"mat{customMaterial.GetInstanceID()}";
        } else
        {
            cacheId += $"mat{sprite.material.GetInstanceID()}";
        }
        if (!_CAHCE.ContainsKey(entry))
        {
            _CAHCE.Add(entry, new Dictionary<string, int>());
        }

        if (_CAHCE[entry].ContainsKey(cacheId))
        {
            extension.index = _CAHCE[entry][cacheId];
            AddExtension(extensions, extension);
            return;
        }

        // process atlases at first
        //ExtensionManager.Serialize(ExtensionManager.GetExtensionName(typeof(WebaAtlasExtensionFactory)), entry, entry.root.Extensions, sprite.atlas);
        var s = new WebaSpriteExtension.Sprite();
        s.width = sprite.width;
        s.height = sprite.height;
        s.isBillboard = sprite.isBillboard;
        s.frustumTest = sprite.frustumTest;
        s.atlasId = WebaAtlasExtensionFactory.GetAtlasIndex(entry, sprite.atlas);
        s.frameName = sprite.frameName;

        GLTF.Schema.Material gltfMat = null;
        if (customMaterial != null)
        {
            gltfMat = ExporterUtils.ConvertMaterial(customMaterial, entry);
        } else if (sprite.material.shader.name != "Weba/Sprite" && sprite.material.shader.name.Contains("Weba/"))
        {
            gltfMat = ExporterUtils.ConvertWebaCustomMaterial(sprite.material, entry);
        }

        if (gltfMat != null)
        {
            var root = entry.root;
            if (root.Materials == null)
            {
                root.Materials = new List<GLTF.Schema.Material>();
            }

            root.Materials.Add(gltfMat);
            var id = new MaterialId { Id = root.Materials.Count - 1, Root = root };
            s.materialId = id;
        }

        globalExtension.sprites.Add(s);

        var index = globalExtension.sprites.Count - 1;
        _CAHCE[entry].Add(cacheId, index);
        extension.index = index;

        AddExtension(extensions, extension);
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaSpriteExtension();

        if (extensionToken != null)
        {
            if (extensionToken.Value["index"] != null)
            {
                extension.isGlobal = false;
                extension.index = (int)extensionToken.Value["index"];
            }
            else
            {
                foreach (JObject sprite in (JArray)extensionToken.Value["sprites"])
                {
                    var sp = new WebaSpriteExtension.Sprite();
                    sp.width = sprite.Value<float>("width");
                    sp.height = sprite.Value<float>("height");
                    sp.isBillboard = sprite.Value<bool>("isBillBoard");
                    sp.frustumTest = sprite.Value<bool>("frustumTest");
                    sp.frameName = sprite.Value<JObject>("atlas").Value<string>("frameName");
                    sp.atlasId = sprite.Value<JObject>("atlas").Value<int>("index");

                    if (sprite["material"] != null)
                    {
                        sp.materialId = new MaterialId { Root = root, Id = sprite.Value<JObject>("material").Value<int>("index") };
                    }

                    extension.sprites.Add(sp);
                }
            }
        }

        return extension;
    }
    }
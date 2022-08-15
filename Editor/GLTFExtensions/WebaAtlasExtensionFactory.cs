using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using GLTF.Schema;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class WebaAtlasExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaAtlas"; }
    public override List<EExtensionType> GetExtensionTypes() { return new List<EExtensionType> { EExtensionType.Global }; }

    private static Dictionary<ExporterEntry, List<WebaAtlas>> ENTRY_ATLASES = new Dictionary<ExporterEntry, List<WebaAtlas>>();
    public static WebaAtlas[] IMPORTED_ATLASES;

    public static int GetAtlasIndex(ExporterEntry entry, WebaAtlas atlas)
    {
        return ENTRY_ATLASES[entry].IndexOf(atlas);
    }

    public override void BeforeExport()
    {
        ENTRY_ATLASES.Clear();
    }

    public override void BeforeImport()
    {
        IMPORTED_ATLASES = null;
    }

    public override void FinishImport()
    {
        IMPORTED_ATLASES = null;
    }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        if (!ENTRY_ATLASES.ContainsKey(entry))
        {
            ENTRY_ATLASES.Add(entry, new List<WebaAtlas>());
        }

        WebaAtlasExtension extension;
        var atlas = component as WebaAtlas;

        if (!extensions.ContainsKey(ExtensionName))
        {
            extension = new WebaAtlasExtension();
            AddExtension(extensions, extension);
        }
        else
        {
            extension = (WebaAtlasExtension)extensions[ExtensionName];
        }

        var list = ENTRY_ATLASES[entry];

        if (list.Contains(atlas))
        {
            return;
        }

        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(atlas.atlasPath);
        if (tex == null)
        {
            Utils.ThrowException("Atlas '" + atlas.name + "' is not saved!");
        }
        var imageId = entry.SaveImage(tex, true, null, maxSize: Math.Max(tex.width, tex.height), flipY: false);
        var json = atlas.ReadJson();
        json["meta"]["image"] = new JObject(new JProperty("index", imageId.Id));

        extension.atlases.Add(new WebaAtlasExtension.Atlas { json = json });
        list.Add(atlas);
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        List<WebaAtlasExtension.Atlas> atlases = new List<WebaAtlasExtension.Atlas>();

        if (extensionToken != null)
        {
            var atlasesToken = extensionToken.Value["atlases"];

            foreach (JObject json in atlasesToken)
            {
                atlases.Add(new WebaAtlasExtension.Atlas { json = json });
            }
        }

        return new WebaAtlasExtension { atlases = atlases };
    }
}
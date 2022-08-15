using System;
using Newtonsoft.Json.Linq;
using GLTF.Math;
using Newtonsoft.Json;
using GLTF.Extensions;
using System.Collections.Generic;
using GLTF.Schema;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class WebaAudioClipsExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaAudioClips"; }
    public override List<EExtensionType> GetExtensionTypes() { return new List<EExtensionType> { EExtensionType.Global }; }

    private static Dictionary<ExporterEntry, List<WebaAudioClip>> ENTRY_CLIPS = new Dictionary<ExporterEntry, List<WebaAudioClip>>();
    private static Dictionary<ExporterEntry, Dictionary<AudioClip, string>> ENTRY_URIS = new Dictionary<ExporterEntry, Dictionary<AudioClip, string>>();

    public static List<string> IMPORTED_URIS = new List<string>();
    public static List<WebaAudioClip> IMPORTED_CLIPS = new List<WebaAudioClip>();

    public static int GetClipIndex(ExporterEntry entry, WebaAudioClip clip)
    {
        return ENTRY_CLIPS[entry].IndexOf(clip);
    }

    public override void BeforeExport()
    {
        ENTRY_CLIPS.Clear();
        ENTRY_URIS.Clear();
    }

    public override void BeforeImport()
    {
        IMPORTED_CLIPS.Clear();
        IMPORTED_URIS.Clear();
    }

    public override void FinishImport()
    {
        IMPORTED_CLIPS.Clear();
        IMPORTED_URIS.Clear();
    }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        if (!ENTRY_CLIPS.ContainsKey(entry))
        {
            ENTRY_CLIPS.Add(entry, new List<WebaAudioClip>());
            ENTRY_URIS.Add(entry, new Dictionary<AudioClip, string>());
        }

        WebaAudioClipsExtension extension;
        var source = component as WebaAudioSource;

        if (!extensions.ContainsKey(ExtensionName))
        {
            extension = new WebaAudioClipsExtension();
            AddExtension(extensions, extension);
        }
        else
        {
            extension = (WebaAudioClipsExtension)extensions["WebaAudioClips"];
        }

        var list = ENTRY_CLIPS[entry];
        var uris = ENTRY_URIS[entry];

        foreach(var c in source.clips)
        {
            var clip = c.clip;
            if (c.clip == null)
            {
                Utils.ThrowException("Clip '" + c.name + "' has no audio source!");
            }

            if (list.Contains(clip))
            {
                continue;
            }

            var newClip = new WebaAudioClipsExtension.AudioClip();
            newClip.name = clip.name;
            newClip.mode = clip.mode;
            newClip.isLazy = clip.isLazy;

            if (uris.ContainsKey(clip.clip))
            {
                newClip.uri = uris[clip.clip];
            }
            else
            {
                newClip.uri = SaveAudio(clip.clip);
            }

            list.Add(clip);
            extension.clips.Add(newClip);
        }
    }

    private string SaveAudio(AudioClip clip)
    {
        string assetPath = AssetDatabase.GetAssetPath(clip);
        var pathes = ExporterUtils.GetAssetOutPath(clip);
        var exportPath = pathes[0];
        var pathInGlTF = pathes[1];

        if (File.Exists(exportPath))
        {
            return pathInGlTF;
        }

        FileUtil.CopyFileOrDirectory(assetPath, exportPath);

        return pathInGlTF;
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        List<WebaAudioClipsExtension.AudioClip> clips = new List<WebaAudioClipsExtension.AudioClip>();

        if (extensionToken != null)
        {
            var clipsToken = extensionToken.Value["clips"];

            foreach (var clipToken in clipsToken)
            {
                var uri = clipToken.Value<string>("uri");
                var tmp = uri.Split('/');
                var name = tmp[tmp.Length - 1];

                clips.Add(new WebaAudioClipsExtension.AudioClip
                { 
                    mode = clipToken.Value<string>("mode") == "Stream" ? EWebaAudioClipMode.Stream : EWebaAudioClipMode.Buffer,
                    isLazy = clipToken.Value<bool>("isLazy"),
                    uri = uri,
                    name = name
                });
            }
        }

        return new WebaAudioClipsExtension { clips = clips };
    }
    /*
    public override void Import(EditorImporter importer, Extension extension)
    {
        importer.taskManager.addTask(LoadClips(importer, (WebaAudioClipsExtension)extension));
    }

    private IEnumerator LoadClips(EditorImporter importer, WebaAudioClipsExtension extension)
    {
        var clips = extension.clips;
        var basePath = Path.Combine(importer.importDirectoryPath, "audios");
        Directory.CreateDirectory(basePath);
        int i = 0;

        foreach (var clip in clips)
        {
            LoadClip(clip, importer.gltfDirectoryPath, basePath, i);
            importer.SetProgress("AUDIO", (i + 1), clips.Count);
            i += 1;

            yield return null;
        }
    }
    */
    private void LoadClip(WebaAudioClipsExtension.AudioClip clip, string gltfPath, string basePath, int i)
    {
        var uri = Path.Combine(gltfPath, clip.uri);
        if (clip.uri != null && File.Exists(uri))
        {
            var tmp = clip.uri.Split('/');
            var name = tmp[tmp.Length - 1];

            if (clip.name != null)
            {
                name = clip.name;
            }

            var path = Path.Combine(basePath, name);

            if (File.Exists(path))
            {
                if (!IMPORTED_URIS.Contains(clip.uri))
                {
                    name = Path.GetFileNameWithoutExtension(name) + "-" + i + Path.GetExtension(name);
                    path = Path.Combine(basePath, name);
                    FileUtil.CopyFileOrDirectory(uri, path);
                    IMPORTED_URIS.Add(clip.uri);
                }
            }
            else
            {
                FileUtil.CopyFileOrDirectory(uri, path);
                IMPORTED_URIS.Add(clip.uri);
            }

            AssetDatabase.Refresh();
            var unityClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);

            var directory = GLTFUtils.getPathProjectFromAbsolute(basePath);
            path = Path.Combine(directory, name + ".asset");
            var WebaClip = ScriptableObject.CreateInstance<WebaAudioClip>();
            WebaClip.clip = unityClip;
            WebaClip.mode = clip.mode;
            WebaClip.isLazy = clip.isLazy;

            AssetDatabase.CreateAsset(WebaClip, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            WebaClip = AssetDatabase.LoadAssetAtPath<WebaAudioClip>(path);

            IMPORTED_CLIPS.Add(WebaClip);
        }
        else
        {
            Debug.LogWarning("Audio clip not found");
        }
    }
}
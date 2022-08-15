using System;
using Newtonsoft.Json.Linq;
using GLTF.Math;
using Newtonsoft.Json;
using GLTF.Extensions;
using System.Collections.Generic;
using GLTF.Schema;
using UnityEngine;

public class WebaAudioSourceExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaAudioSource"; }
    public override List<Type> GetBindedComponents() { return new List<Type> { typeof(WebaAudioSource) }; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        // process clips at first
        if (entry.root.Extensions == null)
        {
            entry.root.Extensions = new Dictionary<string, Extension>();
        }
        //ExtensionManager.Serialize(ExtensionManager.GetExtensionName(typeof(WebaAudioClipsExtensionFactory)), entry, entry.root.Extensions, component);

        var extension = new WebaAudioSourceExtension();
        var source = component as WebaAudioSource;

        extension.isSpaceAudio = source.isSpaceAudio;
        extension.needAutoPlay = source.needAutoPlay;
        extension.defaultClip = source.defaultClip;
        extension.spaceOptions = source.spaceOptions;
        extension.autoPlayOptions = source.autoPlayOptions;

        foreach (var clip in source.clips)
        {
            extension.clips.Add(new KeyValuePair<string, int>(clip.name, WebaAudioClipsExtensionFactory.GetClipIndex(entry, clip.clip)));
        }

        AddExtension(extensions, extension);
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaAudioSourceExtension();

        if (extensionToken != null)
        {
            extension.defaultClip = (string)extensionToken.Value["defaultClip"];
            extension.needAutoPlay = (bool)extensionToken.Value["needAutoPlay"];
            extension.isSpaceAudio = (bool)extensionToken.Value["isSpaceAudio"];
            foreach (var pair in (JObject)extensionToken.Value["clips"])
            {
                extension.clips.Add(new KeyValuePair<string, int>(pair.Key, (int)pair.Value));
            }

            if (extension.needAutoPlay)
            {
                extension.autoPlayOptions = new WebaAudioSourceAutoPlayOptions();
                extension.autoPlayOptions.start = (float)extensionToken.Value["autoPlayOptions"]["start"];
                extension.autoPlayOptions.end = (float)extensionToken.Value["autoPlayOptions"]["end"];
                extension.autoPlayOptions.loop = (bool)extensionToken.Value["autoPlayOptions"]["loop"];
            }

            if (extension.isSpaceAudio)
            {
                extension.spaceOptions = new WebaAudioSourceSpaceOptions();
                extension.spaceOptions.rotatable = (bool)extensionToken.Value["spaceOptions"]["rotatable"];
                extension.spaceOptions.refDistance = (float)extensionToken.Value["spaceOptions"]["refDistance"];
                extension.spaceOptions.maxDistance = (float)extensionToken.Value["spaceOptions"]["maxDistance"];
                extension.spaceOptions.rolloffFactor = (float)extensionToken.Value["spaceOptions"]["rolloffFactor"];
                extension.spaceOptions.coneInnerAngle = (float)extensionToken.Value["spaceOptions"]["coneInnerAngle"];
                extension.spaceOptions.coneOuterAngle = (float)extensionToken.Value["spaceOptions"]["coneOuterAngle"];
                extension.spaceOptions.coneOuterGain = (float)extensionToken.Value["spaceOptions"]["coneOuterGain"];

                var pm = (string)extensionToken.Value["spaceOptions"]["panningModel"];
                if (pm == "equalpower")
                {
                    extension.spaceOptions.panningModel = EWebaAudioPanningModelType.equalpower;
                }
                else
                {
                    extension.spaceOptions.panningModel = EWebaAudioPanningModelType.HRTF;
                }

                var dm = (string)extensionToken.Value["spaceOptions"]["distanceModel"];
                if (pm == "linear")
                {
                    extension.spaceOptions.distanceModel = EWebaAudioDistanceModelType.linear;
                }
                else if (pm == "inverse")
                {
                    extension.spaceOptions.distanceModel = EWebaAudioDistanceModelType.inverse;
                }
                else
                {
                    extension.spaceOptions.distanceModel = EWebaAudioDistanceModelType.exponential;
                }
            }
        }

        return extension;
    }
}

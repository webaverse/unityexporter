using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class WebaAudioClipsExtension : Extension
{
    public struct AudioClip
    {
        public EWebaAudioClipMode mode;
        public bool isLazy;
        public string uri;
        public string name;
    }

    public List<AudioClip> clips = new List<AudioClip>();

    public JProperty Serialize()
    {
        var value = new JArray();

        foreach(var clip in clips)
        {
            var c = new JObject();
            c.Add("mode", clip.mode.ToString());
            c.Add("isLazy", clip.isLazy);
            c.Add("uri", clip.uri);

            value.Add(c);
        }

        return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaAudioClipsExtensionFactory)), new JObject(
            new JProperty("clips", value)
        ));
    }
}
using UnityEngine;

public enum EWebaAudioClipMode
{
    Stream,
    Buffer
}

[CreateAssetMenu(fileName = "WebaAudioClip", menuName = "Weba/AudioClip")]
public class WebaAudioClip : ScriptableObject
{
    public EWebaAudioClipMode mode = EWebaAudioClipMode.Stream;
    public AudioClip clip;
    public bool isLazy = true;
}

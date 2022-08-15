using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EWebaAudioPanningModelType
{
    equalpower,
    HRTF
}

public enum EWebaAudioDistanceModelType
{
    linear,
    inverse,
    exponential
}

[Serializable]
public struct WebaAudioOneClip
{
    public string name;
    public WebaAudioClip clip;
}

[Serializable]
public class WebaAudioSourceSpaceOptions
{
    public bool rotatable;
    public EWebaAudioPanningModelType panningModel = EWebaAudioPanningModelType.HRTF;
    public EWebaAudioDistanceModelType distanceModel = EWebaAudioDistanceModelType.linear;
    public float refDistance = 1;
    public float maxDistance = 10000;
    public float rolloffFactor = 1;
    public float coneInnerAngle = 360;
    public float coneOuterAngle = 0;
    public float coneOuterGain = 0;
}

[Serializable]
public class WebaAudioSourceAutoPlayOptions
{
    public bool loop = true;
    public float start = 0;
    public float end = 0;
}

[AddComponentMenu("Weba/Audio Extension/Weba Audio Source"), ExecuteInEditMode]
public class WebaAudioSource : MonoBehaviour
{
    public WebaAudioOneClip[] clips = { };
    public string defaultClip = "";
    public bool needAutoPlay;
    public WebaAudioSourceAutoPlayOptions autoPlayOptions;
    public bool isSpaceAudio;
    public WebaAudioSourceSpaceOptions spaceOptions;

    public void OnEnable()
    {
        if (clips.Length <= 0)
        {
            return;
        }

        WebaAudioClip ac = null;
        if (defaultClip == "")
        {
            defaultClip = clips[0].name;
            ac = clips[0].clip;
        }
        else
        {
            bool rightDefaultClip = false;
            foreach (var clip in clips)
            {
                if (clip.name == defaultClip)
                {
                    rightDefaultClip = true;
                    ac = clip.clip;
                    break;
                }
            }
            if (!rightDefaultClip)
            {
                defaultClip = clips[0].name;
                ac = clips[0].clip;
            }
        }
    }
}

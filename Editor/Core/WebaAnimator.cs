using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("Weba/Core Components/Weba Animator")]
public class WebaAnimator : MonoBehaviour
{
    [HideInInspector]
    public new string name;
    [HideInInspector]
    public string[] modelAnimations;
    [HideInInspector]
    public string defaultAnimation;
    [HideInInspector]
    public string[] prefixes;
}

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WebaComponent : MonoBehaviour
{
    public static event System.Action OnExport;

    public static void InvokeExport()
    {
        OnExport?.Invoke();
    }

    private void OnEnable()
    {
        OnExport += HandleExport;
    }

    private void OnDisable()
    {
        OnExport -= HandleExport;
    }

    public virtual void HandleExport()
    {
        if (!gameObject.activeInHierarchy || !enabled) return;
        Debug.Log("handling export for component " + name);
    }

    public virtual string Type => "WebaverseUnityExporter";

    public virtual JProperty Serialized => null;
}
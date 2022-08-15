using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using GLTF.Schema;

public enum EWebaMaterialUniformType
{
    SAMPLER_2D = 35678,
    SAMPLER_CUBE = 35680,
    FLOAT = 5126,
    FLOAT_VEC2 = 35664,
    FLOAT_VEC3 = 35665,
    FLOAT_VEC4 = 35666,
    FLOAT_MAT2 = 35674,
    FLOAT_MAT3 = 35675,
    FLOAT_MAT4 = 35677,
    INT = 5124,
    INT_VEC2 = 35667,
    INT_VEC3 = 35668,
    INT_VEC4 = 35669
};

public class WebaMaterialUniform<TValue>
{
    [System.NonSerialized]
    public EWebaMaterialUniformType type;
    public string name;
    public TValue value;
}

[System.Serializable] public class WebaMaterialUniformTexture : WebaMaterialUniform<Texture2D> {
    [System.NonSerialized]
    public TextureId id;
    public WebaMaterialUniformTexture() { type = EWebaMaterialUniformType.SAMPLER_2D; }
}
[System.Serializable] public class WebaMaterialUniformCubeTexture : WebaMaterialUniform<Cubemap> {
    [System.NonSerialized]
    public CubeTextureId id;
    public WebaMaterialUniformCubeTexture() { type = EWebaMaterialUniformType.SAMPLER_CUBE; }
}
[System.Serializable] public class WebaMaterialUniformFloat : WebaMaterialUniform<float> { public WebaMaterialUniformFloat() { type = EWebaMaterialUniformType.FLOAT; } }
[System.Serializable] public class WebaMaterialUniformFloatVec2 : WebaMaterialUniform<Vector2> { public WebaMaterialUniformFloatVec2() { type = EWebaMaterialUniformType.FLOAT_VEC2; } }
[System.Serializable] public class WebaMaterialUniformFloatVec3 : WebaMaterialUniform<Vector3> { public WebaMaterialUniformFloatVec3() { type = EWebaMaterialUniformType.FLOAT_VEC3; } }
[System.Serializable] public class WebaMaterialUniformFloatVec4 : WebaMaterialUniform<Vector4> { public WebaMaterialUniformFloatVec4() { type = EWebaMaterialUniformType.FLOAT_VEC4; } }
[System.Serializable] public class WebaMaterialUniformColor : WebaMaterialUniform<Color> { public WebaMaterialUniformColor() { type = EWebaMaterialUniformType.FLOAT_VEC4; } }
[System.Serializable] public class WebaMaterialUniformFloatMat2 : WebaMaterialUniform<Matrix4x4> { public WebaMaterialUniformFloatMat2() { type = EWebaMaterialUniformType.FLOAT_MAT2; } }
[System.Serializable] public class WebaMaterialUniformFloatMat3 : WebaMaterialUniform<Matrix4x4> { public WebaMaterialUniformFloatMat3() { type = EWebaMaterialUniformType.FLOAT_MAT3; } }
[System.Serializable] public class WebaMaterialUniformFloatMat4 : WebaMaterialUniform<Matrix4x4> { public WebaMaterialUniformFloatMat4() { type = EWebaMaterialUniformType.FLOAT_MAT4; } }
[System.Serializable] public class WebaMaterialUniformInt : WebaMaterialUniform<int> { public WebaMaterialUniformInt() { type = EWebaMaterialUniformType.INT; } }
[System.Serializable] public class WebaMaterialUniformIntVec2 : WebaMaterialUniform<Vector2> { public WebaMaterialUniformIntVec2() { type = EWebaMaterialUniformType.INT_VEC2; } }
[System.Serializable] public class WebaMaterialUniformIntVec3 : WebaMaterialUniform<Vector3> { public WebaMaterialUniformIntVec3() { type = EWebaMaterialUniformType.INT_VEC3; } }
[System.Serializable] public class WebaMaterialUniformIntVec4 : WebaMaterialUniform<Vector4> { public WebaMaterialUniformIntVec4() { type = EWebaMaterialUniformType.INT_VEC4; } }

[System.Serializable] public class WebaMaterialCustomOption
{
    public string name;
    public string value;
}

[CustomEditor(typeof(WebaCustomMaterial))]
public class WebaCustomMaterialInspector : Editor
{
    public virtual string[] GetActiveUniforms() {
        return new string[] {
            "uniformsTexture",
            "uniformsCubeTexture",
            "uniformsFloat",
            "uniformsFloatVec2",
            "uniformsFloatVec3",
            "uniformsFloatVec4",
            "uniformsColor",
            "uniformsFloatMat2",
            "uniformsFloatMat3",
            "uniformsFloatMat4",
            "uniformsInt",
            "uniformsIntVec2",
            "uniformsIntVec3",
            "uniformsIntVec4"
       };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("className"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unityMaterialName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("renderOrder"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cloneForInst"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("transparent"));

        var option = EEditorListOption.ListLabel | EEditorListOption.Buttons | EEditorListOption.ElementLabels;

        EditorList.Show(serializedObject.FindProperty("cutomOptions"), option);

        var activeUniforms = GetActiveUniforms();
        foreach (string key in activeUniforms)
        {
            EditorList.Show(serializedObject.FindProperty(key), option);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

[AddComponentMenu("Weba/Core Components/Weba Custom Material")]
public class WebaCustomMaterial : MonoBehaviour
{
    public string className = "";
    public string unityMaterialName = "";
    public string matScriptPath = null;

    [Header("Options")]
    public int renderOrder = 0;
    public bool cloneForInst = false;
    public bool transparent = false;
    public WebaMaterialCustomOption[] customOptions = { };

    [Header("Uniforms")]
    public WebaMaterialUniformTexture[] uniformsTexture = { };
    public WebaMaterialUniformCubeTexture[] uniformsCubeTexture = { };
    public WebaMaterialUniformFloat[] uniformsFloat = { };
    public WebaMaterialUniformFloatVec2[] uniformsFloatVec2 = { };
    public WebaMaterialUniformFloatVec3[] uniformsFloatVec3 = { };
    public WebaMaterialUniformFloatVec4[] uniformsFloatVec4 = { };
    public WebaMaterialUniformColor[] uniformsColor = { };
    public WebaMaterialUniformFloatMat2[] uniformsFloatMat2 = { };
    public WebaMaterialUniformFloatMat3[] uniformsFloatMat3 = { };
    public WebaMaterialUniformFloatMat4[] uniformsFloatMat4 = { };
    public WebaMaterialUniformInt[] uniformsInt = { };
    public WebaMaterialUniformIntVec2[] uniformsIntVec2 = { };
    public WebaMaterialUniformIntVec3[] uniformsIntVec3 = { };
    public WebaMaterialUniformIntVec4[] uniformsIntVec4 = { };
}

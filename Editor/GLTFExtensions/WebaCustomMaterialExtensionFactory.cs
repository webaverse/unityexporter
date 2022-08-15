using System;
using System.Collections.Generic;
using System.IO;
using GLTF.Schema;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class WebaCustomMaterialExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaCustomMaterial"; }
    public override List<Type> GetBindedComponents() { return new List<Type>(); }
    public override List<EExtensionType> GetExtensionTypes() { return new List<EExtensionType> { EExtensionType.Material, EExtensionType.Node }; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        var material = component as WebaCustomMaterial;
        var extension = new WebaCustomMaterialExtension();

        if (material.matScriptPath != null)
        {
            if (entry.root.Extensions == null)
            {
                entry.root.Extensions = new Dictionary<string, Extension>();
            }

            WebaCustomMaterialExtension globalExtension;
            if (!entry.root.Extensions.ContainsKey(ExtensionName))
            {
                globalExtension = new WebaCustomMaterialExtension { matScripts = new List<string>() };
                AddExtension(entry.root.Extensions, globalExtension);
            }
            else
            {
                globalExtension = (WebaCustomMaterialExtension)entry.root.Extensions[ExtensionName];
            }

            var pathes = ExporterUtils.GetAssetOutPath(material.matScriptPath);
            var exportPath = pathes[0];
            var pathInGlTF = pathes[1];

            if (!File.Exists(exportPath))
            {
                FileUtil.CopyFileOrDirectory(material.matScriptPath, exportPath);
            }

            globalExtension.matScripts.Add(pathInGlTF);
        }

        extension.className = material.className;
        extension.cloneForInst = material.cloneForInst;
        extension.renderOrder = material.renderOrder;
        extension.transparent = material.transparent;
        extension.customOptions = material.customOptions;
        extension.uniformsColor = material.uniformsColor;
        extension.uniformsTexture = material.uniformsTexture;
        extension.uniformsCubeTexture = material.uniformsCubeTexture;
        extension.uniformsFloat = material.uniformsFloat;
        extension.uniformsFloatVec2 = material.uniformsFloatVec2;
        extension.uniformsFloatVec3 = material.uniformsFloatVec3;
        extension.uniformsFloatVec4 = material.uniformsFloatVec4;
        extension.uniformsFloatMat2 = material.uniformsFloatMat2;
        extension.uniformsFloatMat3 = material.uniformsFloatMat3;
        extension.uniformsFloatMat4 = material.uniformsFloatMat4;
        extension.uniformsInt = material.uniformsInt;
        extension.uniformsIntVec2 = material.uniformsIntVec2;
        extension.uniformsIntVec3 = material.uniformsIntVec3;
        extension.uniformsIntVec4 = material.uniformsIntVec4;

        AddExtension(extensions, extension);
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaCustomMaterialExtension();

        if (extensionToken != null)
        {
            extension.className = (string)extensionToken.Value["className"];
            if (extensionToken.Value["renderOrder"] != null)
            {
                extension.renderOrder = (int)extensionToken.Value["renderOrder"];
            }
            if (extensionToken.Value["cloneForInst"] != null)
            {
                extension.cloneForInst = (bool)extensionToken.Value["cloneForInst"];
            }
            if (extensionToken.Value["options"] != null)
            {
                var opts = (JObject)extensionToken.Value["options"];
                var customOptions = new List<WebaMaterialCustomOption>();

                foreach (var pair in opts) {
                    customOptions.Add(new WebaMaterialCustomOption { name = pair.Key, value = (string)pair.Value });
                }

                extension.customOptions = customOptions.ToArray();
            }

            if (extensionToken.Value["uniforms"] != null)
            {
                var uniforms = (JObject)extensionToken.Value["uniforms"];
                var uniformsTexture = new List<WebaMaterialUniformTexture>();
                var uniformsCubeTexture = new List<WebaMaterialUniformCubeTexture>();
                var uniformsFloat = new List<WebaMaterialUniformFloat>();
                var uniformsFloatVec2 = new List<WebaMaterialUniformFloatVec2>();
                var uniformsFloatVec3 = new List<WebaMaterialUniformFloatVec3>();
                var uniformsFloatVec4 = new List<WebaMaterialUniformFloatVec4>();
                var uniformsColor = new List<WebaMaterialUniformColor>();
                var uniformsFloatMat2 = new List<WebaMaterialUniformFloatMat2>();
                var uniformsFloatMat3 = new List<WebaMaterialUniformFloatMat3>();
                var uniformsFloatMat4 = new List<WebaMaterialUniformFloatMat4>();
                var uniformsInt = new List<WebaMaterialUniformInt>();
                var uniformsIntVec2 = new List<WebaMaterialUniformIntVec2>();
                var uniformsIntVec3 = new List<WebaMaterialUniformIntVec3>();
                var uniformsIntVec4 = new List<WebaMaterialUniformIntVec4>();

                foreach (var pair in uniforms)
                {
                    var name = pair.Key;
                    var uniform = pair.Value;
                    var typex = uniform.Value<int>("type");
                    var type = (EWebaMaterialUniformType)typex;

                    switch (type)
                    {
                        case (EWebaMaterialUniformType.FLOAT):
                            uniformsFloat.Add(new WebaMaterialUniformFloat { name = name, value = uniform.Value<float>("value") });
                            break;
                        case (EWebaMaterialUniformType.INT):
                            uniformsInt.Add(new WebaMaterialUniformInt { name = name, value = uniform.Value<int>("value") });
                            break;
                        case (EWebaMaterialUniformType.SAMPLER_2D):
                            var tex = (int)uniform.Value<JObject>("value")["index"];
                            uniformsTexture.Add(new WebaMaterialUniformTexture { name = name, id = new TextureId { Id = tex, Root = root } });
                            break;
                        // todo: support cubemap
                        case (EWebaMaterialUniformType.SAMPLER_CUBE):
                            break;
                        case (EWebaMaterialUniformType.INT_VEC2):
                            var iv2 = uniform.Value<JArray>("value");
                            uniformsIntVec2.Add(new WebaMaterialUniformIntVec2 { name = name, value = new Vector2Int((int)iv2[0], (int)iv2[1]) });
                            break;
                        case (EWebaMaterialUniformType.INT_VEC3):
                            var iv3 = uniform.Value<JArray>("value");
                            uniformsIntVec3.Add(new WebaMaterialUniformIntVec3 { name = name, value = new Vector3Int((int)iv3[0], (int)iv3[1], (int)iv3[2]) });
                            break;
                        case (EWebaMaterialUniformType.INT_VEC4):
                            var iv4 = uniform.Value<JArray>("value");
                            uniformsIntVec4.Add(new WebaMaterialUniformIntVec4 { name = name, value = new Vector4((int)iv4[0], (int)iv4[1], (int)iv4[2], (int)iv4[3]) });
                            break;
                        case (EWebaMaterialUniformType.FLOAT_VEC2):
                            var fv2 = uniform.Value<JArray>("value");
                            uniformsFloatVec2.Add(new WebaMaterialUniformFloatVec2 { name = name, value = new Vector2((float)fv2[0], (float)fv2[1]) });
                            break;
                        case (EWebaMaterialUniformType.FLOAT_VEC3):
                            var fv3 = uniform.Value<JArray>("value");
                            uniformsFloatVec3.Add(new WebaMaterialUniformFloatVec3 { name = name, value = new Vector3((float)fv3[0], (float)fv3[1], (float)fv3[2]) });
                            break;
                        case (EWebaMaterialUniformType.FLOAT_VEC4):
                            var fv4 = uniform.Value<JArray>("value");
                            if (uniform.Value<bool>("isColor"))
                            {
                                var value = Utils.ImportColor(new Color((float)fv4[0], (float)fv4[1], (float)fv4[2], (float)fv4[3]));
                                uniformsColor.Add(new WebaMaterialUniformColor { name = name, value = value });
                            } else
                            {
                                uniformsFloatVec4.Add(new WebaMaterialUniformFloatVec4 { name = name, value = new Vector4((float)fv4[0], (float)fv4[1], (float)fv4[2], (float)fv4[3]) });
                            }
                            
                            break;
                        case (EWebaMaterialUniformType.FLOAT_MAT2):
                            var fm2 = uniform.Value<JArray>("value");
                            var vm2 = new Matrix4x4();
                            vm2.m00 = (float)fm2[0];
                            vm2.m01 = (float)fm2[1];
                            vm2.m10 = (float)fm2[2];
                            vm2.m11 = (float)fm2[3];
                            uniformsFloatMat2.Add(new WebaMaterialUniformFloatMat2 { name = name, value = vm2 });
                            break;
                        case (EWebaMaterialUniformType.FLOAT_MAT3):
                            var fm3 = uniform.Value<JArray>("value");
                            var vm3 = new Matrix4x4();
                            vm3.m00 = (float)fm3[0];
                            vm3.m01 = (float)fm3[1];
                            vm3.m02 = (float)fm3[2];
                            vm3.m10 = (float)fm3[3];
                            vm3.m11 = (float)fm3[4];
                            vm3.m12 = (float)fm3[5];
                            vm3.m20 = (float)fm3[6];
                            vm3.m21 = (float)fm3[7];
                            vm3.m22 = (float)fm3[8];
                            uniformsFloatMat3.Add(new WebaMaterialUniformFloatMat3 { name = name, value = vm3 });
                            break;
                        case (EWebaMaterialUniformType.FLOAT_MAT4):
                            var fm4 = uniform.Value<JArray>("value");
                            uniformsFloatMat4.Add(new WebaMaterialUniformFloatMat4
                            {
                                name = name,
                                value = new Matrix4x4(
                                    new Vector4((float)fm4[0], (float)fm4[4], (float)fm4[8], (float)fm4[12]),
                                    new Vector4((float)fm4[1], (float)fm4[5], (float)fm4[9], (float)fm4[13]),
                                    new Vector4((float)fm4[2], (float)fm4[6], (float)fm4[10], (float)fm4[14]),
                                    new Vector4((float)fm4[3], (float)fm4[7], (float)fm4[11], (float)fm4[15])
                                )
                            });
                            break;
                        default:
                            break;
                    }
                }

                extension.uniformsColor = uniformsColor.ToArray();
                extension.uniformsTexture = uniformsTexture.ToArray();
                extension.uniformsCubeTexture = uniformsCubeTexture.ToArray();
                extension.uniformsFloat = uniformsFloat.ToArray();
                extension.uniformsFloatVec2 = uniformsFloatVec2.ToArray();
                extension.uniformsFloatVec3 = uniformsFloatVec3.ToArray();
                extension.uniformsFloatVec4 = uniformsFloatVec4.ToArray();
                extension.uniformsFloatMat2 = uniformsFloatMat2.ToArray();
                extension.uniformsFloatMat3 = uniformsFloatMat3.ToArray();
                extension.uniformsFloatMat4 = uniformsFloatMat4.ToArray();
                extension.uniformsInt = uniformsInt.ToArray();
                extension.uniformsIntVec2 = uniformsIntVec2.ToArray();
                extension.uniformsIntVec3 = uniformsIntVec3.ToArray();
                extension.uniformsIntVec4 = uniformsIntVec4.ToArray();
            }
        }

        return extension;
    }
    /*
    public override void Import(EditorImporter importer, UnityEngine.Material material, GLTF.Schema.Material gltfMat, Extension extension)
    {
        var mat = (WebaCustomMaterialExtension)extension;
        var className = "Weba/" + mat.className;
        var shader = Shader.Find(className);

        var alphaMode = gltfMat.AlphaMode;
        if (alphaMode == AlphaMode.BLEND)
        {
            mat.transparent = true;
        }
        else if (alphaMode == AlphaMode.MASK)
        {
            mat.transparent = true;
        }

        if (shader == null)
        {
            shader = Shader.Find(className.Replace("Material", ""));
        }

        if (shader == null)
        {
            mat.material = material;
            mat.isComponent = true;
            return;
        }

        material.shader = shader;
        if (material.HasProperty("cloneForInst"))
        {
            material.SetInt("cloneForInst", 1);
        }
        
        material.SetInt("_Mode", (int)alphaMode);
        material.SetFloat("_Cutoff", (float)gltfMat.AlphaCutoff);

        WriteUiforms(importer, material, mat.uniformsTexture);
        WriteUiforms(importer, material, mat.uniformsCubeTexture);
        WriteUiforms(importer, material, mat.uniformsFloat);
        WriteUiforms(importer, material, mat.uniformsFloatVec2);
        WriteUiforms(importer, material, mat.uniformsFloatVec3);
        WriteUiforms(importer, material, mat.uniformsFloatVec4);
        WriteUiforms(importer, material, mat.uniformsColor);
        WriteUiforms(importer, material, mat.uniformsFloatMat2);
        WriteUiforms(importer, material, mat.uniformsFloatMat3);
        WriteUiforms(importer, material, mat.uniformsFloatMat4);
        WriteUiforms(importer, material, mat.uniformsInt);
        WriteUiforms(importer, material, mat.uniformsIntVec2);
        WriteUiforms(importer, material, mat.uniformsIntVec3);
        WriteUiforms(importer, material, mat.uniformsIntVec4);
    }

    private void WriteUiforms<TValue>(EditorImporter importer, UnityEngine.Material material, WebaMaterialUniform<TValue>[] uniforms) {
        foreach (WebaMaterialUniform<TValue> uniform in uniforms)
        {
            var name = uniform.name;
            switch (uniform.type)
            {
                case (EWebaMaterialUniformType.FLOAT):
                    material.SetFloat(name, (uniform as WebaMaterialUniformFloat).value);
                    break;
                case (EWebaMaterialUniformType.INT):
                    material.SetInt(name, (uniform as WebaMaterialUniformInt).value);
                    break;
                case (EWebaMaterialUniformType.SAMPLER_2D):
                    var tex = importer.GetTexture((uniform as WebaMaterialUniformTexture).id.Id);
                    material.SetTexture(name, tex);
                    break;
                // todo: support cubemap
                case (EWebaMaterialUniformType.SAMPLER_CUBE):
                    break;
                case (EWebaMaterialUniformType.FLOAT_VEC2):
                    var fv2 = (uniform as WebaMaterialUniformFloatVec2).value;
                    material.SetFloatArray(name, new List<float> { fv2.x, fv2.y });
                    break;
                case (EWebaMaterialUniformType.FLOAT_VEC3):
                    var fv3 = (uniform as WebaMaterialUniformFloatVec3).value;
                    material.SetFloatArray(name, new List<float> { fv3.x, fv3.y, fv3.z });
                    break;
                case (EWebaMaterialUniformType.FLOAT_VEC4):
                    if (uniform.GetType() == typeof(WebaMaterialUniformColor))
                    {
                        material.SetColor(name, (uniform as WebaMaterialUniformColor).value);
                    }
                    material.SetVector(name, (uniform as WebaMaterialUniformFloatVec4).value);
                    break;
                case (EWebaMaterialUniformType.FLOAT_MAT2):
                    material.SetMatrix(name, (uniform as WebaMaterialUniformFloatMat2).value);
                    break;
                case (EWebaMaterialUniformType.FLOAT_MAT3):
                    material.SetMatrix(name, (uniform as WebaMaterialUniformFloatMat3).value);
                    break;
                case (EWebaMaterialUniformType.FLOAT_MAT4):
                    material.SetMatrix(name, (uniform as WebaMaterialUniformFloatMat4).value);
                    break;
                default:
                    break;
            }
        }
    }*/
}
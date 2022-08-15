using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;

public struct WebaNodeOption
{
    public string type;
    public JToken value;

    public WebaNodeOption(string type, JToken value) {
        this.type = type;
        this.value = value;
    }
}

[AddComponentMenu("Weba/Classes/Weba Node Class")]
public class WebaNodeClass : MonoBehaviour
{
    public object options;

    public virtual JObject Serialize(ExporterEntry entry, WebaNodeExtension extension)
    {
        var type = GetType();
        var className = type.FullName.Replace("WebaNodeClass_", "");
        extension.className = string.IsNullOrEmpty(className) ? extension.className : className;
        var result = new JObject();

        if (type.GetField("options") == null)
        {
            return result;
        }

        var initOptions = type.GetField("options").GetValue(this);
        foreach (var pair in initOptions.GetType().GetFields())
        {
            var option = SerializeValue(entry, pair.GetValue(initOptions));
            result.Add(pair.Name, new JObject(
                new JProperty("type", option.type),
                new JProperty("value", option.value)
            ));
        }

        return result;
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, object option)
    {
        if (option is float)
        {
            return SerializeValue(entry, (float)option);
        }
        else if (option is int)
        {
            return SerializeValue(entry, (int)option);
        }
        else if (option is string)
        {
            return SerializeValue(entry, (string)option);
        }
        else if (option is bool)
        {
            return SerializeValue(entry, (bool)option);
        }
        else if (option is Vector2)
        {
            return SerializeValue(entry, (Vector2)option);
        }
        else if (option is Vector3)
        {
            return SerializeValue(entry, (Vector3)option);
        }
        else if (option is Vector4)
        {
            return SerializeValue(entry, (Vector4)option);
        }
        else if (option is Matrix4x4)
        {
            return SerializeValue(entry, (Matrix4x4)option);
        }
        else if (option is Quaternion)
        {
            return SerializeValue(entry, (Quaternion)option);
        }
        else if (option is Color)
        {
            return SerializeValue(entry, (Color)option);
        }
        else if (option is Texture2D)
        {
            return SerializeValue(entry, (Texture2D)option);
        }
        else if (option is Cubemap)
        {
            return SerializeValue(entry, (Cubemap)option);
        }
        else if (option is WebaAtlas)
        {
            return SerializeValue(entry, (WebaAtlas)option);
        }
        else if (option is Material)
        {
            return SerializeValue(entry, (Material)option);
        }
        else if (option is Array)
        {
            var res = new JArray();
            foreach (var item in (Array)option)
            {
                var value = SerializeValue(entry, item);
                res.Add(new JObject(
                    new JProperty("type", value.type),
                    new JProperty("value", value.value)
                ));
            }

            return new WebaNodeOption("Array", res);
        }
        else
        {
            return SerializeValueUnknown(entry, option);
        }
    }

    public virtual WebaNodeOption SerializeValueUnknown(ExporterEntry entry, object option)
    {
        var result = new JObject();

        foreach (var pair in option.GetType().GetFields())
        {
            var value = SerializeValue(entry, pair.GetValue(option));
            result.Add(pair.Name, new JObject(
                new JProperty("type", value.type),
                new JProperty("value", value.value)
            ));
        }

        return new WebaNodeOption("Object", result);
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, float option)
    {
        return new WebaNodeOption("Float", option);
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, int option)
    {
        return new WebaNodeOption("Int", option);
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, string option)
    {
        return new WebaNodeOption("String", option);
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, bool option)
    {
        return new WebaNodeOption("Bool", option);
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Vector2 option)
    {
        return new WebaNodeOption("Vec2", new JArray { option.x, option.y });
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Vector3 option)
    {
        return new WebaNodeOption("Vec3", new JArray { option.x, option.y, option.z });
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Vector4 option)
    {
        return new WebaNodeOption("Vec4", new JArray { option.x, option.y, option.z, option.w });
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Quaternion option)
    {
        return new WebaNodeOption("Quat", new JArray { option.x, option.y, option.z, option.w });
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Color option)
    {
        return new WebaNodeOption("Color", new JArray { option.r, option.g, option.b, option.a });
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Matrix4x4 option)
    {
        return new WebaNodeOption("Mat4", new JArray {
            option.m00, option.m01, option.m02, option.m03,
            option.m10, option.m11, option.m12, option.m13,
            option.m20, option.m21, option.m22, option.m23,
            option.m30, option.m31, option.m32, option.m33
        });
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Texture2D option)
    {
        return new WebaNodeOption("Tex2D", new JObject(new JProperty("index", entry.SaveTexture(option, true).Id)));
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Cubemap option)
    {
        return new WebaNodeOption("TexCube", new JObject(new JProperty("index", entry.SaveCubeTexture(option, true).Id)));
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, WebaAtlas option)
    {
        //ExtensionManager.Serialize(ExtensionManager.GetExtensionName(typeof(WebaAtlasExtensionFactory)), entry, entry.root.Extensions, option);
        var atlasId = WebaAtlasExtensionFactory.GetAtlasIndex(entry, option);
        return new WebaNodeOption("Atlas", new JObject(new JProperty("index", atlasId)));
    }

    public virtual WebaNodeOption SerializeValue(ExporterEntry entry, Material option)
    {
        return new WebaNodeOption("Mat", new JObject(new JProperty("index", entry.SaveNormalMaterial(option).Id)));
    }
}

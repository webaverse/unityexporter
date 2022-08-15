using System;
using Newtonsoft.Json.Linq;
using GLTF.Math;
using Newtonsoft.Json;
using GLTF.Extensions;
using System.Collections.Generic;
using UnityEngine;
using GLTF.Schema;

public class WebaPhysicBodyExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaPhysicBody"; }
    //public override List<Type> GetBindedComponents() { return new List<Type> { typeof(WebaRigidBody), typeof(BoxCollider), typeof(SphereCollider) }; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        /*
        WebaPhysicBodyExtension extension;

        if (extensions.ContainsKey(ExtensionName))
        {
            extension = (WebaPhysicBodyExtension)extensions[ExtensionName];
        }
        else
        {
            extension = new WebaPhysicBodyExtension();
            AddExtension(extensions, extension);
        }

        if (component is WebaRigidBody)
        {
            extension.go = ((WebaRigidBody)component).gameObject;
            extension.rigidBody = component as WebaRigidBody;
        }
        else if (component is Collider)
        {
            extension.go = ((Collider)component).gameObject;
            extension.colliders.Add(component as Collider);
        }*/
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaPhysicBodyExtension();
        var tmpGo = new GameObject();

        List<Collider> colliders = new List<Collider>();
        var rigidBody = tmpGo.AddComponent<WebaRigidBody>();

        if (extensionToken.Value["mass"] != null)
        {
            rigidBody.mass = (float)extensionToken.Value["mass"];
            rigidBody.friction = (float)extensionToken.Value["friction"];
            rigidBody.restitution = (float)extensionToken.Value["restitution"];
            rigidBody.unControl = (bool)extensionToken.Value["unControl"];
            rigidBody.physicStatic = (bool)extensionToken.Value["physicStatic"];
            rigidBody.sleeping = (bool)extensionToken.Value["sleeping"];
        }

        foreach (JContainer collider in extensionToken.Value["colliders"]) {
            var type = (string)collider["type"];

            switch (type)
            {
                case ("SPHERE"):
                    var sc = tmpGo.AddComponent<SphereCollider>();
                    sc.radius = (float)collider["radius"];
                    sc.center = new  UnityEngine.Vector3(
                        (float)collider["offset"][0],
                        (float)collider["offset"][1],
                        (float)collider["offset"][2]
                    );

                    if (collider["isTrigger"] != null)
                    {
                        sc.isTrigger = (bool)collider["isTrigger"];
                    }

                    colliders.Add(sc);
                    break;
                case ("BOX"):
                    var bc = tmpGo.AddComponent<BoxCollider>();
                    bc.size = new UnityEngine.Vector3(
                        (float)collider["size"][0],
                        (float)collider["size"][1],
                        (float)collider["size"][2]
                    );

                    bc.center = new UnityEngine.Vector3(
                        (float)collider["offset"][0],
                        (float)collider["offset"][1],
                        (float)collider["offset"][2]
                    );

                    if (collider["isTrigger"] != null)
                    {
                        bc.isTrigger = (bool)collider["isTrigger"];
                    }

                    colliders.Add(bc);
                    break;
                default:
                    Debug.LogWarning("In current time, Weba only supports shpere and box collider !");
                    break;
            }
        }

        extension.rigidBody = rigidBody;
        extension.colliders = colliders;
        extension.go = tmpGo;

        return extension;
    }
    /*
    public override void Import(EditorImporter importer, GameObject gameObject, Node gltfNode, Extension extension)
    {
        var physicBody = (WebaPhysicBodyExtension)extension;
        var rigidBody = gameObject.AddComponent<WebaRigidBody>();
        rigidBody.mass = physicBody.rigidBody.mass;
        rigidBody.restitution = physicBody.rigidBody.restitution;
        rigidBody.friction = physicBody.rigidBody.friction;
        rigidBody.unControl = physicBody.rigidBody.unControl;
        rigidBody.physicStatic = physicBody.rigidBody.physicStatic;
        rigidBody.sleeping = physicBody.rigidBody.sleeping;

        foreach (var c in physicBody.colliders)
        {
            if (c is SphereCollider)
            {
                var collider = gameObject.AddComponent<SphereCollider>();
                collider.center = ((SphereCollider)c).center;
                collider.radius = ((SphereCollider)c).radius;
                collider.isTrigger = c.isTrigger;
            }
            else if (c is BoxCollider)
            {
                var collider = gameObject.AddComponent<BoxCollider>();
                collider.center = ((BoxCollider)c).center;
                collider.size = ((BoxCollider)c).size;
                collider.isTrigger = c.isTrigger;
            }
        }

        UnityEngine.Object.DestroyImmediate(physicBody.go);
    }*/
}
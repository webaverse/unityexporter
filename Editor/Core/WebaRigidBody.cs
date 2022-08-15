using UnityEngine;
using System.Collections;

[AddComponentMenu("Weba/Core Components/Weba Rigid Body")]
public class WebaRigidBody : MonoBehaviour
{
    [Header("Body Options")]
    // If this node is toplevel, selfType will default to Actor
    // If not toplevel, it will default to Inherit
    // If parent's type is Component, it must be Component
    public float mass = 1;
    public float friction = 0;
    public float restitution = 0;
    public bool unControl = false;
    public bool physicStatic = false;
    public bool sleeping = false;

    public static WebaRigidBody CreateBodyForPickOnly()
    {
        var body = new WebaRigidBody();
        body.unControl = false;
        body.physicStatic = true;
        body.sleeping = true;

        return body;
    }
}

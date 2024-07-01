using System.Runtime.InteropServices;
using UnityEngine;



/// <summary>
/// Vector3f type Extensions
/// </summary>
namespace XR.Interaction.Input
{
    public static class Vector3Extensions
    {
        public static Vector3 FromFlippedXVector3f(this Vector3f v)
        {
            return new Vector3() { x = -v.x, y = v.y, z = v.z };
        }

        public static Vector3 FromFlippedZVector3f(this Vector3f v)
        {
            return new Vector3() { x = v.x, y = v.y, z = -v.z };
        }

        public static Vector3f FromFlippedZ(this Vector3f v)
        {
            return new Vector3f() { x = v.x, y = v.y, z = -v.z };
        }
        public static Vector3f FromFlippedZ(this Vector3 v)
        {
            return new Vector3f() { x = v.x, y = v.y, z = -v.z };
        }
    }
}
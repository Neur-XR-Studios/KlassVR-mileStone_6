using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XR.Interaction.Input
{
    public static class QuaternionExtensions
    {
        public static Quaternion FromQuatf(this Quatf q)
        {
            return new Quaternion() { x = q.x, y = q.y, z = q.z, w = q.w };
        }

        public static Quaternion FromFlippedXQuatf(this Quatf q)
        {
            return new Quaternion() { x = q.x, y = -q.y, z = -q.z, w = q.w };
        }

        public static Quaternion FromFlippedZQuatf(this Quatf q)
        {
            return new Quaternion() { x = -q.x, y = -q.y, z = q.z, w = q.w };
        }
        public static Quatf FromFlippedX(this Quatf q)
        {
            return new Quatf() { x = q.x, y = -q.y, z = -q.z, w = q.w };
        }
        public static Quatf FromFlippedX(this Quaternion q)
        {
            return new Quatf() { x = q.x, y = -q.y, z = -q.z, w = q.w };
        }
        public static Quaternion FromFlippedXNew(this Quaternion q)
        {
            return new Quaternion() { x = q.x, y = -q.y, z = -q.z, w = q.w };
        }
        public static Quatf FromFlippedZ(this Quatf q)
        {
            return new Quatf() { x = -q.x, y = -q.y, z = q.z, w = q.w };
        }
        public static Quatf FromFlippedZ(this Quaternion q)
        {
            return new Quatf() { x = -q.x, y = -q.y, z = q.z, w = q.w };
        }
    }
}

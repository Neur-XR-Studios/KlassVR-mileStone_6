/*
 * Copyright (c) NoloVR Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the GSXR UnityXR SDK License Agreement (the "License");
 * you may not use the GSXR UnityXR SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://www.gsxr.org.cn/
 *
 * Unless required by applicable law or agreed to in writing, the GSXR UnityXR SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace XR.Interaction
{
    /// <summary>
    /// Represents a curved rectangular section of a
    /// cylinder wall.
    /// </summary>
    public interface ICurvedPlane
    {
        /// <summary>
        /// The cylinder the curved plane lies on
        /// </summary>
        Cylinder Cylinder { get; }

        /// <summary>
        /// The horizontal size of the plane, in degrees
        /// </summary>
        float ArcDegrees { get; }

        /// <summary>
        /// The rotation of the center of the plane relative
        /// to the Cylinder's forward Z axis, in degrees
        /// </summary>
        float Rotation { get; }

        /// <summary>
        /// The bottom of the plane relative to the
        /// Cylinder Y position, in Cylinder local space
        /// </summary>
        float Bottom { get; }

        /// <summary>
        /// The top of the plane relative to the
        /// Cylinder Y position, in Cylinder local space
        /// </summary>
        float Top { get; }
    }
}

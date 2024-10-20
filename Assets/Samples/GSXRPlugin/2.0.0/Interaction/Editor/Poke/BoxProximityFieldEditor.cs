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

using UnityEditor;
using UnityEngine;

namespace XR.Interaction.Editor
{
    [CustomEditor(typeof(BoxProximityField))]
    public class BoxProximityFieldEditor : UnityEditor.Editor
    {
        private SerializedProperty _boxTransformProperty;

        private void Awake()
        {
            _boxTransformProperty = serializedObject.FindProperty("_boxTransform");
        }

        public void OnSceneGUI()
        {
            Handles.color = EditorConstants.PRIMARY_COLOR;

            Transform boxTransform = _boxTransformProperty.objectReferenceValue as Transform;

            if (boxTransform != null)
            {
                using (new Handles.DrawingScope(boxTransform.localToWorldMatrix))
                {
                    Handles.DrawWireCube(Vector3.zero, Vector3.one);
                }
            }
        }
    }
}

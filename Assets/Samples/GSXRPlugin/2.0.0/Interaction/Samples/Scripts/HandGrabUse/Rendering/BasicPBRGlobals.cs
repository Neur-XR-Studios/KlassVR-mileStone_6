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

using UnityEngine;

namespace XR.Interaction.Demo
{
    /// <summary>
    /// Supplies the 'main' directionl light properties to the BasicPBR shader
    /// </summary>
    public class BasicPBRGlobals : MonoBehaviour
    {
        [SerializeField]
        private Light _mainlight;

        private void Update()
        {
            UpateShaderGlobals();
        }

        private void UpateShaderGlobals()
        {
            Light light = _mainlight;
            bool hasLight = light && light.isActiveAndEnabled;
            Shader.SetGlobalVector("_BasicPBRLightDir", hasLight ? light.transform.forward : Vector3.down);
            Shader.SetGlobalColor("_BasicPBRLightColor", hasLight ? light.color : Color.black);
        }
    }
}

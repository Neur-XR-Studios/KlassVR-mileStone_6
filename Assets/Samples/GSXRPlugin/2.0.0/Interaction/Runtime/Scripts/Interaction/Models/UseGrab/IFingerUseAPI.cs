/************************************************************************************
Copyright : Copyright (c) NoloVR Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the GSXR UnityXR SDK License Agreement, available at
https://www.gsxr.org.cn/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using XR.Interaction.Input;

namespace XR.Interaction
{
    public interface IFingerUseAPI
    {
        float GetFingerUseStrength(HandFinger finger);
    }
}

/************************************************************************************
Copyright : Copyright (c) NoloVR Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the GSXR UnityXR SDK License Agreement, available at
https://www.gsxr.org.cn/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using XR.Interaction.GrabAPI;
using XR.Interaction.Input;
using UnityEngine;
using UnityEngine.Assertions;

namespace XR.Interaction
{
    public class UseFingerRawPinchAPI : MonoBehaviour, IFingerUseAPI
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        private IHand Hand { get; set; }

        private IFingerAPI _grabAPI = new FingerRawPinchAPI();

        private int _lastDataVersion = -1;
        protected bool _started;

        protected virtual void Awake()
        {
            Hand = _hand as IHand;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Hand, "Hand not assigned");
            this.EndStart(ref _started);
        }

        public float GetFingerUseStrength(HandFinger finger)
        {
            if (_lastDataVersion != Hand.CurrentDataVersion)
            {
                _lastDataVersion = Hand.CurrentDataVersion;
                _grabAPI.Update(Hand);
            }
            return _grabAPI.GetFingerGrabScore(finger);
        }

        #region Inject
        public void InjectAllUseFingerRawPinchAPI(IHand hand)
        {
            InjectHand(hand);
        }

        public void InjectHand(IHand hand)
        {
            _hand = hand as MonoBehaviour;
            Hand = hand;
        }
        #endregion
    }
}

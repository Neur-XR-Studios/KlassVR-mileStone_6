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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace XR.Interaction.PoseDetection
{
    [Serializable]
    public class FingerFeatureStateThreshold : IFeatureStateThreshold<string>
    {
        public FingerFeatureStateThreshold() { }

        public FingerFeatureStateThreshold(float thresholdMidpoint,
            float thresholdWidth,
            string firstState,
            string secondState)
        {
            _thresholdMidpoint = thresholdMidpoint;
            _thresholdWidth = thresholdWidth;
            _firstState = firstState;
            _secondState = secondState;
        }

        [SerializeField]
        private float _thresholdMidpoint;
        [SerializeField]
        private float _thresholdWidth;
        [SerializeField]
        private string _firstState;
        [SerializeField]
        private string _secondState;

        public float ThresholdMidpoint => _thresholdMidpoint;
        public float ThresholdWidth => _thresholdWidth;
        public float ToFirstWhenBelow => _thresholdMidpoint - _thresholdWidth * 0.5f;
        public float ToSecondWhenAbove => _thresholdMidpoint + _thresholdWidth * 0.5f;
        public string FirstState => _firstState;
        public string SecondState => _secondState;
    }

    [Serializable]
    public class FingerFeatureThresholds : IFeatureStateThresholds<FingerFeature, string>
    {
        public FingerFeatureThresholds() { }

        public FingerFeatureThresholds(FingerFeature feature,
            IEnumerable<FingerFeatureStateThreshold> thresholds)
        {
            _feature = feature;
            _thresholds = new List<FingerFeatureStateThreshold>(thresholds);
        }

        [SerializeField]
        private FingerFeature _feature;
        [SerializeField]
        private List<FingerFeatureStateThreshold> _thresholds;

        public FingerFeature Feature => _feature;
        public IReadOnlyList<IFeatureStateThreshold<string>> Thresholds => _thresholds;
    }

    /// <summary>
    ///  A configuration class that contains a list of threshold boundaries
    /// </summary>
    [CreateAssetMenu(menuName = "XR/Interaction/SDK/Pose Detection/Finger Thresholds")]
    public class FingerFeatureStateThresholds : ScriptableObject,
        IFeatureThresholds<FingerFeature, string>
    {
        [SerializeField]
        private List<FingerFeatureThresholds> _featureThresholds;

        [SerializeField]
        [Tooltip("Length of time that the finger must be in the new state before the feature " +
                 "state provider will use the new value.")]
        private double _minTimeInState;

        public void Construct(List<FingerFeatureThresholds> featureThresholds,
            double minTimeInState)
        {
            _featureThresholds = featureThresholds;
            _minTimeInState = minTimeInState;
        }

        public IReadOnlyList<IFeatureStateThresholds<FingerFeature, string>>
            FeatureStateThresholds => _featureThresholds;

        public double MinTimeInState => _minTimeInState;
    }
}

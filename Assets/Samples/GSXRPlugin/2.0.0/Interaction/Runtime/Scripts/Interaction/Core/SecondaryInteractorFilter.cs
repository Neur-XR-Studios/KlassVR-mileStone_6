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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace XR.Interaction
{
    /// <summary>
    /// Checks for the existence of a Secondary Interactor Connection given a Primary Interaction.
    /// Filters out Interactors which are not Secondary to a hovering or selecting Primary.
    /// </summary>
    public class SecondaryInteractorFilter : MonoBehaviour, IGameObjectFilter
    {
        [SerializeField, Interface(typeof(IInteractable))]
        private MonoBehaviour _primaryInteractable;
        public IInteractable PrimaryInteractable { get; private set; }

        [SerializeField, Interface(typeof(IInteractable))]
        private MonoBehaviour _secondaryInteractable;
        public IInteractable SecondaryInteractable { get; private set; }

        [SerializeField]
        private bool _selectRequired = false;

        private Dictionary<IInteractorView, List<IInteractorView>> _primaryToSecondaryMap;

        protected bool _started = false;

        protected virtual void Awake()
        {
            PrimaryInteractable = _primaryInteractable as IInteractable;
            SecondaryInteractable = _secondaryInteractable as IInteractable;
            _primaryToSecondaryMap = new Dictionary<IInteractorView, List<IInteractorView>>();
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(PrimaryInteractable);
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                if (_selectRequired)
                {
                    PrimaryInteractable.WhenSelectingInteractorViewRemoved +=
                        HandleWhenSelectingInteractorViewRemoved;
                }
                else
                {
                    PrimaryInteractable.WhenInteractorViewRemoved +=
                        HandleWhenInteractorViewRemoved;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                if (_selectRequired)
                {
                    PrimaryInteractable.WhenSelectingInteractorViewRemoved -=
                        HandleWhenSelectingInteractorViewRemoved;
                }
                else
                {
                    PrimaryInteractable.WhenInteractorViewRemoved -=
                        HandleWhenInteractorViewRemoved;
                }
            }
        }

        public bool Filter(GameObject gameObject)
        {
            SecondaryInteractorConnection connection =
                gameObject.GetComponent<SecondaryInteractorConnection>();
            if (connection == null)
            {
                return false;
            }

            IEnumerable<IInteractorView> primaryViews = _selectRequired
                ? PrimaryInteractable.SelectingInteractorViews
                : PrimaryInteractable.InteractorViews;

            foreach (IInteractorView primaryView in primaryViews)
            {
                if (primaryView == connection.PrimaryInteractor)
                {
                    if (!_primaryToSecondaryMap.ContainsKey(primaryView))
                    {
                        _primaryToSecondaryMap.Add(primaryView, new List<IInteractorView>());
                    }

                    List<IInteractorView> secondaryViews = _primaryToSecondaryMap[primaryView];
                    if (!secondaryViews.Contains(connection.SecondaryInteractor))
                    {
                        secondaryViews.Add(connection.SecondaryInteractor);
                    }

                    return true;
                }
            }

            return false;
        }

        private void ClearInteractorsForPrimary(IInteractorView primary)
        {
            if (!_primaryToSecondaryMap.ContainsKey(primary))
            {
                return;
            }

            List<IInteractorView> secondaryViews = _primaryToSecondaryMap[primary];
            foreach(IInteractorView secondaryView in secondaryViews)
            {
                SecondaryInteractable.RemoveInteractorByIdentifier(secondaryView.Identifier);
            }

            _primaryToSecondaryMap.Remove(primary);
        }

        private void HandleWhenInteractorViewRemoved(IInteractorView primaryView)
        {
            ClearInteractorsForPrimary(primaryView);
        }

        private void HandleWhenSelectingInteractorViewRemoved(IInteractorView primaryView)
        {
            ClearInteractorsForPrimary(primaryView);
        }

        #region Inject

        public void InjectAllSecondaryInteractorFilter(
            IInteractable primaryInteractable,
            IInteractable secondaryInteractable,
            bool selectRequired = false)
        {
            InjectPrimaryInteractable(primaryInteractable);
            InjectSecondaryInteractable(secondaryInteractable);
            InjectSelectRequired(selectRequired);
        }

        private void InjectPrimaryInteractable(IInteractable interactableView)
        {
            PrimaryInteractable = interactableView;
            _primaryInteractable = interactableView as MonoBehaviour;
        }

        private void InjectSecondaryInteractable(IInteractable interactable)
        {
            SecondaryInteractable = interactable;
            _secondaryInteractable = interactable as MonoBehaviour;
        }

        private void InjectSelectRequired(bool selectRequired)
        {
            _selectRequired = selectRequired;
        }

        #endregion
    }
}

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
using UnityEngine.Assertions;
using XR.Interaction.Surfaces;

namespace XR.Interaction
{
    public class RayInteractable : PointerInteractable<RayInteractor, RayInteractable>
    {
        [SerializeField, Interface(typeof(ISurface))]
        private MonoBehaviour _surface;
        public ISurface Surface { get; private set; }

        [SerializeField, Optional, Interface(typeof(ISurface))]
        private MonoBehaviour _selectSurface = null;
        private ISurface SelectSurface;

        [SerializeField, Optional, Interface(typeof(IMovementProvider))]
        private MonoBehaviour _movementProvider;
        private IMovementProvider MovementProvider { get; set; }

        [SerializeField, Optional]
        private int _tiebreakerScore = 0;

        #region Properties
        public int TiebreakerScore
        {
            get
            {
                return _tiebreakerScore;
            }
            set
            {
                _tiebreakerScore = value;
            }
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            Surface = _surface as ISurface;
            SelectSurface = _selectSurface as ISurface;
            MovementProvider = _movementProvider as IMovementProvider;
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            Assert.IsNotNull(Surface);
            if (_selectSurface != null)
            {
                Assert.IsNotNull(SelectSurface);
            }
            else
            {
                SelectSurface = Surface;
                _selectSurface = SelectSurface as MonoBehaviour;
            }
            this.EndStart(ref _started);
        }

        public bool Raycast(Ray ray, out SurfaceHit hit, in float maxDistance, bool selectSurface)
        {
            hit = new SurfaceHit();
            ISurface surface = selectSurface ? SelectSurface : Surface;
            return surface.Raycast(ray, out hit, maxDistance);
        }

        public IMovement GenerateMovement(in Pose to, in Pose source)
        {
            if (MovementProvider == null)
            {
                return null;
            }
            IMovement movement = MovementProvider.CreateMovement();
            movement.StopAndSetPose(source);
            movement.MoveTo(to);
            return movement;
        }

        #region Inject

        public void InjectAllRayInteractable(ISurface surface)
        {
            InjectSurface(surface);
        }

        public void InjectSurface(ISurface surface)
        {
            Surface = surface;
            _surface = surface as MonoBehaviour;
        }

        public void InjectOptionalSelectSurface(ISurface surface)
        {
            SelectSurface = surface;
            _selectSurface = surface as MonoBehaviour;
        }

        public void InjectOptionalMovementProvider(IMovementProvider provider)
        {
            _movementProvider = provider as MonoBehaviour;
            MovementProvider = provider;
        }
        #endregion
    }
}

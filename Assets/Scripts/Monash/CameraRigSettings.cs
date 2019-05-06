using UnityEngine;
using System.Collections;

namespace Monash
{

    /// <summary>
    /// Camera rig.
    /// </summary>
    public class CameraRigSettings : MonoBehaviour 
    {
        // ENUM

        public enum CameraRigType
        {
            Single,
            Stereoscopic3D
        }

        // DATA

        public CameraRigType cameraRigType;

        public GameObject stereoscopicCameraPrefab;

        public float eyeSeperation = 0.1f;
        public bool swapped = false;

        public bool calibrate;
        public float calibrationSpeed;

        // PRIVATE

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start()
        {
            SetCamera();
        }
            
        /// <summary>
        /// Sets the camera.
        /// </summary>
        /// <param name="cameraRigType">Camera rig type.</param>
        public void SetCamera()
        {
            switch (cameraRigType)
            {
            case CameraRigType.Stereoscopic3D:
                {
                    SetStereoscopicCamera();
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the stereoscopic camera.
        /// </summary>
        private void SetStereoscopicCamera()
        {
            // Create camera
            GameObject rootCamera = (GameObject)Instantiate(stereoscopicCameraPrefab);
			rootCamera.tag = "MainCamera";
            rootCamera.transform.SetParent(transform.parent);
            rootCamera.transform.localPosition = Vector3.zero;
            rootCamera.transform.localRotation = Quaternion.identity;


            // Duplicate
            GameObject otherStereoCamera = (GameObject)Instantiate(gameObject);
            Destroy(otherStereoCamera.GetComponent<CameraRigSettings>());
            Destroy(otherStereoCamera.GetComponent<AudioListener>());
                    
            // Reparent
            otherStereoCamera.transform.SetParent(rootCamera.transform);
            otherStereoCamera.transform.localPosition = Vector3.zero;
            otherStereoCamera.transform.localRotation = Quaternion.identity;

            transform.SetParent(rootCamera.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            RenderStereo renderStereoCamera = rootCamera.GetComponent<RenderStereo>();
            if (renderStereoCamera != null)
            {
                renderStereoCamera.calibrate = calibrate;
                renderStereoCamera.calibrationSpeed = calibrationSpeed;

                renderStereoCamera.camera1 = gameObject.AddComponent<StereoCamera>();
                renderStereoCamera.camera2 = otherStereoCamera.AddComponent<StereoCamera>();

                renderStereoCamera.Initialise(eyeSeperation, swapped);
            }
        }
    }

}   // Monash
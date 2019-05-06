using UnityEngine;
using System.Collections;

namespace Monash
{

    /// <summary>
    /// Stero camera.
    /// </summary>
    public class StereoCamera : MonoBehaviour 
    {

        [HideInInspector]
        public new Camera camera;
        [HideInInspector]
        public RenderTexture renderTexture;

        public void Initialise()
        {
            camera = GetComponent<Camera>();
            renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.useMipMap = false;
            renderTexture.isPowerOfTwo = false;

            if (camera != null)
            {
                camera.targetTexture = renderTexture;
                camera.enabled = false;
            }
        }

    }

}   // Namespace
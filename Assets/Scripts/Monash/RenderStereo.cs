using UnityEngine;
using System.Collections;

namespace Monash
{
    /// <summary>
    /// Render stereo.
    /// </summary>
    public class RenderStereo : MonoBehaviour 
    {
        // DATA

        public Material steroRendererMaterial;

        [HideInInspector]
        public StereoCamera camera1;
        [HideInInspector]
        public StereoCamera camera2;

        [HideInInspector]
        public bool calibrate;
        [HideInInspector]
        public float calibrationSpeed = 0.05f;
            
        private bool mSwapped;


        // PUBLIC

        /// <summary>
        /// Initialise this instance.
        /// </summary>
        public void Initialise(float initialEyeSeperation, bool swapped)
        {
            if (camera1 != null && camera2 != null)
            {
                mSwapped = swapped;

                camera1.Initialise();
                camera2.Initialise();

                camera1.transform.localPosition = new Vector3(camera1.transform.localPosition.x - initialEyeSeperation, camera1.transform.localPosition.y, camera1.transform.localPosition.z);
                camera2.transform.localPosition = new Vector3(camera2.transform.localPosition.x + initialEyeSeperation, camera2.transform.localPosition.y, camera2.transform.localPosition.z);
            }

        }

        // PRIVATE

        void Update()
        {
            if (calibrate)
            {
				float speed = calibrationSpeed * Time.deltaTime;

                if (Input.GetKey(KeyCode.RightBracket) )
                {
                    camera1.transform.localPosition = new Vector3(camera1.transform.localPosition.x - speed, camera1.transform.localPosition.y, camera1.transform.localPosition.z);
                    camera2.transform.localPosition = new Vector3(camera2.transform.localPosition.x + speed, camera2.transform.localPosition.y, camera2.transform.localPosition.z);

					print( "Eye separation = " + Mathf.Abs(camera1.transform.localPosition.x).ToString("F4"));
                }

                if (Input.GetKey(KeyCode.LeftBracket) )
                {
                    camera1.transform.localPosition = new Vector3(camera1.transform.localPosition.x + speed, camera1.transform.localPosition.y, camera1.transform.localPosition.z);
                    camera2.transform.localPosition = new Vector3(camera2.transform.localPosition.x - speed, camera2.transform.localPosition.y, camera2.transform.localPosition.z);

					print("Eye separation = " + Mathf.Abs(camera1.transform.localPosition.x).ToString("F4"));
                }



                if (Input.GetKeyDown(KeyCode.F1))
                {
                    mSwapped = !mSwapped;
                }
            }
        }

        void OnGUI()
        {
           /* if (calibrate)
            {
                int w = Screen.width, h = Screen.height;

                GUIStyle style = new GUIStyle();

                Rect rect = new Rect(0, h * 2 / 100, w, h * 2 / 100);
                style.alignment = TextAnchor.UpperLeft;
                style.fontSize = h * 2 / 100;
                style.normal.textColor = new Color (0.5f, 0.0f, 0.0f, 1.0f);
                style.font = (Font)Resources.Load("Fonts/RobotoCondensed-Regular");
				string text = string.Format("Eye seperation {0:0.###} Horizontal {1:0.}", Mathf.Abs(camera1.transform.localPosition.x), Input.GetAxis("Horizontal"));
                GUI.Label(rect, text, style);
            }*/
        }

        /// <summary>
        /// Raises the pre render event.
        /// </summary>
        void OnPreRender()
        {
            if (camera1 != null)
            {
                camera1.camera.Render();
            }

            if (camera2 != null)
            {
                camera2.camera.Render();
            }
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void OnPostRender()
        {
            steroRendererMaterial.SetInt("_ScreenHeight", Screen.height);

            if (mSwapped)
            {
                if (camera2 != null)
                {
                    steroRendererMaterial.SetTexture("_SecondaryTex", camera1.renderTexture);
                }

                if (camera1 != null)
                {
                    Graphics.Blit(camera2.renderTexture, null, steroRendererMaterial);
                }
            }
            else
            {
                if (camera2 != null)
                {
                    steroRendererMaterial.SetTexture("_SecondaryTex", camera2.renderTexture);
                }
                    
                if (camera1 != null)
                {
                    Graphics.Blit(camera1.renderTexture, null, steroRendererMaterial);
                }
            }
        }

    }

}   // Namespace
using UnityEngine;

namespace AdvancedHelicopterControllerwithShooting
{
    public class HelicopterSystemManager : MonoBehaviour
    {
        public ControllerType controllerType;
        public CameraType cameraType;
        public bool ShowCockpit = true;
        public bool ShowRadar = true;

        public GameObject cameraFPS;
        public GameObject cameraTPS;
        public static HelicopterSystemManager Instance;

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (controllerType == ControllerType.KeyboardMouse)
            {
                GameCanvas.Instance.Configure_For_PCConsole();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (controllerType == ControllerType.Mobile)
            {
                GameCanvas.Instance.Configure_For_Mobile();
            }

            if (cameraType == CameraType.Interior_FPS)
            {
                cameraFPS.SetActive(true);
                cameraTPS.SetActive(false);
                GameCanvas.Instance.CockpitUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
            }
            else if (cameraType == CameraType.Outdoor_TPS)
            {
                cameraFPS.SetActive(false);
                cameraTPS.SetActive(true);
                GameCanvas.Instance.CockpitUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            }
        }

        public Transform GetCamera()
        {
            if (cameraType == CameraType.Interior_FPS)
            {
                return cameraFPS.transform;
            }
            else
            {
                return cameraTPS.transform;
            }
        }
    }

    public enum ControllerType
    {
        KeyboardMouse,
        Mobile
    }

    public enum CameraType
    {
        Interior_FPS,
        Outdoor_TPS
    }
}
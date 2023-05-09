using UnityEngine;
using UnityEngine.UI;

namespace AdvancedHelicopterControllerwithShooting
{
    public class GameCanvas : MonoBehaviour
    {
        public Button button_Missile;
        public Button button_Machinegun;
        public GameObject joystick;
        public Button button_CameraChange;
        public int isFiringUpdate = 0;
        public Slider slider_Propeller;
        public Slider Slider_CurrentFuel;
        public Text Text_CurrentFuel;

        public Text Text_Ammo_Machinegun;
        public Text Text_Ammo_Missile;

        public static GameCanvas Instance;
        public GameObject GameUI;
        public GameObject CockpitUI;
        public GameObject RadarUI;
        public GameObject GasolineUI;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (HelicopterSystemManager.Instance.ShowCockpit)
            {
                CockpitUI.SetActive(true);
            }
            else
            {
                CockpitUI.SetActive(false);
            }
            if (HelicopterSystemManager.Instance.ShowRadar)
            {
                RadarUI.SetActive(true);
            }
            else
            {
                RadarUI.SetActive(false);
            }
        }

        public void UpdateMotorSlider(float EnginePower)
        {
            slider_Propeller.value = EnginePower;
        }

        public void Configure_For_Mobile()
        {
            joystick.gameObject.SetActive(true);
        }

        public void Configure_For_PCConsole()
        {
            joystick.gameObject.SetActive(false);
            button_CameraChange.GetComponentInChildren<Text>().text = "Camera (C)";
        }

        public void Click_Button_CameraSwitch()
        {
            if (button_CameraChange.IsInteractable())
            {
                if (HelicopterSystemManager.Instance.cameraFPS != null && HelicopterSystemManager.Instance.cameraFPS.activeSelf)
                {
                    HelicopterSystemManager.Instance.cameraFPS.SetActive(false);
                    HelicopterSystemManager.Instance.cameraTPS.SetActive(true);
                    CockpitUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
                }
                else if (HelicopterSystemManager.Instance.cameraTPS != null)
                {
                    HelicopterSystemManager.Instance.cameraFPS.SetActive(true);
                    HelicopterSystemManager.Instance.cameraTPS.SetActive(false);
                    CockpitUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
                }
            }
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.C) && HelicopterSystemManager.Instance.controllerType == ControllerType.KeyboardMouse)
            {
                Click_Button_CameraSwitch();
            }
        }

        public void Hide_GameUI()
        {
            GameUI.SetActive(false);
        }

        public void Propeller_Height_Update()
        {
            HelicopterController.Instance.MotorUpdate(slider_Propeller.value);
        }

        public void Click_Button_MachineGun_Down()
        {
            isFiringUpdate = 1;
        }

        public void Click_Button_Guns_Up()
        {
            isFiringUpdate = 0;
        }

        public void Click_Button_Missle_Down()
        {
            isFiringUpdate = -1;
        }
    }
}
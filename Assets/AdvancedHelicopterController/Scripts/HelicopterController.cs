using UnityEngine;

namespace AdvancedHelicopterControllerwithShooting
{
    public class HelicopterController : MonoBehaviour
    {
        [HideInInspector]
        public static HelicopterController Instance;

        public bool isVirtualJoystick = false;
        public float Health = 100;
        private float TotalHealth = 100;
        public AudioSource audioSource;
        public Rigidbody HelicopterModel;
        public HeliRotorController MainRotorController;
        public HeliRotorController SubRotorController;
        public LayerMask GroundMaskLayer = 1;
        public float TurnForce = 3f;
        public float ForwardForce = 10f;
        public float ForwardTiltForce = 20f;
        public float TurnTiltForce = 30f;
        public float EffectiveHeight = 100f;
        public float Speed = 20;
        public float turnTiltForcePercent = 1.5f;
        public float turnForcePercent = 1.3f;
        private float _engineForce;

        public ParticleSystem Particle_Motor_Left;
        public ParticleSystem Particle_Motor_Right;

        public float EngineForce
        {
            get { return _engineForce; }
            set
            {
                MainRotorController.RotarSpeed = value * 80;
                SubRotorController.RotarSpeed = value * 40;
                audioSource.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
                _engineForce = value;
            }
        }

        private float distanceToGround;
        public float DistanceToGround
        {
            get { return distanceToGround; }
        }

        private Vector3 pointToGround;
        public Vector3 PointToGround
        {
            get { return pointToGround; }
        }

        private Vector2 hMove = Vector2.zero;
        private Vector2 hTilt = Vector2.zero;
        private float hTurn = 0f;
        public bool IsOnGround = false;
        public GameObject HelicopterExplosionParticle;

        public void Awake()
        {
            Instance = this;
            TotalHealth = Health;
        }

        private void Start()
        {
            ForwardForce = Speed;
            MainRotorController = GetComponentInChildren<HeliRotorController>();
        }

        void FixedUpdate()
        {
            InputProcess();
            LiftProcess();
            MoveProcess();
            TiltProcess();
            ConsumeFuel();
        }

        void ConsumeFuel()
        {
            if (Gasoline.Instance.CurrentFuel > 0)
            {
                if (EngineForce > 5)
                {
                    Gasoline.Instance.CurrentFuel = Gasoline.Instance.CurrentFuel - EngineForce * Time.deltaTime * Gasoline.Instance.FuelConsumptionRate;
                    HelicopterModel.drag = 1;
                }
                else if (EngineForce <= 5)
                {
                    HelicopterModel.drag = 0.5f;
                }
            }
            else
            {
                EngineForce = Mathf.Lerp(EngineForce, 0, 0.025f);
                GameCanvas.Instance.UpdateMotorSlider(EngineForce);
                HelicopterModel.drag = 0;
            }
        }

        public void MotorUpdate(float power)
        {
            if (Gasoline.Instance.CurrentFuel > 0)
            {
                if (EffectiveHeight < 200)
                {
                    EffectiveHeight = power;
                }
                else
                {
                    EffectiveHeight = 200;
                }

                if (EffectiveHeight > 0)
                {
                    EffectiveHeight = power;
                }
                else
                {
                    EffectiveHeight = 0;
                }
                EngineForce = power / 10;
                Particle_Motor_Left.Play();
                Particle_Motor_Right.Play();
            }
        }

        private void MoveProcess()
        {
            var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
            hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
            HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
            HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
        }

        private void LiftProcess()
        {
            RaycastHit hit;
            var direction = transform.TransformDirection(Vector3.down);
            var ray = new Ray(transform.position, direction);
            if (Physics.Raycast(ray, out hit, 300, GroundMaskLayer))
            {
                distanceToGround = hit.distance;
                pointToGround = hit.point;
            }
            var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
            upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
            HelicopterModel.AddRelativeForce(Vector3.up * upForce);
        }

        private void TiltProcess()
        {
            hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
            hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
            HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
        }

        public void InputProcess()
        {
            if(HelicopterSystemManager.Instance.controllerType == ControllerType.KeyboardMouse)
            {
                if (!IsOnGround)
                {
                    hMove.x = Input.GetAxis("Horizontal");
                    hMove.y = Input.GetAxis("Vertical");
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    GameCanvas.Instance.slider_Propeller.value += 1f;
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    GameCanvas.Instance.slider_Propeller.value -= 2f;
                }
            }
            else
            {
                if (!IsOnGround)
                {
                    hMove.x = SimpleJoystick.Instance.HorizontalValue;
                    hMove.y = SimpleJoystick.Instance.VerticalValue;
                }
                else
                {
                    hMove.x = 0;
                    hMove.y = 0;
                }
            }
        }

        public void GetDamage(int Damage)
        {
            Health = Health - Damage;
            if (Health <= 0)
            {
                Gasoline.Instance.CurrentFuel = 0;
                GameCanvas.Instance.GasolineUI.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.CompareTag("Ground"))
            {
                ExplodeHelicopter();
            }
             if(collision.collider.CompareTag("Enemy"))
            {
                ExplodeHelicopter();
            }
            if (Gasoline.Instance.CurrentFuel == 0)
            {
                ExplodeHelicopter();
            }
        }




        void ExplodeHelicopter()
        {
            if (HelicopterSystemManager.Instance.cameraFPS.activeSelf)
            {
                HelicopterSystemManager.Instance.cameraFPS.SetActive(false);
                HelicopterSystemManager.Instance.cameraTPS.SetActive(true);
            }
            Instantiate(HelicopterExplosionParticle, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.1f);
            GameCanvas.Instance.Hide_GameUI();
        }

        private void OnCollisionExit()
        {
            IsOnGround = false;
        }
    }
}
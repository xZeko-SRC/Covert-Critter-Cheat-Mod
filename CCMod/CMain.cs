using UnityEngine;
using MelonLoader;
using System.Collections.Generic;

namespace CCMod
{
    public class CMain : MelonMod
    {
        public static GameObject _koss;
        public static bool _noclip = false;
        public static float _kossMass = 1.0f;
        private CMenu menu;
        private static Camera fpsCamera;
        private static List<Camera> originalCameras = new List<Camera>();
        public static bool fpsModeEnabled = false;
        private static List<Renderer> playerRenderers = new List<Renderer>();
        private static float standingCameraHeight = 1.8f;
        private static float crouchingCameraHeight = 1.3f;
        private static GameObject geckoBodyObject;
        private static PlayerFreeMovement freeMovementScript;

        public override void OnInitializeMelon()
        {
            menu = new GameObject("HMenu").AddComponent<CMenu>();
            MelonLogger.Msg("Mod loaded");
        }

        public override void OnUpdate()
        {
            // Handle keyboard shortcut for noclip toggle
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.N))
            {
                ToggleNoclip();
            }

            // Apply noclip behavior based on the current state
            if (_noclip)
                EnableNoclip();
            else
                DisableNoclip();

            // Toggle menu visibility with 'M' key
            if (Input.GetKeyDown(KeyCode.M))
            {
                menu.ToggleMenu();
                MelonLogger.Msg("Menu toggled");
            }
            if (fpsModeEnabled && fpsCamera != null && _koss != null)
            {
                UpdateFPSCamera();
            }
        }

        public static void ToggleNoclip()
        {
            _noclip = !_noclip;
            _koss = GameObject.Find("Game Essentials/Player");

            if (_koss == null)
            {
                MelonLogger.Error("Player object not found!");
                return;
            }

            MelonLogger.Msg($"Noclip toggled: {_noclip}");
        }

        private static void EnableNoclip()
        {
            if (_koss == null) return;

            Rigidbody rb = _koss.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.mass = 0f;
            }
            //Player _koss = GameObject.Find("Game Essentials/Player").GetComponent<PlayerWallHugMovement>();
            PlayerWallHugMovement playerCamo = GameObject.Find("Game Essentials/Player").GetComponent<PlayerWallHugMovement>();

            foreach (Collider collider in _koss.GetComponents<Collider>())
            {
                collider.enabled = false;
            }

            // Noclip movement controls
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
                _koss.transform.position += Vector3.up * 0.05f;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.JoystickButton3))
                _koss.transform.position += Vector3.down * 0.05f;

            if (Input.GetKey(KeyCode.W) || Input.GetAxis("Vertical") > 0f)
                _koss.transform.position += _koss.transform.forward * 0.05f;

            if (Input.GetKey(KeyCode.S) || Input.GetAxis("Vertical") < 0f)
                _koss.transform.position -= _koss.transform.forward * 0.05f;

            if (Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") > 0f)
                _koss.transform.position += _koss.transform.right * 0.05f;

            if (Input.GetKey(KeyCode.A) || Input.GetAxis("Horizontal") < 0f)
                _koss.transform.position -= _koss.transform.right * 0.05f;
        }

        private static void DisableNoclip()
        {
            if (_koss == null) return;

            Rigidbody rb = _koss.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.mass = _kossMass;
            }

            foreach (Collider collider in _koss.GetComponents<Collider>())
            {
                collider.enabled = true;
            }
        }

        public static void ToggleFPSCamera()
        {
            fpsModeEnabled = !fpsModeEnabled;

            if (fpsModeEnabled)
            {
                DisableAllCameras();
                EnableFPSCamera();
                HidePlayerModel();
                MelonLogger.Msg("FPS Camera Enabled");
            }
            else
            {
                EnableAllCameras();
                DisableFPSCamera();
                ShowPlayerModel();
                MelonLogger.Msg("FPS Camera Disabled");
            }
        }

        private static void DisableAllCameras()
        {
            originalCameras.Clear();
            foreach (Camera cam in Camera.allCameras)
            {
                cam.gameObject.SetActive(false);
                originalCameras.Add(cam);
            }
        }

        private static void EnableAllCameras()
        {
            foreach (Camera cam in originalCameras)
            {
                cam.gameObject.SetActive(true);
            }
        }

        private static void EnableFPSCamera()
        {
            _koss = GameObject.Find("Game Essentials/Player");
            if (_koss == null)
            {
                MelonLogger.Error("Player object not found!");
                return;
            }

            if (fpsCamera == null)
            {
                GameObject cameraObj = new GameObject("FPSCamera");
                fpsCamera = cameraObj.AddComponent<Camera>();
            }

            fpsCamera.transform.SetParent(_koss.transform);
            fpsCamera.transform.localPosition = new Vector3(0, standingCameraHeight, 0);
            fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            fpsCamera.gameObject.SetActive(true);

            // Find all renderers in the player's hierarchy
            playerRenderers.Clear();
            Renderer[] renderers = _koss.GetComponentsInChildren<Renderer>(true);
            playerRenderers.AddRange(renderers);

            if (playerRenderers.Count > 0)
            {
                MelonLogger.Msg($"Found {playerRenderers.Count} renderers in the player's hierarchy");
            }
            else
            {
                MelonLogger.Error("No renderers found in the player's hierarchy");
            }
        }

        private static void DisableFPSCamera()
        {
            if (fpsCamera != null)
            {
                fpsCamera.gameObject.SetActive(false);
            }
        }

        private static void UpdateFPSCamera()
        {
            if (_koss == null) return;

            // Check if Koss is crouching
            bool isCrouching = IsPlayerCrouching();

            // Adjust the camera height based on the crouch state
            float targetHeight = isCrouching ? crouchingCameraHeight : standingCameraHeight;
            fpsCamera.transform.localPosition = new Vector3(0, targetHeight, 0);
            fpsCamera.transform.rotation = _koss.transform.rotation;
        }

        private static bool IsPlayerCrouching()
        {
            // Replace this with the actual way to check if the player is crouching
            // Assuming your player has a component called "PlayerFreeMovement"
            PlayerFreeMovement movement = _koss.GetComponent<PlayerFreeMovement>();
            return movement != null && movement.Crouched;
        }

        private static void HidePlayerModel()
        {
            if (playerRenderers.Count > 0)
            {
                foreach (var renderer in playerRenderers)
                {
                    renderer.enabled = false;
                }
                MelonLogger.Msg("All player renderers disabled");
            }
        }

        private static void ShowPlayerModel()
        {
            if (playerRenderers.Count > 0)
            {
                foreach (var renderer in playerRenderers)
                {
                    renderer.enabled = true;
                }
                MelonLogger.Msg("All player renderers enabled");
            }
        }


        public override void OnGUI()
        {
            menu.OnGUI();
        }
    }
}

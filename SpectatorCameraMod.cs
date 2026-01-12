using BepInEx;
using UnityEngine;
using System.Reflection;

namespace SpectatorCamera
{
    [BepInPlugin("com.mayonegai.spectatorcamera", "Spectator Camera", "1.0.0")]
    public class SpectatorCameraMod : BaseUnityPlugin
    {
        private bool spectatorActive = false;
        private float moveSpeed = 15f;
        private float fastMoveSpeed = 30f;
        
        private Camera mainCamera;
        private Transform originalCameraParent;
        private GameObject playerObject;
        private Vector3 savedPlayerPos;
        private bool playerWasFrozen = false;

        void Awake()
        {
            Logger.LogInfo("Spectator Camera Mod loaded - Press F9 to toggle");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                ToggleSpectator();
            }

            if (spectatorActive)
            {
                HandleSpectatorMovement();
            }

            // Speed adjustment
            if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals))
            {
                moveSpeed += 5f;
                Logger.LogInfo($"Speed: {moveSpeed}");
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
            {
                moveSpeed = Mathf.Max(1f, moveSpeed - 5f);
                Logger.LogInfo($"Speed: {moveSpeed}");
            }
        }

        void ToggleSpectator()
        {
            spectatorActive = !spectatorActive;

            if (spectatorActive)
            {
                EnableSpectator();
            }
            else
            {
                DisableSpectator();
            }
        }

        void EnableSpectator()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Logger.LogError("Main camera not found!");
                spectatorActive = false;
                return;
            }

            // Unparent camera from player
            originalCameraParent = mainCamera.transform.parent;
            mainCamera.transform.SetParent(null);
            Logger.LogInfo($"Camera unparented from: {(originalCameraParent != null ? originalCameraParent.name : "null")}");

            // Find player
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                playerObject = GameObject.Find("Player");
            }

            if (playerObject != null)
            {
                // Save position and freeze player
                savedPlayerPos = playerObject.transform.position;
                
                // Disable all movement scripts
                var scripts = playerObject.GetComponents<MonoBehaviour>();
                foreach (var script in scripts)
                {
                    if (script != null && (script.GetType().Name.Contains("Player") || 
                        script.GetType().Name.Contains("Controller") ||
                        script.GetType().Name.Contains("Movement") ||
                        script.GetType().Name.Contains("Input")))
                    {
                        script.enabled = false;
                    }
                }
                playerWasFrozen = true;
            }

            // Unlock cursor and camera
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            
            Logger.LogInfo("Spectator mode ON - WASD to move, Mouse to look, Shift for fast, +/- for speed, F9 to exit");
        }

        void DisableSpectator()
        {
            // Reparent camera
            if (mainCamera != null && originalCameraParent != null)
            {
                mainCamera.transform.SetParent(originalCameraParent);
                Logger.LogInfo("Camera reparented");
            }

            // Re-enable player scripts
            if (playerObject != null && playerWasFrozen)
            {
                var scripts = playerObject.GetComponents<MonoBehaviour>();
                foreach (var script in scripts)
                {
                    if (script != null)
                    {
                        script.enabled = true;
                    }
                }
                playerWasFrozen = false;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Logger.LogInfo("Spectator mode OFF");
        }

        void HandleSpectatorMovement()
        {
            if (mainCamera == null) return;

            float speed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
            float actualSpeed = speed * Time.deltaTime;

            // WASD movement
            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) move += mainCamera.transform.forward;
            if (Input.GetKey(KeyCode.S)) move -= mainCamera.transform.forward;
            if (Input.GetKey(KeyCode.A)) move -= mainCamera.transform.right;
            if (Input.GetKey(KeyCode.D)) move += mainCamera.transform.right;
            if (Input.GetKey(KeyCode.Space)) move += Vector3.up;
            if (Input.GetKey(KeyCode.LeftControl)) move -= Vector3.up;

            mainCamera.transform.position += move.normalized * actualSpeed;

            // Free mouse look
            float mouseX = Input.GetAxis("Mouse X") * 2f;
            float mouseY = Input.GetAxis("Mouse Y") * 2f;

            Vector3 currentRot = mainCamera.transform.eulerAngles;
            float pitch = currentRot.x - mouseY;
            float yaw = currentRot.y + mouseX;

            // Normalize pitch to -180 to 180
            if (pitch > 180) pitch -= 360;
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            mainCamera.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        void OnDestroy()
        {
            if (spectatorActive)
            {
                DisableSpectator();
            }
        }
    }
}
using UnityEngine;
using MelonLoader;
using CCMod;
using System.Security.Cryptography;
using static Cinemachine.DocumentationSortingAttribute;

namespace CCMod
{
    public class CMenu : MonoBehaviour
    {
        private Rect _main;
        public static bool VisualVisible;
        public bool Visible = true;
        private float maxSpeed = 20f;   // Set a reasonable maximum speed
        private float minSpeed = 1f;    // Set a reasonable minimum speed
        private float speedStep = 1f;   // Amount to adjust speed per button press
        private bool healthEnabled = false;
        public GameObject _koss;

        public void ToggleMenu()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Visible = !Visible;
            }
        }

        private void Update()
        {
            ToggleMenu();
            _koss = GameObject.Find("Game Essentials/Player");
        }
        public void OnGUI()
        {
            if (!Visible)
            {
                return;
            }
            _main = GUILayout.Window(0, _main, new GUI.WindowFunction(Draw), "", GUILayout.Width(250f), GUILayout.Height(150f)); // Empty string for window title
            
        }
        public void Draw(int id)
        {
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label); titleStyle.richText = true; titleStyle.alignment = TextAnchor.UpperCenter; titleStyle.fontSize = 20;
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.label); buttonStyle.richText = true; buttonStyle.alignment = TextAnchor.UpperCenter; buttonStyle.fontSize = 15;
            GUIStyle textStyle = new GUIStyle(GUI.skin.label); textStyle.richText = true; textStyle.alignment = TextAnchor.UpperCenter; textStyle.fontSize = 13;

            GUILayout.Label("<color=#72d4d3>Covert Critter</color> <color=#FFFF00>Cheats</color>", titleStyle);
            GUILayout.Label("   Open and Close Mod Menu: M", textStyle);
            GUILayout.Label("--------------------------------------------------------");

            float buttonWidth = (_main.width - 50) / 3;  // Adjusting for spacing
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<color=#72d4d3>Speed   Up</color>", buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(40)))
            {
                PlayerFreeMovement playerMove = GameObject.Find("Game Essentials/Player").GetComponent<PlayerFreeMovement>();
                if (playerMove.walkSpeed < maxSpeed)
                {
                    playerMove.walkSpeed += speedStep;
                }
                if (playerMove.crouchSpeed < maxSpeed)
                {
                    playerMove.crouchSpeed += speedStep;
                }
                if (playerMove.runSpeed < maxSpeed)
                {
                    playerMove.runSpeed += speedStep;
                }
            }

            if (GUILayout.Button("<color=#72d4d3>Reset Speed</color>", buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(40)))
            {
                PlayerFreeMovement playerMove = GameObject.Find("Game Essentials/Player").GetComponent<PlayerFreeMovement>();
                playerMove.crouchSpeed = 1.5f;
                playerMove.walkSpeed = 2f; //Crouched 1.5 /Walk 2 Sprint is 3.5
                playerMove.runSpeed = 3.5f;
            }

            if (GUILayout.Button("<color=#72d4d3>Speed Down</color>", buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(40)))
            {
                PlayerFreeMovement playerMove = GameObject.Find("Game Essentials/Player").GetComponent<PlayerFreeMovement>();
                if (playerMove.walkSpeed > minSpeed)
                {
                    playerMove.walkSpeed -= speedStep;
                }
                if (playerMove.crouchSpeed < maxSpeed)
                {
                    playerMove.crouchSpeed -= speedStep;
                }
                if (playerMove.runSpeed < maxSpeed)
                {
                    playerMove.runSpeed -= speedStep;
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button(healthEnabled ? "<color=#FFFF00>Disable Unlimited Health</color>" : "<color=#72d4d3>Enable Unlimited Health</color>", buttonStyle))
            {
                // Find the player's health component
                HealthScript playerHealth = GameObject.Find("Game Essentials/Player/data/Capsule").GetComponent<HealthScript>();

                if (playerHealth != null)
                {
                    // Toggle healthEnabled
                    healthEnabled = !healthEnabled;

                    // Enable or disable unlimited health based on the toggle
                    playerHealth.InfiniteHP = healthEnabled;
                }
            }

            //GUILayout.Label("--------------------------------------------------------");
            if (GUILayout.Button("<color=#72d4d3>Infinite Camo Time</color>", buttonStyle))
            {
                PlayerWallHugMovement playerCamo = GameObject.Find("Game Essentials/Player").GetComponent<PlayerWallHugMovement>();
                playerCamo.camoTime = 99999;
            }

            if (GUILayout.Button("<color=#72d4d3>Give All KeyCards</color>", buttonStyle))
            {
                PlayerMoveController.Instance.AddKeyCard(Keycard.RED_KEY);
                PlayerMoveController.Instance.AddKeyCard(Keycard.GREEN_KEY);
                PlayerMoveController.Instance.AddKeyCard(Keycard.BLUE_KEY);
            }
            if (GUILayout.Button(CMain._noclip ? "<color=#FFFF00>Disable Noclip</color>" : "<color=#72d4d3>Enable Noclip</color>", buttonStyle))
            {
                CMain.ToggleNoclip();
                MelonLogger.Msg($"Noclip toggled from menu: {CMain._noclip}");
            }
            GUILayout.Label("FPS works but movement is janky", textStyle);
            if (GUILayout.Button(CMain.fpsModeEnabled ? "<color=#FFFF00>Disable FPS Camera</color>" : "<color=#72d4d3>Enable FPS Camera</color>", buttonStyle))
            {
                CMain.ToggleFPSCamera();
            }
            //GUILayout.Space(10f);
            GUILayout.Label("--------------------------------------------------------");

            if (GUILayout.Button("<color=#72d4d3>More Mod Menus</color>", buttonStyle))
            {
                Application.OpenURL("https://github.com/xZeko-SRC");
            }
            GUILayout.Label("                Created by xZeKo");
            GUI.DragWindow();
        }
    }
}

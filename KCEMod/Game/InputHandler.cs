using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Layouts;

namespace KCEMod.Game
{
    [RegisterTypeInIl2CppWithInterfaces(new Type[] { typeof(IPointerMoveHandler), typeof(IPointerUpHandler), typeof(IPointerDownHandler) })]
    public class InputHandler : MonoBehaviour
    {

        private const int NUM_LANES = 7;

        public InputHandler(IntPtr ptr) : base(ptr) { }

        private GameManager TheGameManager;

        private Gamemode1PlayerHandler GameHandler;

        private InputState[] InputStates = new InputState[NUM_LANES];
        private bool justTouched;
        private GameObject[] Lanes = new GameObject[NUM_LANES];

        public void ClearInputState()
        {
            for (int i = 0; i < NUM_LANES; i++)
            {
                InputStates[i].IsDown = false;
                InputStates[i].Dirty = false;
            }
            Melon<Core>.Logger.Msg("Input cleared");
        }

        public void ApplyInputState()
        {
            for (int i = 0; i < NUM_LANES; i++)
            {
                if (InputStates[i].Dirty)
                {
                    if (InputStates[i].IsDown)
                    {
                        GameHandler.HitLine(Lanes[i], InputStates[i].TrackedTouch);
                    }
                    else
                    {
                        GameHandler.StopLine(Lanes[i]);
                    }

                    InputStates[i].Dirty = false;
                }
            }
        }

        private const float Width = 1920;

        public void Update()
        {
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
            {
                justTouched = true;

                bool[] inputStates = new bool[7];

                foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
                {
                    Vector2 touchPos = this.GameHandler.m_mainCamera.ScreenToViewportPoint(touch.screenPosition);
                    if (touchPos.y < 0.3)
                    {
                        for (int i = 0; i < NUM_LANES; i++)
                        {
                            if (touchPos.x > Areas[i].MinX / Width && touchPos.x < Areas[i].MaxX / Width)
                            {
                                if (touch.isInProgress)
                                {
                                    inputStates[i] = true;
                                    InputStates[i].IsDown = true;
                                    InputStates[i].TrackedTouch = touch;
                                } else if (!inputStates[i])
                                {
                                    InputStates[i].IsDown = false;
                                    InputStates[i].TrackedTouch = null;
                                }
                            }
                            else if (i < NUM_LANES - 1 && touchPos.x >= Areas[i].MaxX / Width && touchPos.x <= Areas[i + 1].MinX / Width)
                            {
                                if (touch.isInProgress)
                                {
                                    inputStates[i] = true;
                                    InputStates[i].IsDown = true;
                                    InputStates[i].TrackedTouch = touch;
                                    inputStates[i + 1] = true;
                                    InputStates[i + 1].IsDown = true;
                                    InputStates[i + 1].TrackedTouch = touch;
                                }
                                else if (!inputStates[i])
                                {
                                    InputStates[i].IsDown = false;
                                    InputStates[i].TrackedTouch = null;
                                    InputStates[i + 1].IsDown = false;
                                    InputStates[i + 1].TrackedTouch = null;
                                }
                            }
                        }
                        
                    }
                }

                for (int i = 0; i < NUM_LANES; i++)
                {
                    if (!inputStates[i] && InputStates[i].IsDown)
                    {
                        InputStates[i].IsDown = false;
                        InputStates[i].TrackedTouch = null;
                    }
                }

                ApplyInputState();
            }
        }

        public void Awake()
        {
            EnhancedTouchSupport.Enable();
            Melon<Core>.Logger.Msg("Advanced Input Handler Enabled");

            GameHandler = this.gameObject.GetComponent<Gamemode1PlayerHandler>();

            GameObject colliders = GameObject.Find("Colliders");

            int[] indexes = [5, 0, 1, 2, 3, 4, 6];

            for (int i = 0; i < NUM_LANES; i++)
            {
                Lanes[i] = colliders.transform.GetChild(indexes[i]).gameObject;
            }
        }

        public void OnEnable()
        {
            
        }

        public void OnDisable()
        {

        }

        private readonly TouchArea[] Areas = [
            new TouchArea { MinX = 0, MaxX = 237, Lane = 0 },
            new TouchArea { MinX = 327, MaxX = 529, Lane = 1 },
            new TouchArea { MinX = 575, MaxX = 815, Lane = 2 },
            new TouchArea { MinX = 830, MaxX = 1086, Lane = 3 },
            new TouchArea { MinX = 1104, MaxX = 1338, Lane = 4 },
            new TouchArea { MinX = 1395, MaxX = 1594, Lane = 5 },
            new TouchArea { MinX = 1681, MaxX = 1920, Lane = 6 }
            ];

        private struct TouchArea
        {
            public int MinX, MaxX;
            public int Lane;
        }

        private struct InputState
        {
            private bool _IsDown;
            public bool IsDown { get => _IsDown; set
                {
                    if (value != _IsDown)
                    {
                        Dirty = true;
                        _IsDown = value;
                    }
                }
            }

            public bool Dirty;
            public UnityEngine.InputSystem.EnhancedTouch.Touch TrackedTouch;
        };
    }
}

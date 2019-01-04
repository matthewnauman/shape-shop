using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ShapeShop.GameEngine
{
    /// This class handles all keyboard and gamepad actions in the game.
    public static class InputManager
    {
        /// Action Enumeration
        /// The actions that are possible within the game.
        public enum Action
        {  
            StepPrevious,
            StepNext,
            ContinuousPrevious,
            ContinuousNext,
            BothPrevious,
            BothNext,

            MovePuzzleCursor,
            ShowHint,

            MoveCursorUp,
            MoveCursorDown,
            MoveCursorLeft,
            MoveCursorRight,

            RotateShape90CW,
            RotateShape90CCW,
            FlipShapeVertical,
            FlipShapeHorizontal,

            QuickShapeSelect,
            ToggleCursorMode,
//            ShowPuzzleOutline,
            ViewPuzzleStatistics,
            ViewPuzzleSetStatistics,
            ResetPuzzleShapes,

            GrabShape,
            DropSelectedShape,
            ResetSelectedShape,

            DeleteFile,

            Pause,
            Ok,
            Back,
            ExitGame,
            MenuCursorUp,
            MenuCursorDown,
            Help,

            ScrollFaster,
            ScrollSlower,

            DebugPuzzleCoordinates,
            DebugPanel,
            DebugMenuPanel,
            DebugSafeArea,
            DebugMatrixPanel,
            DebugClear,            

            Test,

            TotalActionCount,
        }


        /// Readable names of each action.
        private static readonly string[] actionNames = 
            {
                "Pause the Action",

                "Move Cursor - Up",
                "Move Cursor - Down",
                "Move Cursor - Left",
                "Move Cursor - Right",
                "Move Cursor",

                "Pick Up Piece",
                "Drop the Piece",
                "Reset Piece",

                "Main Menu",
                "Ok",
                "Back",
                "Exit Game",
                "Menu Cursor - Up",
                "Menu Cursor - Down",

                "Show Debug Puzzle Coordinates",
                "Show Debug Background Coordinates",
                "Show Debug Panel",
                "Clear Debug Information",
        
            };

        private static PlayerIndex playerIndex = PlayerIndex.One;
        public static PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        private static bool isPlayerIndexDetected = false;
        public static bool IsPlayerIndexDetected
        {
            get { return isPlayerIndexDetected; }
        }

        public static void ClearPlayerIndex()
        {
            isPlayerIndexDetected = false;
            playerIndex = PlayerIndex.One;
        }

        public static void DetectPlayerIndex()
        {
            if (!isPlayerIndexDetected)
            {
                for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
                {
                    if (GamePad.GetState(index).Buttons.A == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.B == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.X == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.Y == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.Start == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.Back == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.LeftStick == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }
                    else if (GamePad.GetState(index).Buttons.RightStick == ButtonState.Pressed)
                    {
                        playerIndex = index;
                        isPlayerIndexDetected = true;
                        break;
                    }

                }
            }
        }

        /// Returns the readable name of the given action.
        public static string GetActionName(Action action)
        {
            int index = (int)action;

            if ((index < 0) || (index > actionNames.Length))
            {
                throw new ArgumentException("action");
            }

            return actionNames[index];
        }


        // GamePad controls expressed as one type, unified with button semantics.
        public enum GamePadButtons
        {
            Start,
            Back,
            A,
            B,
            X,
            Y,

            DPadUp,
            DPadDown,
            DPadLeft,
            DPadRight,
            DPadAny,

            LeftStickUp,
            LeftStickDown,
            LeftStickLeft,
            LeftStickRight,
            LeftStickAny,

            RightStickUp,
            RightStickDown,
            RightStickLeft,
            RightStickRight,
            RightStickAny,

            LeftShoulder,
            RightShoulder,
            LeftTrigger,
            RightTrigger,
            LeftStick,
            RightStick,
        }


        // A combination of gamepad and keyboard keys mapped to a particular action.
        public class ActionMap
        {
            /// List of GamePad controls to be mapped to a given action.
            public List<GamePadButtons> gamePadButtons = new List<GamePadButtons>();

            /// List of Keyboard controls to be mapped to a given action.
            public List<Keys> keyboardKeys = new List<Keys>();
        }


        // Constants
        // The value of an analog control that reads as a "pressed button".
        const float analogLimit = 0.25f;
        const float triggerLimit = 0.0f;

        // Keyboard Data
        // The state of the keyboard as of the last update.
        private static KeyboardState currentKeyboardState;
        public static KeyboardState CurrentKeyboardState
        {
            get { return currentKeyboardState; }
        }

        // The state of the keyboard as of the previous update.
        private static KeyboardState previousKeyboardState;

        // Check if a key is pressed.
        public static bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        // Check if a key was just pressed in the most recent update.
        public static bool IsKeyTriggered(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key)) &&
                (!previousKeyboardState.IsKeyDown(key));
        }

        // GamePad Data
        // The state of the gamepad as of the last update.
        private static GamePadState currentGamePadState;
        public static GamePadState CurrentGamePadState
        {
            get { return currentGamePadState; }
        }

        // The state of the gamepad as of the previous update.
        private static GamePadState previousGamePadState;
        public static GamePadState PreviousGamePadState
        {
            get { return previousGamePadState; }
        }

        // GamePadButton Pressed Queries
        // Check if the gamepad's Start button is pressed.
        public static bool IsGamePadStartPressed()
        {
            return (currentGamePadState.Buttons.Start == ButtonState.Pressed);
        }

        // Check if the gamepad's Back button is pressed.
        public static bool IsGamePadBackPressed()
        {
            return (currentGamePadState.Buttons.Back == ButtonState.Pressed);
        }

        // Check if the gamepad's A button is pressed.
        public static bool IsGamePadAPressed()
        {
            return (currentGamePadState.Buttons.A == ButtonState.Pressed);
        }

        // Check if the gamepad's B button is pressed.
        public static bool IsGamePadBPressed()
        {
            return (currentGamePadState.Buttons.B == ButtonState.Pressed);
        }

        // Check if the gamepad's X button is pressed.
        public static bool IsGamePadXPressed()
        {
            return (currentGamePadState.Buttons.X == ButtonState.Pressed);
        }

        // Check if the gamepad's Y button is pressed.
        public static bool IsGamePadYPressed()
        {
            return (currentGamePadState.Buttons.Y == ButtonState.Pressed);
        }

        // Check if the gamepad's LeftShoulder button is pressed.
        public static bool IsGamePadLeftShoulderPressed()
        {
            return (currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed);
        }

        // Check if the gamepad's RightShoulder button is pressed.
        public static bool IsGamePadRightShoulderPressed()
        {
            return (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed);
        }

        // Check if the gamepad's LeftStick button is pressed.
        public static bool IsGamePadLeftStickPressed()
        {
            return (currentGamePadState.Buttons.LeftStick == ButtonState.Pressed);
        }

        // Check if the gamepad's RightStick button is pressed.
        public static bool IsGamePadRightStickPressed()
        {
            return (currentGamePadState.Buttons.RightStick == ButtonState.Pressed);
        }

        // Check if Up on the gamepad's directional pad is pressed.
        public static bool IsGamePadDPadUpPressed()
        {
            return (currentGamePadState.DPad.Up == ButtonState.Pressed);
        }

        // Check if Down on the gamepad's directional pad is pressed.
        public static bool IsGamePadDPadDownPressed()
        {
            return (currentGamePadState.DPad.Down == ButtonState.Pressed);
        }


        // Check if Left on the gamepad's directional pad is pressed.
        public static bool IsGamePadDPadLeftPressed()
        {
            return (currentGamePadState.DPad.Left == ButtonState.Pressed);
        }


        // Check if Right on the gamepad's directional pad is pressed.
        public static bool IsGamePadDPadRightPressed()
        {
            return (currentGamePadState.DPad.Right == ButtonState.Pressed);
        }

        // Check if the gamepad's left trigger is pressed.
        public static bool IsGamePadLeftTriggerPressed()
        {
            return (currentGamePadState.Triggers.Left > triggerLimit);
        }

        // Check if the gamepad's right trigger is pressed.
        public static bool IsGamePadRightTriggerPressed()
        {
            return (currentGamePadState.Triggers.Right > triggerLimit);
        }

        // Check if Up on the gamepad's left analog stick is pressed.
        public static bool IsGamePadLeftStickUpPressed()
        {
            return (currentGamePadState.ThumbSticks.Left.Y > analogLimit);
        }

        // Check if Down on the gamepad's left analog stick is pressed.
        public static bool IsGamePadLeftStickDownPressed()
        {
            return (-1f * currentGamePadState.ThumbSticks.Left.Y > analogLimit);
        }

        // Check if Left on the gamepad's left analog stick is pressed.
        public static bool IsGamePadLeftStickLeftPressed()
        {
            return (-1f * currentGamePadState.ThumbSticks.Left.X > analogLimit);
        }

        // Check if Right on the gamepad's left analog stick is pressed.
        public static bool IsGamePadLeftStickRightPressed()
        {
            return (currentGamePadState.ThumbSticks.Left.X > analogLimit);
        }

        public static bool IsGamePadLeftStickAnyPressed()
        {
            return (IsGamePadLeftStickUpPressed() || IsGamePadLeftStickDownPressed() ||
                    IsGamePadLeftStickLeftPressed() || IsGamePadLeftStickRightPressed());
        }

        // Check if Up on the gamepad's left analog stick is pressed.
        public static bool IsGamePadRightStickUpPressed()
        {
            return (currentGamePadState.ThumbSticks.Right.Y > analogLimit);
        }

        // Check if Down on the gamepad's left analog stick is pressed.
        public static bool IsGamePadRightStickDownPressed()
        {
            return (-1f * currentGamePadState.ThumbSticks.Right.Y > analogLimit);
        }

        // Check if Left on the gamepad's left analog stick is pressed.
        public static bool IsGamePadRightStickLeftPressed()
        {
            return (-1f * currentGamePadState.ThumbSticks.Right.X > analogLimit);
        }

        // Check if Right on the gamepad's left analog stick is pressed.
        public static bool IsGamePadRightStickRightPressed()
        {
            return (currentGamePadState.ThumbSticks.Right.X > analogLimit);
        }

        public static bool IsGamePadRightStickAnyPressed()
        {
            return (IsGamePadRightStickUpPressed() || IsGamePadRightStickDownPressed() ||
                    IsGamePadRightStickLeftPressed() || IsGamePadRightStickRightPressed());
        }

        public static bool IsGamePadDPadAnyDirectionPressed()
        {
            return (IsGamePadDPadUpPressed() || IsGamePadDPadDownPressed() ||
                    IsGamePadDPadRightPressed() || IsGamePadDPadLeftPressed());
        }

        // Check if the GamePadKey value specified is pressed.
        private static bool IsGamePadButtonPressed(GamePadButtons gamePadKey)
        {
            switch (gamePadKey)
            {
                case GamePadButtons.Start:
                    return IsGamePadStartPressed();

                case GamePadButtons.Back:
                    return IsGamePadBackPressed();

                case GamePadButtons.A:
                    return IsGamePadAPressed();

                case GamePadButtons.B:
                    return IsGamePadBPressed();

                case GamePadButtons.X:
                    return IsGamePadXPressed();

                case GamePadButtons.Y:
                    return IsGamePadYPressed();

                case GamePadButtons.LeftShoulder:
                    return IsGamePadLeftShoulderPressed();

                case GamePadButtons.RightShoulder:
                    return IsGamePadRightShoulderPressed();

                case GamePadButtons.LeftStick:
                    return IsGamePadLeftStickPressed();

                case GamePadButtons.RightStick:
                    return IsGamePadRightStickPressed();

                case GamePadButtons.LeftTrigger:
                    return IsGamePadLeftTriggerPressed();

                case GamePadButtons.RightTrigger:
                    return IsGamePadRightTriggerPressed();

                case GamePadButtons.DPadUp:
                    return IsGamePadDPadUpPressed();

                case GamePadButtons.DPadDown:
                    return IsGamePadDPadDownPressed();

                case GamePadButtons.DPadLeft:
                    return IsGamePadDPadLeftPressed();

                case GamePadButtons.DPadRight:
                    return IsGamePadDPadRightPressed();

                case GamePadButtons.DPadAny:
                    return IsGamePadDPadAnyDirectionPressed();

                case GamePadButtons.LeftStickUp:
                    return IsGamePadLeftStickUpPressed();

                case GamePadButtons.LeftStickDown:
                    return IsGamePadLeftStickDownPressed();

                case GamePadButtons.LeftStickLeft:
                    return IsGamePadLeftStickLeftPressed();

                case GamePadButtons.LeftStickRight:
                    return IsGamePadLeftStickRightPressed();

                case GamePadButtons.LeftStickAny:
                    return IsGamePadLeftStickAnyPressed();

                case GamePadButtons.RightStickUp:
                    return IsGamePadRightStickUpPressed();

                case GamePadButtons.RightStickDown:
                    return IsGamePadRightStickDownPressed();

                case GamePadButtons.RightStickLeft:
                    return IsGamePadRightStickLeftPressed();

                case GamePadButtons.RightStickRight:
                    return IsGamePadRightStickRightPressed();

                case GamePadButtons.RightStickAny:
                    return IsGamePadRightStickAnyPressed();

            }

            return false;
        }


        // GamePadButton Triggered Queries
        // Check if the gamepad's Start button was just pressed.
        public static bool IsGamePadStartTriggered()
        {
            return ((currentGamePadState.Buttons.Start == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.Start == ButtonState.Released));
        }

        // Check if the gamepad's Back button was just pressed.
        public static bool IsGamePadBackTriggered()
        {
            return ((currentGamePadState.Buttons.Back == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.Back == ButtonState.Released));
        }

        // Check if the gamepad's A button was just pressed.
        public static bool IsGamePadATriggered()
        {
            return ((currentGamePadState.Buttons.A == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.A == ButtonState.Released));
        }

        // Check if the gamepad's B button was just pressed.
        public static bool IsGamePadBTriggered()
        {
            return ((currentGamePadState.Buttons.B == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.B == ButtonState.Released));
        }

        // Check if the gamepad's X button was just pressed.
        public static bool IsGamePadXTriggered()
        {
            return ((currentGamePadState.Buttons.X == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.X == ButtonState.Released));
        }

        // Check if the gamepad's Y button was just pressed.
        public static bool IsGamePadYTriggered()
        {
            return ((currentGamePadState.Buttons.Y == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.Y == ButtonState.Released));
        }

        // Check if the gamepad's LeftShoulder button was just pressed.
        public static bool IsGamePadLeftShoulderTriggered()
        {
            return ((currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.LeftShoulder == ButtonState.Released));
        }

        // Check if the gamepad's RightShoulder button was just pressed.
        public static bool IsGamePadRightShoulderTriggered()
        {
            return ((currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.RightShoulder == ButtonState.Released));
        }

        // Check if the gamepad's LeftShoulder button was just pressed.
        public static bool IsGamePadLeftStickTriggered()
        {
            return ((currentGamePadState.Buttons.LeftStick == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.LeftStick == ButtonState.Released));
        }

        // Check if the gamepad's RightShoulder button was just pressed.
        public static bool IsGamePadRightStickTriggered()
        {
            return ((currentGamePadState.Buttons.RightStick == ButtonState.Pressed) &&
                    (previousGamePadState.Buttons.RightStick == ButtonState.Released));
        }

        // Check if Up on the gamepad's directional pad was just pressed.
        public static bool IsGamePadDPadUpTriggered()
        {
            return ((currentGamePadState.DPad.Up == ButtonState.Pressed) &&
                    (previousGamePadState.DPad.Up == ButtonState.Released));
        }

        // Check if Down on the gamepad's directional pad was just pressed.
        public static bool IsGamePadDPadDownTriggered()
        {
            return ((currentGamePadState.DPad.Down == ButtonState.Pressed) &&
                    (previousGamePadState.DPad.Down == ButtonState.Released));
        }

        // Check if Left on the gamepad's directional pad was just pressed.
        public static bool IsGamePadDPadLeftTriggered()
        {
            return ((currentGamePadState.DPad.Left == ButtonState.Pressed) &&
                    (previousGamePadState.DPad.Left == ButtonState.Released));
        }

        // Check if Right on the gamepad's directional pad was just pressed.
        public static bool IsGamePadDPadRightTriggered()
        {
            return ((currentGamePadState.DPad.Right == ButtonState.Pressed) &&
                    (previousGamePadState.DPad.Right == ButtonState.Released));
        }

        // Check if the gamepad's left trigger was just pressed.
        public static bool IsGamePadLeftTriggerTriggered()
        {
            return ((currentGamePadState.Triggers.Left > analogLimit) &&
                    (previousGamePadState.Triggers.Left < analogLimit));
        }

        // Check if the gamepad's right trigger was just pressed.
        public static bool IsGamePadRightTriggerTriggered()
        {
            return ((currentGamePadState.Triggers.Right > analogLimit) &&
                    (previousGamePadState.Triggers.Right < analogLimit));
        }

        // Check if Up on the gamepad's left analog stick was just pressed.
        public static bool IsGamePadLeftStickUpTriggered()
        {
            return ((currentGamePadState.ThumbSticks.Left.Y > analogLimit) &&
                    (previousGamePadState.ThumbSticks.Left.Y < analogLimit));
        }

        // Check if Down on the gamepad's left analog stick was just pressed.
        public static bool IsGamePadLeftStickDownTriggered()
        {
            return ((-1f * currentGamePadState.ThumbSticks.Left.Y > analogLimit) &&
                    (-1f * previousGamePadState.ThumbSticks.Left.Y < analogLimit));
        }

        // Check if Left on the gamepad's left analog stick was just pressed.
        public static bool IsGamePadLeftStickLeftTriggered()
        {
            return ((-1f * currentGamePadState.ThumbSticks.Left.X > analogLimit) &&
                    (-1f * previousGamePadState.ThumbSticks.Left.X < analogLimit));
        }

        // Check if Right on the gamepad's left analog stick was just pressed.
        public static bool IsGamePadLeftStickRightTriggered()
        {
            return ((currentGamePadState.ThumbSticks.Left.X > analogLimit) &&
                    (previousGamePadState.ThumbSticks.Left.X < analogLimit));
        }

        public static bool IsGamePadLeftStickAnyTriggered()
        {
            return (IsGamePadLeftStickUpTriggered() || IsGamePadLeftStickDownTriggered() ||
                    IsGamePadLeftStickLeftTriggered() || IsGamePadLeftStickRightTriggered());
        }

        // Check if Up on the gamepad's left analog stick was just pressed.
        public static bool IsGamePadRightStickUpTriggered()
        {
            return ((currentGamePadState.ThumbSticks.Right.Y > analogLimit) &&
                    (previousGamePadState.ThumbSticks.Right.Y < analogLimit));
        }

        // Check if Down on the gamepad's left analog stick was just pressed.
        public static bool IsGamePadRightStickDownTriggered()
        {
            return ((-1f * currentGamePadState.ThumbSticks.Right.Y > analogLimit) &&
                    (-1f * previousGamePadState.ThumbSticks.Right.Y < analogLimit));
        }

        // Check if Left on the gamepad's left analog stick was just pressed.
        public static bool IsGamePadRightStickLeftTriggered()
        {
            return ((-1f * currentGamePadState.ThumbSticks.Right.X > analogLimit) &&
                    (-1f * previousGamePadState.ThumbSticks.Right.X < analogLimit));
        }

        // Check if Right on the gamepad's left analog stick was just pressed.
        public static bool IsGamePadRightStickRightTriggered()
        {
            return ((currentGamePadState.ThumbSticks.Right.X > analogLimit) &&
                    (previousGamePadState.ThumbSticks.Right.X < analogLimit));
        }

        public static bool IsGamePadRightStickAnyTriggered()
        {
            return (IsGamePadRightStickUpTriggered() || IsGamePadRightStickDownTriggered() ||
                    IsGamePadRightStickLeftTriggered() || IsGamePadRightStickRightTriggered());
        }

        // Check if the GamePadKey value specified was pressed this frame.
        private static bool IsGamePadButtonTriggered(GamePadButtons gamePadKey)
        {
            switch (gamePadKey)
            {
                case GamePadButtons.Start:
                    return IsGamePadStartTriggered();

                case GamePadButtons.Back:
                    return IsGamePadBackTriggered();

                case GamePadButtons.A:
                    return IsGamePadATriggered();

                case GamePadButtons.B:
                    return IsGamePadBTriggered();

                case GamePadButtons.X:
                    return IsGamePadXTriggered();

                case GamePadButtons.Y:
                    return IsGamePadYTriggered();

                case GamePadButtons.LeftShoulder:
                    return IsGamePadLeftShoulderTriggered();

                case GamePadButtons.RightShoulder:
                    return IsGamePadRightShoulderTriggered();

                case GamePadButtons.LeftStick:
                    return IsGamePadLeftStickTriggered();

                case GamePadButtons.RightStick:
                    return IsGamePadRightStickTriggered();

                case GamePadButtons.LeftTrigger:
                    return IsGamePadLeftTriggerTriggered();

                case GamePadButtons.RightTrigger:
                    return IsGamePadRightTriggerTriggered();

                case GamePadButtons.DPadUp:
                    return IsGamePadDPadUpTriggered();

                case GamePadButtons.DPadDown:
                    return IsGamePadDPadDownTriggered();

                case GamePadButtons.DPadLeft:
                    return IsGamePadDPadLeftTriggered();

                case GamePadButtons.DPadRight:
                    return IsGamePadDPadRightTriggered();

                case GamePadButtons.LeftStickUp:
                    return IsGamePadLeftStickUpTriggered();

                case GamePadButtons.LeftStickDown:
                    return IsGamePadLeftStickDownTriggered();

                case GamePadButtons.LeftStickLeft:
                    return IsGamePadLeftStickLeftTriggered();

                case GamePadButtons.LeftStickRight:
                    return IsGamePadLeftStickRightTriggered();

                case GamePadButtons.LeftStickAny:
                    return IsGamePadLeftStickAnyTriggered();

                case GamePadButtons.RightStickUp:
                    return IsGamePadRightStickUpTriggered();

                case GamePadButtons.RightStickDown:
                    return IsGamePadRightStickDownTriggered();

                case GamePadButtons.RightStickLeft:
                    return IsGamePadRightStickLeftTriggered();

                case GamePadButtons.RightStickRight:
                    return IsGamePadRightStickRightTriggered();

                case GamePadButtons.RightStickAny:
                    return IsGamePadRightStickAnyTriggered();
            }

            return false;
        }


        /// Action Mapping

        /// The action mappings for the game.
        private static ActionMap[] actionMaps;
        public static ActionMap[] ActionMaps
        {
            get { return actionMaps; }
        }


        /// Reset the action maps to their default values.
        private static void ResetActionMaps()
        {
            //            actionMaps = new ActionMap[(int)(int)Action.TotalActionCount];

            actionMaps = new ActionMap[(int)Action.TotalActionCount];

            // debug actions
            actionMaps[(int)Action.Test] = new ActionMap();
            actionMaps[(int)Action.Test].keyboardKeys.Add(Keys.T);

            actionMaps[(int)Action.DebugPanel] = new ActionMap();
            actionMaps[(int)Action.DebugPanel].keyboardKeys.Add(Keys.F1);

            actionMaps[(int)Action.DebugMenuPanel] = new ActionMap();
            actionMaps[(int)Action.DebugMenuPanel].keyboardKeys.Add(Keys.F2);

            actionMaps[(int)Action.DebugPuzzleCoordinates] = new ActionMap();
            actionMaps[(int)Action.DebugPuzzleCoordinates].keyboardKeys.Add(Keys.F3);

            actionMaps[(int)Action.DebugSafeArea] = new ActionMap();
            actionMaps[(int)Action.DebugSafeArea].keyboardKeys.Add(Keys.F4);

            actionMaps[(int)Action.DebugMatrixPanel] = new ActionMap();
            actionMaps[(int)Action.DebugMatrixPanel].keyboardKeys.Add(Keys.F5);

            actionMaps[(int)Action.DebugClear] = new ActionMap();
            actionMaps[(int)Action.DebugClear].keyboardKeys.Add(Keys.Escape);            




            // puzzle picker control
            actionMaps[(int)Action.StepPrevious] = new ActionMap();
            actionMaps[(int)Action.StepPrevious].gamePadButtons.Add(GamePadButtons.LeftShoulder);

            actionMaps[(int)Action.StepNext] = new ActionMap();
            actionMaps[(int)Action.StepNext].gamePadButtons.Add(GamePadButtons.RightShoulder);

            // puzzle picker control
            actionMaps[(int)Action.ContinuousPrevious] = new ActionMap();
            actionMaps[(int)Action.ContinuousPrevious].gamePadButtons.Add(GamePadButtons.LeftTrigger);

            actionMaps[(int)Action.ContinuousNext] = new ActionMap();
            actionMaps[(int)Action.ContinuousNext].gamePadButtons.Add(GamePadButtons.RightTrigger);

            // puzzle picker control
            actionMaps[(int)Action.BothPrevious] = new ActionMap();
            actionMaps[(int)Action.BothPrevious].gamePadButtons.Add(GamePadButtons.LeftShoulder);
            actionMaps[(int)Action.BothPrevious].gamePadButtons.Add(GamePadButtons.LeftTrigger);

            actionMaps[(int)Action.BothNext] = new ActionMap();
            actionMaps[(int)Action.BothNext].gamePadButtons.Add(GamePadButtons.RightShoulder);
            actionMaps[(int)Action.BothNext].gamePadButtons.Add(GamePadButtons.RightTrigger);

            actionMaps[(int)Action.ViewPuzzleStatistics] = new ActionMap();
            actionMaps[(int)Action.ViewPuzzleStatistics].gamePadButtons.Add(GamePadButtons.X);

            actionMaps[(int)Action.ToggleCursorMode] = new ActionMap();
            actionMaps[(int)Action.ToggleCursorMode].gamePadButtons.Add(GamePadButtons.X);

            actionMaps[(int)Action.ViewPuzzleSetStatistics] = new ActionMap();
            actionMaps[(int)Action.ViewPuzzleSetStatistics].gamePadButtons.Add(GamePadButtons.Y);

            actionMaps[(int)Action.ShowHint] = new ActionMap();
            actionMaps[(int)Action.ShowHint].gamePadButtons.Add(GamePadButtons.Y);

            actionMaps[(int)Action.QuickShapeSelect] = new ActionMap();
            actionMaps[(int)Action.QuickShapeSelect].gamePadButtons.Add(GamePadButtons.LeftStickAny);

            actionMaps[(int)Action.ResetPuzzleShapes] = new ActionMap();
            actionMaps[(int)Action.ResetPuzzleShapes].gamePadButtons.Add(GamePadButtons.RightStick);

            actionMaps[(int)Action.GrabShape] = new ActionMap();
            actionMaps[(int)Action.GrabShape].gamePadButtons.Add(GamePadButtons.A);

            actionMaps[(int)Action.DropSelectedShape] = new ActionMap();
            actionMaps[(int)Action.DropSelectedShape].gamePadButtons.Add(GamePadButtons.A);

            actionMaps[(int)Action.ResetSelectedShape] = new ActionMap();
            actionMaps[(int)Action.ResetSelectedShape].gamePadButtons.Add(GamePadButtons.B);

            actionMaps[(int)Action.MovePuzzleCursor] = new ActionMap();
            actionMaps[(int)Action.MovePuzzleCursor].gamePadButtons.Add(GamePadButtons.LeftStickAny);

            actionMaps[(int)Action.MoveCursorUp] = new ActionMap();
            actionMaps[(int)Action.MoveCursorUp].gamePadButtons.Add(GamePadButtons.DPadUp);

            actionMaps[(int)Action.MoveCursorDown] = new ActionMap();
            actionMaps[(int)Action.MoveCursorDown].gamePadButtons.Add(GamePadButtons.DPadDown);

            actionMaps[(int)Action.MoveCursorLeft] = new ActionMap();
            actionMaps[(int)Action.MoveCursorLeft].gamePadButtons.Add(GamePadButtons.DPadLeft);

            actionMaps[(int)Action.MoveCursorRight] = new ActionMap();
            actionMaps[(int)Action.MoveCursorRight].gamePadButtons.Add(GamePadButtons.DPadRight);
            
            actionMaps[(int)Action.RotateShape90CCW] = new ActionMap();
            actionMaps[(int)Action.RotateShape90CCW].gamePadButtons.Add(GamePadButtons.LeftTrigger);
            actionMaps[(int)Action.RotateShape90CCW].gamePadButtons.Add(GamePadButtons.RightStickLeft);

            actionMaps[(int)Action.RotateShape90CW] = new ActionMap();
            actionMaps[(int)Action.RotateShape90CW].gamePadButtons.Add(GamePadButtons.RightTrigger);
            actionMaps[(int)Action.RotateShape90CW].gamePadButtons.Add(GamePadButtons.RightStickRight);
            
            actionMaps[(int)Action.FlipShapeHorizontal] = new ActionMap();
            actionMaps[(int)Action.FlipShapeHorizontal].gamePadButtons.Add(GamePadButtons.RightShoulder);
            actionMaps[(int)Action.FlipShapeHorizontal].gamePadButtons.Add(GamePadButtons.RightStickDown);

            actionMaps[(int)Action.FlipShapeVertical] = new ActionMap();
            actionMaps[(int)Action.FlipShapeVertical].gamePadButtons.Add(GamePadButtons.LeftShoulder);
            actionMaps[(int)Action.FlipShapeVertical].gamePadButtons.Add(GamePadButtons.RightStickUp);

            actionMaps[(int)Action.DeleteFile] = new ActionMap();
            actionMaps[(int)Action.DeleteFile].gamePadButtons.Add(GamePadButtons.X);

            actionMaps[(int)Action.Pause] = new ActionMap();
//            actionMaps[(int)Action.Pause].gamePadButtons.Add(GamePadButtons.Start);

            actionMaps[(int)Action.Help] = new ActionMap();
            actionMaps[(int)Action.Help].gamePadButtons.Add(GamePadButtons.Start);
//            actionMaps[(int)Action.Help].gamePadButtons.Add(GamePadButtons.Back);

            actionMaps[(int)Action.Ok] = new ActionMap();
            actionMaps[(int)Action.Ok].gamePadButtons.Add(GamePadButtons.A);

            actionMaps[(int)Action.Back] = new ActionMap();
            actionMaps[(int)Action.Back].gamePadButtons.Add(GamePadButtons.B);

            actionMaps[(int)Action.MenuCursorUp] = new ActionMap(); 
            actionMaps[(int)Action.MenuCursorUp].gamePadButtons.Add(GamePadButtons.DPadUp);
            actionMaps[(int)Action.MenuCursorUp].gamePadButtons.Add(GamePadButtons.LeftStickUp);

            actionMaps[(int)Action.MenuCursorDown] = new ActionMap();
            actionMaps[(int)Action.MenuCursorDown].gamePadButtons.Add(GamePadButtons.DPadDown);
            actionMaps[(int)Action.MenuCursorDown].gamePadButtons.Add(GamePadButtons.LeftStickDown);

            actionMaps[(int)Action.ScrollFaster] = new ActionMap();
            actionMaps[(int)Action.ScrollFaster].gamePadButtons.Add(GamePadButtons.RightTrigger);

            actionMaps[(int)Action.ScrollSlower] = new ActionMap();
            actionMaps[(int)Action.ScrollSlower].gamePadButtons.Add(GamePadButtons.LeftTrigger);

            actionMaps[(int)Action.ExitGame] = new ActionMap();
            actionMaps[(int)Action.ExitGame].gamePadButtons.Add(GamePadButtons.B);

        }


        /// Check if an action has been pressed.
        public static bool IsActionPressed(Action action)
        {
            // for command log debugging
            if (IsActionMapPressed(actionMaps[(int)action]))
            {
//                Debug.WriteLine(":::::: [" + DateTime.Now.TimeOfDay + "] PRESSED: " + action.ToString());
                return true;
            }
            return false;
        }

        /// Check if an action was just performed in the most recent update.
        public static bool IsActionTriggered(Action action)
        {
            // for command log debugging
            if (IsActionMapTriggered(actionMaps[(int)action]))
            {
//                Debug.WriteLine(":::::: [" + DateTime.Now.TimeOfDay + "] TRIGGGERED: " + action.ToString());
                return true;
            }
            return false;
        }

        /// Check if an action map has been pressed.
        private static bool IsActionMapPressed(ActionMap actionMap)
        {
            for (int i = 0; i < actionMap.keyboardKeys.Count; i++)
            {
                if (IsKeyPressed(actionMap.keyboardKeys[i]))
                {
                    return true;
                }
            }
            if (currentGamePadState.IsConnected)
            {
                for (int i = 0; i < actionMap.gamePadButtons.Count; i++)
                {
                    if (IsGamePadButtonPressed(actionMap.gamePadButtons[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// Check if an action map has been triggered this frame.
        private static bool IsActionMapTriggered(ActionMap actionMap)
        {
            for (int i = 0; i < actionMap.keyboardKeys.Count; i++)
            {
                if (IsKeyTriggered(actionMap.keyboardKeys[i]))
                {
                    return true;
                }
            }
            if (currentGamePadState.IsConnected)
            {
                for (int i = 0; i < actionMap.gamePadButtons.Count; i++)
                {
                    if (IsGamePadButtonTriggered(actionMap.gamePadButtons[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// Initialization
        /// Initializes the default control keys for all actions.
        public static void Initialize()
        {
            ResetActionMaps();
        }

        /// Updating        
        /// Updates the keyboard and gamepad control states.
        public static void Update()
        {
            // update the keyboard state
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            // update the gamepad state
            previousGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(playerIndex);
        }

        public static float CalculateAngle(Vector2 v)
        {
            float angle = 0.0f;
            if (v != Vector2.Zero)
            {
                v.Normalize();
                angle = (float)Math.Acos(v.Y);
                if (v.X < 0.0f)
                    angle = -angle;
            }
            return angle;
        }  


    }
}

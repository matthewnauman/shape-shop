using Microsoft.Xna.Framework;
using ShapeShop.GameEngine;
using System;

namespace ShapeShop.UI
{
    /// Enum describes the screen transition state.
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
        FinishedExiting,
    }

    /// A screen is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as screens.
    public abstract class GameScreen
    {
        // Properties
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        private bool isPopup = false;
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        private TimeSpan transitionOnTime = TimeSpan.Zero;
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        TimeSpan transitionOffTime = TimeSpan.Zero;
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        float transitionPosition = 1;
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        public Color TransitionColor
        {
            get { return new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha, TransitionAlpha); }
        }

        /// Gets the current alpha of the screen transition, ranging
        /// from 255 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        public byte TransitionAlpha
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }


        /// Gets the current screen transition state.
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenState screenState = ScreenState.TransitionOn;


        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set
            {
                bool fireEvent = !isExiting && value;
                isExiting = value;
                if (fireEvent && (Exiting != null))
                {
                    Exiting(this, EventArgs.Empty);
                }
            }
        }

        bool isExiting = false;


        /// Checks whether this screen is active and can respond to user input.
        public bool IsActive
        {
            get
            {
                return (!otherScreenHasFocus && screenState == ScreenState.Active);
            }
        }

        bool otherScreenHasFocus;


        /// Gets the manager that this screen belongs to.
        private ScreenManager screenManager;
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        public event EventHandler Exiting;

        // Initialization
        /// Load graphics content for the screen.
        public virtual void LoadContent() { }

        /// Unload content for the screen.
        public virtual void UnloadContent() { }


        // Update and Draw
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen)
        {
            if (screenState == ScreenState.FinishedExiting)
            {
                ScreenManager.RemoveScreen(this);
            }

            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    screenState = ScreenState.FinishedExiting;
//                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Active;
                }
            }
        }

        /// Helper for updating the screen transition position.
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if ((transitionPosition <= 0) || (transitionPosition >= 1))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        public virtual void HandleInput() { }


        /// This is called when the screen should draw itself.
        public virtual void Draw(GameTime gameTime) { }

        // Public Methods
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        public virtual void ExitScreen()
        {
            // flag that it should transition off and then exit.
            IsExiting = true;
            // If the screen has a zero transition time, remove it immediately.
            if (TransitionOffTime == TimeSpan.Zero)
            {
                ScreenManager.RemoveScreen(this);
            }
        }

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.UI;
using System.Collections.Generic;

namespace ShapeShop.GameEngine
{
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    public class ScreenManager : DrawableGameComponent
    {
        // Fields

        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        private ContentManager globalContent;
        public ContentManager GlobalContent
        {
            get { return globalContent; }
        }

        private ContentManager sessionContent;
        public ContentManager SessionContent
        {
            get { return sessionContent; }
        }

        SpriteBatch spriteBatch;

        private Texture2D blankTexture;
        public Texture2D BlankTexture
        {
            get { return blankTexture; }
        }

        private Texture2D transTexture;
        public Texture2D TransTexture
        {
            get { return transTexture; }
        }

        private Texture2D frameTexture;
        public Texture2D FrameTexture
        {
            get { return frameTexture; }
        }

        private Texture2D selectTexture;
        public Texture2D SelectTexture
        {
            get { return selectTexture; }
        }

        bool isInitialized;
        bool traceEnabled;


        // Properties

        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }


        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }


        // Initialization

        /// Constructs a new screen manager component.
        public ScreenManager(Game game) : base(game)
        {
        }


        /// Initializes the screen manager component.
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }


        /// Load your graphics content.
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            globalContent = Game.Content;
            sessionContent = new ContentManager(Game.Content.ServiceProvider, Game.Content.RootDirectory);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            blankTexture = globalContent.Load<Texture2D>(@"Textures\1x1");
            transTexture = globalContent.Load<Texture2D>(@"Textures\1x1-trans");
            selectTexture = globalContent.Load<Texture2D>(@"Textures\Buttons\AButton");
            frameTexture = globalContent.Load<Texture2D>(@"Textures\Debug\DebugTile");
            
            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }


        /// Unload your graphics content.
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }


        // Update and Draw

        /// Allows each screen to run logic.
        public override void Update(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput();

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (traceEnabled)
                TraceScreens();
        }


        /// Prints a list of all the screens, for debugging.
        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
                screenNames.Add(screen.GetType().Name);
        }


        /// Tells each screen to draw itself.
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }


        // Public Methods

        /// Adds a new screen to the screen manager.
        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);
        }


        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);            
        }


        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        public void FadeBackBufferToBlack(int alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT),
                             new Color((byte)0, (byte)0, (byte)0, (byte)alpha));

            spriteBatch.End();
        }
        

    }
}

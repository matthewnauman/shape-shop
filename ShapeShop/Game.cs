using Microsoft.Xna.Framework;
using ShapeShop.GameEngine;
using ShapeShop.UI;
using Windows.UI.ViewManagement;

namespace ShapeShop
{
    public class ShapeShop : Game
    {
        public static readonly int PREFERRED_WIDTH = 1280;
        public static readonly int PREFERRED_HEIGHT = 720;

        //Fields
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        
        // Initialization
        /// The main game constructor.
        public ShapeShop()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = PREFERRED_WIDTH;
            graphics.PreferredBackBufferHeight = PREFERRED_HEIGHT;

            // add the audio manager
            AudioManager.Initialize(this, @"Content/ShapeShopAudio.xgs", @"Content/Wave Bank.xwb", @"Content/Sound Bank.xsb");

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
//            Components.Add(new DebugTools(this));
        }

        /// Allows the game to perform any initialization it needs to 
        /// before starting to run.  This is where it can query for any required 
        /// services and load any non-graphic related content.  Calling base.Initialize 
        /// will enumerate through any components and initialize them as well.
        protected override void Initialize()
        {
            InputManager.Initialize();

            base.Initialize();

            PuzzleEngine.Viewport = graphics.GraphicsDevice.Viewport;

            screenManager.AddScreen(new SplashScreen());
        }

        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        protected override void LoadContent()
        {
            Fonts.LoadContent(Content);

            base.LoadContent();
        }

        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        protected override void UnloadContent()
        {
            Fonts.UnloadContent();

            base.UnloadContent();
        }


        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputManager.Update();

            base.Update(gameTime);
        }

        /*
        // Draw
        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
        */

    }


    // Entry Point

    /// The main entry point for the application.
    public static class Program
    {
        static void Main()
        {
            // set window size
            ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            var factory = new MonoGame.Framework.GameFrameworkViewSource<ShapeShop>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);

        }
    }

}

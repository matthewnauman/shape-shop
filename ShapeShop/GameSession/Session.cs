using Microsoft.Xna.Framework.Content;
using ShapeShop.GameEngine;
using ShapeShop.UI;
using ShapeShopData.Models;
using System;
using System.IO;

namespace ShapeShop.GameSession
{
    class Session
    {       
        /// The single Session instance that can be active at a time.
        private static Session singleton;

        /// Player
        private Player player;
        public static Player Player
        {
            get { return singleton?.player; }
        }

        /// The ScreenManager used to manage all UI in the game.
        private ScreenManager screenManager;
        public static ScreenManager ScreenManager
        {
            get { return singleton?.screenManager; }
        }

        /// Returns true if there is an active session.
        public static bool IsActive
        {
            get { return singleton != null; }
        }

        public static void SetPlayer(Player player)
        {
            singleton.player = player;
        }

//        private static bool signedIn = false;
//        private static XboxLiveContext xboxLiveContext;

        /// Initialization
        /// Private constructor of a Session object.
        /// The lack of public constructors forces the singleton model.
        private Session(ScreenManager screenManager)
        {
            // assign the parameter
            this.screenManager = screenManager ?? throw new ArgumentNullException("screenManager");    
        }

        /// Starting a New Session
        /// Start a new session based on the data provided.
        public static void StartNewSession(GameStartDescription gameStartDescription, ScreenManager screenManager, MainGameScreen introScreen)
        {
            // check the parameters
            if (gameStartDescription == null)
            {
                throw new ArgumentNullException("gameStartDescripton");
            }
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (introScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // end any existing session
            EndSession();

            // create a new singleton
            singleton = new Session(screenManager);

            ContentManager sessionContent = screenManager.SessionContent;

            singleton.player = new Player(gameStartDescription.SaveSlotNumber);

            PuzzleSet puzzleSet = sessionContent.Load<PuzzleSet>(Path.Combine(@"PuzzleSets", gameStartDescription.PuzzleSetContentName)).Clone() as PuzzleSet;
            puzzleSet.LoadContent(sessionContent);

            puzzleSet.ResetPuzzleLocks();

            PuzzleEngine.LoadContent(sessionContent, puzzleSet);
            PuzzleEngine.MainScreen.CreditsPanel.LoadContent(sessionContent);

            foreach (Puzzle puzzle in puzzleSet.Puzzles)
            {
                puzzle.IsRenderPortrait = true;
            }
            PuzzleEngine.IsCheckRender = true;

//            gamer = SignedInGamer.SignedInGamers[InputManager.PlayerIndex];
        }


        /// Ending a Session
        /// End the current session.
        public static void EndSession()
        {
            // exit the gameplay screen
            // -- store the gameplay session, for re-entrance
            if (singleton != null)
            {
                AudioManager.PopMusic();

                // clear the singleton
                singleton.player = null;
                singleton.screenManager.SessionContent.Unload();
                singleton = null;
            }
        }

        public static void QuickSave()
        {
            if (PuzzleEngine.MainScreen.SlotPanel.CurrentSlot.SaveGameDescription == null)
            {
                SaveGameDescription sgd = new SaveGameDescription();
                sgd.SaveSlot = PuzzleEngine.MainScreen.SlotPanel.CurrentSlotIdx + 1;
                SaveSession(sgd);
            }
            else
            {
                SaveSession(PuzzleEngine.MainScreen.SlotPanel.CurrentSlot.SaveGameDescription);
            }
        }

        // session load/save stuff
        /// Start a new session, using the data in the given save game.
        /// <param name="saveGameDescription">The description of the save game.</param>
        /// <param name="screenManager">The ScreenManager for the new session.</param>
        public static void LoadSession(SaveGameDescription saveGameDescription, ScreenManager screenManager, MainGameScreen introScreen)
        {
            // check the parameters
            if (saveGameDescription == null)
            {
                throw new ArgumentNullException("saveGameDescription");
            }
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (introScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // end any existing session
            EndSession();

            // create the new session
            singleton = new Session(screenManager);

            PuzzleEngine.MainScreen.CreditsPanel.LoadContent(screenManager.SessionContent);
            StorageManager.Instance.LoadSession(saveGameDescription.FileName);
        }

        /// Save the current state of the session.
        /// <param name="overwriteDescription">
        /// The description of the save game to over-write, if any.
        public static void SaveSession(SaveGameDescription overwriteDescription)
        {
            StorageManager.Instance.SaveSession(overwriteDescription);
        }

        /// Delete the save game specified by the description.
        /// <param name="saveGameDescription">The description of the save game.</param>
        public static void DeleteSaveGame(SaveGameDescription saveGameDescription)
        {
            // check the parameters
            if (saveGameDescription == null)
            {
                throw new ArgumentNullException("saveGameDescription");
            }

            // get the storage device and load the session
            StorageManager.Instance.DeleteSaveGame(saveGameDescription);
        }

    }
}

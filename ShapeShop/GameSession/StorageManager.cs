using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShop.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace ShapeShop.GameSession
{
    public enum StorageState
    {
        Idle,

        ReadyCheckAvailableSpace,
        CheckAvailableSpace,

        ReadyFirstPass,
        ReadyToSave,
        ReadyToLoad,
        ReadyToDelete,
        ReadyToRefreshSaveGames,
    }

    public class StorageManager
    {
        // singleton ////////////////
        public static StorageManager Instance { get; } = new StorageManager();
        /////////////////////////////

        public static readonly int MAX_SAVE_GAMES = 3;
        private const float SPACE_MINIMUM = 1000000; // in bytes; 1MB

        private static readonly string FILENAME1 = "SaveGameDescription1.xml";
        private static readonly string FILENAME2 = "SaveGameDescription2.xml";
        private static readonly string FILENAME3 = "SaveGameDescription3.xml";

        private static readonly List<string> FILENAMES = new List<string>()
        {
            FILENAME1,
            FILENAME2,
            FILENAME3
        };

        private XmlSerializer serializerPlayerSaveData;
        private XmlSerializer serializerSaveGameDescription;

        private StorageDeviceIndicator sdIndicator;
        private StorageState state = StorageState.Idle;
//        private StorageState targetState;

        private MainGameScreen parentScreen;

        private string loadFilename;
        private SaveGameDescription overwriteDescription;
        private SaveGameDescription deleteDescription;

        private IAsyncResult storageCheckResult;

        // Save game descriptions for the current set of save games.
        private Dictionary<int, SaveGameDescription> saveGameDescriptionsDict = new Dictionary<int, SaveGameDescription>();
        public Dictionary<int, SaveGameDescription> SaveGameDescriptionsDict
        {
            get { return saveGameDescriptionsDict; }
        }

        private bool isFirstPassDone = false;
        public bool IsFirstPassDone
        {
            get { return isFirstPassDone; }
        }

        private bool isRefreshRequired = false;
        public bool IsRefreshRequired
        {
            get { return isRefreshRequired; }
            set { isRefreshRequired = value; }
        }

        private StorageManager()
        {
            serializerPlayerSaveData = new XmlSerializer(typeof(PlayerSaveData));
            serializerSaveGameDescription = new XmlSerializer(typeof(SaveGameDescription));
            sdIndicator = new StorageDeviceIndicator();
        }

        public void LoadContent(ContentManager content, MainGameScreen parentScreen)
        {
            sdIndicator.LoadContent(content);
            this.parentScreen = parentScreen;
        }

        public void MakeFirstPass()
        {
            state = StorageState.CheckAvailableSpace;
        }

        public void SaveSession(SaveGameDescription description)
        {
            overwriteDescription = description;
            state = StorageState.ReadyToSave;
        }

        public void LoadSession(string filename)
        {
            loadFilename = filename;
            state = StorageState.ReadyToLoad;
        }

        public void RefreshSaveGameDescriptions()
        {
            state = StorageState.ReadyToRefreshSaveGames;
        }

        public void DeleteSaveGame(SaveGameDescription description)
        {
            deleteDescription = description;
            state = StorageState.ReadyToDelete;
        }

        public void Update(GameTime gameTime)
        {
            try
            {
                sdIndicator.Update(gameTime);

                switch (state)
                {
                    case StorageState.CheckAvailableSpace:
                        checkAvailableSpace();
                        break;
                    case StorageState.ReadyCheckAvailableSpace:
                        readyCheckAvailableSpace();
                        break;
                    case StorageState.ReadyFirstPass:
                        refreshSaveGameDescriptions();
                        isFirstPassDone = true;
                        state = StorageState.Idle;
                        break;
                    case StorageState.ReadyToSave:
                        saveGame();
                        RefreshSaveGameDescriptions();
                        break;
                    case StorageState.ReadyToLoad:
                        loadGame();
                        state = StorageState.Idle;
                        break;
                    case StorageState.ReadyToDelete:
                        deleteGame();
                        RefreshSaveGameDescriptions();
                        break;
                    case StorageState.ReadyToRefreshSaveGames:
                        refreshSaveGameDescriptions();
                        state = StorageState.Idle;
                        break;
                }
            }
            catch (Exception)
            {
//                Debug.WriteLine($"Exception {e}");
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (sdIndicator != null && state != StorageState.Idle)
            {
                sdIndicator.Draw(spriteBatch, Color.White);
            }
        }

        private void refreshSaveGameDescriptions()
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain())   // GetUserStoreForAssembly
            {
                saveGameDescriptionsDict = new Dictionary<int, SaveGameDescription>();

                if (isoStore.FileExists(FILENAME1))
                {
                    using (Stream fileStream = isoStore.OpenFile(FILENAME1, FileMode.Open))
                    {
                        saveGameDescriptionsDict[1] = serializerSaveGameDescription.Deserialize(fileStream) as SaveGameDescription;
                    }
                }

                if (isoStore.FileExists(FILENAME2))
                {
                    using (Stream fileStream = isoStore.OpenFile(FILENAME2, FileMode.Open))
                    {
                        saveGameDescriptionsDict[2] = serializerSaveGameDescription.Deserialize(fileStream) as SaveGameDescription;
                    }
                }

                if (isoStore.FileExists(FILENAME3))
                {
                    using (Stream fileStream = isoStore.OpenFile(FILENAME3, FileMode.Open))
                    {
                        saveGameDescriptionsDict[3] = serializerSaveGameDescription.Deserialize(fileStream) as SaveGameDescription;
                    }
                }
            }

            // update UI
            parentScreen.SlotPanel.RefreshSlots();
        }

        private void saveGame()
        {
            try
            {
                // open the container
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain())
                {
                    string filename;
                    string descriptionFilename;

                    // get the filenames
                    if (overwriteDescription == null ||
                        overwriteDescription.FileName == null ||
                        overwriteDescription.FileName == "")
                    {
                        filename = "SaveGame" + overwriteDescription.SaveSlot + ".xml";
                        descriptionFilename = "SaveGameDescription" + overwriteDescription.SaveSlot + ".xml";
                    }
                    else
                    {
                        filename = overwriteDescription.FileName;
                        descriptionFilename = "SaveGameDescription" + Path.GetFileNameWithoutExtension(overwriteDescription.FileName).Substring(8) + ".xml";
                    }

                    if (isoStore.FileExists(filename))
                    {
                        isoStore.DeleteFile(filename);
                    }

                    using (Stream stream = isoStore.CreateFile(filename))
                    {
                        serializerPlayerSaveData.Serialize(stream, new PlayerSaveData(Session.Player, PuzzleEngine.CurrentPuzzleSet));
                    }

                    // create the save game description
                    SaveGameDescription description = new SaveGameDescription();
                    description.FileName = Path.GetFileName(filename);
                    description.PuzzlesSolved = PuzzleEngine.CurrentPuzzleSet.Statistics.PuzzlesSolved;
                    description.PuzzlesTotal = PuzzleEngine.CurrentPuzzleSet.Puzzles.Count;
                    description.SolveTime = PuzzleEngine.CurrentPuzzleSet.Statistics.SecondsTaken;
                    description.PuzzleSetName = PuzzleEngine.CurrentPuzzleSet.Name;
                    if (SignInManager.Instance.SignedIn)
                    {
                        description.Gamertag = SignInManager.Instance.GamerTag;
                    }
                    else
                    {
                        description.Gamertag = "Save_" + overwriteDescription.SaveSlot;
                    }
                    description.TimeStamp = DateTime.Now;
                    description.SaveSlot = overwriteDescription.SaveSlot;

                    using (Stream stream = isoStore.CreateFile(descriptionFilename))
                    {
                        serializerSaveGameDescription.Serialize(stream, description);
                    }
                }
            }
            catch (Exception)
            {
//                Debug.WriteLine($"StorageManager#saveGame Exception: {e}");
            }
        }

        private void loadGame()
        {
            try
            {
                PlayerSaveData psd;
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain())
                {
                    using (Stream stream = isoStore.OpenFile(loadFilename, FileMode.Open))
                    {
                        psd = serializerPlayerSaveData.Deserialize(stream) as PlayerSaveData;
                    }
                }

                Player player = new Player(psd, Session.ScreenManager.SessionContent);
                Session.SetPlayer(player);

                parentScreen.PickerPanel.LoadPickerEntries(false);
                parentScreen.LoadProcessingComplete();
            }
            catch (Exception)
            {
//                System.Diagnostics.Debug.WriteLine($"StorageManager#loadGame Exception: {e}");
//                System.Diagnostics.Debug.WriteLine($"{e.StackTrace}");
            }
        }

        private void deleteGame()
        {
            try
            {
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain())
                {
                    isoStore.DeleteFile(deleteDescription.FileName);
                    isoStore.DeleteFile("SaveGameDescription" + deleteDescription.SaveSlot + ".xml");
                }
            }
            catch (Exception)
            {
//                Debug.WriteLine($"StorageManager#deleteGame Exception: {e}");
//                Debug.WriteLine($"{e.StackTrace}");
            }
        }

        private void checkAvailableSpace()
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain())
            {
                if (isoStore.AvailableFreeSpace < SPACE_MINIMUM)
                {
//                    Debug.WriteLine("WARN: Less than 1mb available. Free space: " + (isoStore.AvailableFreeSpace / 1000) + "kb");

                    storageCheckResult = Guide.BeginShowMessageBox(InputManager.PlayerIndex,
                                                                  "Error",
                                                                  "The configured storage device does not have enough free space.\n\nShape Shop requires at least 1MB of available space to save your progress.",
                                                                  new List<String>() { "Exit Shape Shop" },
                                                                  0,
                                                                  MessageBoxIcon.Alert,
                                                                  null,
                                                                  null);
                    state = StorageState.ReadyCheckAvailableSpace;
                }
                else
                {
                    state = StorageState.ReadyFirstPass;
                }
            }
        }

        private void readyCheckAvailableSpace()
        {
            if (storageCheckResult.IsCompleted)
            {
                parentScreen.Mode = MainGameScreen.MainGameScreenMode.Exiting;
                parentScreen.ExitScreen();
            }
        }

        public void Reset()
        {
            isFirstPassDone = false;
            storageCheckResult = null;
        }

    }
}
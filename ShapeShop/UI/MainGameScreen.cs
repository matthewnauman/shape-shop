using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShop.GameSession;
using ShapeShopData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShapeShop.UI
{
    public class MainGameScreen : GameScreen
    {
        public enum MainGameScreenMode
        {
            ShowSignIn,
            //            ShowSignIn,
            SigningIn,
            HandleSignInFailure,
            ShowSignedOutMessage,
//            HandleSignedOutResult,

            ResetGame,
            Closed,
            DelayOpening,
            Opening,
            Open,
            Closing,
            Exiting,
            Loading,
            SlotPanel,
            PickerPanel,
            PuzzlePanel,
            CreditsPanel,
            Paused,
        };

        private enum LoadingMode
        {
            Starting,
            Processing,
            Finishing,
            Finished,
        }

        private enum PauseMode
        {
            Starting,
            Paused,
            Stopping,
            Waiting,
        }

        public static readonly float MARGIN_LEFTRIGHT = 127f;

        private const float ANCHOR_DOORCLOSED_LEFT_X = 0;
        private const float ANCHOR_DOORSHADOWCLOSED_LEFT_X = 0;

        private const float ANCHOR_DOORCLOSED_RIGHT_X = 591; // 641
        private const float ANCHOR_DOORSHADOWCLOSED_RIGHT_X = 460f;

        private const float ANCHOR_DOOROPEN_LEFT_X = -614f;
        private const float ANCHOR_DOORSHADOWOPEN_LEFT_X = -614f;
        private const float ANCHOR_DOOROPEN_RIGHT_X = 1103f;
        private const float ANCHOR_DOORSHADOWOPEN_RIGHT_X = 922;

        private const float DEFAULT_DOORSPEED_LEFT = 12f;
        private const float DEFAULT_DOORSPEED_RIGHT = 10f;
        private const float LOAD_ALPHA_STEP = .02f;
        private const float PAUSE_ALPHA_STEP = .035f;
        private const float LOADING_PAUSE_LIMIT = 1.5f;
        private const float OPENING_DELAY_MAX = .5f;

        private readonly string loadingText = "Loading...";
        private readonly string pauseText = "Paused";

        private float loadingPauseTimer = 0f;
        private float loadingAlpha = 0f;
        private float pauseAlpha = 0f;
        private float openingDelayTimer = 0;

        private Task<bool> signInTask;
        private SignInMessageBoxScreen signInScreen;
        private SigningInMessageBoxScreen signingInScreen;
        private SignOutMessageBoxScreen signOutScreen;
        private bool isFromSigninCancel = false;

        private HelpPanel helpPanel;
        public HelpPanel HelpPanel
        {
            get { return helpPanel; }
        }

        private SlotPanel slotPanel;
        public SlotPanel SlotPanel
        {
            get { return slotPanel; }
        }

        private PickerPanel pickerPanel;
        public PickerPanel PickerPanel
        {
            get { return pickerPanel; }
        }

        private PuzzlePanel puzzlePanel;
        public PuzzlePanel PuzzlePanel
        {
            get { return puzzlePanel; }
        }

        private CreditsPanel creditsPanel;
        public CreditsPanel CreditsPanel
        {
            get { return creditsPanel; }
        }

        private Texture2D blankTexture, transitionBackgroundTexture, backgroundTexture, doorRightTexture, doorLeftTexture, trackTexture, doorRightShadowTexture, doorLeftShadowTexture;
        private Vector2 backgroundPosition, doorRightPosition, doorLeftPosition, trackPosition, loadingTextPosition, pauseTextPosition, doorRightShadowPosition, doorLeftShadowPosition;

        private MainGameScreenMode mode = MainGameScreenMode.ShowSignIn;
        public MainGameScreenMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private LoadingMode loadingMode = LoadingMode.Starting;
        private PauseMode pauseMode = PauseMode.Waiting;

        private bool isNewGame = true;
        private bool isLoadProcessingFirstPassDone = false;

        private bool isStorageMessageDisplayed = false;

        private bool isLoadConfirmed = false;
        public bool IsLoadConfirmed
        {
            get { return isLoadConfirmed; }
        }

        private bool isReadyToLoad = false;
        public bool IsReadyToLoad
        {
            get { return isReadyToLoad; }
            set { isReadyToLoad = value; }
        }

        private bool isReadyForPuzzle = false;
        public bool IsReadyForPuzzle
        {
            get { return isReadyForPuzzle; }
            set { isReadyForPuzzle = value; }
        }

        private readonly bool isCogsRunning = false;
        public bool IsCogsRunning
        {
            get { return isCogsRunning; }
        }

        private Boolean isBackToPicker = false;
        public bool IsBackToPicker
        {
            get { return isBackToPicker; }
            set { isBackToPicker = value; }
        }

        private Boolean isPaused = false;
        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        private List<BackgroundCogSprite> cogSpritesList;

        // shader stuff
        private Effect sepiaShader;
        private Texture2D renderedTexture;

        // the current gamer signed-in under InputManager.PlayerIndex
        public SignedInGamer CurrentGamer
        {
            get { return Gamer.SignedInGamers[InputManager.PlayerIndex]; }
        }

        public MainGameScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);

            helpPanel = new HelpPanel(this);
            slotPanel = new SlotPanel(this);
            creditsPanel = new CreditsPanel(this);

            // give PuzzleEngine a pointer
            PuzzleEngine.MainScreen = this;

            // register with SignInManager
            SignInManager.Instance.RegisterMainGameScreen(this);
        }

        public void LoadProcessingComplete()
        {
            loadingMode = LoadingMode.Finishing;
            isLoadProcessingFirstPassDone = false;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.GlobalContent;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            blankTexture = content.Load<Texture2D>(@"Textures\1x1");
            transitionBackgroundTexture = content.Load<Texture2D>(@"Textures\GameScreens\introDoorsBackground");
            backgroundTexture = content.Load<Texture2D>(@"Textures\GameScreens\introBackground");
            doorRightTexture = content.Load<Texture2D>(@"Textures\GameScreens\introDoorRight");
            doorLeftTexture = content.Load<Texture2D>(@"Textures\GameScreens\introDoorLeft");
            doorRightShadowTexture = content.Load<Texture2D>(@"Textures\GameScreens\introDoorRightShadow");
            doorLeftShadowTexture = content.Load<Texture2D>(@"Textures\GameScreens\introDoorLeftShadow");
            trackTexture = content.Load<Texture2D>(@"Textures\GameScreens\track");

            backgroundPosition = Vector2.Zero;
            doorRightPosition = new Vector2(ANCHOR_DOORCLOSED_RIGHT_X, 0);
            doorRightShadowPosition = new Vector2(ANCHOR_DOORSHADOWCLOSED_RIGHT_X, 0);
            doorLeftPosition = Vector2.Zero;
            doorLeftShadowPosition = Vector2.Zero;
            trackPosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (trackTexture.Width / 2), 0);
            helpPanel.LoadContent(content);
            slotPanel.LoadContent(content);

            cogSpritesList = new List<BackgroundCogSprite>
            {
                new BackgroundCogSprite(content.Load<Texture2D>(@"Textures\Cogs\Cog07"),
                               content.Load<Texture2D>(@"Textures\Cogs\cog07bolt"),
                               TransitionColor,
                               new Vector2(495, -168),
                               null,
                               1f,
                               0f,
                               2,
                               BackgroundCogSprite.RotationDirection.CCW),
                new BackgroundCogSprite(content.Load<Texture2D>(@"Textures\Cogs\Cog06"),
                               null,
                               TransitionColor,
                               new Vector2(-182, -503),
                               null,
                               1f,
                               0f,
                               1,
                               BackgroundCogSprite.RotationDirection.CW),
                new BackgroundCogSprite(content.Load<Texture2D>(@"Textures\Cogs\Cog05"),
                               null,
                               TransitionColor,
                               new Vector2(702, 103),
                               null,
                               1f,
                               0f,
                               2,
                               BackgroundCogSprite.RotationDirection.CW),
                new BackgroundCogSprite(content.Load<Texture2D>(@"Textures\Cogs\Cog04"),
                               null,
                               TransitionColor,
                               new Vector2(735, -478),
                               null,
                               1f,
                               0f,
                               1,
                               BackgroundCogSprite.RotationDirection.CCW),
                new BackgroundCogSprite(content.Load<Texture2D>(@"Textures\Cogs\Cog03"),
                               content.Load<Texture2D>(@"Textures\Cogs\cog03bolt"),
                               TransitionColor,
                               new Vector2(-7, -184),
                               null,
                               1f,
                               0f,
                               2,
                               BackgroundCogSprite.RotationDirection.CCW),
                new BackgroundCogSprite(content.Load<Texture2D>(@"Textures\Cogs\Cog02"),
                               content.Load<Texture2D>(@"Textures\Cogs\cog02bolt"),
                               TransitionColor,
                               new Vector2(670, 82),
                               null,
                               1f,
                               0f,
                               1,
                               BackgroundCogSprite.RotationDirection.CCW),
                new BackgroundCogSprite(content.Load<Texture2D>(@"Textures\Cogs\Cog01"),
                               content.Load<Texture2D>(@"Textures\Cogs\cog01bolt"),
                               TransitionColor,
                               new Vector2(-97, 41),
                               null,
                               1f,
                               0f,
                               2,
                               BackgroundCogSprite.RotationDirection.CW)
            };

            sepiaShader = content.Load<Effect>(@"Effects\SepiaScreen");

            renderedTexture = new Texture2D(ScreenManager.GraphicsDevice,
                                                   ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                   ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight,
                                                   false,
                                                   SurfaceFormat.Color);

            loadingTextPosition = new Vector2(ShapeShop.PREFERRED_WIDTH / 2 - Fonts.Console72OutlinedFont.MeasureString(loadingText).X / 2,
                                              ShapeShop.PREFERRED_HEIGHT / 2 - Fonts.Console72OutlinedFont.MeasureString(loadingText).Y / 2);

            pauseTextPosition = new Vector2(ShapeShop.PREFERRED_WIDTH / 2 - Fonts.Console72OutlinedFont.MeasureString(pauseText).X / 2,
                                            ShapeShop.PREFERRED_HEIGHT / 2 - Fonts.Console72OutlinedFont.MeasureString(pauseText).Y / 2);

            StorageManager.Instance.LoadContent(content, this);
            StorageManager.Instance.MakeFirstPass();

//            await SignInManager.Instance.SignIn();
        }

        public void PausePuzzlePanel()
        {
            mode = MainGameScreenMode.Paused;
            pauseMode = PauseMode.Starting;
            puzzlePanel.stopCogs();
            stopCogs(true);
            AudioManager.PopMusic();
            PuzzleEngine.IsTimerDisabled = true;
            IsPaused = true;
            resetTexture();                      
        }

        public override void HandleInput()
        {
            if (IsActive)
            {
                switch (mode)
                {
                    case MainGameScreenMode.Open:
                        break;
                    case MainGameScreenMode.Closed:
                        if (isStorageMessageDisplayed)
                        {
                            if (InputManager.IsActionTriggered(InputManager.Action.Ok))
                            {
                                mode = MainGameScreenMode.DelayOpening;
                                AudioManager.PlayCue("doorOpen");
                                AudioManager.PlayMusic("bgCogsTurning");
                            }
                            else if (InputManager.IsActionTriggered(InputManager.Action.ExitGame))
                            {
                                ExitGame(true);
                            }
                        }
                        break;
                    case MainGameScreenMode.Opening:
                        break;
                    case MainGameScreenMode.SlotPanel:
                        // move these backs into slotPanel and pickerPanel
                        if (InputManager.IsActionTriggered(InputManager.Action.Back))
                        {
                            helpPanel.Off(true);
                            mode = MainGameScreenMode.Closing;                        
                            AudioManager.PlayCue("doorClose");
                            slotPanel.Close();

                            return;
                        }
                        slotPanel.HandleInput();
                        break;
                    case MainGameScreenMode.PickerPanel:
                        if (InputManager.IsActionTriggered(InputManager.Action.Back))
                        {
                            ShowConfirmEndSession();
                            return;
                        }
                        pickerPanel.HandleInput();
                        break;
                    case MainGameScreenMode.PuzzlePanel:
                        if (puzzlePanel.PanelState == PuzzlePanel.PuzzlePanelState.Running && 
                            InputManager.IsActionTriggered(InputManager.Action.Pause))
                        {
                            PausePuzzlePanel();
                        }
                        else
                        {
                            puzzlePanel.HandleInput();
                        }
                        break;
                    case MainGameScreenMode.CreditsPanel:

                        break;
                    case MainGameScreenMode.Paused:
                        if (InputManager.IsActionTriggered(InputManager.Action.Pause))
                        {
                            pauseMode = PauseMode.Stopping;
                        }

                        break;
                    case MainGameScreenMode.Closing:
                        break;
                    case MainGameScreenMode.Exiting:
                        break;
                }
            }
        }

        private void endSession()
        {
            Session.EndSession();
            PuzzleEngine.EndSession();
            
            puzzlePanel = null;
            pickerPanel = null;
            creditsPanel = new CreditsPanel(this);
            sessionReset();
        }

        private void sessionReset()
        {
            loadingMode = LoadingMode.Starting;
            isLoadConfirmed = false;
            isReadyToLoad = false;
            isReadyForPuzzle = false;
            slotPanel.PanelState = SlotPanel.SlotPanelState.Closed;
            slotPanel.SelectMode = SlotPanel.SelectorMode.Docked;
        }

        private void fullReset()
        {
            mode = MainGameScreenMode.ShowSignIn;

            doorRightPosition = new Vector2(ANCHOR_DOORCLOSED_RIGHT_X, 0);
            doorRightShadowPosition = new Vector2(ANCHOR_DOORSHADOWCLOSED_RIGHT_X, 0);
            doorLeftPosition = Vector2.Zero;
            doorLeftShadowPosition = Vector2.Zero;

            helpPanel.Off(false);
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!InputManager.IsPlayerIndexDetected)
            {
//                System.Diagnostics.Debug.WriteLine($"-BEFOR- IsPlayerIndexDetected: {InputManager.IsPlayerIndexDetected}");
                InputManager.DetectPlayerIndex();
//                System.Diagnostics.Debug.WriteLine($"-AFTER- IsPlayerIndexDetected: {InputManager.IsPlayerIndexDetected}");
            }

            StorageManager.Instance.Update(gameTime);

            if (PuzzleEngine.IsCheckRender)
            {
                PuzzleEngine.RenderTextures();
            }

            switch (mode)
            {
                case MainGameScreenMode.ShowSignIn:
                    if (signInTask == null)
                    {
                        signInTask = SignInManager.Instance.SignIn();
                    }
                    /*
                    else if (signInTask.IsCompleted)
                    {
                        if (signInTask.Result)
                        {
                            // sign-in successful
                            mode = MainGameScreenMode.Closed;
                        }
                        else
                        {
                            // sign-in failure
                            mode = MainGameScreenMode.HandleSignInFailure;
                        }
                        signInTask = null;
                    }
                    */
                    else
                    {
                        ShowSigningInMessage();
                        mode = MainGameScreenMode.SigningIn;
                    }
                    break;
                case MainGameScreenMode.SigningIn:
                    if (signInTask.IsCompleted)
                    {
                        signingInScreen.ExitScreen();

                        if (signInTask.Result)
                        {
                            // sign-in successful
                            mode = MainGameScreenMode.Closed;
                        }
                        else
                        {
                            // sign-in failure
                            mode = MainGameScreenMode.HandleSignInFailure;
                        }
                        signInTask = null;
                    }
                    break;
                case MainGameScreenMode.HandleSignInFailure:
//                    if (!Guide.IsVisible)
//                    {
                    ShowSignInWarning();                        
                    mode = MainGameScreenMode.Closed;
//                    }

                    break;
                case MainGameScreenMode.ShowSignedOutMessage:
                    //#if XBOX
                    //                    if (!Guide.IsVisible)
                    //#endif
                    ShowSignOutWarning();
                    mode = MainGameScreenMode.Closed;
//                    signInTask = null;
//                    }
                    break;

/*                        
                case MainGameScreenMode.HandleSignedOutResult:
                    if (signedOutResult.IsCompleted)
                    {
                        mode = MainGameScreenMode.ResetGame;
                    }
                    break;
*/





                case MainGameScreenMode.ResetGame:
                    InputManager.Initialize();
                    InputManager.ClearPlayerIndex();
                    StorageManager.Instance.Reset();
                    endSession();

                    ScreenManager.AddScreen(new SplashScreen(false));

                    slotPanel.Reset();
                    AudioManager.StopMusic();

                    doorRightPosition = new Vector2(ANCHOR_DOORCLOSED_RIGHT_X, 0);
                    doorRightShadowPosition = new Vector2(ANCHOR_DOORSHADOWCLOSED_RIGHT_X, 0);
                    doorLeftPosition = Vector2.Zero;
                    doorLeftShadowPosition = Vector2.Zero;

                    helpPanel.Off(false);

                    mode = MainGameScreenMode.Closed;

                    break;
                case MainGameScreenMode.Open:
                    break;
                case MainGameScreenMode.Closed:
                    if (!isStorageMessageDisplayed && IsActive && StorageManager.Instance.IsFirstPassDone)
                    {
                        ShowStorageWarning(StorageDeviceWarningType.AutoSaveWarn);
                    }
                    break;
                case MainGameScreenMode.DelayOpening:
                    openingDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (openingDelayTimer >= OPENING_DELAY_MAX)
                    {
                        openingDelayTimer = 0;
                        mode = MainGameScreenMode.Opening;
                        slotPanel.Open();
                    }
                    break;
                case MainGameScreenMode.Opening:
                    if (StorageManager.Instance.IsRefreshRequired)
                    {
                        StorageManager.Instance.RefreshSaveGameDescriptions();
                        StorageManager.Instance.IsRefreshRequired = false;
                    }

                    doorRightPosition.X += DEFAULT_DOORSPEED_RIGHT;
                    doorRightShadowPosition.X += DEFAULT_DOORSPEED_RIGHT;
                    doorLeftPosition.X -= DEFAULT_DOORSPEED_LEFT;
                    doorLeftShadowPosition.X -= DEFAULT_DOORSPEED_LEFT;

                    if (doorRightPosition.X >= ANCHOR_DOOROPEN_RIGHT_X)
                    {
                        doorRightPosition.X = ANCHOR_DOOROPEN_RIGHT_X;
                    }
                    if (doorLeftPosition.X <= ANCHOR_DOOROPEN_LEFT_X)
                    {
                        doorLeftPosition.X = ANCHOR_DOOROPEN_LEFT_X;
                    }
                    if (doorLeftPosition.X == ANCHOR_DOOROPEN_LEFT_X &&
                        doorRightPosition.X == ANCHOR_DOOROPEN_RIGHT_X)
                    {
                        mode = MainGameScreenMode.SlotPanel;
                    }
                    break;
                case MainGameScreenMode.SlotPanel:
                    if (isReadyToLoad)
                    {
                        isLoadConfirmed = false;

                        AudioManager.PopMusic();

                        stopCogs(true);
                        resetTexture();

                        mode = MainGameScreenMode.Loading;
                        loadingMode = LoadingMode.Starting;
                    }
                    break;
                case MainGameScreenMode.PickerPanel:
                    if (isReadyForPuzzle)
                    {
                        PuzzleEngine.SetPuzzle();

                        mode = MainGameScreenMode.PuzzlePanel;
                        puzzlePanel.Open();
                        isReadyForPuzzle = false;
                    }
                    break;
                case MainGameScreenMode.PuzzlePanel:
                    if (isBackToPicker)
                    {
                        mode = MainGameScreenMode.PickerPanel;
                        pickerPanel.Open();
                        isBackToPicker = false;
                    }
                    break;
                case MainGameScreenMode.CreditsPanel:

                    break;
                case MainGameScreenMode.Closing:
                    doorRightPosition.X -= DEFAULT_DOORSPEED_RIGHT;
                    doorRightShadowPosition.X -= DEFAULT_DOORSPEED_RIGHT;
                    doorLeftPosition.X += DEFAULT_DOORSPEED_LEFT;
                    doorLeftShadowPosition.X += DEFAULT_DOORSPEED_LEFT;

                    if (doorRightPosition.X <= ANCHOR_DOORCLOSED_RIGHT_X)
                    {
                        doorRightPosition.X = ANCHOR_DOORCLOSED_RIGHT_X;
                    }
                    if (doorLeftPosition.X >= ANCHOR_DOORCLOSED_LEFT_X)
                    {
                        doorLeftPosition.X = ANCHOR_DOORCLOSED_LEFT_X;
                    }
                    if (doorLeftPosition.X == ANCHOR_DOORCLOSED_LEFT_X &&
                        doorRightPosition.X == ANCHOR_DOORCLOSED_RIGHT_X)
                    {
                        mode = MainGameScreenMode.Closed;

                        AudioManager.PopMusic();

                        endSession();
                    }
                    break;
                case MainGameScreenMode.Exiting:
                    if (ScreenState == ScreenState.FinishedExiting)
                    {
                        ScreenManager.Game.Exit();
                    }
                    break;
                case MainGameScreenMode.Loading:
                    switch (loadingMode)
                    {
                        case LoadingMode.Starting:
                            loadingAlpha += LOAD_ALPHA_STEP;
                            if (loadingAlpha >= 1)
                            {
                                loadingMode = LoadingMode.Processing;
                                loadingAlpha = 1;
                            }
                            break;
                        case LoadingMode.Finishing:
                            loadingPauseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (loadingPauseTimer >= LOADING_PAUSE_LIMIT)
                            {
                                loadingAlpha -= LOAD_ALPHA_STEP;
                                if (loadingAlpha <= 0)
                                {
                                    loadingPauseTimer = 0;

                                    loadingAlpha = 0;
                                    startCogs(true);

                                    AudioManager.PlayMusic("bgCogsTurning");

                                    loadingMode = LoadingMode.Finished;
                                }
                            }
                            break;
                        case LoadingMode.Finished:
                            mode = MainGameScreenMode.PickerPanel;
                            pickerPanel.Open();
                            break;
                        case LoadingMode.Processing:
                            // load new or saved game
                            if (isNewGame)
                            {
                                GameStartDescription gsd = ScreenManager.GlobalContent.Load<GameStartDescription>("NewGameDescription");
                                gsd.SaveSlotNumber = slotPanel.CurrentSlot.SlotNumber;
                                pickerPanel = new PickerPanel(this, gsd);
                                puzzlePanel = new PuzzlePanel(this);
                                pickerPanel.LoadContent(ScreenManager.SessionContent);
                                puzzlePanel.LoadContent(ScreenManager.SessionContent);
                                
                                Session.QuickSave();
                                
                                LoadProcessingComplete();
                            }
                            else if (!isLoadProcessingFirstPassDone)
                            {
                                pickerPanel = new PickerPanel(this, slotPanel.CurrentSlot.SaveGameDescription);
                                puzzlePanel = new PuzzlePanel(this);
                                pickerPanel.LoadContent(ScreenManager.SessionContent);
                                puzzlePanel.LoadContent(ScreenManager.SessionContent);
                                isLoadProcessingFirstPassDone = true;
                            }

                            break;
                    }
                    break;

                case MainGameScreenMode.Paused:

                    switch (pauseMode)
                    {
                        case PauseMode.Waiting:
                            break;

                        case PauseMode.Starting:
                            pauseAlpha += PAUSE_ALPHA_STEP;
                            if (pauseAlpha >= 1)
                            {
                                pauseMode = PauseMode.Paused;
                                pauseAlpha = 1;
                            }
                            break;

                        case PauseMode.Paused:
                            break;

                        case PauseMode.Stopping:
                            pauseAlpha -= PAUSE_ALPHA_STEP;
                            if (pauseAlpha <= 0)
                            {
                                pauseMode = PauseMode.Waiting;
                                mode = MainGameScreenMode.PuzzlePanel;
                                startCogs(true);

                                AudioManager.PlayMusic("bgCogsTurning");

                                puzzlePanel.startCogs();
                                PuzzleEngine.IsTimerDisabled = false;
                                pauseAlpha = 0;

                                IsPaused = false;
                            }
                            break;
                    }
                    break;
            }

            if (mode != MainGameScreenMode.Closed && mode != MainGameScreenMode.Exiting)
            {
                if (helpPanel != null)    { helpPanel.Update(gameTime); }
                if (slotPanel != null)    { slotPanel.Update(gameTime); }
                if (pickerPanel != null)  { pickerPanel.Update(gameTime); }
                if (puzzlePanel != null)  { puzzlePanel.Update(gameTime); }
                if (creditsPanel != null) { creditsPanel.Update(gameTime); }
            }

            if (cogSpritesList != null)
            {
                foreach (BackgroundCogSprite cog in cogSpritesList)
                {
                    cog.Update(gameTime);
                }
            }

        }

        private void resetTexture()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            GraphicsDevice device = ScreenManager.GraphicsDevice;

            RenderTarget2D tmpRenderTarget = new RenderTarget2D(device,
                                                                device.PresentationParameters.BackBufferWidth,
                                                                device.PresentationParameters.BackBufferHeight);

            RenderTarget2D sepiaRenderTarget = new RenderTarget2D(device,
                                                                  device.PresentationParameters.BackBufferWidth,
                                                                  device.PresentationParameters.BackBufferHeight);

            device.SetRenderTarget(tmpRenderTarget);
            device.Clear(Color.Transparent);
            draw(spriteBatch);

            device.SetRenderTarget(sepiaRenderTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, sepiaShader);
            spriteBatch.Draw(tmpRenderTarget, 
                             Vector2.Zero, 
                             Color.White);
            spriteBatch.End();

            device.SetRenderTarget(null);

            renderedTexture = sepiaRenderTarget;

            tmpRenderTarget = null;
            sepiaRenderTarget = null;
        }

        private void stopCogs(bool isHardStop)
        {
            if (isHardStop)
            {
                foreach (BackgroundCogSprite cog in cogSpritesList)
                {
                    cog.Stop(true);
                }
            }
        }

        private void startCogs(bool isQuickStart)
        {
            if (isQuickStart)
            {
                foreach (BackgroundCogSprite cog in cogSpritesList)
                {
                    cog.Start(true);
                }
            }
        }

        public void ShowSignInWarning()
        {
            signInScreen = new SignInMessageBoxScreen();
            signInScreen.Accepted += signInAccepted;
            signInScreen.Cancelled += signInCancelled;
            ScreenManager.AddScreen(signInScreen);
        }

        public void ShowSigningInMessage()
        {
            signingInScreen = new SigningInMessageBoxScreen();
            signingInScreen.Exiting += signingInExiting;
            ScreenManager.AddScreen(signingInScreen);
        }

        public void ShowSignOutWarning()
        {
            signOutScreen = new SignOutMessageBoxScreen();
            signOutScreen.Ok += signOutOk;
            ScreenManager.AddScreen(signOutScreen);
        }

        private void signInAccepted(object sender, EventArgs e)
        {
            mode = MainGameScreenMode.ShowSignIn;
        }

        private void signInCancelled(object sender, EventArgs e)
        {
            isFromSigninCancel = true;
            ExitGame(false);
        }

        private void signingInExiting(object sender, EventArgs e)
        {

        }

        private void signOutOk(object sender, EventArgs e)
        {
            mode = MainGameScreenMode.ResetGame;
        }

        public void ShowConfirmReplay()
        {
            MessageBoxScreen messageBoxScreen = new MessageBoxScreen("You have completed\nthis puzzle already.\nWould you like to\nsolve it again?", false);
            messageBoxScreen.Accepted += confirmReplayMessageBoxAccepted;
            messageBoxScreen.Cancelled += confirmReplayMessageBoxCancelled;
            ScreenManager.AddScreen(messageBoxScreen);

        }

        private void confirmReplayMessageBoxAccepted(object sender, EventArgs e)
        {
            // make sure puzzle stats are subtracted from puzzleSet stats
            PuzzleEngine.IsOnReplay = true;
            helpPanel.Off(true);
            pickerPanel.ReplayConfirmed();
        }

        private void confirmReplayMessageBoxCancelled(object sender, EventArgs e)
        {
        }

        public void ShowConfirmEndSession()
        {
            MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Are you sure you\nwant to quit this\ngame?", false);
            messageBoxScreen.Accepted += confirmEndSessionMessageBoxAccepted;
            messageBoxScreen.Cancelled += confirmExitMessageBoxCancelled;
            ScreenManager.AddScreen(messageBoxScreen);
        }

        private void confirmEndSessionMessageBoxAccepted(object sender, EventArgs e)
        {
            helpPanel.Off(true);

            mode = MainGameScreenMode.Closing;
            AudioManager.PlayCue("doorClose");

            slotPanel.Reset();
            pickerPanel.Close();
        }

        private void confirmExitMessageBoxCancelled(object sender, EventArgs e)
        {
            if (isFromSigninCancel)
            {
                isFromSigninCancel = false;
                signInScreen = new SignInMessageBoxScreen();
                signInScreen.Accepted += signInAccepted;
                signInScreen.Cancelled += signInCancelled;
                ScreenManager.AddScreen(signInScreen);
            }
        }


        public void ShowConfirmBackToPickerPanel()
        {
            confirmBackToPickerPanel();
        }

        private void confirmBackToPickerPanel()
        {
            helpPanel.Off(true);

            puzzlePanel.ResetPuzzleName();
            PuzzleEngine.ResetCurrentPuzzleStatistics();
            PuzzleEngine.ResetCurrentPuzzleShapes(false, false);
            puzzlePanel.PanelState = PuzzlePanel.PuzzlePanelState.PreShutdown;
        }

        public void ShowConfirmLoad()
        {
            MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Are you sure you\nwant to load this\ngame?", false);
            messageBoxScreen.Accepted += confirmLoadMessageBoxAccepted;
            messageBoxScreen.Cancelled += confirmLoadMessageBoxCancelled;
            ScreenManager.AddScreen(messageBoxScreen);
        }

        public void BypassConfirmLoad()
        {
            confirmLoadMessageBoxAccepted(null, null);
        }

        private void confirmLoadMessageBoxAccepted(object sender, EventArgs e)
        {
            isLoadConfirmed = true;
            isNewGame = false;
            slotPanel.Close();
        }

        private void confirmLoadMessageBoxCancelled(object sender, EventArgs e)
        {
        }

        public void BypassConfirmNew()
        {
            confirmNewMessageBoxAccepted(null, null);
        }

        private void confirmNewMessageBoxAccepted(object sender, EventArgs e)
        {
            isLoadConfirmed = true;
            isNewGame = true;
            slotPanel.Close();
        }

        private void confirmNewMessageBoxCancelled(object sender, EventArgs e)
        {
        }

        public void ShowStorageWarning(StorageDeviceWarningType type)
        {
            StorageDeviceWarningScreen storageWarningScreen = new StorageDeviceWarningScreen(type);
            storageWarningScreen.Continue += storageWarningContinue;
            ScreenManager.AddScreen(storageWarningScreen);
        }
          
        private void storageWarningContinue(object sender, EventArgs e)
        {
            isStorageMessageDisplayed = true;
        }

        public void ShowConfirmDelete()
        {
            MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Are you sure you\nwant to delete this\nsave game?", false);
            messageBoxScreen.Accepted += confirmDeleteMessageBoxAccepted;
            messageBoxScreen.Cancelled += confirmDeleteMessageBoxCancelled;
            ScreenManager.AddScreen(messageBoxScreen);
        }

        /// Callback for the Delete Game confirmation message box.
        private void confirmDeleteMessageBoxAccepted(object sender, EventArgs e)
        {
            Session.DeleteSaveGame(StorageManager.Instance.SaveGameDescriptionsDict[slotPanel.CurrentSlot.SlotNumber]);
        }

        /// Callback for the Delete Game confirmation message box.
        private void confirmDeleteMessageBoxCancelled(object sender, EventArgs e)
        {
        }

        private void confirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ExitScreen();
            mode = MainGameScreenMode.Exiting;
        }

        protected void ExitGame(bool confirm)
        {
            if (confirm)
            {
                string message = String.Empty;
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen("Quit and go back to\nthe Dashboard?", false);
                confirmExitMessageBox.Accepted += confirmExitMessageBoxAccepted;
                //            confirmExitMessageBox.Cancelled += confirmExitMessageBoxCancelled;
                ScreenManager.AddScreen(confirmExitMessageBox);
            }
            else
            {
                ExitScreen();
                mode = MainGameScreenMode.Exiting;
            }
        }

        private void draw(SpriteBatch spriteBatch)
        {
            if (mode != MainGameScreenMode.Exiting)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

                spriteBatch.Draw(backgroundTexture, backgroundPosition, TransitionColor);

                // background cogs
                foreach (BackgroundCogSprite cog in cogSpritesList)
                {
                    cog.Draw(spriteBatch, TransitionColor);
                }

                // background track
                spriteBatch.Draw(trackTexture, trackPosition, TransitionColor);

                spriteBatch.Draw(doorRightShadowTexture,
                                 doorRightShadowPosition,
                                 TransitionColor);
                spriteBatch.Draw(doorLeftShadowTexture,
                                 doorLeftShadowPosition,
                                 TransitionColor);

                spriteBatch.End();


                if (slotPanel != null &&
                    slotPanel.PanelState != SlotPanel.SlotPanelState.Closed)
                {
                    slotPanel.Draw(spriteBatch);
                }
                if (pickerPanel != null &&
                    pickerPanel.PanelState != PickerPanel.PickerPanelState.Closed &&
                    PuzzleEngine.MainScreen != null)
                {
                    pickerPanel.Draw(spriteBatch);
                }
                if (puzzlePanel != null &&
                    puzzlePanel.PanelState != PuzzlePanel.PuzzlePanelState.Closed &&
                    PuzzleEngine.MainScreen != null)
                {
                    puzzlePanel.Draw(spriteBatch);
                }
                if (creditsPanel != null &&
                    creditsPanel.PanelState != CreditsPanel.CreditsPanelState.Closed &&
                    PuzzleEngine.MainScreen != null)
                {
                    creditsPanel.Draw(spriteBatch);
                }

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

                // draw doors
                spriteBatch.Draw(doorRightTexture, doorRightPosition, TransitionColor);
                spriteBatch.Draw(doorLeftTexture, doorLeftPosition, TransitionColor);

                if (helpPanel != null)
                {
                    helpPanel.Draw(spriteBatch);
                }

                // draw cursor                
                if (puzzlePanel != null && 
                    puzzlePanel.PanelState == PuzzlePanel.PuzzlePanelState.Running && 
                    PuzzleEngine.IsCursorModeActive &&
                    !PuzzleEngine.IsAShapeSelected)
                {
                    PuzzleEngine.Cursor.Draw(spriteBatch);
                }

                spriteBatch.End();

            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, ShapeShop.PREFERRED_HEIGHT, ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT),
                             null,
                             Color.Black,
                             0f,
                             Vector2.Zero,
                             SpriteEffects.None,
                             0);

            spriteBatch.Draw(blankTexture,
                             new Rectangle(ShapeShop.PREFERRED_WIDTH, 0, ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT),
                             null,
                             Color.Black,
                             0f,
                             Vector2.Zero,
                             SpriteEffects.None,
                             0);

            spriteBatch.Draw(blankTexture,
                             new Rectangle(ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT, ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT),
                             null,
                             Color.Black,
                             0f,
                             Vector2.Zero,
                             SpriteEffects.None,
                             0);

            spriteBatch.End();
        }

        /// Draws the menu.
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            // transition in and out with a solid bg texture so you dont see the seam in the doors
            if (ScreenState == ScreenState.TransitionOn || ScreenState == ScreenState.TransitionOff)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                spriteBatch.Draw(transitionBackgroundTexture, Vector2.Zero, TransitionColor);
                spriteBatch.End();
                return;
            }

            draw(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            switch (mode)
            {
                case MainGameScreenMode.Loading:
                    spriteBatch.Draw(renderedTexture, 
                                     Vector2.Zero, 
                                     new Color(1f, 1f, 1f, loadingAlpha));

                    // Pulsate the size of Loading text
                    //                    double time = gameTime.TotalGameTime.TotalSeconds;
                    //                    float pulsate = (float)Math.Sin(time * 6) + 1;
                    //                    float scale = 1 + pulsate * 0.05f;
                    //float scale = 1;

                    spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                           loadingText,
                                           loadingTextPosition,
                                           new Color(1, 1, 1, loadingAlpha),
                                           0f,
                                           Vector2.Zero,
                                           1,
                                           SpriteEffects.None,
                                           0);
                    break;

                case MainGameScreenMode.Paused:
                    spriteBatch.Draw(renderedTexture,
                                     Vector2.Zero,
                                     new Color(1f, 1f, 1f, pauseAlpha));

                    // Pulsate the size of Loading text
                    //                    double time = gameTime.TotalGameTime.TotalSeconds;
                    //                    float pulsate = (float)Math.Sin(time * 6) + 1;
                    //                    float scale = 1 + pulsate * 0.05f;
                    //float scale = 1;

                    spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                           pauseText,
                                           pauseTextPosition,
                                           new Color(1, 1, 1, pauseAlpha),
                                           0f,
                                           Vector2.Zero,
                                           1,
                                           SpriteEffects.None,
                                           0);
                    break;
            }

            StorageManager.Instance.Draw(spriteBatch);

            spriteBatch.End();

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }















        public string GetDebugInformation()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.Append(" IntroScreenMode : " + mode)
                  .Append('\n')
                  .Append("   isCogsRunning : " + isCogsRunning)
                  .Append('\n')
                  .Append(" LoadingMode : " + loadingMode)
                  .Append('\n')
                  .Append("   loadingPauseTimer : " + loadingPauseTimer)
                  .Append('\n')
                  .Append("   loadingAlpha : " + loadingAlpha)
                  .Append('\n')
                  .Append("   isNewGame : " + isNewGame)
                  .Append('\n')
                  .Append("   isLoadConfirmed : " + isLoadConfirmed)
                  .Append('\n')
                  .Append("   isReadyToLoad : " + isReadyToLoad)
                  .Append('\n')
                  .Append("   isReadyForPuzzle : " + isReadyForPuzzle)
                  .Append('\n');

            return retStr.ToString();
        }


    }
}

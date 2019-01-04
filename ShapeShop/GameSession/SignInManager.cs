using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.System;
using ShapeShop.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace ShapeShop.GameSession
{
    public class SignInManager
    {
        // singleton ////////////////
        public static SignInManager Instance { get; } = new SignInManager();
        /////////////////////////////

        private XboxLiveContext xboxLiveContext;
        private XboxLiveUser primaryUser;
        private MainGameScreen mainGameScreen;

        public bool SignedIn { get; private set; } = false;
//        public bool CancelledSignIn { get; private set; } = false;

        public String GamerTag
        {
            get { return primaryUser.Gamertag; }
        }

        private SignInManager()
        {
        }
        
        public void RegisterMainGameScreen(MainGameScreen mgs)
        {
            mainGameScreen = mgs;
        }

        public async Task<bool> SignIn()
        {
//            CancelledSignIn = true;

            // Get a list of the active Windows users.
            IReadOnlyList<Windows.System.User> users = await Windows.System.User.FindAllAsync();

            // Acquire the CoreDispatcher which will be required for SignInSilentlyAsync and SignInAsync.
            Windows.UI.Core.CoreDispatcher UIDispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            //Windows.UI.Xaml.Window.Current.CoreWindow.Dispatcher;
            //CoreApplication.GetCurrentView().CoreWindow.Dispatcher;

            try
            {
                // 1. Create an XboxLiveUser object to represent the user
                //        XboxLiveUser primaryUser = new XboxLiveUser(users[0]);
                primaryUser = new XboxLiveUser();

                // 2. Sign-in silently to Xbox Live
                SignInResult signInSilentResult = await primaryUser.SignInSilentlyAsync(UIDispatcher);

                switch (signInSilentResult.Status)
                {
                    case SignInStatus.Success:
                        SignedIn = true;
                        break;
                    case SignInStatus.UserInteractionRequired:
                        //3. Attempt to sign-in with UX if required
                        SignInResult signInLoud = await primaryUser.SignInAsync(UIDispatcher);
                        switch (signInLoud.Status)
                        {
                            case SignInStatus.Success:
                                SignedIn = true;
                                break;
                            case SignInStatus.UserCancel:
                                // present in-game UX that allows the user to retry the sign-in operation. (For example, a sign-in button)
//                                CancelledSignIn = true;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                if (SignedIn)
                {
                    // 4. Create an Xbox Live context based on the interacting user
                    xboxLiveContext = new XboxLiveContext(primaryUser);

                    //add the sign out event handler
                    XboxLiveUser.SignOutCompleted += OnSignOut;
                }
            }
            catch (Exception)
            {
//                System.Diagnostics.Debug.WriteLine($"Session#SignIn : Unexpected Exception: {e.Message}");
            }

            return SignedIn;
        }

        public void OnSignOut(object sender, SignOutCompletedEventArgs e)
        {
            // 6. When the game exits or the user signs-out, release the XboxLiveUser object and XboxLiveContext object by setting them to null
            signOutPrimaryUser();
        }

        private void signOutPrimaryUser()
        {
//            CancelledSignIn = false;
            SignedIn = false;
            primaryUser = null;
            xboxLiveContext = null;

            mainGameScreen.Mode = MainGameScreen.MainGameScreenMode.ShowSignedOutMessage;
        }

    }
}

using PDFReader.Common;
using RDPDFLib.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PDFReader
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        public ContinuationManager ContinuationManager { get; private set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            //Initialize PDF enviroment.
            String inst_path = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            PDFGlobal.SetCMapsPath(inst_path + "\\Assets\\cmaps.dat", inst_path + "\\Assets\\umaps.dat");
            PDFGlobal.FontFileListStart();
            PDFGlobal.FontFileListAdd(inst_path + "\\Assets\\font\\argbsn00lp.ttf");
            PDFGlobal.FontFileListEnd();

            PDFGlobal.SetCMYKICC(inst_path + "\\Assets\\cmyk_rgb.dat");

            int face_first = 0;
            int face_count = PDFGlobal.GetFaceCount();
            String fname = null;
            while (face_first < face_count)
            {
                fname = PDFGlobal.GetFaceName(face_first);
                if (fname != null && fname.Length > 0) break;
                face_first++;
            }

            // set default font for fixed width font.
            if (!PDFGlobal.SetDefaultFont("", "AR PL SungtiL GB", true) && fname != null)
                PDFGlobal.SetDefaultFont("", fname, true);
            // set default font for non-fixed width font.
            if (!PDFGlobal.SetDefaultFont("", "AR PL SungtiL GB", false) && fname != null)
                PDFGlobal.SetDefaultFont("", fname, false);

            if (!PDFGlobal.SetAnnotFont("AR PL SungtiL GB") && fname != null)
                PDFGlobal.SetAnnotFont(fname);

            PDFGlobal.LoadStdFont(0, inst_path + "\\Assets\\font\\0");
            PDFGlobal.LoadStdFont(1, inst_path + "\\Assets\\font\\1");
            PDFGlobal.LoadStdFont(2, inst_path + "\\Assets\\font\\2");
            PDFGlobal.LoadStdFont(3, inst_path + "\\Assets\\font\\3");
            PDFGlobal.LoadStdFont(4, inst_path + "\\Assets\\font\\4");
            PDFGlobal.LoadStdFont(5, inst_path + "\\Assets\\font\\5");
            PDFGlobal.LoadStdFont(6, inst_path + "\\Assets\\font\\6");
            PDFGlobal.LoadStdFont(7, inst_path + "\\Assets\\font\\7");
            PDFGlobal.LoadStdFont(8, inst_path + "\\Assets\\font\\8");
            PDFGlobal.LoadStdFont(9, inst_path + "\\Assets\\font\\9");
            PDFGlobal.LoadStdFont(10, inst_path + "\\Assets\\font\\10");
            PDFGlobal.LoadStdFont(11, inst_path + "\\Assets\\font\\11");
            PDFGlobal.LoadStdFont(12, inst_path + "\\Assets\\font\\12");
            PDFGlobal.LoadStdFont(13, inst_path + "\\Assets\\font\\13");

            //============================================================================
            //Please read this CAREFULLY
            //We are using following method to get your product ID
            //Windows.Phone.Management.Deployment.InstallationManager.FindPackagesForCurrentPublisher().First().Id.ProductId;
            //The method returns product ID with ALL LETTERS IN CAPITAL CASE. And our activation mechanism is case sensitive.
            //So, please make sure that YOU REGISTER LICENSE WITH PRODUCT ID RETURNED FROM THIS METHOD
            //============================================================================
            //

            //Register standard license
            //bool r = PDFGlobal.ActiveLicense(0, "Radaee", "Radaeepdf@gmail.com", "JBRA9T-Y0DIRR-H3CRUZ-WAJQ9H-ZEC20W-2XZ21C");

            //Register professional license
            //bool r = PDFGlobal.ActiveLicense(1, "Radaee", "Radaeepdf@gmail.com", "E7GTWR-BJIMZV-H3CRUZ-WAJQ9H-ZEC20W-2XZ21C");

            //Register premium license

            Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
            String str = package.Id.Name;

            bool rd = PDFGlobal.ActiveLicense(2, "Radaee", "Radaeepdf@gmail.com", "NVFWCW-ESABJE-H3CRUZ-WAJQ9H-ZEC20W-2XZ21C");
            if (!rd)
            {
                rd = PDFGlobal.ActiveLicense(2, "Radaee", "Radaeepdf@gmail.com", "GVYMZC-D1UBM3-H3CRUZ-WAJQ9H-ZEC20W-2XZ21C");
            }

            Frame rootFrame = CreateRootFrame(); //Window.Current.Content as Frame;
            await RestoreStatusAsync(e.PreviousExecutionState);

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private Frame CreateRootFrame()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        private async Task RestoreStatusAsync(ApplicationExecutionState previousExecutionState)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            ContinuationManager = new ContinuationManager();
            Frame rootFrame = CreateRootFrame();
            await RestoreStatusAsync(e.PreviousExecutionState);

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage));
            }

            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                //Frame scenarioFrame = MainPage.Current.FindName("MainPage") as Frame;
                if (MainPage.Current.Frame != null)
                {
                    // Call ContinuationManager to handle continuation activation
                    ContinuationManager.Continue(continuationEventArgs, MainPage.Current.Frame);
                }
            }

            Window.Current.Activate();
        }
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
#region Author

// Author ILYA GOLOVACH (aka IngweLand)
// http://ingweland.com
// ingweland@gmail.com
// Created: 03 12 2016

#endregion

#region

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Xamarin.Forms.Platform.Android;

#endregion

namespace PND.Android
{
    [Activity(Label = "PND", MainLauncher = true,
         ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity, IDialogInterfaceOnCancelListener
    {
        private const int PlayServiceErrorDialogRequestCode = 1;

        public void OnCancel(IDialogInterface dialog)
        {
            //handle the scenario when user canceled Google Play Service error dialog
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());

            //we should check if Google Play Service is available. It is required for push notifications
            if (checkPlayServiceIsAvailable())
            {
                var intent = new Intent(this, typeof(GcmRegistrationIntentService));
                StartService(intent);
            }
        }

        private bool checkPlayServiceIsAvailable()
        {
            //check if Google play service is available
            var apiInstance = GoogleApiAvailability.Instance;
            var resultCode = apiInstance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                // Google Play Service check failed - display the error to the user:
                if (apiInstance.IsUserResolvableError(resultCode))
                {
                    //show automatic dialog when there are problems with Google play services
                    //the dialog will contain appropriate message and possible resolution
                    //(like button which will either open play store to download google play service apk
                    //or open settings to enable google play services)
                    //Callback will process cases when user cancels that dialog
                    //
                    //https://developers.google.com/android/reference/com/google/android/gms/common/GoogleApiAvailability#getErrorDialog(android.app.Activity, int, int, android.content.DialogInterface.OnCancelListener)
                    apiInstance.GetErrorDialog(this, resultCode, PlayServiceErrorDialogRequestCode, this).Show();
                }
                else
                {
                    //the app cannot resolve the issue with google play services if we came here
                    //technically, you may call Finish() to close your app completely
                    //however, it's unlikely scenario in your case, because the absence of push notifications in your app may not be critical 
                    //
                    //Finish();
                }
                return false;
            }

            //google play services found. everything is ok.
            return true;
        }
    }
}
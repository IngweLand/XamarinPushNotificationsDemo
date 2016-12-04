#region Author

// Author ILYA GOLOVACH (aka IngweLand)
// http://ingweland.com
// ingweland@gmail.com
// Created: 03 12 2016

#endregion

#region

using System;
using System.Diagnostics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

#endregion

namespace PND.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
            LoadApplication(new App());

            //register for remote notifications
            //this code will trigger the alert where the user can accept/decline showing the notifications
            var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert | UIUserNotificationType.Badge |
                UIUserNotificationType.Sound,
                null);
            UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            //we can detect if the app was launched by tapping on notification in case the app itself
            //was forced closed previously
            //In such case options dictionary will contain the contents of notification
            //Note, that RegisteredForRemoteNotifications() will also be called in such scenario
            if (options != null)
            {
                NSDictionary notification =
                    options.ObjectForKey(UIApplication.LaunchOptionsRemoteNotificationKey) as NSDictionary;
                if (notification != null)
                {
                    var avAlert = new UIAlertView("Notification from closed", "", null, "OK", null);
                    avAlert.Show();
                }
            }

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application,
            NSData deviceToken)
        {
            var token = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(token))
            {
                token = token.Trim('<').Trim('>');
            }

            Debug.WriteLine($"<|> {token}");

            //you may handle it here or more logically in Shared project
            //where you will also handle sending similar token from Android app

            //You may wish to implement some logic here which will update the remote app server only when token has been changed
            //You may store obtained token on the device, and check new token against stored one each time when this method is called
            //Then you will update your app server with new token if required.
            //This also can be placed in shared code
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            //Process push notification token registration failure here
            //Display alert, log or simply ignore
        }

        public override void DidReceiveRemoteNotification(UIApplication application,
            NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            //This method is called whenever remote notification is received by the app
            //It is called in both scenarios: when the app is in foreground or background
            //You may get the contents of received notification in this method if required
            //
            //Following code shows how to process notification from foreground or background.
            //The payload of notification is: {"aps":{"alert":"Testing.. (27)","badge":15,"sound":"default"}}

            //get the whole notification payload
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

            //extract alert text content
            string alert = string.Empty;
            if (aps.ContainsKey(new NSString("alert")))
                alert = (aps[new NSString("alert")] as NSString).ToString();

            //show alert if we found some text
            if (!string.IsNullOrEmpty(alert))
            {
                UIAlertView avAlert;

                //This is the most common way of getting the state of the app (whether it went from background or was in foreground
                //However, some people reports that it's not ideal in few edge cases
                //Thus, you may need to implement more dofisticated logic to get the current app state (and previous state)
                //You may include this into shared project because Android app also requires handling if these scenarios
                var state = application.ApplicationState;
                if (state == UIApplicationState.Inactive || state == UIApplicationState.Background)
                {
                    avAlert = new UIAlertView("Notification from  background", alert, null, "OK", null);
                }
                else
                {
                    avAlert = new UIAlertView("Notification from  foreground", alert, null, "OK", null);
                }

                avAlert.Show();
            }

            //Unlike Android, iOS allows setting icon badge directly from push notification
            //If you want/need to set it manually, do following
            //application.ApplicationIconBadgeNumber = 88;
        }
    }
}
#region Author

// Author ILYA GOLOVACH (aka IngweLand)
// http://ingweland.com
// ingweland@gmail.com
// Created: 04 12 2016

#endregion

#region

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Media;
using Android.Net;
using Android.OS;

#endregion

namespace PND.Android
{
   [Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        public override void OnMessageReceived(string from, Bundle data)
        {
            // get notification data
            //we should use keys of our JSON payload here.
            //for exemple, following code will get the the value of "message" key from JSON payload:
            //data:{"message":"This is my message"}
            //
            //The payload can be quite complex. Please, read more about it's structure at
            //https://developers.google.com/cloud-messaging/http-server-ref
            var message = data.GetString("message");

            //you can perform any other notification processing here
            //technically, not all remote notifications should result in visible notifaction UI
            //some of them may trigger other actions in your app
            //or the data from them may be stored in database etc

            //Android does not have default mechanisms of not showing notifications when the app is in foreground
            //You should implement such procedure manually.
            //For example, you may store some flag when you app goes to foreground/background (possibly in shared project)
            //Then you will check this flag here and decide if you need to show the notification or skip it

            // Show notification UI if required
            //we are passing just single string here, but onviously can create more complex method, with multiple arguments if required
            showNotification(message);
        }

        private void showNotification(string message)
        {
            //You can build quite complex notification, or absolutely simple one, depending on your needs
            //Following example created simple notification, with icon, title and content text.
            //However, there are posibilities to add more UI controls, like buttons, text inputs etc.
            //There are also options to bundle similar notifications, or to update the notification (if the user has not dismissed it yet)
            //You may find more at: https://developer.android.com/guide/topics/ui/notifiers/notifications.html

            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            //set sound if you want
            //Uri defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_android_white_24dp)
                //You can construct the title from the notification payload
                //This is a good place to set the equivalent of badge number
                //for example, title could be "25 Unread messages"
                .SetContentTitle("Awesome title")
                .SetContentText(message)
                .SetAutoCancel(true)
                //.SetSound(defaultSoundUri)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            notificationManager.Notify(0 /* This is notification ID. You may need it (and store it) if you want to manipulate this notification later */,
                notificationBuilder.Build());
        }
    }
}
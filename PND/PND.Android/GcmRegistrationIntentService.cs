#region Author

// Author ILYA GOLOVACH (aka IngweLand)
// http://ingweland.com
// ingweland@gmail.com
// Created: 03 12 2016

#endregion

#region

using System;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;

#endregion

namespace PND.Android
{
   [Service (Exported = false)]
    public class GcmRegistrationIntentService : IntentService
    {
        //sender id is project number in google-service.json
        //or it could be found in the Cloud Messaging tab of the Firebase console Settings pane
        //https://console.firebase.google.com/project/_/settings/cloudmessaging
        private const string SenderId = "";
        private static object thisLock = new object();
        private static String[] Topics = {"global"};

        public GcmRegistrationIntentService() : base("GcmRegistrationIntentService")
        {
        }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                lock (thisLock)
                {
                    var instanceId = InstanceID.GetInstance(this);
                    var token = instanceId.GetToken(
                        SenderId, GoogleCloudMessaging.InstanceIdScope, null);

                    sendRegistrationTokenToAppServer(token);

                    //subscribe to topic messages here as well
                    //however, it may not be the use-case of your app
                    //subscribeToTopics(token);
                }
            }
            catch (Exception e)
            {
                //handle exception somehow, based on the needs of your app
                //you may just log it or notify the user that notifications will not come
            }
        }

        private void sendRegistrationTokenToAppServer(string token)
        {
            Debug.WriteLine($"<|> {token}");

            //you may handle it here or more logically in Shared project
            //where you will also handle sending similar token from iOS app
        }

        void subscribeToTopics(string token)
        {
            foreach (var topic in Topics)
            {
                var cgmPubSub = GcmPubSub.GetInstance(this);
                cgmPubSub.Subscribe(token, "/topics/" + topic, null);
            }
        }
    }
}
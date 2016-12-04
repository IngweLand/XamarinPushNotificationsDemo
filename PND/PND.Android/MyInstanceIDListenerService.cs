#region Author

// Author ILYA GOLOVACH (aka IngweLand)
// http://ingweland.com
// ingweland@gmail.com
// Created: 03 12 2016

#endregion

#region

using Android.App;
using Android.Content;
using Android.Gms.Gcm.Iid;

#endregion

namespace PND.Android
{
   [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    public class MyInstanceIDListenerService : InstanceIDListenerService
    {
        public override void OnTokenRefresh()
        {
            //we received new token. let's process it
            var intent = new Intent(this, typeof(GcmRegistrationIntentService));
            StartService(intent);
        }
    }
}
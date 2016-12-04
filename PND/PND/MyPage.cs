using System;
using Xamarin.Forms;

namespace PND
{
    class MyPage : ContentPage
    {
        public MyPage()
        {
            var lbl = new Label
            {
                Text = "Ready",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            this.Content = lbl;
        }
    }
}

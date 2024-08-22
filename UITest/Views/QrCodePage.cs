using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace UITest.Views
{
    public class QrCodePage : ContentPage
    {
        public QrCodePage(string msg, string name)
        {
            var layout = new StackLayout
            {
                Spacing = 10,
                Padding = new Thickness(10, 20, 0, 0)
            };
            BarcodeGeneratorView barcode = new BarcodeGeneratorView
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 300,
                WidthRequest = 300,
            };
            barcode.Format = BarcodeFormat.QrCode;
            barcode.Value = msg;

            var qrcodeTitle = new Label()
            {
                Text = UITest.Properties.Resources.action_id_generateqrcode + ": " + name,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            var button = new Button
            {
                Text = UITest.Properties.Resources.alert_id_ok,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(20, 10, 20, 10)
            };
            button.Clicked += async (sender1, e1) => { await Navigation.PopModalAsync(); };

            layout.Children.Add(qrcodeTitle);
            layout.Children.Add(barcode);
            layout.Children.Add(button);
            Content = layout;
        }
    }
}

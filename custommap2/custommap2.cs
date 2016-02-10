using System;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace custommap2
{
    public class App : Application
    {
        public EventHandler<PositionErrorEventArgs> PositionError;
        public EventHandler<PositionEventArgs> PositionChanged;

        public LatLong MyPosition { get; set; }

        public Position CurrentPosition { get; set; }
        public static App Self { get; private set; }

        public static Size ScreenSize { get; set; }

        public Color FaddedGreyText { get; private set; } = Color.FromRgb(218, 218, 218);
        public Color Grey { get; private set; } = Color.FromRgb(85, 85, 85);

        public App()
        {
            App.Self = this;

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = locator.GetPositionAsync(timeoutMilliseconds: 10000).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    if (MyPosition == null)
                        MyPosition = new LatLong(t.Result.Latitude, t.Result.Longitude);
                }
            }).ConfigureAwait(true);

            PositionChanged += (object sender, PositionEventArgs e) =>
            {
                MyPosition.UpdatePosition(e.Position.Latitude, e.Position.Longitude);
                MessagingCenter.Send<App>(this, "LocChange");
            };

            PositionError += (object sender, PositionErrorEventArgs e) =>
            {
                MessagingCenter.Send<App, string>(this, "LocError", e.Error.ToString());
            };

            MainPage = new NavigationPage(new MappingPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}


using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace custommap2
{
    public class MappingPage : ContentPage
    {
        CustomMap map;
        Pin pin;
        bool moveMapToUserLocationOnNextUpdate = false, uicompleted = false;
        Distance DefaultMapDistance = Distance.FromMiles(.05);

        readonly double zoom = 360 / (Math.Pow(2, 4)); // 4 == zoom level

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CrossGeolocator.Current.StartListeningAsync(1, 50, false);

            MessagingCenter.Subscribe<App, string>(this, "LocError", async (s, e) => await DisplayAlert("Mapping Error", e, "OK"));
            MessagingCenter.Subscribe<App>(this, "LocChange", (t) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    map.MoveToRegion(new MapSpan(new Position(App.Self.MyPosition.Latitude, App.Self.MyPosition.Longitude),
                                                 53.43800, 2.96764));
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CrossGeolocator.Current.StopListeningAsync();

            MessagingCenter.Unsubscribe<App, string>(this, "LocError");
            MessagingCenter.Unsubscribe<App>(this, "LocChange");
        }

        void MoveMapTo(LatLong location)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), DefaultMapDistance));
        }

        void OnMoveToUserLocation()
        {
            var lastKnownPosition = App.Self.MyPosition.LastKnownLocation();

            if (lastKnownPosition != null)
                MoveMapTo(lastKnownPosition);
            else
                moveMapToUserLocationOnNextUpdate = true;
        }

        public MappingPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            if (Device.OS == TargetPlatform.iOS)
                Padding = new Thickness(0, 20, 0, 0);

            CreateUI();
        }

        void CreateUI()
        {
            if (App.Self.MyPosition == null)
                App.Self.MyPosition = new LatLong(53.43800, 2.96764);

            var userLocation = App.Self.MyPosition.LastKnownLocation();
            moveMapToUserLocationOnNextUpdate = true;

            var initialLocation = userLocation ?? new LatLong(53.43800, 2.96764);
            var mapStartPosition = new Position(initialLocation.Latitude, initialLocation.Longitude);

            BackgroundColor = App.Self.FaddedGreyText;

            map = new CustomMap(new MapSpan(mapStartPosition, zoom, zoom))
            {
                HasZoomEnabled = true,
                HasScrollEnabled = true,
                HeightRequest = App.ScreenSize.Height * .35,
                WidthRequest = App.ScreenSize.Width,
                MapType = MapType.Hybrid,
                IsShowingUser = true,
            };
            map.MoveToRegion(MapSpan.FromCenterAndRadius(mapStartPosition, DefaultMapDistance));

            var imgPin = new Image
            {
                Source = "pin.png",
                HeightRequest = 30,
                WidthRequest = 30,
            };
            var mapXMidPos = (App.ScreenSize.Width / 2) - (imgPin.WidthRequest / 2);
            var mapYMidPos = (map.HeightRequest / 2) - (imgPin.HeightRequest);

            var imgPointer = new Image
            {
                Source = "mylocation.png",
                WidthRequest = 30,
                HeightRequest = 30,
            };

            var tapPointer = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command((a) =>
                {
                    OnMoveToUserLocation();
                })
            };
            imgPointer.GestureRecognizers.Add(tapPointer);

            var relLayout = new RelativeLayout
            {
                WidthRequest = App.ScreenSize.Width,
                HeightRequest = App.ScreenSize.Height * .35
            };
            relLayout.Children.Add(map, Constraint.Constant(0), Constraint.Constant(0));
            relLayout.Children.Add(imgPin, Constraint.Constant(mapXMidPos), Constraint.Constant(mapYMidPos));
            relLayout.Children.Add(new CustomRoundedBox { CornerRadius = 5, Color = Color.White, WidthRequest = 35, HeightRequest = 35, Opacity = .5 },
                                       Constraint.Constant(App.ScreenSize.Width - 44),
                                       Constraint.Constant(map.HeightRequest - 43));
            relLayout.Children.Add(imgPointer, Constraint.Constant(App.ScreenSize.Width - 40),
                                       Constraint.Constant(map.HeightRequest - 40));

            int s = 0, pad = 0, up = -8;
            double mult = .4;
            double heightMutl = .45;

            Device.OnPlatform(iOS: () =>
            {
                if (App.ScreenSize.Height > 568)
                {
                    s = 0;
                    pad = 8;
                    heightMutl = .42;
                }
                else if (App.ScreenSize.Height == 568)
                {
                    s = 4;
                    pad = 4;
                    heightMutl = .4;
                }
                else
                {
                    s = 6;
                    pad = 8;
                    mult = .38;
                    heightMutl = .34;
                    up = -4;
                }
            },
            Android: () =>
            {
                s = 2;
                pad = 4;
                heightMutl = .42;
            });


            var lblCarpark = new Label
            {
                Text = "You are here",
                FontSize = 26 - s,
                TextColor = Color.Blue,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var lblDragMap = new Label
            {
                Text = "Drag to position",
                FontSize = 14 - s + 2,
                TextColor = Color.Blue,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
            };

            var lblConfirm = new Label
            {
                Text = "Confirm",
                FontSize = 12 - s + 3,
                WidthRequest = App.ScreenSize.Width * mult,
                HeightRequest = 28,
                TextColor = Color.Blue,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            map.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "VisibleRegion" && uicompleted)
                {
                    var mp = sender as Map;
                    if (map.VisibleRegion != null)
                    {
                        // do something with the change of position
                    }
                }
            };

            var tapConfirm = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() => SaveLocation())
            };
            lblConfirm.GestureRecognizers.Add(tapConfirm);

            var lblGPSMessage = new Label
            {
                Text = "GPS warning",
                FontSize = 14 - s,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.End,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = App.Self.Grey
            };

            var midStack = new StackLayout()
            {
                WidthRequest = App.ScreenSize.Width * .8,
                HeightRequest = App.ScreenSize.Height * .45,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = App.Self.FaddedGreyText,
                Children =
            {
                new StackLayout
                {
                    Padding = new Thickness(0, pad, 0, pad),
                    Children = { lblCarpark }
                },
                new StackLayout
                {
                    Padding = new Thickness(0, pad),
                    Children = { lblDragMap }
                },
                new StackLayout
                {
                    Padding = new Thickness(0, pad, 0, 0),
                    Children = { lblConfirm }
                },
            }
            };

            var mainStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = App.Self.FaddedGreyText,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = App.ScreenSize.Height,
                Children =
            {
                new StackLayout
                {
                    HeightRequest = App.ScreenSize.Height * .4,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    Children =
                    {
                                   new StackLayout
                        {
                            Padding = new Thickness(0, -6, 0, 8),
                            Children = { relLayout }
                        },
                    }
                },
                midStack,
                new StackLayout
                {
                    VerticalOptions = LayoutOptions.End,
                    BackgroundColor = App.Self.FaddedGreyText,
                    Padding = new Thickness(0, up, 0, 0),
                    Children = { lblGPSMessage }
                }
            }
            };

            Content = mainStack;
            uicompleted = true;
        }

        void SaveLocation()
        {
            var location = new LatLong(map.VisibleRegion.Center.Latitude, map.VisibleRegion.Center.Longitude);

            // save the location to the internal settings
        }
    }
}
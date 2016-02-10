using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using custommap2;
using custommap2.Droid;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace custommap2.Droid
{
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        bool isDrawnDone;
        GoogleMap map;
        CustomMap formsMap;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            var mapView = Control as MapView;
            if (mapView == null)
                return;
            formsMap = Element as CustomMap;

            if (formsMap != null && map == null)
            {
                mapView.GetMapAsync(this);
            }
        }

        void CreateMarker()
        {
            var markerWithIcon = new MarkerOptions();

            var id = UIFileUtils.GetResourceIdFromFilename("pin.png");

            markerWithIcon.SetPosition(new LatLng(formsMap.MapPinLocation.Latitude, formsMap.MapPinLocation.Longitude));
            markerWithIcon.SetTitle("A blue pin");
            markerWithIcon.SetSnippet(string.Empty);

            markerWithIcon.SetIcon(BitmapDescriptorFactory.FromResource(id));
            map.MyLocationEnabled = formsMap.IsShowingUser;
            map.SetIndoorEnabled(false);

            try
            {
                map.AddMarker(markerWithIcon);
            }
            catch (NullPointerException ex)
            {
                System.Console.WriteLine("Exception : {0}--{1}", ex.Message, ex.InnerException);
            }
            isDrawnDone = true;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.UiSettings.MyLocationButtonEnabled = false;
            map.UiSettings.ZoomControlsEnabled = false;
            CreateMarker();
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (changed)
            {
                isDrawnDone = false;
            }
        }
    }
}


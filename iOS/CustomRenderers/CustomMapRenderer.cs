using System.Drawing;
using CoreGraphics;
using CoreLocation;
using custommap2;
using custommap2.iOS;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace custommap2.iOS
{
    public class CustomMapRenderer : ViewRenderer<CustomMap, MKMapView>
    {
        MKMapView mkMapView;
        CustomMap map;
        MKPointAnnotation annotation = null;

        protected override void OnElementChanged(ElementChangedEventArgs<CustomMap> e)
        {
            base.OnElementChanged(e);
            map = e.NewElement;
            if (e.NewElement == null)
                return;

            SetNativeControl(new MKMapView(CGRect.Empty));
            var mapModel = Element;
            mkMapView = Control;

            mkMapView.RegionChanged += delegate
            {
                if (Element == null)
                    return;
                mapModel.VisibleRegion = new MapSpan(new Position(mkMapView.Region.Center.Latitude, mkMapView.Region.Center.Longitude), mkMapView.Region.Span.LatitudeDelta, mkMapView.Region.Span.LongitudeDelta);
            };


            mkMapView.GetViewForAnnotation += (MKMapView mapView, IMKAnnotation annotation) =>
            {
                MKAnnotationView anView = null;

                if (annotation is MKPointAnnotation)
                {
                    anView = (MKAnnotationView)mapView.DequeueReusableAnnotation(annotationIdentifier);

                    if (anView == null)
                        anView = new MKAnnotationView(annotation, annotationIdentifier);

                    anView.Image = GetImage("pin.png");
                    anView.CanShowCallout = true;
                    anView.CenterOffset = new CGPoint(0, -14f);
                }
                return anView;
            };

            if (annotation == null)
            {
                annotation = new MKPointAnnotation();
                mkMapView.AddAnnotation(annotation);
            }
            var currentPinLocation = map.MapPinLocation;
            annotation.Coordinate = new CLLocationCoordinate2D(currentPinLocation.Latitude, currentPinLocation.Longitude);
            mkMapView.Region = new MKCoordinateRegion(
                new CLLocationCoordinate2D(currentPinLocation.Latitude, currentPinLocation.Longitude),
                new MKCoordinateSpan(0.001, 0.001)
            );

            MessagingCenter.Subscribe<Map, MapSpan>(this, "MapMoveToRegion", MapMoveToRegion, mapModel);
            if (mapModel.LastMoveToRegion != null)
            {
                MoveToRegion(mapModel.LastMoveToRegion);
            }

            Control.ZoomEnabled = Element.HasZoomEnabled;
            Control.ShowsUserLocation = Element.IsShowingUser;
            Control.MapType = MKMapType.Hybrid;
            Control.UserInteractionEnabled = true;
        }

        private void MoveToRegion(MapSpan mapSpan)
        {
            var center = mapSpan.Center;
            var region = new MKCoordinateRegion(new CLLocationCoordinate2D(center.Latitude, center.Longitude), new MKCoordinateSpan(mapSpan.LatitudeDegrees, mapSpan.LongitudeDegrees));
            Control.SetRegion(region, true);
        }

        void MapMoveToRegion(Map map, MapSpan span)
        {
            MoveToRegion(span);
        }

        protected string annotationIdentifier = "PinAnnotation";

        UIImage GetImage(string imageName)
        {
            var image = new UIImageView
            {
                Image = new UIImage(imageName).Scale(new SizeF { Height = 30, Width = 20 }),
            };
            return image.Image;
        }
    }

}



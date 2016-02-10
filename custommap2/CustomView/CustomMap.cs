using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace custommap2
{
    public class CustomMap : Map
    {
        public static readonly BindableProperty MapPinLocationProperty =
            BindableProperty.Create<CustomMap, LatLong>(p => p.MapPinLocation, new LatLong(53.4299, -2.9615));

        public LatLong MapPinLocation
        {
            get {
                return (LatLong)GetValue(MapPinLocationProperty);
            }
            set {
                SetValue(MapPinLocationProperty, value);
            }
        }

        private MapSpan visibleRegion;

        public MapSpan LastMoveToRegion { get; private set; }

        public new MapSpan VisibleRegion
        {
            get { return visibleRegion; }
            set {
                if (visibleRegion == value)
                {
                    return;
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                OnPropertyChanging("VisibleRegion");
                visibleRegion = value;
                OnPropertyChanged("VisibleRegion");
            }
        }

        public CustomMap()
        {
        }

        public CustomMap(MapSpan map)
        {
            visibleRegion = map;
        }
    }
}


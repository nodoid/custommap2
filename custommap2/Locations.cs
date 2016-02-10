namespace custommap2
{
    public class LatLong
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public LatLong(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public LatLong()
        { }

        public void UpdatePosition(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public LatLong LastKnownLocation()
        {
            return this;
        }
    }

    public class StoredPostion
    {
        public double Latitude { get; set; } = 53.43800;
        public double Longitude { get; set; } = 2.96764;
    }
}


namespace CargoTrack.WebUI.Models
{
    public class RouteMapViewModel
    {
        public string MapId { get; set; } = "routeMap";
        public double? OriginLat { get; set; }
        public double? OriginLng { get; set; }
        public double? DestinationLat { get; set; }
        public double? DestinationLng { get; set; }
        public string OriginLabel { get; set; }
        public string DestinationLabel { get; set; }
        public int ProgressPercent { get; set; }
        public string AccentHex { get; set; } = "#E8A93B";
    }
}
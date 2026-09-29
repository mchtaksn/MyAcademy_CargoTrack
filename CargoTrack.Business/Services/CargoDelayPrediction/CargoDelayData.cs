using Microsoft.ML.Data;

namespace CargoTrack.Business.Services.CargoDelayPrediction
{
    public class CargoDelayData
    {
        public float Weight { get; set; }
        public float Desi { get; set; }
        public string CargoType { get; set; }
        public bool SameCity { get; set; }
        public float ShipmentDayOfWeek { get; set; }
        public bool Label { get; set; }
    }

    public class CargoDelayPredictionResult
    {
        [ColumnName("PredictedLabel")]
        public bool WillBeDelayed { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
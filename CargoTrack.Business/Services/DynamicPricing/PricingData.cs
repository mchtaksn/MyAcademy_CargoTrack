using Microsoft.ML.Data;

namespace CargoTrack.Business.Services.DynamicPricing
{
    public class PricingInputData
    {
        public float Weight { get; set; }
        public float Desi { get; set; }
        public float DistanceKm { get; set; }
        public float IsPeakSeason { get; set; } // 1.0 = Yoğun Dönem (Efsane Cuma, Bayram vb.), 0.0 = Normal
        public float CargoTypeFactor { get; set; } // 1.0 = Standart, 1.5 = Acil, 1.8 = Kırılabilir

        [ColumnName("Score")]
        public float Price { get; set; } // Regression Hedef Değişkeni (TL)
    }

    public class PricingPredictionResult
    {
        [ColumnName("Score")]
        public float PredictedPrice { get; set; }
    }
}
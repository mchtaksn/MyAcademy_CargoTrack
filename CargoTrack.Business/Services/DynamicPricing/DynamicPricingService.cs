using Microsoft.AspNetCore.Hosting;
using Microsoft.ML;

namespace CargoTrack.Business.Services.DynamicPricing
{
    public interface IDynamicPricingService
    {
        void TrainModel(List<PricingInputData> trainingData);
        float PredictPrice(float weight, float desi, float distanceKm, bool isPeakSeason, string cargoType);
    }

    public class DynamicPricingService : IDynamicPricingService
    {
        private readonly string _modelPath;
        private static readonly MLContext _mlContext = new(seed: 0);
        private static ITransformer? _model;
        private static PredictionEngine<PricingInputData, PricingPredictionResult>? _predictionEngine;

        public DynamicPricingService(IWebHostEnvironment env)
        {
            _modelPath = Path.Combine(env.ContentRootPath, "App_Data", "dynamicPricingModel.zip");

            if (_model is null && File.Exists(_modelPath))
            {
                using var stream = File.OpenRead(_modelPath);
                _model = _mlContext.Model.Load(stream, out _);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<PricingInputData, PricingPredictionResult>(_model);
            }
        }

        public void TrainModel(List<PricingInputData> trainingData)
        {
            var trainDataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(PricingInputData.Weight),
                    nameof(PricingInputData.Desi),
                    nameof(PricingInputData.DistanceKm),
                    nameof(PricingInputData.IsPeakSeason),
                    nameof(PricingInputData.CargoTypeFactor))
                .Append(_mlContext.Regression.Trainers.FastTree());

            _model = pipeline.Fit(trainDataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<PricingInputData, PricingPredictionResult>(_model);

            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);
            using var saveStream = File.Create(_modelPath);
            _mlContext.Model.Save(_model, trainDataView.Schema, saveStream);
        }

        public float PredictPrice(float weight, float desi, float distanceKm, bool isPeakSeason, string cargoType)
        {
            if (_predictionEngine is null)
            {
                // Model eğitilmemişse varsayılan kural tabanlı bir fiyat döndür
                float basePrice = 50f + (weight * 5f) + (distanceKm * 0.8f);
                return isPeakSeason ? basePrice * 1.25f : basePrice;
            }

            float cargoFactor = cargoType switch
            {
                "Urgent" => 1.5f,
                "Fragile" => 1.8f,
                "Heavy" => 2.0f,
                _ => 1.0f
            };

            var input = new PricingInputData
            {
                Weight = weight,
                Desi = desi,
                DistanceKm = distanceKm,
                IsPeakSeason = isPeakSeason ? 1.0f : 0.0f,
                CargoTypeFactor = cargoFactor
            };

            var result = _predictionEngine.Predict(input);
            return Math.Max(result.PredictedPrice, 30.0f); // Minimum taban fiyat 30 TL
        }
    }
}
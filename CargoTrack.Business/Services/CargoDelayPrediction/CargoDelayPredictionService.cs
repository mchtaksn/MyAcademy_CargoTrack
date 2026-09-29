using CargoTrack.DataAccess.Repositories.CargoMovements;
using CargoTrack.DataAccess.Repositories.Cargos;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Business.Services.CargoDelayPrediction
{
    public class CargoDelayPredictionService : ICargoDelayPredictionService
    {
        private readonly ICargoRepository _cargoRepository;
        private readonly ICargoMovementRepository _cargoMovementRepository;
        private readonly string _modelPath;

        private static readonly MLContext _mlContext = new(seed: 0);
        private static ITransformer? _model;
        private static PredictionEngine<CargoDelayData, CargoDelayPredictionResult>? _predictionEngine;
        private static int _lastTrainingDataCount;

        public bool IsModelReady => _model is not null;
        public int LastTrainingDataCount => _lastTrainingDataCount;

        public CargoDelayPredictionService(
            ICargoRepository cargoRepository,
            ICargoMovementRepository cargoMovementRepository,
            IWebHostEnvironment env)
        {
            _cargoRepository = cargoRepository;
            _cargoMovementRepository = cargoMovementRepository;
            _modelPath = Path.Combine(env.ContentRootPath, "App_Data", "cargoDelayModel.zip");

            if (_model is null && File.Exists(_modelPath))
            {
                using var stream = File.OpenRead(_modelPath);
                _model = _mlContext.Model.Load(stream, out _);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<CargoDelayData, CargoDelayPredictionResult>(_model);
            }
        }

        public async Task<int> TrainAsync()
        {
            var data = await BuildTrainingDataAsync();

            if (data.Count < 10)
            {
                throw new ValidationException(
                    $"Model eğitmek için yeterli veri yok. En az 10 teslim edilmiş kargo gerekli, şu an {data.Count} tane var.");
            }

            var trainData = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding("CargoTypeEncoded", nameof(CargoDelayData.CargoType))
                .Append(_mlContext.Transforms.Conversion.ConvertType("SameCityEncoded", nameof(CargoDelayData.SameCity), Microsoft.ML.Data.DataKind.Single))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    nameof(CargoDelayData.Weight),
                    nameof(CargoDelayData.Desi),
                    "CargoTypeEncoded",
                    "SameCityEncoded",
                    nameof(CargoDelayData.ShipmentDayOfWeek)))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            _model = pipeline.Fit(trainData);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<CargoDelayData, CargoDelayPredictionResult>(_model);
            _lastTrainingDataCount = data.Count;

            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);
            using var saveStream = File.Create(_modelPath);
            _mlContext.Model.Save(_model, trainData.Schema, saveStream);

            return data.Count;
        }

        public CargoDelayPredictionResult Predict(double weight, double width, double length, double height, string cargoType, bool sameCity, DateTime shipmentDate)
        {
            if (_predictionEngine is null)
            {
                throw new ValidationException("Model henüz eğitilmedi. Önce 'Modeli Eğit' butonuna basın.");
            }

            var desi = Math.Round(width * length * height / 3000, 1);

            var input = new CargoDelayData
            {
                Weight = (float)weight,
                Desi = (float)desi,
                CargoType = cargoType,
                SameCity = sameCity,
                ShipmentDayOfWeek = (float)shipmentDate.DayOfWeek
            };

            return _predictionEngine.Predict(input);
        }

        private async Task<List<CargoDelayData>> BuildTrainingDataAsync()
        {
            var deliveredCargos = await _cargoRepository.GetDeliveredForTrainingAsync();
            var result = new List<CargoDelayData>();

            foreach (var cargo in deliveredCargos)
            {
                var movements = await _cargoMovementRepository.GetByCargoIdAsync(cargo.Id);
                var deliveredMovement = movements.FirstOrDefault(m => m.NewStatus == CargoStatus.Delivered);
                if (deliveredMovement is null) continue;

                var desi = Math.Round(cargo.Width * cargo.Length * cargo.Height / 3000, 1);
                var sameCity = cargo.OriginBranch.CityId == cargo.DestinationBranch.CityId;
                var wasDelayed = deliveredMovement.MovementDate > cargo.EstimatedArrivalDate;

                result.Add(new CargoDelayData
                {
                    Weight = (float)cargo.Weight,
                    Desi = (float)desi,
                    CargoType = cargo.CargoType.ToString(),
                    SameCity = sameCity,
                    ShipmentDayOfWeek = (float)cargo.ShipmentDate.DayOfWeek,
                    Label = wasDelayed
                });
            }

            return result;
        }
    }
}
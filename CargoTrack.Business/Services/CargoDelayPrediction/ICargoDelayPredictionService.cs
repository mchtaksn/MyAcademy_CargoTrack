namespace CargoTrack.Business.Services.CargoDelayPrediction
{
    public interface ICargoDelayPredictionService
    {
        bool IsModelReady { get; }
        int LastTrainingDataCount { get; }
        Task<int> TrainAsync();
        CargoDelayPredictionResult Predict(double weight, double width, double length, double height, string cargoType, bool sameCity, DateTime shipmentDate);
    }
}
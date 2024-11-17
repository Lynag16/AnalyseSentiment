using Microsoft.ML;
using AnalyseSentiment.Models;

namespace AnalyseSentiment.Services
{
    public class SentimentService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly string _yelpDataPath;

        public SentimentService()
        {
            _mlContext = new MLContext();
            _yelpDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "commentdata.txt");
            _model = BuildAndTrainModel();
        }

        public ITransformer BuildAndTrainModel()
        {
            var dataView = _mlContext.Data.LoadFromTextFile<SentimentData>(_yelpDataPath, hasHeader: false, separatorChar: '\t');
            var splitDataView = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            
            var estimator = _mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            
            return estimator.Fit(splitDataView.TrainSet);
        }

        public SentimentPrediction GetPredictionForReview(string reviewContent)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
            var sampleStatement = new SentimentData { SentimentText = reviewContent };
            return predictionEngine.Predict(sampleStatement);
        }
    }
}

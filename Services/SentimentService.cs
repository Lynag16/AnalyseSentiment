using Microsoft.ML;
using AnalyseSentiment.Models;
using System;
using System.IO;

namespace AnalyseSentiment.Services
{
    public class SentimentService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        public SentimentService()
        {
            _mlContext = new MLContext();
            string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "commentdata.txt"); 

            if (!File.Exists(dataPath))
            {
                throw new FileNotFoundException($"The data file '{dataPath}' does not exist.");
            }

            // Load and split data
            var splitDataView = LoadData(dataPath);

            // Train the model
            _model = BuildAndTrainModel(splitDataView.TrainSet);

            if (_model == null)
            {
                Console.WriteLine("Model training failed.");
            }
            else
            {
                Console.WriteLine("Model training completed successfully.");
                var predictions = _model.Transform(splitDataView.TestSet);
                var metrics = _mlContext.BinaryClassification.Evaluate(predictions);

                // Output metrics for evaluation
                Console.WriteLine($"Accuracy: {metrics.Accuracy}");
                Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve}");
                Console.WriteLine($"F1 Score: {metrics.F1Score}");
            }
        }

        private DataOperationsCatalog.TrainTestData LoadData(string dataPath)
        {
            IDataView dataView = _mlContext.Data.LoadFromTextFile<SentimentData>(
                dataPath, hasHeader: false, separatorChar: '\t');

            return _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        }

        //  train the model
        private ITransformer BuildAndTrainModel(IDataView trainData)
        {
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
    
    
    //     var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
    // .Append(_mlContext.BinaryClassification.Trainers.LightGbm(labelColumnName: "Label", featureColumnName: "Features"));




            return pipeline.Fit(trainData);
        }

        // Method to predict sentiment for a given text
        public SentimentPrediction Predict(string text)
        {
            try
            {
                Console.WriteLine($"Predicting sentiment for: {text}");

                var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
                var input = new SentimentData { SentimentText = text };
                var prediction = predictionEngine.Predict(input);
                

                Console.WriteLine($"Sentiment predicted as: {(prediction.Prediction ? "Positive" : "Negative")}, Probability: {prediction.Probability}");

                return prediction;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during prediction: {ex.Message}");
                return null;
            }
        }
    }
}

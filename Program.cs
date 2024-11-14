using Microsoft.ML;
using System;
using System.IO;

using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using WebsiteCommentPredictor;

namespace WebsiteCommentPredictor
{
    public class Program
    {
        static void Main(string[] args)
        {
            string yelpDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "commentdata.txt");
            MLContext mlContext = new MLContext();

            // Charger les data
            var splitDataView = LoadData(mlContext, yelpDataPath);
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);

            
            Console.WriteLine("Enter a comment to analyze sentiment:");
            while (true)
            {
                string userInput = Console.ReadLine();

             
                if (string.IsNullOrEmpty(userInput))
                    break;

             
                GetPredictionForReviewContent(mlContext, model, userInput);

                Console.WriteLine("Enter another comment to analyze sentiment (or press Enter to exit):");
            }
        }

        static DataOperationsCatalog.TrainTestData LoadData(MLContext mlContext, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(
                dataPath,
                hasHeader: false,
                separatorChar: '\t'
            );

            var splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            return splitDataView;
        }

        static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",
                    inputColumnName: nameof(SentimentData.SentimentText))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: "Label",
                    featureColumnName: "Features"));

            ITransformer model = estimator.Fit(splitTrainSet);
            return model;
        }

        static void GetPredictionForReviewContent(MLContext mlContext, ITransformer model, string reviewContent)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            var sampleStatement = new SentimentData
            {
                SentimentText = reviewContent
            };

            var predictionResult = predictionEngine.Predict(sampleStatement);
            Console.WriteLine($"Sentiment predicted as: {(Convert.ToBoolean(predictionResult.Prediction) ? "Positive" : "Negative")} ");
        }
    }
}

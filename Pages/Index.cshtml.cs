//this is Index.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AnalyseSentiment.Models;
using AnalyseSentiment.Services;

namespace AnalyseSentiment.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SentimentService _sentimentService;
        

        public IndexModel(SentimentService sentimentService)
        {
            _sentimentService = sentimentService;
            Console.WriteLine("User input: " + UserInput);
            
        }


        
        [BindProperty]
        public string UserInput { get; set; } = string.Empty; 

        public SentimentPrediction? PredictionResult { get; private set; }

[ValidateAntiForgeryToken]       
public void OnPost()

{
    Console.WriteLine("^^^^^^^^^^^^^^^^^^^^ ");
   
        if (!string.IsNullOrEmpty(UserInput))
        {
         
            Console.WriteLine("Processing input: " + UserInput);

            PredictionResult = _sentimentService.Predict(UserInput);

            Console.WriteLine($"Prediction Result: {PredictionResult?.Prediction}, Probability: {PredictionResult?.Probability}");
        }
        else
        {
            Console.WriteLine("User input is empty.");
        }
    }

    }
}

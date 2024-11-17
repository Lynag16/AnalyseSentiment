using Microsoft.AspNetCore.Mvc;
using AnalyseSentiment.Models;
using AnalyseSentiment.Services;

namespace AnalyseSentiment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SentimentController : ControllerBase
    {
        private readonly SentimentService _SentimentService;

        public SentimentController(SentimentService SentimentService)
        {
            _SentimentService = SentimentService;
        }

        [HttpPost("predict")]
        public ActionResult<SentimentPrediction> PredictSentiment([FromBody] string reviewContent)
        {
            var prediction = _SentimentService.GetPredictionForReview(reviewContent);
            return Ok(new 
            { 
                Sentiment = prediction.Prediction ? "Positive" : "Negative",
                Probability = prediction.Probability 
            });
        }
    }
}

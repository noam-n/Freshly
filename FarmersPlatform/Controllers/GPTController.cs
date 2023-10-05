using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;

namespace FarmersPlatform.Controllers
{
    public class GPTController : Controller
    {


        [HttpPost]
        public IActionResult GetResult(int farmerId, string farmName, string location, string description, string style)
        {
            string apikey = "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";
            string answer = string.Empty;
            var openai = new OpenAIAPI(apikey);
            CompletionRequest completeion = new CompletionRequest();

            string prompt = $"please write a {style} description of my farm which" +
                $" name is {farmName} located in {location}, follow the guidlines of this description:" +
                $"{description}." +
                $"make it no longer than 335 characthers.";


            completeion.Prompt = prompt;
            completeion.Model = OpenAI_API.Models.Model.DavinciText;
            completeion.MaxTokens = 2000;
            var result = openai.Completions.CreateCompletionsAsync(completeion);
            if (result != null)
            {
                foreach (var item in result.Result.Completions)
                {
                    answer = item.Text.Trim();
                }
                return RedirectToAction("PersonalDetails", "Farmer", new { farmerId, answer });
            }
            
            return RedirectToAction("PersonalDetails", "Farmer", new { farmerId });
        }
    }
}

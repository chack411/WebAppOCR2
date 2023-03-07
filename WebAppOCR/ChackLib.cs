using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChackLib
{
    public class OcrLib
    {
        public static async Task<string> DoOcr(Uri imageUrl)
        {
            using (var client = new HttpClient())
            {
                var postData = "{\"url\": \"" + imageUrl.ToString() + "\"}";
                StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");
                var url = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("Vision_API_Subscription_Key"));
                var httpResponse = await client.PostAsync(url, content);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return ConvertToText(await httpResponse.Content.ReadAsStringAsync());
                }
            }
            return null;
        }

        public static async Task<string> DoOcr(Stream image)
        {
            using (var client = new HttpClient())
            {
                var content = new StreamContent(image);
                var url = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("Vision_API_Subscription_Key"));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var httpResponse = await client.PostAsync(url, content);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return ConvertToText(await httpResponse.Content.ReadAsStringAsync());
                }
            }
            return null;
        }

        public static string ConvertToText(string jsonData)
        {
            OcrData ocrData = JsonConvert.DeserializeObject<OcrData>(jsonData);

            OcrResult ocrResult = new OcrResult();

            foreach (Region region in ocrData.Regions)
            {
                foreach (Line line in region.Lines)
                {
                    foreach (Word word in line.Words)
                    {
                        ocrResult.Text += word.Text;
                        if (ocrData.Language != "ja")
                            ocrResult.Text += " ";
                    }
                    ocrResult.Text += "\n";
                }
            }

            return ocrResult.Text;
        }
    }

    public class OcrData
    {
        public string Language { get; set; }
        public string Orientation { get; set; }
        public List<Region> Regions { get; set; }
    }

    public class Region
    {
        public List<Line> Lines { get; set; }
    }

    public class Line
    {
        public List<Word> Words { get; set; }
    }

    public class Word
    {
        public string Text { get; set; }
    }

    public class OcrResult
    {
        public string Text { get; set; }
    }

    public class NoodleFinder
    {
        public static async Task<Prediction> Noodle(Uri imageUrl)
        {
            using (var client = new HttpClient())
            {
                var postData = "{\"Url\": \"" + imageUrl.ToString() + "\"}";
                StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");
                //var url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/5487c67b-7cee-4f68-8cf2-3027c939eb66/url?iterationId=3798ed34-4f42-43f0-b7c9-d506b0630410";
                var url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/650711c1-cd6e-4e11-85e8-34f4bc82d0af/classify/iterations/Iteration1/url";
                client.DefaultRequestHeaders.Add("Prediction-Key", Environment.GetEnvironmentVariable("Noodle_Prediction_Key"));
                var httpResponse = await client.PostAsync(url, content);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return GetTopPredictionResult(await httpResponse.Content.ReadAsStringAsync());
                }
            }
            return null;
        }

        public static async Task<Prediction> Noodle(Stream image)
        {
            using (var client = new HttpClient())
            {
                var content = new StreamContent(image);
                //var url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/5487c67b-7cee-4f68-8cf2-3027c939eb66/image?iterationId=3798ed34-4f42-43f0-b7c9-d506b0630410";
                var url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/650711c1-cd6e-4e11-85e8-34f4bc82d0af/classify/iterations/Iteration1/image";
                client.DefaultRequestHeaders.Add("Prediction-Key", Environment.GetEnvironmentVariable("Noodle_Prediction_Key"));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var httpResponse = await client.PostAsync(url, content);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return GetTopPredictionResult(await httpResponse.Content.ReadAsStringAsync());
                }
            }
            return null;
        }

        public static Prediction GetTopPredictionResult(string jsonData)
        {
            NoodlePrediction NoodleData = JsonConvert.DeserializeObject<NoodlePrediction>(jsonData);

            Prediction result = new Prediction
            {
                TagName = "",
                Probability = 0.0F
            };

            foreach (Prediction prediction in NoodleData.Predictions)
            {
                if (prediction.Probability > result.Probability)
                {
                    result = prediction;
                    // result.TagName = "Jiro"; // for debug.
                }
            }

            return result;
        }

        public static string ConvertToText(Prediction result)
        {
            string msg;

            if (result.TagName == "Udon")
                msg = string.Format("うどん です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.TagName == "OkinawaSoba")
                msg = string.Format("沖縄そば です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.TagName == "Soba")
                msg = string.Format("蕎麦 です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.TagName == "Ramen")
                msg = string.Format("ラーメン です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.TagName == "HiyashiChuka")
                msg = string.Format("冷やし中華 です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.TagName == "Jiro")
                msg = string.Format("二郎 です (確度 {0})", result.Probability.ToString("P1"));
            else
                return "判別できませんでした";

            if (result.Probability < 0.6F)
                msg = "たぶん " + msg;

            return msg;
        }
    }

    public class NoodlePrediction
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public Prediction[] Predictions { get; set; }
    }

    public class Prediction
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public float Probability { get; set; }
    }
}

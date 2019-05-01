using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer.Vision.API
{
    public class Image
    {
        private static readonly List<VisualFeatureTypes> features =
    new List<VisualFeatureTypes>()
    {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
    };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imagePath">  @"C:\Test\LocalImage.jpg" if local ,   "https//...jpg" if remote</param>
        /// <param name="subscriptionKey"></param>
        /// <returns></returns>

        public Task Analyze(string imagePath, string subscriptionKey)
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });
            computerVision.Endpoint = "https://westcentralus.api.cognitive.microsoft.com";
            bool isLocal = true; // false if remote
            return analyzeAsync(computerVision, imagePath, isLocal);
        }
        #region private

        private static async Task analyzeAsync(ComputerVisionClient computerVision, string imageUrl, bool isLocal)
        {
            string result = string.Empty;
            ImageAnalysis analysis = new ImageAnalysis();
            if (isLocal)
            {
                if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    return;
                }

                analysis = await computerVision.AnalyzeImageAsync(imageUrl, features);

            }
            else
            {
                if (!File.Exists(imageUrl))
                {
                    return;
                }

                using (Stream imageStream = File.OpenRead(imageUrl))
                {
                    analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, features);

                }
            }
            result = getResult(analysis, imageUrl);
        }

        private static string getResult(ImageAnalysis analysis, string imageUri)
        {
            string result = string.Empty;
            Console.WriteLine(imageUri);
            if (analysis.Description.Captions.Count != 0)
            {
                result = analysis.Description.Captions[0].Text;
            }
            else
            {
                result = "No description detected";
            }
            return result;
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Newbe.Claptrap.DevTools.Translation
{
    public class Translator : ITranslator
    {
        public delegate Translator Factory(string endpoint,
            string route,
            string subscriptionKey);

        private readonly string _endpoint;
        private readonly string _route;
        private readonly string _subscriptionKey;
        private readonly ILogger<Translator> _logger;

        public Translator(
            string endpoint,
            string route,
            string subscriptionKey,
            ILogger<Translator> logger)
        {
            _endpoint = endpoint;
            _route = route;
            _subscriptionKey = subscriptionKey;
            _logger = logger;
        }

        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<TranslationResult[]> TranslateAsync(string sourceText, IEnumerable<string> targetCultures)
        {
            object[] body = {new {Text = sourceText}};
            using var request = new HttpRequestMessage();
            var requestBody = JsonConvert.SerializeObject(body);
            // Build the request.
// Set the method to Post.
            request.Method = HttpMethod.Post;
// Construct the URI and add headers.
            var q = string.Join("&", targetCultures.Select(x => $"to={x}")
                .Concat(new[] {"from=en"}));

            request.RequestUri = new Uri(_endpoint + _route + "&" + q);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", "eastasia");

// Send the request and get response.
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
// Read response as a string.
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
// Deserialize the response using the classes created earlier.
            var deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
            return deserializedOutput;
        }

        public async Task<Dictionary<string, LocalizationFile>> TranslateAsync(LocalizationFile file,
            string[] targetCultures)
        {
            var keywords = await File.ReadAllLinesAsync("keywords.txt");
            keywords = keywords.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var re = targetCultures
                .ToDictionary(x => x, x => new LocalizationFile
                {
                    Culture = x,
                    Items = Enumerable.Empty<LocalizationItem>()
                });
            foreach (var item in file.Items)
            {
                var source = item.SourceText;
                var dic = new Dictionary<Guid, string>();
                source = Regex.Replace(source, "(\\{[\\w]+\\})", match =>
                {
                    var newGuid = Guid.NewGuid();
                    var value = match.Groups[0].Value;
                    dic.Add(newGuid, $"<mstrans:dictionary translation=\"{value}\">{value}</mstrans:dictionary>");
                    return newGuid.ToString();
                });

                foreach (var keyword in keywords)
                {
                    source = source.Replace(keyword,
                        $"<mstrans:dictionary translation=\"{keyword}\">{keyword}</mstrans:dictionary>");
                }

                foreach (var (k, value) in dic)
                {
                    source = source.Replace(k.ToString(), value);
                }

                _logger.LogInformation("text sending : {source}", source);
                var translationResults = await TranslateAsync(source, targetCultures);
                var translationResult = translationResults.First();
                foreach (var translation in translationResult.Translations)
                {
                    var localizationFile = re[translation.To];
                    localizationFile.Items = localizationFile.Items.Concat(new[]
                    {
                        new LocalizationItem
                        {
                            Key = item.Key,
                            SourceText = item.SourceText,
                            Text = translation.Text
                        },
                    });
                }

                _logger.LogInformation("translated : {source}", item.SourceText);
            }

            return re;
        }
    }
}
namespace DistanceBetweenCities
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    public static class Distance
    {
        private static readonly HttpClient Client = new HttpClient();

        private static IConfiguration configuration;

        private static string apiKey;

        public static async Task<int> Run()
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
            configuration = builder.Build();
            apiKey = configuration["MapQuestApiKey"];

            if (apiKey == "key")
            {
                System.Console.WriteLine("Dont forget to enter your API key in appsettings.json");
                return 1;
            }

            System.Console.Write("Enter the first city: ");
            string city1 = Console.ReadLine();
            System.Console.Write("Enter the second city: ");
            string city2 = Console.ReadLine();

            double distance = await CalculateDistance(city1, city2);

            System.Console.WriteLine($"The distance between {city1} and {city2} is {String.Format("{0:0.00}", (distance / 1000))} kilometers");

            return 0;
        }

        private static async Task<double> CalculateDistance(string city1, string city2)
        {
            double[] city1LatLng = await GetLatLngAsync(city1);
            double[] city2LatLng = await GetLatLngAsync(city2);

            // algorythm and explanation by Chris Veness at: https://www.movable-type.co.uk/scripts/latlong.html
            const double R = 6371e3;
            double f1 = city1LatLng[0] * Math.PI / 180;
            double f2 = city2LatLng[0] * Math.PI / 180;
            double dFi = (city1LatLng[0] - city2LatLng[0]) * Math.PI / 180;
            double dLam = (city1LatLng[1] - city2LatLng[1]) * Math.PI / 180;

            double a = Math.Sin(dFi / 2) * Math.Sin(dFi / 2) +
                       Math.Cos(f1) * Math.Cos(f2) *
                       Math.Sin(dLam / 2) * Math.Sin(dLam / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double result = R * c;

            return result;
        }

        private static async Task<double[]> GetLatLngAsync(string city)
        {
            string response = await Client.GetStringAsync($"http://open.mapquestapi.com/geocoding/v1/address?key={apiKey}&location={city}");

            var jsonOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
            };

            using (JsonDocument document = JsonDocument.Parse(response, jsonOptions))
            {
                JsonElement root = document.RootElement;
                JsonElement resultsElement = root.GetProperty("results");

                foreach (var field in resultsElement.EnumerateArray())
                {
                    if (field.TryGetProperty("locations", out JsonElement locationsElement))
                    {
                        foreach (var item in locationsElement.EnumerateArray())
                        {
                            if (item.TryGetProperty("displayLatLng", out JsonElement latLngElement))
                            {
                                double[] result = new double[2];
                                int cnt = 0;
                                foreach (var item2 in latLngElement.EnumerateObject())
                                {
                                    if (item2.Name == "lat")
                                    {
                                        result[cnt] = item2.Value.GetDouble();
                                        cnt++;
                                    }
                                    else
                                    {
                                        result[cnt] = item2.Value.GetDouble();
                                        cnt++;
                                    }
                                }

                                return result;
                            }
                        }
                    }
                }

                throw new Exception("Error parsing JSON");
            }
        }
    }
}
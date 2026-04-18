using System.Text;
using System.Text.Json;
using Saferide.Models;

namespace Saferide.Services
{
    public class RoutingService
    {
        // To send HTTP requests to Google API
        private readonly HttpClient _httpClient;

        // Store API key from config
        private readonly string _apiKey;

        // Constructor
        public RoutingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // Pulls API key from appsettings.json
            _apiKey = configuration["GoogleMaps:ApiKey"]
                ?? throw new Exception("API Key missing");
        }

        // MAIN METHOD: Compute route between two locations
        // async means to define an asynchronous method that can run without blocking the main thread
        public async Task<RouteResult?> ComputeRouteAsync(Location origin, Location destination)
        {
            // Google Routes API endpoint
            string url = "https://routes.googleapis.com/directions/v2:computeRoutes";

            // Build request body using Location class
            var requestBody = new
            {
                origin = new
                {
                    location = new
                    {
                        latLng = new
                        {
                            latitude = origin.GetLat(),     // from the Location class
                            longitude = origin.GetLon()
                        }
                    }
                },
                destination = new
                {
                    location = new
                    {
                        latLng = new
                        {
                            latitude = destination.GetLat(),
                            longitude = destination.GetLon()
                        }
                    }
                },
                travelMode = "DRIVE",                  // driving directions
                routingPreference = "TRAFFIC_AWARE",   // includes traffic
                computeAlternativeRoutes = false,      // only need 1 route
                units = "IMPERIAL"                    // miles instead of km
            };

            // Convert object to JSON string
            var json = JsonSerializer.Serialize(requestBody);

            // Create HTTP request
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Attach JSON body
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            request.Headers.Add("X-Goog-Api-Key", _apiKey);

            // what fields we want back
            request.Headers.Add("X-Goog-FieldMask",
                "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");

            // Send request to Google
            var response = await _httpClient.SendAsync(request); // await means to pause execution until a task completes

            // Read response as string
            var responseText = await response.Content.ReadAsStringAsync();

            // If request failed then throw error
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode} - {responseText}");
            }

            // Convert JSON response to C# object
            var result = JsonSerializer.Deserialize<ComputeRoutesResponse>(responseText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Get first route from response
            var route = result?.Routes?.FirstOrDefault();

            if (route == null)
                return null;

            // Return clean result object
            return new RouteResult
            {
                Duration = route.Duration,
                DistanceMeters = route.DistanceMeters,
                EncodedPolyline = route.Polyline?.EncodedPolyline
            };
        }
    }

    // Clean result returned to app
    public class RouteResult
    {
        public string? Duration { get; set; }
        public int DistanceMeters { get; set; }
        public string? EncodedPolyline { get; set; }
    }

    // Classes to match Google JSON response
    public class ComputeRoutesResponse
    {
        public List<RouteDto>? Routes { get; set; }
    }

    public class RouteDto
    {
        public string? Duration { get; set; }
        public int DistanceMeters { get; set; }
        public PolylineDto? Polyline { get; set; }
    }

    public class PolylineDto
    {
        public string? EncodedPolyline { get; set; }
    }
}
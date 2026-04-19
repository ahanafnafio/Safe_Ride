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
        public async Task<DriverEtaResult?> ComputeRouteMatrixAsync(List<Driver> drivers, Location pickup)
        {
            // Only consider drivers who are available and have a current location
            var availableDrivers = drivers
                .Where(d => d.IsAvailable() && d.GetCurrentLocation() != null)
                .ToList();

            // If no valid drivers exist, return null
            if (availableDrivers.Count == 0)
            {
                return null;
            }

            // Google Route Matrix endpoint
            string url = "https://routes.googleapis.com/distanceMatrix/v2:computeRouteMatrix";

            // Build the list of origins from all available drivers
            var origins = availableDrivers.Select(d => new
            {
                waypoint = new
                {
                    location = new
                    {
                        latLng = new
                        {
                            latitude = d.GetCurrentLocation()!.GetLat(),
                            longitude = d.GetCurrentLocation()!.GetLon()
                        }
                    }
                }
            }).ToList();

            // We only have one destination: the rider's pickup location
            var destinations = new[]
            {
                new
                {
                    waypoint = new
                    {
                        location = new
                        {
                            latLng = new
                            {
                                latitude = pickup.GetLat(),
                                longitude = pickup.GetLon()
                            }
                        }
                    }
                }
            };

            // Build JSON request body
            var requestBody = new
            {
                origins = origins,
                destinations = destinations,
                travelMode = "DRIVE",
                routingPreference = "TRAFFIC_AWARE"
            };

            // Convert request body to JSON
            var json = JsonSerializer.Serialize(requestBody);

            // Create HTTP request
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Attach JSON body
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Add API key
            request.Headers.Add("X-Goog-Api-Key", _apiKey);

            // Ask only for the fields we need
            request.Headers.Add("X-Goog-FieldMask", "originIndex,destinationIndex,duration,distanceMeters");

            // Send request to Google
            var response = await _httpClient.SendAsync(request);

            // Read response body as text
            var responseText = await response.Content.ReadAsStringAsync();

            // Throw if Google returns an error
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Route Matrix error: {response.StatusCode} - {responseText}");
            }

            // ComputeRouteMatrix returns a JSON array of matrix elements
            var elements = JsonSerializer.Deserialize<List<RouteMatrixElementDto>>(
                responseText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (elements == null || elements.Count == 0)
            {
                return null;
            }

            DriverEtaResult? bestResult = null;

            foreach (var element in elements)
            {
                // Skip bad origin indexes just in case
                if (element.OriginIndex < 0 || element.OriginIndex >= availableDrivers.Count)
                {
                    continue;
                }

                // Parse Google duration like "420s" into an integer number of seconds
                int durationSeconds = 0;
                if (!string.IsNullOrEmpty(element.Duration))
                {
                    durationSeconds = (int)Math.Round(
                        double.Parse(element.Duration.Replace("s", ""))
                    );
                }

                var candidate = new DriverEtaResult
                {
                    Driver = availableDrivers[element.OriginIndex],
                    DurationSeconds = durationSeconds,
                    DistanceMeters = element.DistanceMeters
                };

                // Keep the smallest ETA
                if (bestResult == null || candidate.DurationSeconds < bestResult.DurationSeconds)
                {
                    bestResult = candidate;
                }
            }

            return bestResult;
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

    // Represents one driver's ETA result from the matrix call
    public class DriverEtaResult
    {
        public Driver Driver { get; set; } = null!;
        public int DurationSeconds { get; set; }
        public int DistanceMeters { get; set; }
    }

    // Matches one element returned by Google Route Matrix
    public class RouteMatrixElementDto
    {
        public int OriginIndex { get; set; }
        public int DestinationIndex { get; set; }
        public string? Duration { get; set; }
        public int DistanceMeters { get; set; }
    }
}
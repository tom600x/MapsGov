using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using System.Xml.Linq;

namespace MapsGov.Controllers
{
    public class MapLocation
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class MapController : Controller
    {        private readonly IConfiguration _config;
        private readonly string? _azureMapsKey;
        private readonly bool _useManagedIdentity;
        private readonly string _azureMapsGovEndpoint;
        private readonly string _azureMapsDomain;

        public MapController(IConfiguration config)
        {
            _config = config;
            _azureMapsKey = _config["AzureMaps:SubscriptionKey"];
            _useManagedIdentity = _config.GetValue<bool>("AzureMaps:UseManagedIdentity");
            _azureMapsGovEndpoint = _config["AzureMaps:GovEndpoint"] ?? "https://atlas.azure.us";
            _azureMapsDomain = _config["AzureMaps:Domain"] ?? "atlas.azure.us";
        }        public IActionResult Index()
        {
            // Mock locations
            var locations = new List<MapLocation>
            {
                new MapLocation { Name = "Seattle", Latitude = 47.6062, Longitude = -122.3321 },
                new MapLocation { Name = "Redmond", Latitude = 47.673988, Longitude = -122.121513 },
                new MapLocation { Name = "Bellevue", Latitude = 47.6101, Longitude = -122.2015 }
            };
            // No longer needed as we're using the API endpoint for authentication
            // ViewBag.AzureMapsKey = _azureMapsKey;
            return View(locations);
        }

 

        // Show the geocode form
        [HttpGet]
        public IActionResult Geocode()
        {
            return View();
        }

        /// <summary>
        /// Endpoint to get Azure Maps authentication information
        /// Uses Managed Identity when deployed to Azure and configured, otherwise falls back to subscription key
        /// </summary>
        [HttpGet]
        [Route("api/maps/auth")]
        public async Task<IActionResult> GetMapsAuth()
        {
            // If not using managed identity, return the subscription key
            if (!_useManagedIdentity)
            {
                return Json(new 
                { 
                    authType = "subscriptionKey",
                    subscriptionKey = _azureMapsKey,
                    domain = _azureMapsDomain
                });
            }

            try
            {
                // Create DefaultAzureCredential with Azure Government authority host
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzureGovernment
                });

                // Get token for Azure Maps in Azure Government
                var token = await credential.GetTokenAsync(
                    new TokenRequestContext(new[] { $"{_azureMapsGovEndpoint}/.default" }));

                return Json(new 
                { 
                    authType = "aad", 
                    token = token.Token,
                    domain = _azureMapsDomain,
                    expiresOn = token.ExpiresOn
                });
            }
            catch (Exception ex)
            {
                // Log the exception (in a production app)
                // Fall back to subscription key if managed identity fails
                return Json(new 
                { 
                    authType = "subscriptionKey",
                    subscriptionKey = _azureMapsKey,
                    domain = _azureMapsDomain,
                    error = ex.Message
                });
            }
        }

        // Handle geocode form submission
        [HttpPost]
        public async Task<IActionResult> Geocode(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                ViewBag.Error = "Please enter an address.";
                return View();
            }
            var url = $"https://atlas.azure.us/search/address/json?api-version=1.0&subscription-key={_azureMapsKey}&query={System.Net.WebUtility.UrlEncode(address)}";
            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            var json = JObject.Parse(response);
            ViewBag.Result = json;
            ViewBag.Address = address;
            return View();
        }

        // In-memory storage for demo pin location
        private static MapLocation DemoPinLocation = new MapLocation { Name = "Demo Pin", Latitude = 47.6062, Longitude = -122.3321 };

        // Show the move pin demo page
        [HttpGet]
        public IActionResult MovePin()
        {
            ViewBag.AzureMapsKey = _azureMapsKey;
            return View(DemoPinLocation);
        }

        // Update pin location (AJAX POST)
        [HttpPost]
        public IActionResult UpdatePin([FromBody] MapLocation newLocation)
        {
            if (newLocation != null)
            {
                DemoPinLocation.Latitude = newLocation.Latitude;
                DemoPinLocation.Longitude = newLocation.Longitude;
                // Placeholder: Save to external storage here
            }
            return Json(new { success = true, location = DemoPinLocation });
        }
    }
}

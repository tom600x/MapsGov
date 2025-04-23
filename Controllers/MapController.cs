using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
 
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
    {
        private readonly IConfiguration _config;
        private readonly string? _azureMapsKey;

        public MapController(IConfiguration config)
        {
            _config = config;
            _azureMapsKey = _config["AzureMaps:SubscriptionKey"];
        }

        public IActionResult Index()
        {
            // Mock locations
            var locations = new List<MapLocation>
            {
                new MapLocation { Name = "Seattle", Latitude = 47.6062, Longitude = -122.3321 },
                new MapLocation { Name = "Redmond", Latitude = 47.673988, Longitude = -122.121513 },
                new MapLocation { Name = "Bellevue", Latitude = 47.6101, Longitude = -122.2015 }
            };
            ViewBag.AzureMapsKey = _azureMapsKey;
            return View(locations);
        }

 

        // Show the geocode form
        [HttpGet]
        public IActionResult Geocode()
        {
            return View();
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
    }
}

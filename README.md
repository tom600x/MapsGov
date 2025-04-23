# MapsGov ASP.NET Core MVC Application

This project is a C# ASP.NET Core MVC (.NET 8) web application that uses Azure Maps via the v1 REST API (not the SDK). The map key is read from a local `secrets.json` file for security. Mock locations are provided in a controller, and the map is displayed in a view using the Azure Maps web control.

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An [Azure Government](https://portal.azure.us/) account with an Azure Maps resource (Gov cloud endpoint)

### Setup
1. **Clone the repository**
2. **Create a `secrets.json` file in the project root** (if not present):

   ```json
   {
     "AzureMaps": {
       "SubscriptionKey": "YOUR_AZURE_GOV_MAPS_SUBSCRIPTION_KEY"
     }
   }
   ```
   - Replace `YOUR_AZURE_GOV_MAPS_SUBSCRIPTION_KEY` with your Azure Maps subscription key from your Azure Government account.
   - `secrets.json` is excluded from source control by default (see `.gitignore`).

3. **Build and run the application:**
   ```sh
   dotnet run
   ```

4. **Access the app:**
   Open your browser to the URL shown in the terminal (usually `https://localhost:5001/`).

### Notes
- This app is configured to use the Azure Government cloud endpoint for Azure Maps (`atlas.azure.us`).
- Do **not** store your subscription key in `appsettings.json` or commit it to source control.
- For production, use a secure secret store or environment variables.

## Project Structure
- `Controllers/MapController.cs`: Provides mock locations and injects the Azure Maps key into views.
- `Views/Map/Index.cshtml`: Displays the map using the Azure Maps web control.
- `Views/Map/MovePin.cshtml`: Demo page to move a pin on the map, save its location in memory, and demonstrate how to update pin locations interactively. The controller includes a placeholder for saving to external storage.
- `secrets.json`: Stores your Azure Maps subscription key (local only).

## Security
- Never share or commit your `secrets.json` file.
- Always use a key from an Azure Government Maps account for compliance.

---

For more information, see the [Azure Maps documentation for US Government](https://learn.microsoft.com/en-us/azure/azure-government/documentation-government-welcome)

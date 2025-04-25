# MapsGov ASP.NET Core MVC Application

This project is a C# ASP.NET Core MVC (.NET 8) web application that uses Azure Maps via the v1 REST API (not the SDK). The application supports two authentication methods for Azure Maps:
1. Subscription Key - used primarily for local development
2. Managed Identity - used when deployed to Azure Government cloud

Mock locations are provided in a controller, and the map is displayed in a view using the Azure Maps web control.

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An [Azure Government](https://portal.azure.us/) account with an Azure Maps resource (Gov cloud endpoint)

### Setup
1. **Clone the repository**
2. **Create or modify the `secrets.json` file in the project root** (if not present):

   ```json
   {
     "AzureMaps": {
       "SubscriptionKey": "YOUR_AZURE_GOV_MAPS_SUBSCRIPTION_KEY",
       "UseManagedIdentity": false,
       "GovEndpoint": "https://atlas.azure.us",
       "Domain": "atlas.azure.us"
     }
   }
   ```
   - Replace `YOUR_AZURE_GOV_MAPS_SUBSCRIPTION_KEY` with your Azure Maps subscription key from your Azure Government account.
   - Set `UseManagedIdentity` to `false` for local development or when not using Managed Identity.
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
- For local development, use the subscription key from `secrets.json`.
- For production in Azure Government, use Managed Identity authentication by setting `UseManagedIdentity` to `true`.

## Project Structure
- `Controllers/MapController.cs`: Provides mock locations and handles Azure Maps authentication for both subscription key and managed identity scenarios.
- `Views/Map/Index.cshtml`: Displays the map using the Azure Maps web control with dynamic authentication.
- `Views/Map/MovePin.cshtml`: Demo page to move a pin on the map, save its location in memory, and demonstrate how to update pin locations interactively. The controller includes a placeholder for saving to external storage.
- `secrets.json`: Stores your Azure Maps configuration settings including subscription key and authentication preferences (local only).

## Security
- Never share or commit your `secrets.json` file.
- Always use a key from an Azure Government Maps account for compliance.
- When deploying to Azure Government, use Managed Identity authentication for improved security.

## Deploying to Azure Government with Managed Identity

To deploy this application to Azure Government cloud and use Managed Identity for Azure Maps authentication:

1. **Deploy the application to Azure App Service** in the Azure Government cloud.

2. **Enable a Managed Identity** for your App Service:
   - Navigate to your App Service in the Azure Government portal
   - Select "Identity" from the left menu
   - Under "System assigned" tab, set Status to "On" and save
   - Note the Object ID that gets generated

3. **Assign the appropriate role** to your Managed Identity:
   - Navigate to your Azure Maps account in the Azure Government portal
   - Select "Access control (IAM)" from the left menu
   - Click "+ Add" and select "Add role assignment"
   - Choose "Azure Maps Data Reader" role
   - Select "Managed identity" for "Assign access to"
   - Select your App Service's managed identity from the list
   - Click "Save"

4. **Configure your App Service application settings**:
   - Add or update the following application settings:
     ```
     AzureMaps:UseManagedIdentity = true
     AzureMaps:GovEndpoint = https://atlas.azure.us
     AzureMaps:Domain = atlas.azure.us
     ```

5. **Restart your App Service** to apply the changes.

With these settings, your application will use the Managed Identity to authenticate with Azure Maps instead of a subscription key when running in Azure Government cloud.

---

For more information, see the [Azure Maps documentation for US Government](https://learn.microsoft.com/en-us/azure/azure-government/documentation-government-welcome)

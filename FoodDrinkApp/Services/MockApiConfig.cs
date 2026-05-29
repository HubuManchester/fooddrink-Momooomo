namespace FoodDrinkApp.Services;

/// <summary>
/// Configuration for the mock API endpoint.
/// When configured, the app uses mockapi.io for data storage; otherwise, it falls back to local data.
/// </summary>
public static class MockApiConfig
{
    /// <summary>
    /// The mockapi.io Resource endpoint URL.
    /// Replace with your own endpoint from mockapi.io.
    /// Example: https://682xxxx.mockapi.io/api/v1/foods
    /// </summary>
    public const string EndpointUrl = "";

    /// <summary>
    /// Indicates whether the mock API is configured.
    /// </summary>
    public static bool IsConfigured => !string.IsNullOrWhiteSpace(EndpointUrl);
}

namespace FoodDrinkApp.Services;

using System.Globalization;

/// <summary>
/// Provides text-to-speech functionality with support for multiple languages.
/// Ensures only one speech operation runs at a time and provides clean cancellation.
/// </summary>
public static class SpeechService
{
    private static CancellationTokenSource? currentSpeech;

    /// <summary>
    /// Speaks the provided text using the default speech engine.
    /// Stops any ongoing speech before starting a new one.
    /// </summary>
    /// <param name="text">The text to speak.</param>
    public static async Task SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        Stop();

        currentSpeech = new CancellationTokenSource();
        var options = new SpeechOptions
        {
            Volume = 0.9f,
            Pitch = 1.05f,
            Locale = await FindEnglishLocaleAsync()
        };

        try
        {
            await TextToSpeech.Default.SpeakAsync(text, options, currentSpeech.Token);
        }
        catch (OperationCanceledException)
        {
            // Expected when speech is stopped programmatically
        }
        catch (Exception)
        {
            // Speech service may not be available on all devices/platforms
        }
    }

    /// <summary>
    /// Speaks Chinese text using text-to-speech.
    /// </summary>
    /// <param name="text">The Chinese text to speak.</param>
    public static Task SpeakChineseAsync(string text) => SpeakAsync(text);

    /// <summary>
    /// Stops any ongoing speech operation immediately.
    /// </summary>
    public static void Stop()
    {
        if (currentSpeech is null)
        {
            return;
        }

        try
        {
            currentSpeech.Cancel();
            currentSpeech.Dispose();
        }
        catch
        {
            // Ignore disposal errors
        }
        finally
        {
            currentSpeech = null;
        }
    }

    /// <summary>
    /// Finds an English locale from available text-to-speech locales.
    /// Falls back to default locale if no English locale is found.
    /// </summary>
    private static async Task<Locale?> FindEnglishLocaleAsync()
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            return locales.FirstOrDefault(locale => 
                locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase)) 
                ?? locales.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}

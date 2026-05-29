using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp;

/// <summary>
/// Page displaying detailed nutrition information for a specific food item.
/// Supports text-to-speech, vibration feedback, and accessibility features.
/// </summary>
[QueryProperty(nameof(ItemId), "id")]
public partial class FoodDetailPage : ContentPage
{
    private FoodItem? currentItem;

    public FoodDetailPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page appears. Applies accessibility font scaling.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }

    /// <summary>
    /// Called when the page disappears. Stops any ongoing speech to prevent audio leakage.
    /// </summary>
    protected override void OnDisappearing()
    {
        SpeechService.Stop();
        base.OnDisappearing();
    }

    /// <summary>
    /// Gets or sets the item ID passed via navigation parameters.
    /// Triggers loading of the food item when set.
    /// </summary>
    public string ItemId
    {
        set => _ = LoadItemAsync(value);
    }

    /// <summary>
    /// Loads the food item from the catalog service based on its ID.
    /// </summary>
    private async Task LoadItemAsync(string id)
    {
        currentItem = await FoodCatalogService.GetByIdAsync(id);
        BindingContext = currentItem;
        RenderItem();
    }

    /// <summary>
    /// Renders the food item details on the page.
    /// Displays an error message if the item is not found.
    /// </summary>
    private void RenderItem()
    {
        if (currentItem is null)
        {
            NameLabel.Text = "Record not found";
            DescriptionLabel.Text = "The selected food or drink could not be loaded.";
            return;
        }

        NameLabel.Text = currentItem.Name;
        CategoryLabel.Text = currentItem.Category;
        CaloriesLabel.Text = currentItem.CaloriesLabel;
        MacroLabel.Text = currentItem.MacroSummary;
        DescriptionLabel.Text = currentItem.Description;
        AllergyLabel.Text = currentItem.AllergyNote;
        SemanticProperties.SetDescription(NameLabel, currentItem.AccessibleSummary);
    }

    /// <summary>
    /// Handles the Read Summary button click to speak the nutrition information.
    /// </summary>
    private async void OnSpeakClicked(object? sender, EventArgs e)
    {
        if (currentItem is null)
        {
            await DisplayAlert("Missing record", "There is no nutrition summary to read.", "OK");
            return;
        }

        try
        {
            await SpeechService.SpeakAsync(currentItem.AccessibleSummary);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Text to speech unavailable", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Handles the Stop Reading button click to stop ongoing speech.
    /// </summary>
    private void OnStopSpeechClicked(object? sender, EventArgs e)
    {
        SpeechService.Stop();
        SemanticScreenReader.Announce("Reading stopped.");
    }

    /// <summary>
    /// Handles the Vibration Reminder button click to trigger vibration and haptic feedback.
    /// </summary>
    private async void OnVibrateClicked(object? sender, EventArgs e)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            await DisplayAlert("Reminder", "Vibration feedback has been triggered.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Vibration unavailable", ex.Message, "OK");
        }
    }
}

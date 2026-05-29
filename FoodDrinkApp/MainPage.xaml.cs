using FoodDrinkApp.Services;

namespace FoodDrinkApp;

/// <summary>
/// Main page displaying the food and drink catalog with search functionality.
/// Supports pull-to-refresh and navigation to detail and add pages.
/// </summary>
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page appears. Applies accessibility font scaling and loads food items.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await LoadFoodItemsAsync(SearchFoodBar.Text);
    }

    /// <summary>
    /// Loads food items from the catalog service based on search query.
    /// </summary>
    /// <param name="query">Optional search query to filter items.</param>
    private async Task LoadFoodItemsAsync(string? query = null)
    {
        FoodCollection.ItemsSource = await FoodCatalogService.SearchAsync(query);
    }

    /// <summary>
    /// Handles navigation to the add item page when the Add button is clicked.
    /// </summary>
    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddItemPage));
    }

    /// <summary>
    /// Handles navigation to the food detail page when a Details button is clicked.
    /// </summary>
    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string id)
        {
            await Shell.Current.GoToAsync($"{nameof(FoodDetailPage)}?id={Uri.EscapeDataString(id)}");
        }
    }

    /// <summary>
    /// Handles real-time search as the user types in the search bar.
    /// </summary>
    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        await LoadFoodItemsAsync(e.NewTextValue);
    }

    /// <summary>
    /// Handles search when the search button is pressed on the keyboard.
    /// </summary>
    private async void OnSearchButtonPressed(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchFoodBar.Text);
    }

    /// <summary>
    /// Handles pull-to-refresh to reload the food list.
    /// </summary>
    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchFoodBar.Text);
        FoodRefreshView.IsRefreshing = false;
        var source = FoodCatalogService.LastLoadUsedMockApi ? "mockapi.io" : "local fallback data";
        SemanticScreenReader.Announce($"Food and drink list refreshed. Current source: {source}.");
    }
}

using System.Text.Json.Serialization;

namespace FoodDrinkApp.Models;

/// <summary>
/// Represents a food or drink item with nutrition information.
/// Used for storing and displaying food catalog entries.
/// </summary>
public sealed class FoodItem
{
    /// <summary>
    /// Unique identifier for the food item.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Name of the food or drink.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category of the food (e.g., Breakfast, Lunch, Dinner, Snack, Drink).
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Description or preparation details.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Calorie content per serving.
    /// </summary>
    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    /// <summary>
    /// Protein content in grams.
    /// </summary>
    [JsonPropertyName("protein")]
    public int Protein { get; set; }

    /// <summary>
    /// Carbohydrate content in grams.
    /// </summary>
    [JsonPropertyName("carbs")]
    public int Carbs { get; set; }

    /// <summary>
    /// Fat content in grams.
    /// </summary>
    [JsonPropertyName("fat")]
    public int Fat { get; set; }

    /// <summary>
    /// Allergy information or dietary restrictions.
    /// </summary>
    [JsonPropertyName("allergyNote")]
    public string AllergyNote { get; set; } = string.Empty;

    /// <summary>
    /// Search tags for filtering and searching.
    /// </summary>
    [JsonPropertyName("tags")]
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Formatted calories label for display.
    /// </summary>
    [JsonIgnore]
    public string CaloriesLabel => $"{Calories} kcal";

    /// <summary>
    /// Summary of macronutrients for display.
    /// </summary>
    [JsonIgnore]
    public string MacroSummary => $"Protein {Protein}g, carbs {Carbs}g, fat {Fat}g";

    /// <summary>
    /// Accessible summary for screen readers, combining all key information.
    /// </summary>
    [JsonIgnore]
    public string AccessibleSummary => $"{Name}. {Category}. {Calories} kcal. {MacroSummary}. {AllergyNote}";
}

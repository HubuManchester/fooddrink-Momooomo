using System.Net.Http.Json;
using System.Text.Json;
using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Manages food and drink data operations with support for remote API and local fallback.
/// Handles searching, retrieving, and adding food items.
/// </summary>
public static class FoodCatalogService
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(12)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly List<FoodItem> LocalFallbackItems =
    [
        new()
        {
            Name = "Berry Yogurt Bowl",
            Category = "Breakfast",
            Description = "Greek yogurt with mixed berries, oats, and a small drizzle of honey.",
            Calories = 340,
            Protein = 24,
            Carbs = 42,
            Fat = 8,
            AllergyNote = "Contains dairy and gluten.",
            Tags = "healthy breakfast yogurt berries"
        },
        new()
        {
            Name = "Avocado Toast",
            Category = "Breakfast",
            Description = "Sourdough bread topped with smashed avocado, cherry tomatoes, and poached eggs.",
            Calories = 420,
            Protein = 16,
            Carbs = 38,
            Fat = 24,
            AllergyNote = "Contains gluten and eggs.",
            Tags = "breakfast avocado toast vegetarian"
        },
        new()
        {
            Name = "Oatmeal with Banana",
            Category = "Breakfast",
            Description = "Rolled oats cooked with milk, topped with sliced banana, cinnamon, and walnuts.",
            Calories = 380,
            Protein = 12,
            Carbs = 62,
            Fat = 10,
            AllergyNote = "Contains dairy and nuts.",
            Tags = "breakfast oatmeal banana healthy"
        },
        new()
        {
            Name = "Scrambled Eggs with Bacon",
            Category = "Breakfast",
            Description = "Fluffy scrambled eggs with crispy bacon strips and whole wheat toast.",
            Calories = 520,
            Protein = 32,
            Carbs = 28,
            Fat = 34,
            AllergyNote = "Contains eggs and gluten.",
            Tags = "breakfast eggs bacon protein"
        },
        new()
        {
            Name = "Pancake Stack",
            Category = "Breakfast",
            Description = "Fluffy buttermilk pancakes with maple syrup and fresh strawberries.",
            Calories = 580,
            Protein = 10,
            Carbs = 88,
            Fat = 18,
            AllergyNote = "Contains gluten, eggs, and dairy.",
            Tags = "breakfast pancake syrup sweet"
        },
        new()
        {
            Name = "Chicken Brown Rice Box",
            Category = "Lunch",
            Description = "Grilled chicken breast with brown rice, spinach, cucumber, and lemon dressing.",
            Calories = 520,
            Protein = 38,
            Carbs = 58,
            Fat = 14,
            AllergyNote = "No common allergens recorded.",
            Tags = "meal prep protein lunch"
        },
        new()
        {
            Name = "Caesar Salad",
            Category = "Lunch",
            Description = "Romaine lettuce with grilled chicken, parmesan, croutons, and Caesar dressing.",
            Calories = 480,
            Protein = 34,
            Carbs = 22,
            Fat = 28,
            AllergyNote = "Contains dairy, gluten, and eggs.",
            Tags = "salad lunch chicken caesar"
        },
        new()
        {
            Name = "Vegetable Stir Fry",
            Category = "Lunch",
            Description = "Mixed vegetables with tofu in ginger soy sauce, served with jasmine rice.",
            Calories = 420,
            Protein = 18,
            Carbs = 56,
            Fat = 14,
            AllergyNote = "Contains soy and gluten.",
            Tags = "vegetarian lunch stir fry asian"
        },
        new()
        {
            Name = "Turkey Sandwich",
            Category = "Lunch",
            Description = "Sliced turkey with cheese, lettuce, tomato, and mustard on whole grain bread.",
            Calories = 450,
            Protein = 32,
            Carbs = 42,
            Fat = 16,
            AllergyNote = "Contains gluten and dairy.",
            Tags = "lunch sandwich turkey protein"
        },
        new()
        {
            Name = "Mushroom Risotto",
            Category = "Lunch",
            Description = "Creamy arborio rice with wild mushrooms, parmesan, and fresh thyme.",
            Calories = 560,
            Protein = 14,
            Carbs = 72,
            Fat = 22,
            AllergyNote = "Contains dairy.",
            Tags = "lunch risotto mushroom vegetarian"
        },
        new()
        {
            Name = "Tomato Wholegrain Pasta",
            Category = "Dinner",
            Description = "Wholegrain pasta with tomato sauce, basil, and roasted vegetables.",
            Calories = 610,
            Protein = 18,
            Carbs = 92,
            Fat = 16,
            AllergyNote = "Contains gluten.",
            Tags = "vegetarian dinner pasta"
        },
        new()
        {
            Name = "Grilled Salmon",
            Category = "Dinner",
            Description = "Atlantic salmon fillet with roasted potatoes, asparagus, and lemon butter sauce.",
            Calories = 680,
            Protein = 46,
            Carbs = 38,
            Fat = 36,
            AllergyNote = "Contains fish and dairy.",
            Tags = "dinner salmon fish protein"
        },
        new()
        {
            Name = "Beef Steak",
            Category = "Dinner",
            Description = "Prime beef sirloin with mashed potatoes, grilled mushrooms, and red wine jus.",
            Calories = 820,
            Protein = 52,
            Carbs = 42,
            Fat = 48,
            AllergyNote = "Contains dairy.",
            Tags = "dinner steak beef protein"
        },
        new()
        {
            Name = "Chicken Curry",
            Category = "Dinner",
            Description = "Tender chicken pieces in coconut curry sauce with basmati rice and naan bread.",
            Calories = 740,
            Protein = 36,
            Carbs = 68,
            Fat = 32,
            AllergyNote = "Contains gluten and dairy.",
            Tags = "dinner curry chicken asian"
        },
        new()
        {
            Name = "Vegetable Lasagna",
            Category = "Dinner",
            Description = "Layers of pasta with ricotta, spinach, zucchini, and marinara sauce.",
            Calories = 580,
            Protein = 24,
            Carbs = 62,
            Fat = 26,
            AllergyNote = "Contains gluten and dairy.",
            Tags = "vegetarian dinner lasagna pasta"
        },
        new()
        {
            Name = "Greek Salad",
            Category = "Snack",
            Description = "Cucumber, tomatoes, olives, red onion, and feta cheese with olive oil dressing.",
            Calories = 280,
            Protein = 10,
            Carbs = 18,
            Fat = 20,
            AllergyNote = "Contains dairy.",
            Tags = "snack salad greek vegetarian"
        },
        new()
        {
            Name = "Hummus with Pita",
            Category = "Snack",
            Description = "Creamy chickpea hummus served with warm pita bread and carrot sticks.",
            Calories = 320,
            Protein = 12,
            Carbs = 48,
            Fat = 10,
            AllergyNote = "Contains gluten.",
            Tags = "snack hummus pita mediterranean"
        },
        new()
        {
            Name = "Mixed Nuts",
            Category = "Snack",
            Description = "A handful of almonds, cashews, walnuts, and dried cranberries.",
            Calories = 200,
            Protein = 6,
            Carbs = 14,
            Fat = 16,
            AllergyNote = "Contains nuts.",
            Tags = "snack nuts healthy protein"
        },
        new()
        {
            Name = "Apple with Peanut Butter",
            Category = "Snack",
            Description = "Fresh apple slices with natural peanut butter and a sprinkle of granola.",
            Calories = 280,
            Protein = 8,
            Carbs = 32,
            Fat = 14,
            AllergyNote = "Contains nuts.",
            Tags = "snack apple peanut butter healthy"
        },
        new()
        {
            Name = "Protein Bar",
            Category = "Snack",
            Description = "Chocolate protein bar with oats, almonds, and dark chocolate chips.",
            Calories = 240,
            Protein = 20,
            Carbs = 24,
            Fat = 8,
            AllergyNote = "Contains nuts and soy.",
            Tags = "snack protein bar gym fitness"
        },
        new()
        {
            Name = "Iced Matcha Latte",
            Category = "Drink",
            Description = "Matcha, milk, and ice. A lower-sugar version is recommended.",
            Calories = 180,
            Protein = 8,
            Carbs = 22,
            Fat = 6,
            AllergyNote = "Contains dairy unless plant-based milk is selected.",
            Tags = "drink caffeine matcha latte"
        },
        new()
        {
            Name = "Cappuccino",
            Category = "Drink",
            Description = "Double espresso with steamed milk foam. Classic Italian coffee style.",
            Calories = 120,
            Protein = 6,
            Carbs = 10,
            Fat = 4,
            AllergyNote = "Contains dairy.",
            Tags = "drink coffee caffeine espresso"
        },
        new()
        {
            Name = "Fresh Orange Juice",
            Category = "Drink",
            Description = "Freshly squeezed orange juice with no added sugar or preservatives.",
            Calories = 110,
            Protein = 2,
            Carbs = 26,
            Fat = 0,
            AllergyNote = "No common allergens.",
            Tags = "drink juice orange vitamin c"
        },
        new()
        {
            Name = "Green Smoothie",
            Category = "Drink",
            Description = "Blended spinach, banana, mango, and coconut water with chia seeds.",
            Calories = 220,
            Protein = 6,
            Carbs = 44,
            Fat = 4,
            AllergyNote = "Contains seeds.",
            Tags = "drink smoothie green healthy detox"
        },
        new()
        {
            Name = "Iced Americano",
            Category = "Drink",
            Description = "Espresso shots diluted with cold water and ice. Zero sugar, zero fat.",
            Calories = 15,
            Protein = 1,
            Carbs = 2,
            Fat = 0,
            AllergyNote = "No common allergens.",
            Tags = "drink coffee caffeine zero sugar"
        },
        new()
        {
            Name = "Hot Chocolate",
            Category = "Drink",
            Description = "Rich cocoa powder mixed with steamed milk and topped with whipped cream.",
            Calories = 280,
            Protein = 10,
            Carbs = 38,
            Fat = 12,
            AllergyNote = "Contains dairy.",
            Tags = "drink chocolate cocoa warm sweet"
        },
        new()
        {
            Name = "Sparkling Water",
            Category = "Drink",
            Description = "Refreshing carbonated water with natural lemon flavor. Zero calories.",
            Calories = 0,
            Protein = 0,
            Carbs = 0,
            Fat = 0,
            AllergyNote = "No common allergens.",
            Tags = "drink water sparkling zero calorie"
        },
        new()
        {
            Name = "Mango Lassi",
            Category = "Drink",
            Description = "Traditional Indian yogurt smoothie with fresh mango and cardamom.",
            Calories = 260,
            Protein = 10,
            Carbs = 42,
            Fat = 6,
            AllergyNote = "Contains dairy.",
            Tags = "drink lassi mango yogurt indian"
        }
    ];

    private static List<FoodItem> cachedItems = new(LocalFallbackItems);

    /// <summary>
    /// Indicates whether the last data load came from mockapi.io or local fallback.
    /// </summary>
    public static bool LastLoadUsedMockApi { get; private set; }

    /// <summary>
    /// Searches for food items matching the given query.
    /// Search matches against name, category, description, and tags.
    /// </summary>
    /// <param name="query">The search query. If null or empty, returns all items.</param>
    /// <returns>A sorted list of matching food items.</returns>
    public static async Task<IReadOnlyList<FoodItem>> SearchAsync(string? query)
    {
        var items = await GetAllAsync();

        if (string.IsNullOrWhiteSpace(query))
        {
            return items.OrderBy(item => item.Name).ToList();
        }

        var normalised = query.Trim();
        return items
            .Where(item =>
                item.Name.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Category.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Tags.Contains(normalised, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Name)
            .ToList();
    }

    /// <summary>
    /// Retrieves all food items from API or local fallback.
    /// </summary>
    /// <returns>A list of all food items.</returns>
    public static async Task<IReadOnlyList<FoodItem>> GetAllAsync()
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var response = await HttpClient.GetAsync(MockApiConfig.EndpointUrl);
                response.EnsureSuccessStatusCode();
                var items = await response.Content.ReadFromJsonAsync<List<FoodItem>>(JsonOptions);

                if (items is { Count: > 0 })
                {
                    cachedItems = items;
                    LastLoadUsedMockApi = true;
                    return cachedItems;
                }
            }
            catch
            {
                // Fall back to local cache below.
            }
        }

        LastLoadUsedMockApi = false;
        return cachedItems;
    }

    /// <summary>
    /// Retrieves a food item by its ID.
    /// First attempts to fetch from API, then falls back to cached data.
    /// </summary>
    /// <param name="id">The ID of the food item to retrieve.</param>
    /// <returns>The food item if found; otherwise null.</returns>
    public static async Task<FoodItem?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var item = await HttpClient.GetFromJsonAsync<FoodItem>(
                    $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}",
                    JsonOptions);

                if (item is not null)
                {
                    return item;
                }
            }
            catch
            {
                // Fall back to the last loaded cache below.
            }
        }

        return cachedItems.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Adds a new food item to the catalog.
    /// If API is configured, posts to remote server; otherwise adds to local cache.
    /// </summary>
    /// <param name="item">The food item to add.</param>
    /// <returns>The added food item.</returns>
    public static async Task<FoodItem> AddAsync(FoodItem item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync(MockApiConfig.EndpointUrl, item, JsonOptions);
                response.EnsureSuccessStatusCode();

                var created = await response.Content.ReadFromJsonAsync<FoodItem>(JsonOptions);
                if (created is not null)
                {
                    cachedItems.Add(created);
                    return created;
                }
            }
            catch
            {
                // Fall back to local cache if API fails
            }
        }

        cachedItems.Add(item);
        return item;
    }
}
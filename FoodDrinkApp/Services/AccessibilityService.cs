using System.Runtime.CompilerServices;

namespace FoodDrinkApp.Services;

/// <summary>
/// Provides accessibility services including font scaling for users with visual needs.
/// Maintains original font sizes to allow toggling between normal and large text modes.
/// </summary>
public static class AccessibilityService
{
    private const double LargeTextScale = 1.22;
    private static readonly ConditionalWeakTable<BindableObject, FontSizeStore> OriginalFontSizes = new();

    /// <summary>
    /// Gets or sets whether large text mode is enabled.
    /// When enabled, text throughout the app is scaled up by 22%.
    /// </summary>
    public static bool LargeTextEnabled { get; set; }

    /// <summary>
    /// Applies font scaling to a root element and all its visual children.
    /// Only affects supported controls: Label, Button, Entry, Editor, Picker, SearchBar.
    /// </summary>
    /// <param name="root">The root element to apply font scaling to.</param>
    public static void ApplyFontScale(Element root)
    {
        ApplyToElement(root);

        if (root is not IVisualTreeElement visualTreeElement)
        {
            return;
        }

        foreach (var child in visualTreeElement.GetVisualChildren().OfType<Element>())
        {
            ApplyFontScale(child);
        }
    }

    /// <summary>
    /// Applies font scaling to a single element based on the current LargeTextEnabled setting.
    /// </summary>
    private static void ApplyToElement(Element element)
    {
        var scale = LargeTextEnabled ? LargeTextScale : 1.0;

        switch (element)
        {
            case Label label:
                label.FontSize = GetOriginalFontSize(label, label.FontSize) * scale;
                break;
            case Button button:
                button.FontSize = GetOriginalFontSize(button, button.FontSize) * scale;
                break;
            case Entry entry:
                entry.FontSize = GetOriginalFontSize(entry, entry.FontSize) * scale;
                break;
            case Editor editor:
                editor.FontSize = GetOriginalFontSize(editor, editor.FontSize) * scale;
                break;
            case Picker picker:
                picker.FontSize = GetOriginalFontSize(picker, picker.FontSize) * scale;
                break;
            case SearchBar searchBar:
                searchBar.FontSize = GetOriginalFontSize(searchBar, searchBar.FontSize) * scale;
                break;
        }
    }

    /// <summary>
    /// Retrieves the original font size for a control, storing it if not already cached.
    /// </summary>
    /// <param name="control">The control to get the original font size for.</param>
    /// <param name="currentSize">The current font size of the control.</param>
    private static double GetOriginalFontSize(BindableObject control, double currentSize)
    {
        var store = OriginalFontSizes.GetOrCreateValue(control);
        if (!store.HasValue)
        {
            store.Value = currentSize > 0 ? currentSize : 14;
            store.HasValue = true;
        }

        return store.Value;
    }

    /// <summary>
    /// Helper class to store original font sizes for controls.
    /// </summary>
    private sealed class FontSizeStore
    {
        public bool HasValue { get; set; }
        public double Value { get; set; }
    }
}

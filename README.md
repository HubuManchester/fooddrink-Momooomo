# NutriBite - Food and Drink Tracking App

A cross-platform mobile application for tracking food and drink intake with nutrition information, built with .NET MAUI.

## Features

### Core Functionality
- **Food Catalog**: Browse and search food items with detailed nutrition information
- **Add New Items**: Create new food entries with calories, protein, carbs, and fat data
- **View Details**: View comprehensive nutrition information for each food item
- **Category Filtering**: Filter food items by category (Breakfast, Lunch, Dinner, Snack, Drink)
- **Search**: Search food items by name or tags

### Mobile Hardware Integration
- **Camera**: Capture food photos using device camera
- **Location/Geocoding**: Get current location and display country/city information
- **Text-to-Speech**: Read food information and help content aloud
- **Vibration/Haptic Feedback**: Provide tactile feedback for user interactions

### Accessibility Features (WCAG 2.0 Compliant)
- **Large Text Mode**: Enlarge text for better readability
- **Dark/Light Theme**: Switch between themes for comfortable viewing
- **Screen Reader Support**: Semantic announcements for screen reader compatibility

## Technologies Used

- **Framework**: .NET MAUI 9.0
- **Language**: C# 12
- **UI**: XAML with MVVM architecture
- **Platforms**: Windows, Android (iOS support ready)
- **Hardware APIs**: 
  - MediaPicker (Camera)
  - Geolocation & Geocoding (Location)
  - TextToSpeech (Speech)
  - Vibration & HapticFeedback (Tactile)

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 with .NET MAUI workload
- For Android: Android SDK (API Level 34+)
- For Windows: Windows 10 19041 or later

### Building the App

```bash
# Build for Windows
dotnet build -f net9.0-windows10.0.19041.0

# Build for Android (requires Android SDK)
dotnet build -f net9.0-android

# Build for iOS (requires macOS)
dotnet build -f net9.0-ios
```

### Running the App

```bash
# Run on Windows
dotnet run -f net9.0-windows10.0.19041.0

# Run on Android emulator
dotnet run -f net9.0-android
```

## Project Structure

```
FoodDrinkApp/
├── App.xaml              # Application entry point and resources
├── AppShell.xaml         # Shell navigation structure
├── MainPage.xaml         # Main food catalog page
├── Pages/
│   ├── AddItemPage.xaml  # Add new food item page
│   ├── FoodDetailPage.xaml # Food item details page
│   ├── HardwarePage.xaml   # Hardware features demo page
│   └── SettingsPage.xaml   # Accessibility settings page
├── Services/
│   ├── AccessibilityService.cs  # Accessibility features
│   ├── FoodCatalogService.cs    # Data management service
│   ├── MockApiConfig.cs         # Mock API configuration
│   └── SpeechService.cs         # Text-to-speech service
├── Models/
│   └── FoodItem.cs              # Food item data model
└── Resources/
    └── Images/                  # App icons and images
```

## API Configuration

The app supports integration with mockapi.io for remote data storage. To configure:

1. Create an account at [mockapi.io](https://mockapi.io)
2. Create a new resource with the following schema:
   - name (string)
   - category (string)
   - description (string)
   - calories (number)
   - protein (number)
   - carbs (number)
   - fat (number)
   - allergyNote (string)
   - tags (string)
3. Update `Services/MockApiConfig.cs` with your endpoint URL

If no API endpoint is configured, the app uses local sample data.

## Accessibility Features

### Large Text Mode
- Toggle in Settings page to enlarge all text elements
- Font scaling applied dynamically across all pages

### Theme Switching
- System default, Light, or Dark theme options
- Consistent color scheme across all pages

### Screen Reader Support
- Semantic properties set for all interactive elements
- Announcements for status changes and user feedback

## Hardware Features Demo

The Hardware page demonstrates:

1. **Camera**: Capture food photos
2. **Location**: Get current coordinates and address
3. **Text-to-Speech**: Read help content aloud
4. **Vibration**: Trigger device vibration
5. **Haptic Feedback**: Provide tactile response

## Development Notes

### Code Quality
- XML documentation for all public methods and classes
- Consistent naming conventions following .NET standards
- Error handling with try-catch blocks throughout
- Input validation with user feedback

### Testing
- Manual testing on Windows and Android emulators
- All hardware features tested on physical devices
- Accessibility features tested with screen readers

## License

This project is for educational purposes as part of the Mobile Computing course (6G6Z0014).

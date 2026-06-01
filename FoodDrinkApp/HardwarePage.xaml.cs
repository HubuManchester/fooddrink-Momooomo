using FoodDrinkApp.Services;
using Microsoft.Maui.Devices.Sensors;

namespace FoodDrinkApp;

/// <summary>
/// Page demonstrating mobile hardware capabilities including camera, location,
/// gyroscope, compass, shake detection, text-to-speech, vibration, and haptic feedback.
/// </summary>
public partial class HardwarePage : ContentPage
{
    private int feedbackTestCount;
    private int shakeCount;
    private bool gyroActive;
    private bool shakeActive;
    private DateTime lastShakeTime = DateTime.MinValue;

    public HardwarePage()
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
    /// Called when the page disappears. Stops all sensors and ongoing speech.
    /// </summary>
    protected override void OnDisappearing()
    {
        StopGyroscope();
        StopShakeDetection();
        SpeechService.Stop();
        base.OnDisappearing();
    }

    /// <summary>
    /// Handles the Photo button click to capture a food photo using the camera.
    /// </summary>
    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                SetStatus("This device does not support camera capture.");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null)
            {
                SetStatus("Photo capture cancelled.");
                return;
            }

            await using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            FoodPhoto.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            SetStatus("Food photo captured successfully.");
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (PermissionException)
        {
            SetStatus("Camera permission was denied. Enable camera access in device settings.");
        }
        catch (Exception ex)
        {
            SetStatus($"Camera error: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the Locate button click to get the current location and geocode address.
    /// </summary>
    private async void OnGetLocationClicked(object? sender, EventArgs e)
    {
        try
        {
            SetStatus("Getting location...");
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location is null)
            {
                SetStatus("Current location could not be found.");
                return;
            }

            CoordinateLabel.Text = $"Latitude {location.Latitude:F5}, longitude {location.Longitude:F5}";
            LocationLabel.Text = await BuildAddressTextAsync(location);
            SetStatus("Country, city, and coordinates have been loaded.");
        }
        catch (PermissionException)
        {
            SetStatus("Location permission was denied. Enable location access in device settings.");
        }
        catch (Exception ex)
        {
            SetStatus($"Location error: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the gyroscope toggle button click to start or stop gyroscope monitoring.
    /// </summary>
    private void OnGyroToggled(object? sender, EventArgs e)
    {
        if (gyroActive)
        {
            StopGyroscope();
        }
        else
        {
            StartGyroscope();
        }
    }

    /// <summary>
    /// Starts the gyroscope and orientation sensors for compass functionality.
    /// </summary>
    private void StartGyroscope()
    {
        try
        {
            if (!Gyroscope.Default.IsSupported)
            {
                SetStatus("Gyroscope is not supported on this device.");
                return;
            }

            Gyroscope.Default.ReadingChanged += OnGyroscopeReadingChanged;
            Gyroscope.Default.Start(SensorSpeed.UI);

            if (OrientationSensor.Default.IsSupported)
            {
                OrientationSensor.Default.ReadingChanged += OnOrientationReadingChanged;
                OrientationSensor.Default.Start(SensorSpeed.UI);
            }

            gyroActive = true;
            GyroToggleButton.Text = "Stop";
            GyroLabel.Text = "Gyroscope: Starting...";
            CompassLabel.Text = OrientationSensor.Default.IsSupported ? "Compass: Starting..." : "Compass: Not supported";
            SetStatus("Gyroscope and compass started.");
        }
        catch (Exception ex)
        {
            SetStatus($"Gyroscope error: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops the gyroscope and orientation sensors.
    /// </summary>
    private void StopGyroscope()
    {
        try
        {
            Gyroscope.Default.ReadingChanged -= OnGyroscopeReadingChanged;
            Gyroscope.Default.Stop();

            if (OrientationSensor.Default.IsSupported)
            {
                OrientationSensor.Default.ReadingChanged -= OnOrientationReadingChanged;
                OrientationSensor.Default.Stop();
            }

            gyroActive = false;
            GyroToggleButton.Text = "Start";
            GyroLabel.Text = "Gyroscope: Not active";
            CompassLabel.Text = "Compass: Not active";
        }
        catch
        {
            // Ignore errors when stopping
        }
    }

    /// <summary>
    /// Handles gyroscope sensor updates to display rotation rates.
    /// </summary>
    private void OnGyroscopeReadingChanged(object? sender, GyroscopeChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var data = e.Reading;
            GyroLabel.Text = $"Gyroscope: X:{data.AngularVelocity.X:F2} Y:{data.AngularVelocity.Y:F2} Z:{data.AngularVelocity.Z:F2}";
        });
    }

    /// <summary>
    /// Handles orientation sensor updates to calculate compass heading.
    /// </summary>
    private void OnOrientationReadingChanged(object? sender, OrientationSensorChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var data = e.Reading;
            var heading = CalculateCompassHeading(data.Orientation[0], data.Orientation[1], data.Orientation[2], data.Orientation[3]);
            CompassLabel.Text = $"Compass: {heading:F0}° {GetCompassDirection(heading)}";
        });
    }

    /// <summary>
    /// Calculates compass heading from orientation quaternion data.
    /// </summary>
    private static double CalculateCompassHeading(double x, double y, double z, double w)
    {
        var yaw = Math.Atan2(2 * (w * z + x * y), 1 - 2 * (y * y + z * z));
        var degrees = yaw * (180 / Math.PI);
        if (degrees < 0)
        {
            degrees += 360;
        }
        return degrees;
    }

    /// <summary>
    /// Returns compass direction string from heading degrees.
    /// </summary>
    private static string GetCompassDirection(double heading)
    {
        return heading switch
        {
            >= 337.5 or < 22.5 => "N",
            >= 22.5 and < 67.5 => "NE",
            >= 67.5 and < 112.5 => "E",
            >= 112.5 and < 157.5 => "SE",
            >= 157.5 and < 202.5 => "S",
            >= 202.5 and < 247.5 => "SW",
            >= 247.5 and < 292.5 => "W",
            >= 292.5 and < 337.5 => "NW",
            _ => "N"
        };
    }

    /// <summary>
    /// Handles the shake detection toggle button click.
    /// </summary>
    private void OnShakeToggled(object? sender, EventArgs e)
    {
        if (shakeActive)
        {
            StopShakeDetection();
        }
        else
        {
            StartShakeDetection();
        }
    }

    /// <summary>
    /// Starts the accelerometer for shake detection.
    /// </summary>
    private void StartShakeDetection()
    {
        try
        {
            if (!Accelerometer.Default.IsSupported)
            {
                SetStatus("Accelerometer is not supported on this device.");
                return;
            }

            Accelerometer.Default.ReadingChanged += OnAccelerometerReadingChanged;
            Accelerometer.Default.Start(SensorSpeed.Game);
            shakeActive = true;
            ShakeToggleButton.Text = "Disable";
            ShakeStatusLabel.Text = "Shake detection: Active";
            SetStatus("Shake detection enabled. Shake your device!");
        }
        catch (Exception ex)
        {
            SetStatus($"Accelerometer error: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops the accelerometer.
    /// </summary>
    private void StopShakeDetection()
    {
        try
        {
            Accelerometer.Default.ReadingChanged -= OnAccelerometerReadingChanged;
            Accelerometer.Default.Stop();
            shakeActive = false;
            ShakeToggleButton.Text = "Enable";
            ShakeStatusLabel.Text = "Shake detection: Disabled";
        }
        catch
        {
            // Ignore errors when stopping
        }
    }

    /// <summary>
    /// Handles accelerometer updates to detect shake gestures.
    /// Uses a simple threshold-based shake detection algorithm.
    /// </summary>
    private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var data = e.Reading;
        var magnitude = Math.Sqrt(data.Acceleration.X * data.Acceleration.X +
                                   data.Acceleration.Y * data.Acceleration.Y +
                                   data.Acceleration.Z * data.Acceleration.Z);

        if (magnitude > 2.5)
        {
            var now = DateTime.Now;
            if ((now - lastShakeTime).TotalMilliseconds > 500)
            {
                lastShakeTime = now;
                shakeCount++;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ShakeCountLabel.Text = $"Shake count: {shakeCount}";
                    ShakeStatusLabel.Text = "Shake detected!";
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
                    HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                    SetStatus($"Shake #{shakeCount} detected! Device orientation may affect detection.");
                });
            }
        }
    }

    /// <summary>
    /// Builds a formatted address string from a location using geocoding.
    /// Falls back to region-based address if geocoding fails.
    /// </summary>
    private static async Task<string> BuildAddressTextAsync(Location location)
    {
        try
        {
            var placemarks = await Geocoding.Default.GetPlacemarksAsync(location);
            var placemark = placemarks?.FirstOrDefault();
            var address = FormatPlacemark(placemark);

            if (!string.IsNullOrWhiteSpace(address))
            {
                return address;
            }
        }
        catch
        {
            // Geocoding may fail on some devices or simulators
        }

        return BuildFallbackAddress(location);
    }

    /// <summary>
    /// Formats placemark information into a readable address string.
    /// </summary>
    private static string FormatPlacemark(Placemark? placemark)
    {
        if (placemark is null)
        {
            return string.Empty;
        }

        var parts = new[]
        {
            placemark.CountryName,
            placemark.AdminArea,
            placemark.Locality,
            placemark.SubLocality,
            placemark.Thoroughfare
        }
        .Where(part => !string.IsNullOrWhiteSpace(part))
        .Distinct()
        .ToArray();

        return parts.Length == 0 ? string.Empty : string.Join(" / ", parts);
    }

    /// <summary>
    /// Provides fallback address based on geographic region when geocoding is unavailable.
    /// </summary>
    private static string BuildFallbackAddress(Location location)
    {
        if (IsNear(location, 37.422, -122.084, 0.08))
        {
            return "United States / California / Mountain View";
        }

        if (location.Latitude is >= 37.0 and <= 38.2 && location.Longitude is >= -123.2 and <= -121.5)
        {
            return "United States / California / San Francisco Bay Area";
        }

        if (location.Latitude is >= 18 and <= 54 && location.Longitude is >= 73 and <= 135)
        {
            return "China / Current city requires a real device or available geocoding service";
        }

        return "Coordinates were found, but country and city were not returned by this device.";
    }

    /// <summary>
    /// Checks if a location is near a specific coordinate within tolerance.
    /// </summary>
    private static bool IsNear(Location location, double latitude, double longitude, double tolerance)
    {
        return Math.Abs(location.Latitude - latitude) <= tolerance &&
               Math.Abs(location.Longitude - longitude) <= tolerance;
    }

    /// <summary>
    /// Handles the Read Help button click to read help content aloud.
    /// </summary>
    private async void OnReadHelpClicked(object? sender, EventArgs e)
    {
        try
        {
            const string helpText = "NutriBite records foods and drinks, shows nutrition details, and uses camera, location, gyroscope, shake detection, speech, and haptic feedback to make meal tracking more practical.";
            await SpeechService.SpeakAsync(helpText);
            SetStatus("Reading help content aloud.");
        }
        catch (Exception ex)
        {
            SetStatus($"Text to speech error: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the Stop Speech button click to stop ongoing text-to-speech.
    /// </summary>
    private void OnStopSpeechClicked(object? sender, EventArgs e)
    {
        SpeechService.Stop();
        SetStatus("Reading stopped.");
    }

    /// <summary>
    /// Handles the Haptic Feedback button click to trigger vibration and haptic feedback.
    /// </summary>
    private void OnFeedbackClicked(object? sender, EventArgs e)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(450));
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            feedbackTestCount++;
            FeedbackCountLabel.Text = $"Haptic feedback tests: {feedbackTestCount}";
            SetStatus("Vibration and haptic feedback triggered. The changing counter can be used for screen-recorded verification.");
        }
        catch (Exception ex)
        {
            SetStatus($"Feedback error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the status label and announces the message to screen readers.
    /// </summary>
    private void SetStatus(string message)
    {
        HardwareStatusLabel.Text = message;
        SemanticScreenReader.Announce(message);
    }
}
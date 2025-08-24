using Meadow;
using Meadow.Foundation.Leds;
using System;

namespace LedHoodiesCommon;

/// <summary>
/// Interface for an APA102 LED display system that can be based on timing.
/// </summary>
public interface ILedDisplay
{
    /// <summary>
    /// Set the appropriate color and brightness for all LEDs for the given time.
    /// </summary>
    /// <param name="apa102">An instance of the APA102 LED controller.</param>
    /// <param name="currentTicks">The current ticks in the system.</param>
    void DrawDisplay(Apa102 apa102, long currentTicks);
}

interface ITimingLedDisplay
{
    void DrawDisplay(long currentTicks);
}
class Apa102TimingLedDisplay : ITimingLedDisplay
{
    Apa102 Apa102 { get; set; }
    public int NumberOfLeds { get; set; }
    public float MaxBrightness { get; set; }

    public Apa102TimingLedDisplay(Apa102 apa102, float maxBrightness) : base()
    {
        Apa102 = apa102;
        NumberOfLeds = Apa102.NumberOfLeds;
        MaxBrightness = maxBrightness;
    }

    long priorTicks = 0;
    public void DrawDisplay(long currentTicks)
    {
        if (priorTicks == 0)
        {
            // Maybe do initial setup here.
            priorTicks = currentTicks;
        }

        long ticksElapsed = currentTicks - priorTicks;
        Resolver.Log.Info($"Ticks Delta: {ticksElapsed / TimeSpan.TicksPerMillisecond}ms");
        priorTicks = currentTicks;

        Resolver.Log.Info($"DrawDisplay: Apa102TimingLedDisplay");
        var rand = new Random();
        Apa102.Clear();
        for (int i = 0; i < Apa102.NumberOfLeds; i++)
        {
            Apa102.SetLed(i, ColorHelpers.GetRandomColor(rand), brightness: MaxBrightness);
        }
        Apa102.Show();
    }
}

interface IEventXYTiltDisplay
{
    void MoveLeft();
    void MoveRight();
}

// class CliDisplay : ITimingLedDisplay
// {
// }

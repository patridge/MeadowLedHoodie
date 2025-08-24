using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Leds;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LedHoodiesCommon;

namespace FeatherLedHoodie;

public class MeadowApp : App<F7FeatherV2>
{
    public bool IsUpdating = false;

    RgbPwmLed? onboardLed;
    Apa102? apa102;
    const int numberOfLeds = 51; // longer, high-density strand
    // const int numberOfLeds = 15; // short strand
    float MaxBrightness { get; set; } = 0.05f;
    // float tiltAngleThreshold = 0.25f;
    readonly TimeSpan sensorUpdateTime = TimeSpan.FromMilliseconds(100);
    // RobotEyeDisplay ledDisplay = new RobotEyeDisplay(numberOfLeds);
    // SnakeDisplay snakeDisplay = new SnakeDisplay((int)Math.Floor(numberOfLeds / 4.0), Color.Red, maxBrightness);
    // StaticDisplay staticDisplay = new StaticDisplay(numberOfLeds, Color.Red, maxBrightness);
    bool clearStripOnDoneUpdating = false;
    
    ILedDisplay? currentDisplay;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        onboardLed = new RgbPwmLed(
            redPwmPin: Device.Pins.OnboardLedRed,
            greenPwmPin: Device.Pins.OnboardLedGreen,
            bluePwmPin: Device.Pins.OnboardLedBlue,
            CommonType.CommonAnode);

        // NOTE: On Project Lab, MikroBus2 shares the SPI bus with the display. Using the display can cause strip to display some unexpected colors on some LEDs.
        apa102 = new Apa102(Device.CreateSpiBus(), numberOfLeds, Apa102.PixelOrder.BGR);
        apa102.Brightness = MaxBrightness;
        // apa102.Clear();

        currentDisplay = new RandomVariationStaticDisplay(numberOfLeds, MaxBrightness);

        return base.Initialize();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        onboardLed?.SetColor(Color.Red);

        StartUpdating();

        return base.Initialize();
    }

    private void StartUpdating()
    {
        IsUpdating = true;

        DrawLights(currentDisplay!);
        _ = Task.Run(async () =>
        {
            while (IsUpdating)
            {
                DrawLights(currentDisplay!);
                await Task.Delay(1000).ConfigureAwait(false);
            }
        });
    }
    private void StopUpdating()
    {
        if (clearStripOnDoneUpdating)
        {
            apa102?.Clear();
            apa102?.Show();
        }
        IsUpdating = false;
    }
    
    long priorDTNTicks = DateTime.Now.Ticks;
    // long priorEnvTickCount = Environment.TickCount;
    void DrawLights(ILedDisplay ledDisplay)
    {
        long currentDTNTicks = DateTime.Now.Ticks;
        // long currentEnvTickCount = Environment.TickCount;
        long ticksElapsed = currentDTNTicks - priorDTNTicks;
        Resolver.Log.Info($"DT.N.Ticks Delta: {(ticksElapsed) / TimeSpan.TicksPerMillisecond}ms");
        // Resolver.Log.Info($"Env.TickCount Delta: {(currentEnvTickCount - priorEnvTickCount) / TimeSpan.TicksPerMillisecond}ms");
        priorDTNTicks = currentDTNTicks;
        // priorEnvTickCount = currentEnvTickCount;

        // Resolver.Log.Info((ledDisplay as SnakeDisplay)!.ToString(apa102!.NumberOfLeds));
        ledDisplay.DrawDisplay(apa102!, ticksElapsed);
        // // Resolver.Log.Trace($"ShowCursor: {cursorLocation}");
        // apa102!.Clear();
        // // apa102.SetLed(cursorLocation, cursorColor);
        // for (int i = 0; i < numberOfLeds; i++)
        // {
        //     // var brightness = ledDisplay.GetLedBrightness(i - cursorLocation);
        //     // apa102.SetLed(i, cursorColor, brightness);
        //     var color = snakeDisplay.GetLedColor(i);
        //     var brightness = snakeDisplay.GetLedBrightness(i);
        //     apa102.SetLed(i, color, brightness);
        // }
        // apa102.Show();
    }

    class TimedCalculationBasedDisplay : ILedDisplay
    {
        public TimedCalculationBasedDisplay(int numberOfLeds, float maxBrightness, Func<int, long> drawLedForTimeFunction)
        {
            NumberOfLeds = numberOfLeds;
            MaxBrightness = maxBrightness;
        }

        public int NumberOfLeds { get; set; }
        public float MaxBrightness { get; set; }

        public void DrawDisplay(Apa102 apa102, long ticksElapsed)
        {
            Resolver.Log.Info($"DrawDisplay: TimedCalculationBasedDisplay");
            apa102.Clear();

            for (int i = 0; i < apa102.NumberOfLeds; i++)
            {
                // drawLedForTimeFunction(i, ticksElapsed);
                // apa102.SetLed(i, randomColor, brightness: MaxBrightness);
            }

            apa102.Show();
        }
    }
    
    class RandomSingleStaticDisplay : ILedDisplay
    {
        static Random rand = new Random();
        public RandomSingleStaticDisplay(int numberOfLeds, float maxBrightness)
        {
            NumberOfLeds = numberOfLeds;
            MaxBrightness = maxBrightness;
        }

        public int NumberOfLeds { get; set; }
        public float MaxBrightness { get; set; }
        public void DrawDisplay(Apa102 apa102, long _)
        {
            var randomColor = ColorHelpers.GetRandomColor(rand);
            Resolver.Log.Info($"DrawDisplay: RandomSingleStaticDisplay");
            apa102.Clear();
            for (int i = 0; i < apa102.NumberOfLeds; i++) {
                apa102.SetLed(i, randomColor, brightness: MaxBrightness);
            }
            apa102.Show();
        }
    }
    class RandomVariationStaticDisplay : ILedDisplay
    {
        public RandomVariationStaticDisplay(int numberOfLeds, float maxBrightness)
        {
            NumberOfLeds = numberOfLeds;
            MaxBrightness = maxBrightness;
        }

        public int NumberOfLeds { get; set; }
        public float MaxBrightness { get; set; }

        public void DrawDisplay(Apa102 apa102, long _)
        {
            Resolver.Log.Info($"DrawDisplay: RandomVariationStaticDisplay");
            var rand = new Random();
            apa102.Clear();
            for (int i = 0; i < apa102.NumberOfLeds; i++) {
                apa102.SetLed(i, ColorHelpers.GetRandomColor(rand), brightness: MaxBrightness);
            }
            apa102.Show();
        }
    }
}
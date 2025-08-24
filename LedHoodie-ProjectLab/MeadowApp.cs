using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using LedHoodiesCommon;

namespace ProjectLabLedHoodie;

public class MeadowApp : App<F7CoreComputeV2>
{
    IProjectLabHardware? projLab;
    MicroGraphics? graphics;

    public bool IsUpdating = false;

    Apa102? apa102;
    const int numberOfLeds = 51; // longer, high-density strand
    //const int numberOfLeds = 15; // short strand
    float MaxBrightness { get; set; } = 0.05f;
    float tiltAngleThreshold = 0.25f;
    readonly TimeSpan sensorUpdateTime = TimeSpan.FromMilliseconds(100);
    // RobotEyeDisplay ledDisplay = new RobotEyeDisplay(numberOfLeds);
    // SnakeDisplay snakeDisplay = new SnakeDisplay((int)Math.Floor(numberOfLeds / 4.0), Color.Red, maxBrightness);
    // StaticDisplay staticDisplay = new StaticDisplay(numberOfLeds, Color.Red, maxBrightness);
    bool clearStripOnDoneUpdating = false;

    ILedDisplay? currentDisplay;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        projLab = ProjectLab.Create();
        graphics = new MicroGraphics(projLab!.Display!);

        projLab.LeftButton!.Clicked += ButtonLeft_Clicked;
        projLab.RightButton!.Clicked += ButtonRight_Clicked;

        projLab.DownButton!.Clicked += ButtonDown_Clicked;
        projLab.UpButton!.Clicked += ButtonUp_Clicked;

        projLab.RgbLed!.SetColor(Color.Green);

        // NOTE: On Project Lab, MikroBus2 shares the SPI bus with the display. Using the display can cause strip to display some unexpected colors on some LEDs.
        apa102 = new Apa102(projLab.MikroBus1.SpiBus, numberOfLeds, Apa102.PixelOrder.BGR);
        apa102.Brightness = MaxBrightness;
        // apa102.Clear();

        currentDisplay = new RandomVariationStaticDisplay(numberOfLeds, MaxBrightness);

        graphics.Clear();
        graphics.DrawText(0, 0, "Initializing ...", Color.White);
        graphics.Show();

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        StartUpdating();

        return Task.CompletedTask;
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

    private void ButtonUp_Clicked(object sender, EventArgs e)
    {
        if (IsUpdating)
        {
            Resolver.Log.Info("ButtonUp_Clicked: Stopping updates.");
            StopUpdating();
        }
        else
        {
            Resolver.Log.Info("ButtonUp_Clicked: Starting updates.");
            StartUpdating();
        }
    }

    private void ButtonDown_Clicked(object sender, EventArgs e)
    {
    }

    private void ButtonRight_Clicked(object sender, EventArgs e)
    {
    }

    private void ButtonLeft_Clicked(object sender, EventArgs e)
    {
    }

    private void OnAccelerometerUpdated(object sender, IChangeResult<Acceleration3D> e) {
        // Resolver.Log.Info($"Accel Gravity: {e.New.X.Gravity}, {e.New.Y.Gravity}, {e.New.Z.Gravity}g");
        var gravityAngle = new Vector3(
            (float) e.New.X.Gravity,
            (float) e.New.Y.Gravity,
            (float) e.New.Z.Gravity
        );

        if (gravityAngle.Y > tiltAngleThreshold)
        {
            // cursorLocation += 1;
            // if (cursorLocation >= numberOfLeds) { cursorLocation = numberOfLeds - 1; }
            // DrawLights(ledDisplay);
            // currentDisplay!.MoveRight();
            // DrawLights(currentDisplay);
        }
        else if (gravityAngle.Y < -tiltAngleThreshold)
        {
            // cursorLocation -= 1;
            // if (cursorLocation < 0) { cursorLocation = 0; }
            // DrawLights(ledDisplay);
            // currentDisplay!.MoveLeft();
            // DrawLights(currentDisplay);
        }
    }
    
    long priorDTNTicks = DateTime.UtcNow.Ticks;
    // long priorEnvTickCount = Environment.TickCount;
    void DrawLights(ILedDisplay ledDisplay)
    {
        long currentDTNTicks = DateTime.UtcNow.Ticks;
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
    
    class StaticDisplay : ILedDisplay
    {
        public StaticDisplay(int numberOfLeds, Color staticColor, float maxBrightness)
        {
            StaticColor = staticColor;
            NumberOfLeds = numberOfLeds;
            MaxBrightness = maxBrightness;
        }

        public Color StaticColor { get; set; }
        public int NumberOfLeds { get; set; }
        public float MaxBrightness { get; set; }

        public void DrawDisplay(Apa102 apa102, long _)
        {
            Resolver.Log.Info($"DrawDisplay: StaticDisplay");
            apa102.Clear();
            for (int i = 0; i < NumberOfLeds; i++)
            {
                apa102.SetLed(i, StaticColor, MaxBrightness);
            }
            apa102.Show();
        }
    }
    class SnakeDisplay : ILedDisplay, IEventXYTiltDisplay
    {
        int BodyLength { get; set; }
        public Color SnakeColor { get; set; }
        LinkedList<SnakeSegment> Body { get; set; }
        /// <summary>
        /// Allow for customizing the off color, such as a specific color for the background.
        /// </summary>
        public Color OffColor { get; set; } = Color.Black;
        /// <summary>
        /// Allow for customizing the off brightness, such as a specific brightness for the background.
        /// </summary>
        public float OffBrightness { get; set; } = 0.0f;

        public SnakeDisplay(int bodyLength, Color snakeColor, float maxBrightness)
        {
            BodyLength = bodyLength;
            SnakeColor = snakeColor;
            // Body created with body length items.
            Body = new LinkedList<SnakeSegment>();
            for (int i = 0; i < BodyLength; i++)
            {
                // Start all body segments at initial location.
                // TODO: Adjust brightness based on distance from head.
                Body.AddLast(new SnakeSegment() { SegmentLedIndex = 0, Color = snakeColor, Brightness = maxBrightness });
            }
        }

        // TODO: Show brightness-fading body behind the head, with a length of NumberOfLeds.
        // When the head moves, the tail should move with it, potentially overlapping body segments.
        // Head cannot move off either end of the strip.

        public void MoveLeft()
        {
            var head = Body.First;
            if (head == null) { return; }
            // Move the head one index to the left.
            int newHeadIndex = Body.First.Value.SegmentLedIndex - 1;
            Resolver.Log.Debug($"MoveLeft: newHeadIndex: {newHeadIndex}, numberOfLeds: {numberOfLeds}");
            if (newHeadIndex < 0) {
                // Head is at the end of the strip and won't move.
                // Clamp the new index and trigger the move down the body nodes.
                newHeadIndex = 0;
            }

            head.Move(newHeadIndex);
        }
        public void MoveRight()
        {
            var head = Body.First;
            if (head == null) { return; }
            // Move the head one index to the right.
            int newHeadIndex = Body.First.Value.SegmentLedIndex + 1;
            Resolver.Log.Debug($"MoveRight: newHeadIndex: {newHeadIndex}, numberOfLeds: {numberOfLeds}");
            if (newHeadIndex >= numberOfLeds) {
                // Head is at the end of the strip and won't move.
                // Clamp the new index and trigger the move down the body nodes.
                newHeadIndex = numberOfLeds - 1;
            }

            head.Move(newHeadIndex);
        }
        // TODO: Handle brightness and/or color variation when drawing the body.
        // float GetLedBrightness(int ledPosition)
        // {
        //     return Body.FirstOrDefault(segment => segment.SegmentIndex == ledPosition)?.Brightness ?? OffBrightness;
        // }
        // Color GetLedColor(int ledPosition)
        // {
        //     return Body.FirstOrDefault(segment => segment.SegmentIndex == ledPosition)?.Color ?? OffColor;
        // }
        public void DrawDisplay(Apa102 apa102, long _)
        {
            apa102.Clear();
            // Draw body segments from last to first, so segments closer to head take visual priority (e.g., dimmer tail node overridden by brighter head node).
            var segmentNode = Body.Last;
            while (segmentNode != null)
            {
                float segmentBrightness = segmentNode.Value.Brightness;
                Color segmentColor = segmentNode.Value.Color;
                apa102.SetLed(segmentNode.Value.SegmentLedIndex, segmentColor, segmentBrightness);
                segmentNode = segmentNode.Previous;
            }
            // foreach (var segment in Body.rev)
            // {
            //     float segmentBrightness = segment.Brightness;
            //     Color segmentColor = segment.Color;
            //     apa102.SetLed(segment.SegmentLedIndex, segmentColor, segmentBrightness);
            // }
            apa102.Show();
        }

        public string ToString(int displayLength)
        {
            var textDisplay = new string(' ', displayLength);
            foreach (var segment in Body)
            {
                textDisplay = textDisplay.Remove(segment.SegmentLedIndex, 1).Insert(segment.SegmentLedIndex, "*");
            }
            return $"[{textDisplay}]";
        }
    }
    class RobotEyeDisplay : ILedDisplay
    {
        public RobotEyeDisplay(int numberOfLeds)
        {
            // NumberOfLeds = numberOfTrailLeds;
            TrailLength = numberOfLeds / 3;
            // LedColors = new Color[NumberOfLeds];
            // LedBrightness = new float[NumberOfLeds];
        }

        public void MoveLeft()
        {
            CurrentHeadIndex -= 1;
            if (CurrentHeadIndex < 0) { CurrentHeadIndex = 0; }
        }
        public void MoveRight()
        {
            CurrentHeadIndex += 1;
            if (CurrentHeadIndex >= numberOfLeds) { CurrentHeadIndex = numberOfLeds - 1; }
        }

        public int CurrentHeadIndex { get; private set; }
        public int TrailLength { get; }
        public Color MeteorColor { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="cursorDeltaIndex">
        /// Delta of current LED from current cursor LED location. A `cursorDetlaIndex` of 0 is the cursor, -1 is one behind the cursor, 2 is two ahead of the cursor, etc.
        /// </param>
        float GetLedBrightness(int ledPosiiton)
        {
            int cursorDeltaIndex = ledPosiiton - CurrentHeadIndex;
            if (cursorDeltaIndex == 0)
            {
                return 1.0f;
            }
            else if (cursorDeltaIndex > TrailLength)
            {
                return 0.0f;
            }
            else {
                // For Meteor Trail, we want the cursor to be the brightest, and then trail off behind the cursor.
                return 1.0f / (float)Math.Pow(2, Math.Abs(cursorDeltaIndex));
            }
        }
        Color GetLedColor(int ledPosiiton)
        {
            return MeteorColor;
        }
        public void DrawDisplay(Apa102 apa102, long _)
        {
            apa102.Clear();
            for (int i = 0; i < numberOfLeds; i++)
            {
                var brightness = GetLedBrightness(i);
                var color = GetLedColor(i);
                apa102.SetLed(i, color, brightness);
            }
            apa102.Show();
        }
    }
}

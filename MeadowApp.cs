using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace ProjectLabLedHoodie;

public class MeadowApp : App<F7CoreComputeV2>
{
    IProjectLabHardware? projLab;
    MicroGraphics? graphics;
    IBadgePage? ledStripPage = new LedStripPage();

    public bool IsUpdating = false;

    const int numberOfLeds = 51; // longer, high-density strand
    //const int numberOfLeds = 15; // short strand
    // float MaxBrightness { get; set; } = 0.05f;
    // float tiltAngleThreshold = 0.25f;
    // readonly TimeSpan sensorUpdateTime = TimeSpan.FromMilliseconds(100);
    // RobotEyeDisplay ledDisplay = new RobotEyeDisplay(numberOfLeds);
    // SnakeDisplay snakeDisplay = new SnakeDisplay((int)Math.Floor(numberOfLeds / 4.0), Color.Red, maxBrightness);
    // StaticDisplay staticDisplay = new StaticDisplay(numberOfLeds, Color.Red, maxBrightness);
    bool clearStripOnDoneUpdating = false;

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
        ledStripPage!.Init(projLab);

        graphics.Clear();
        graphics.DrawText(0, 0, "Initializing ...", Color.White);
        graphics.Show();

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        // Start updating LED strip (page[0])
        ledStripPage!.StartUpdating(projLab!, graphics!);

        return Task.CompletedTask;
    }
    
    private void ButtonUp_Clicked(object sender, EventArgs e)
    {
        ledStripPage!.Up();
    }

    private void ButtonDown_Clicked(object sender, EventArgs e)
    {
        ledStripPage!.Down();
    }

    private void ButtonRight_Clicked(object sender, EventArgs e)
    {
        ledStripPage!.Right();
    }

    private void ButtonLeft_Clicked(object sender, EventArgs e)
    {
        ledStripPage!.Left();
    }
}

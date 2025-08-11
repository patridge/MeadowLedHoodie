using Meadow.Foundation.Leds;

namespace FeatherLedHoodie;

interface ILedDisplay
{
    // float GetLedBrightness(int ledPosiiton);
    // Color GetLedColor(int ledPosiiton);
    void MoveLeft();
    void MoveRight();
    void DrawDisplay(Apa102 apa102, long ticksElapsed);
    // TODO: ??? Add a method to add a frame to any animations, to allow for "settling" or "momentum" of movements.
    // TODO: ??? Add timing system to keep from speeding up animation just because we are sampling tilt faster.
}

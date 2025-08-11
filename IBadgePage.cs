using Meadow.Devices;
using Meadow.Foundation.Graphics;

namespace ProjectLabLedHoodie
{
    public interface IBadgePage
    {
        void Init(IProjectLabHardware projLab);

        void StartUpdating(IProjectLabHardware projLab, MicroGraphics graphics);

        void StopUpdating();

        void Reset();

        void Left();
        void Right();
        void Up();
        void Down();
    }
}
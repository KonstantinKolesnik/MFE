
namespace MFE.SmartNetwork.Network
{
    public enum ControlLineType : byte
    {
        //Relay, WaterSensor, PHSensor, ORPSensor, TemperatureSensor, ConductivitySensor + SalinitySensor

        Digital,
        Analog,
        PWM,
        OneWire
    }
}

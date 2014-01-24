
namespace Gadgeteer.Modules.LoveElectronics
{
    /// <summary>Use One-Shot if very low power consuption is required</summary>
    public enum MCP342XConversionMode
    {
        /// <summary>
        /// Requests a value, waits for conversion then returns value
        /// </summary
        OneShot,
        Continuous
    }
}

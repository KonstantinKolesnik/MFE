
namespace Gadgeteer.Modules.LoveElectronics
{
    /// <summary>
    /// The resolution of the MCP324X
    /// Resolution affects convertion time (more bits, slower conversion)
    /// </summary>
    public enum MCP324XResolution : byte
    {
        /// <summary>
        /// Twelve Bit resolution.
        /// </summary>
        TwelveBits,
        /// <summary>
        /// Fourteen Bit resolution.
        /// </summary>
        FourteenBits,
        /// <summary>
        /// Sixteen Bit resolution.
        /// </summary>
        SixteenBits,
        /// <summary>
        /// Eighteen Bit resolution.
        /// </summary>
        EighteenBits
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MFE.GraphicsDemo
{
    
    internal partial class Resources
    {
        private static System.Resources.ResourceManager manager;
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if ((Resources.manager == null))
                {
                    Resources.manager = new System.Resources.ResourceManager("MFE.GraphicsDemo.Resources", typeof(Resources).Assembly);
                }
                return Resources.manager;
            }
        }
        internal static Microsoft.SPOT.Font GetFont(Resources.FontResources id)
        {
            return ((Microsoft.SPOT.Font)(Microsoft.SPOT.ResourceUtility.GetObject(ResourceManager, id)));
        }
        internal static string GetString(Resources.StringResources id)
        {
            return ((string)(Microsoft.SPOT.ResourceUtility.GetObject(ResourceManager, id)));
        }
        internal static byte[] GetBytes(Resources.BinaryResources id)
        {
            return ((byte[])(Microsoft.SPOT.ResourceUtility.GetObject(ResourceManager, id)));
        }
        [System.SerializableAttribute()]
        internal enum BinaryResources : short
        {
            Home = -29342,
            reWalls = -26200,
            Drive = -19534,
            test_24b = -15341,
            Background_800_600 = 3700,
            Keyboard = 3946,
            Database = 6837,
            ButtonBackground = 7118,
            Settings = 10758,
            Logo = 16403,
            Bar = 16851,
            Mouse = 26648,
            Operation = 29388,
        }
        [System.SerializableAttribute()]
        internal enum FontResources : short
        {
            CourierNew_10 = -22402,
            small = 13070,
            NinaB = 18060,
            SegoeUI_BoldItalian_32 = 27220,
            LucidaSansUnicode_8 = 30591,
        }
        [System.SerializableAttribute()]
        internal enum StringResources : short
        {
            String1 = 1228,
        }
    }
}

namespace MFE.Graphics.Touching
{
    public enum TouchGesture : uint
    {
        NoGesture = 0,          //Can be used to represent an error gesture or unknown gesture

        // Standard Win7 Gestures
        Begin = 1,              // Used to identify the beginning of a Gesture Sequence; App can use this to highlight UIElement or some other sort of notification.
        End = 2,                // Used to identify the end of a gesture sequence; Fired when last finger involved in a gesture is removed.

        // Standard stylus (single touch) gestues
        Right = 3,
        UpRight = 4,
        Up = 5,
        UpLeft = 6,
        Left = 7,
        DownLeft = 8,
        Down = 9,
        DownRight = 10,
        Tap = 11,
        DoubleTap = 12,

        // Multi-touch gestures
        Zoom = 114,             // Equivalent to your "Pinch" gesture
        Pan = 115,              // Equivalent to your "Scroll" gesture
        Rotate = 116,
        TwoFingerTap = 117,
        Rollover = 118,         // Press and tap

        // Additional NetMF gestures
        UserDefined = 200,
    }
}

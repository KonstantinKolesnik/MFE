using MFE.Core;
using Microsoft.SPOT;
using Microsoft.SPOT.Touch;

namespace MFE.Graphics
{
    public static class CalibrationManager
    {
        #region Fields
        private class CalibrationPointsID { }
        private static ExtendedWeakReference ewrCalibrationPoints;
        private static CalibrationPoints calibrationPoints = null;
        #endregion

        #region Properties
        public static bool IsCalibrated
        {
            get { return calibrationPoints != null; }
        }
        public static CalibrationPoints CalibrationPoints
        {
            get { return calibrationPoints; }
        }
        #endregion

        #region Constructor
        static CalibrationManager()
        {
            LoadCalibrationPoints();
        }
        #endregion

        #region Public methods
        private static void LoadCalibrationPoints()
        {
            ewrCalibrationPoints = ExtendedWeakReference.RecoverOrCreate(typeof(CalibrationPointsID), 0, ExtendedWeakReference.c_SurvivePowerdown | ExtendedWeakReference.c_SurviveBoot);
            ewrCalibrationPoints.Priority = (int)ExtendedWeakReference.PriorityLevel.System;
            calibrationPoints = (CalibrationPoints)ewrCalibrationPoints.Target;
        }
        public static void SaveCalibrationPoints()
        {
            ewrCalibrationPoints.Target = calibrationPoints;
            //if (!Utils.IsEmulator)
            //    Util.FlushExtendedWeakReferences();
        }

        public static void PrepareCalibrationPoints()
        {
            calibrationPoints = new CalibrationPoints();

            int pointCount = 0;
            Touch.ActiveTouchPanel.GetCalibrationPointCount(ref pointCount);

            calibrationPoints.ScreenX = new short[pointCount];
            calibrationPoints.ScreenY = new short[pointCount];
            calibrationPoints.TouchX = new short[pointCount];
            calibrationPoints.TouchY = new short[pointCount];

            // Get the points for calibration.
            for (int index = 0; index < pointCount; index++)
            {
                int x = 0;
                int y = 0;
                Touch.ActiveTouchPanel.GetCalibrationPoint(index, ref x, ref y);
                calibrationPoints.ScreenX[index] = (short)x;
                calibrationPoints.ScreenY[index] = (short)y;
            }
        }
        public static void StartCalibration()
        {
            Touch.ActiveTouchPanel.StartCalibration();
        }
        public static void ApplyCalibrationPoints()
        {
            if (Touch.ActiveTouchPanel != null)
                Touch.ActiveTouchPanel.SetCalibration(calibrationPoints.Count, calibrationPoints.ScreenX, calibrationPoints.ScreenY, calibrationPoints.TouchX, calibrationPoints.TouchY);
        }
        #endregion
    }
}

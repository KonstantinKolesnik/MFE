using Microsoft.SPOT;
using System;

namespace MFE.Storage
{
    public static class FlashManager
    {
        #region Fields
        private static ExtendedWeakReference ewr;
        #endregion

        #region Load / save
        public static object LoadFromFlash(Type dataType, uint id)
        {
            ewr = ExtendedWeakReference.RecoverOrCreate(dataType, id, ExtendedWeakReference.c_SurvivePowerdown | ExtendedWeakReference.c_SurviveBoot);

            // Indicate how important this data is. The CLR discards from first to last:
            // OkayToThrowAway items
            // NiceToHave items
            // Important items
            // Critical items
            // System items.
            // In practice, System items are virtually never discarded.
            ewr.Priority = (Int32)ExtendedWeakReference.PriorityLevel.System;

            return ewr.Target;
        }
        public static void SaveToFlash(object data)
        {
            ewr.Target = data;
        }
        //public static void DeleteFromFlash()
        //{
        //    // Data recovering and storing must be kept in pairs.
        //    // Calling Recover without setting Target frees the FLASH completely from this EWR.
        //    ExtendedWeakReference.Recover(typeof(Options), 0);
        //}
        //public static void RestoreLastSaved() // Use this method to rewrite current settings using last saved data.
        //{
        //    // First, mark the stored data as unrecovered so we can Recover them.
        //    optionsReference.PushBackIntoRecoverList();
        //    // Try to recover them, but do not create them if they do not exist.
        //    ExtendedWeakReference restoreReference = ExtendedWeakReference.Recover(typeof(Options), 0);
        //    if (restoreReference != null && restoreReference.Target != null)
        //    {
        //        options = (Options)restoreReference.Target; // If they do, use them to refresh current settings.
        //        restoreReference.PushBackIntoRecoverList(); // Since we found the data, we have to put them back.
        //    }
        //    else
        //        Debug.Print("Could not restore settings.");
        //}
        #endregion
    }
}

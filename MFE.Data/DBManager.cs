using GHI.Premium.SQLite;
using MFE.Core;
using Microsoft.SPOT;
using System;
using System.IO;

namespace MFE.Data
{
    public class DBManager
    {
        #region Fields
        public const string MEMORY_FILENAME = ":memory:";
        
        private string fileName = null;
        private Database db = new Database();
        #endregion

        #region Properties
        public long Size
        {
            get
            {
                long res = 0;

                if (!Utils.StringIsNullOrEmpty(fileName) && File.Exists(fileName))
                    using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        res = fs.Length;
                
                return res;
            }
        }
        #endregion

        // Summary:
        //   Opens or creates SQLite database with the specified path. The path could be to RAM ":memory:" or to a file "\\SD\\mydatabase.dat"
        // Parameters:
        //   path:
        //     :memory: to create database in RAM
        public bool Open(string path)
        {
            try
            {
                db.Open(path);
                fileName = path;
            }
            catch (Exception e)
            {
                fileName = null;
                Debug.Print(e.Message);
                Debug.Print(Database.GetLastError());
                return false;
            }

            return true;
        }
        public void Close()
        {
            db.Close();
            fileName = null;
        }








    }
}

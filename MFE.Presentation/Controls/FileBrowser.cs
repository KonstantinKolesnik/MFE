using System.Collections;
using System.IO;
using GHIElectronics.NETMF.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.IO;
using Microsoft.SPOT.Presentation.Controls;

namespace Typhoon.MF.Presentation.Controls
{
    public class FileBrowser : ListBox
    {
        public enum StorageType
        {
            SD,
            USB
        }

        #region Fields
        private Font font;
        private StorageType storageType;
        private PersistentStorage storage;
        private ArrayList volumes = new ArrayList(); // Volumes
        private ArrayList folderList = new ArrayList(); // Folder navigation history
        #endregion

        #region Constructor
        public FileBrowser(StorageType deviceType, Font font)
        {
            this.storageType = deviceType;
            this.font = font;
            
            if (deviceType == StorageType.SD)
            {
                //Font font = Resources.GetFont(Resources.FontResources.Small);

                // Check for an SD card
                //Bitmap LCD = new Bitmap(SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight);
                //LCD.Clear();
                //LCD.DrawText("Initializing SD card...", font, Color.White, 88, (SystemMetrics.ScreenHeight - font.Height) / 2);
                //LCD.Flush();

                try
                {
                    storage = new PersistentStorage("SD");
                    storage.MountFileSystem();
                }
                catch
                {
                    storage = null;

                    //LCD.Clear();
                    //LCD.DrawText("No SD card found.", font, Color.White, 88, (SystemMetrics.ScreenHeight - font.Height) / 2);
                    //LCD.Flush();

                    //Thread.Sleep(2000);
                    //Exit();
                }

                //LCD.Dispose();
                //LCD = null;
            }

            GetVolumes();
            PopulateVolumes();
        }
        public void Dispose()
        {
            if (storageType == FileBrowser.StorageType.SD && storage != null)
            {
                storage.Dispose();
                storage = null;
            }
        }
        #endregion

        #region Public methods
        public void Update()
        {
            GetVolumes();
            if (OnRemovedVolume())
            {
                // Go back to displaying the volume list
                PopulateVolumes();

                // Clear the selection
                SelectedIndex = -1;
            }
        }
        #endregion

        #region Event handlers
        private void Item_TouchDown(object sender, TouchEventArgs e)
        {
            FileBrowserItem item = (FileBrowserItem)sender;
            if (SelectedItem != item)
                SelectedItem = item;
            else
                PerformItemAction();
        }
        #endregion

        #region Private methods
        private void GetVolumes()
        {
            volumes.Clear();
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory());
            for (int i = 0; i < dirs.Length; i++)
                if (dirs[i].IndexOf(storageType == StorageType.USB ? "USB" : "SD") > -1)
                    volumes.Add(dirs[i]);
        }
        private void PopulateVolumes()
        {
            Items.Clear();
            folderList.Clear();

            FileBrowserItem item;
            if (volumes.Count == 0) // No volumes available -- display a notice
            {
                if (storageType == StorageType.USB)
                {
                    item = new FileBrowserItem("No USB storage device.", font);
                    item.FileType = FileBrowserItem.TYPE_UNKNOWN;
                    item.IsSelectable = false;
                    Items.Add(item);
                }
            }
            else // Display the volumes
            {
                int index;
                string name;
                VolumeInfo info;

                for (int i = 0; i < volumes.Count; i++)
                {
                    info = new VolumeInfo((string)volumes[i]);
                    if (info.IsFormatted)
                    {
                        name = (string)volumes[i];

                        // Parse out the name
                        index = name.LastIndexOf('\\');
                        if (index > -1)
                            name = name.Substring(index + 1, name.Length - (index + 1));

                        // Create the new list item
                        item = new FileBrowserItem(name, font);
                        item.FileType = "folder";
                        item.FilePath = (string)volumes[i];
                        item.TouchDown += Item_TouchDown;

                        Items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Determine if the folder we were viewing was on a volume that was removed.
        /// </summary>
        /// <returns>True or False</returns>
        private bool OnRemovedVolume()
        {
            if (folderList.Count > 0)
            {
                for (int i = 0; i < volumes.Count; i++)
                {
                    if (((string)folderList[folderList.Count - 1]).IndexOf((string)volumes[i]) > -1)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Performs an action based on the item type.
        /// </summary>
        private void PerformItemAction()
        {
            if (SelectedIndex == -1)
                return;

            FileBrowserItem item = (FileBrowserItem)Items[SelectedIndex];
            if (item.IsFolder) // Open the folder
            {
                folderList.Add(item.FilePath);
                PopulateFolder(item.FilePath);
            }
            else if (item.IsFolderUp) // Move to the parent folder
            {
                int lastIndex = folderList.Count - 1;
                string path = folderList[lastIndex] + "\\..";
                folderList.RemoveAt(lastIndex);

                PopulateFolder(path);
            }
            //else if (SystemInfo.SystemID.Model == (byte)SystemModelType.ChipworkX && item.IsExtension(FileListBoxItem.EXT_MP3))// Play the MP3
            //{
            //    Utilities.MP3Control.SetSource(item.FilePath);
            //    Utilities.MP3Control.Play();
            //}
        }

        /// <summary>
        /// Opens a folder and updates the list box.
        /// </summary>
        /// <param name="path">Folder path</param>
        private void PopulateFolder(string path)
        {
            Items.Clear();

            // Add the folder up option
            if (folderList.Count > 0)
            {
                FileBrowserItem item = new FileBrowserItem("[...]", font);
                item.FileType = FileBrowserItem.TYPE_FOLDER_UP;
                item.TouchDown += Item_TouchDown;
                Items.Add(item);
            }

            // Build the list box items
            GetItems(Directory.GetDirectories(path), FileBrowserItem.TYPE_FOLDER);
            GetItems(Directory.GetFiles(path), FileBrowserItem.TYPE_FILE);

            // Update the selection
            SelectedIndex = (Items.Count == 0 ? -1 : 0);
        }

        /// <summary>
        /// Loops through an array of items and adds them to the list box.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="type">"FOLDER" or null</param>
        private void GetItems(string[] items, string type)
        {
            FileBrowserItem item;
            for (int i = 0; i < items.Length; i++)
            {
                string path = items[i];

                // If the deviceType is not within the file path ignore this item
                if (path.IndexOf(storageType.ToString()) == -1)
                    continue;

                string label = "";
                int index = path.LastIndexOf('\\');
                if (index > -1)
                    label = path.Substring(index + 1, path.Length - (index + 1));

                if (type != FileBrowserItem.TYPE_FOLDER)
                {
                    index = items[i].LastIndexOf('.');
                    type = items[i].Substring(index, items[i].Length - index);
                }

                item = new FileBrowserItem(label, font);
                item.FileType = type;
                item.FilePath = items[i];
                item.TouchDown += Item_TouchDown;

                Items.Add(item);
            }
        }
        #endregion
    }
}

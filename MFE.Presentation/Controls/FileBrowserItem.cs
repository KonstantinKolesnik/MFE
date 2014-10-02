using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace Typhoon.MF.Presentation.Controls
{
    public class FileBrowserItem : ListBoxItem
    {
        // Visual aids
        private Font font;
        private Bitmap icon = null;
        private int iconSize = 16;
        private int iconOffset;
        private Brush selectedBrush;

        // File properties
        private string fileName;
        private string fileType;
        private string filePath;

        // File types
        public const string TYPE_FILE = "file";
        public const string TYPE_FOLDER = "folder";
        public const string TYPE_FOLDER_UP = "folderUp";
        public const string TYPE_UNKNOWN = "unknown";
        public const string EXT_MP3 = ".mp3";

        #region Properties
        public string FileName
        {
            get { return fileName; }
        }
        public string FileType
        {
            get { return fileType; }
            set
            {
                fileType = value;
                UpdateIcon();
            }
        }
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
        public bool IsFolder
        {
            get { return (fileType == TYPE_FOLDER); }
        }
        public bool IsFolderUp
        {
            get { return (fileType == TYPE_FOLDER_UP); }
        }
        #endregion

        public FileBrowserItem(string label, Font font)
        {
            this.font = font;
            Width = SystemMetrics.ScreenWidth;
            Height = 30;

            iconOffset = (Height - iconSize) / 2;
            selectedBrush = new SolidColorBrush(Colors.Blue);

            fileName = label;
            fileType = String.Empty;
            filePath = String.Empty;

            Text text = new Text(font, label);
            text.ForeColor = Color.White;
            text.Height = font.Height;
            text.Width = Width;
            text.VerticalAlignment = VerticalAlignment.Top;
            int top = (Height - font.Height) / 2;
            text.SetMargin(Height + iconOffset, top, 0, 0);

            Child = text;

            UpdateIcon();
        }

        public override void OnRender(DrawingContext dc)
        {
            if (IsSelected)
                dc.DrawRectangle(selectedBrush, null, 0, 0, Width, Height);
            else
                dc.DrawRectangle(null, null, 0, 0, Width, Height);

            if (icon != null)
                dc.DrawImage(icon, iconOffset, iconOffset);
        }

        public bool IsExtension(string ext)
        {
            return (fileType.ToLower() == ext);
        }

        #region Private methods
        private void UpdateIcon()
        {
            // Check types whose format is trusted
            //switch (fileType)
            //{
            //    case TYPE_FOLDER: icon = Resources.GetBitmap(Resources.BitmapResources.folder); break;
            //    case TYPE_FOLDER_UP: icon = Resources.GetBitmap(Resources.BitmapResources.folder_up); break;
            //    case TYPE_UNKNOWN: icon = Resources.GetBitmap(Resources.BitmapResources.unknown); break;
            //}

            if (icon != null)
                return;

            // Check types whose format isn't trusted
            //switch (fileType.ToLower())
            //{
            //    case ".cs":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_cs);
            //        break;
            //    case ".csproj":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_csproj);
            //        break;
            //    case ".user":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_csprojuser);
            //        break;
            //    case ".dll":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_dll);
            //        break;
            //    case ".exe":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_exe);
            //        break;
            //    case ".jpeg":
            //    case ".jpg":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_jpg);
            //        break;
            //    case ".gif":
            //    case ".png":
            //    case ".bmp":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_image);
            //        break;
            //    case ".rtf":
            //    case ".resx":
            //    case ".txt":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_text);
            //        break;
            //    case ".htm":
            //    case ".html":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_html);
            //        break;
            //    case ".pdf":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_pdf);
            //        break;
            //    case ".mov":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_mov);
            //        break;
            //    case ".vbs":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_vbs);
            //        break;
            //    case ".zip":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_zip);
            //        break;
            //    case EXT_MP3:
            //    case ".wmv":
            //    case ".avi":
            //    case ".mpeg":
            //    case ".mp4":
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_video);
            //        break;
            //    default:
            //        icon = Resources.GetBitmap(Resources.BitmapResources.file_other);
            //        break;
            //}

            Invalidate();
        }
        protected override void OnIsSelectedChanged(bool isSelected)
        {
            Invalidate();
        }
        #endregion
    }
}

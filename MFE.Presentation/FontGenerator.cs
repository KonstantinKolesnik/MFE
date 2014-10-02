/*
 * Copyright 2011, JASDev International
 *
 * The FEZ Touch Font Generator is based on the excellent font editor called The Dot Factory by Eran "Pavius" Duchan http://www.pavius.net.
 * 
 * The FEZ Touch Font Generator is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License as published by the Free 
 * Software Foundation, either version 3 of the License, or (at your option) 
 * any later version. The FEZ Touch Font Generator is distributed in the hope that it will be 
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more 
 * details. You should have received a copy of the GNU General Public License along 
 * with the FEZ Touch Font Generator. If not, see http://www.gnu.org/licenses/.
 */

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Resources;
using Microsoft.SPOT;

namespace Typhoon.MF.Presentation
{
    public partial class MainForm : Form
    {
        #region CONSTANTS

        // formatting strings
        public const string ApplicationVersion = "1.1";

        #endregion


        #region GLOBAL VARIABLES

        static string fileName = "";

        #endregion


        #region INTERNAL CLASSES

        internal class FontInfo
        {
            public string FontName;
            public int FontSize;
            public ushort AvgCharWidth;
            public ushort MaxCharWidth;
            public ushort CharHeight;
            public char StartChar;
            public char EndChar;
            public CharacterGenerationInfo[] Characters;
            public Font WindowsFont;
            public string GeneratedChars;
        }

        internal class ComboBoxItem
        {
            public string name;
            public string value;

            // ctor
            public ComboBoxItem(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            // override ToString() function
            public override string ToString()
            {
                // use name
                return this.name;
            }
        }

        internal class BitmapBorder
        {
            public int bottomY = 0;
            public int rightX = 0;
            public int topY = int.MaxValue;
            public int leftX = int.MaxValue;
        }

        internal class CharacterGenerationInfo
        {
            // Constructors
            public CharacterGenerationInfo(FontInfo fontInfo, char character)
            {
                this.fontInfo = fontInfo;
                this.Character = character;
                this.BitmapOriginal = null;
                this.BitmapToGenerate = null;
                this.BitmapArray = new List<ushort>();
                this.Width = 0;
                this.Height = 0;
                this.BitmapOffset = 0;
                this.BitmapLength = 0;
            }

            // properties
            internal FontInfo fontInfo;
            public char Character;
            public Bitmap BitmapOriginal;
            public Bitmap BitmapToGenerate;
            public List<ushort> BitmapArray;
            public ushort Width;
            public ushort Height;
            public ushort BitmapOffset;
            public ushort BitmapLength;
        }

        #endregion


        #region MISC METHODS
        // update input font
        private void UpdateSelectedFont()
        {
            // set text name in the text box
            this.inputFont.Text = this.fontDlgInputFont.Font.Name;

            // add to text
            this.inputFont.Text += " " + Math.Round(this.fontDlgInputFont.Font.Size) + "pts";

            // check if bold
            if (this.fontDlgInputFont.Font.Bold)
            {
                // add to text
                this.inputFont.Text += " / Bold";
            }

            // check if italic
            if (this.fontDlgInputFont.Font.Italic)
            {
                // add to text
                this.inputFont.Text += " / Italic";
            }

            // set the font in the text box
            this.txtInputText.Font = this.fontDlgInputFont.Font;

            // save into settings
            Properties.Settings.Default.InputFont = this.fontDlgInputFont.Font;
            Properties.Settings.Default.Save();
        }


        // populate preformatted text
        private void PopulateTextInsertCheckbox()
        {
            string all = "", numbers = "", letters = "", uppercaseLetters = "", lowercaseLetters = "", symbols = "";

            // generate characters
            for (char character = ' '; character < 127; ++character)
            {
                // add to all
                all += character;

                // classify letter
                if (Char.IsNumber(character)) numbers += character;
                else if (Char.IsSymbol(character)) symbols += character;
                else if (Char.IsLetter(character) && Char.IsLower(character)) { letters += character; lowercaseLetters += character; }
                else if (Char.IsLetter(character) && !Char.IsLower(character)) { letters += character; uppercaseLetters += character; }
            }

            // add items
            this.textInsert.Items.Add(new ComboBoxItem("All", all));
            this.textInsert.Items.Add(new ComboBoxItem("Numbers (0-9)", numbers));
            this.textInsert.Items.Add(new ComboBoxItem("Letters (A-z)", letters));
            this.textInsert.Items.Add(new ComboBoxItem("Lowercase letters (a-z)", lowercaseLetters));
            this.textInsert.Items.Add(new ComboBoxItem("Uppercase letters (A-Z)", uppercaseLetters));

            // use first
            this.textInsert.SelectedIndex = 0;
        }

        private void GenerateFontFileOutput(Font font, StringBuilder stringBuilder)
        {
            // do nothing if no chars defined
            if (this.txtInputText.Text.Length == 0)
            {
                stringBuilder.Append("No characters selected!");
                return;
            }

            // add comment block
            stringBuilder.AppendFormat("//\r\n");
            stringBuilder.AppendFormat("// Copyright 2011, JASDev International\r\n");
            stringBuilder.AppendFormat("// This file was generated automatically by FEZ Touch Font Generator.\r\n");
            stringBuilder.AppendFormat("// Go to http://www.jasdev.com for more information.\r\n");
            stringBuilder.AppendFormat("//\r\n\r\n");

            // add "using" block
            stringBuilder.Append("using System;\r\n");
            stringBuilder.Append("using Microsoft.SPOT;\r\n");
            stringBuilder.Append("using Microsoft.SPOT.Hardware;\r\n");
            stringBuilder.Append("using GHIElectronics.NETMF.FEZ;\r\n");
            stringBuilder.Append("\r\n");

            // add "namespace" start block
            stringBuilder.Append("namespace FEZTouch.Fonts\r\n");
            stringBuilder.Append("{\r\n");

            // add "font class"
            this.GenerateFontClassOutput(font, stringBuilder);

            // add "namespace" end block
            stringBuilder.Append("}\r\n");
        }

        #endregion


        #region FONT OUTPUT METHODS

        private void GenerateFontClassOutput(Font font, StringBuilder stringBuilder)
        {
            // create font info and char bitmaps
            FontInfo fontInfo = this.CreateFontInfo(font);

            // check for max character size
    //        if (fontInfo.MaxCharWidth > 255 || fontInfo.charHeight > 255)
    //        {
    //            throw new ArgumentException("The font selected is too large to fit into the maximum character bitmap size of 255 x 255 pixels.");
    //        }

            // save filename
            fileName = string.Format("Font{0}", fontInfo.FontName);

            // add "font class" start block
            stringBuilder.AppendFormat("\tpublic class Font{0} : FEZ_Components.FEZTouch.Font\r\n", fontInfo.FontName);
            stringBuilder.Append("\t{\r\n");

            // add "constructor" block
            stringBuilder.AppendFormat("\t\t// CONSTRUCTOR\r\n");
            stringBuilder.AppendFormat("\t\tpublic Font{0}()\r\n", fontInfo.FontName);
            stringBuilder.Append("\t\t{\r\n");

            stringBuilder.AppendFormat("\t\t\t// set properties\r\n");
            stringBuilder.AppendFormat("\t\t\tthis.avgWidth = {0};\r\n", fontInfo.AvgCharWidth);
            stringBuilder.AppendFormat("\t\t\tthis.maxWidth = {0};\r\n", fontInfo.MaxCharWidth);
            stringBuilder.AppendFormat("\t\t\tthis.height = {0};\r\n", fontInfo.CharHeight);
            stringBuilder.AppendFormat("\t\t\tthis.startChar = '{0}';\r\n", fontInfo.StartChar);
            stringBuilder.AppendFormat("\t\t\tthis.endChar = '{0}';\r\n", fontInfo.EndChar);

            stringBuilder.Append("\r\n");
            stringBuilder.AppendFormat("\t\t\t// attach string pointers to const strings\r\n");
            stringBuilder.AppendFormat("\t\t\tthis.charDescriptors = const{0}Descriptors;\r\n", fontInfo.FontName);
            stringBuilder.AppendFormat("\t\t\tthis.charBitmaps = const{0}Bitmaps;\r\n", fontInfo.FontName);
            stringBuilder.Append("\t\t}\r\n");

            // add "char descriptor array" block
            stringBuilder.Append("\r\n");
            stringBuilder.AppendFormat("\t\t// character descriptor array\r\n");
            this.GenerateCharDescriptorOutput(fontInfo, stringBuilder);

            // add "char bitmap array" block
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\t\t// character bitmap array\r\n");
            this.GenerateCharBitmapOutput(fontInfo, stringBuilder);

            // add "font class" end block
            stringBuilder.Append("\t}\r\n");
        }

        private void GenerateCharDescriptorOutput(FontInfo fontInfo, StringBuilder stringBuilder)
        {
            // output character descriptor array
            stringBuilder.AppendFormat("\t\tpublic const string const{0}Descriptors = \r\n", fontInfo.FontName);

            // start this character bitmap
            stringBuilder.AppendFormat("\t\t\t//  [width][height][offset][length]   [character]\r\n");

            // output character descriptors
            ushort charMask = 0x8000;
            ushort charWidth = 0x0000;
            ushort charHeight = 0x0000;
            ushort bitmapOffset = 0x0000;
            ushort bitmapLength = 0x0000;
            char charValue = ' ';
            string lineTerminator = "";
            for (int charIndex = 0; charIndex < fontInfo.Characters.Length; charIndex++)
            {
                // skip empty bitmaps
                if (fontInfo.Characters[charIndex].BitmapToGenerate == null) continue;

                // get character info, always set msb to 1
                charWidth = (ushort)(fontInfo.Characters[charIndex].Width | charMask);
                charHeight = (ushort)(fontInfo.Characters[charIndex].Height | charMask);
                bitmapOffset = (ushort)(fontInfo.Characters[charIndex].BitmapOffset | charMask);
                bitmapLength = (ushort)(fontInfo.Characters[charIndex].BitmapLength | charMask);
                charValue = fontInfo.Characters[charIndex].Character;

                // terminate this character descriptor
                if (fontInfo.Characters[charIndex].Character == fontInfo.EndChar)
                {
                    lineTerminator = "; ";
                }
                else
                {
                    lineTerminator = " +";
                }

                // output this character descriptor
                stringBuilder.AppendFormat("\t\t\t    \"\\u{0:X4}\\u{1:X4}\\u{2:X4}\\u{3:X4}\"{4}    // [{5}]\r\n",
                                            charWidth,
                                            charHeight,
                                            bitmapOffset,
                                            bitmapLength, 
                                            lineTerminator,
                                            charValue);
            }
        }

        private void GenerateCharBitmapOutput(FontInfo fontInfo, StringBuilder stringBuilder)
        {
            // output character bitmap array
            stringBuilder.AppendFormat("\t\tpublic const string const{0}Bitmaps = \r\n", fontInfo.FontName);

            // output character descriptors
            for (int charIndex = 0; charIndex < fontInfo.Characters.Length; charIndex++)
            {
                // skip empty bitmaps
                if (fontInfo.Characters[charIndex].BitmapToGenerate == null) continue;

                // output this character bitmap
                for (int bitmapIndex = 0; bitmapIndex < fontInfo.Characters[charIndex].BitmapArray.Count; bitmapIndex++)
                {
                    if (bitmapIndex == 0)
                    {
                        stringBuilder.AppendFormat("\t\t\t\"\\u{0:X4}", fontInfo.Characters[charIndex].BitmapArray[bitmapIndex]);
                    }
                    else
                    {
                        stringBuilder.AppendFormat("\\u{0:X4}", fontInfo.Characters[charIndex].BitmapArray[bitmapIndex]);
                    }
                }

                // terminate this character bitmap
                if (fontInfo.Characters[charIndex].Character == fontInfo.EndChar)
                {
                    stringBuilder.AppendFormat("\";\r\n");
                }
                else
                {
                    stringBuilder.AppendFormat("\" +\r\n");
                }
            }
        }

        private void ApplySyntaxColoringToOutput(StringBuilder stringBuilder, RichTextBox outputTextBox)
        {
            // clear the current text
            outputTextBox.Text = "";

            // split output string
            Regex r = new Regex("\\n");
            String[] lines = r.Split(stringBuilder.ToString());

            // for now don't syntax color for more than 2000 lines
            if (lines.Length > 1500)
            {
                // just set text
                outputTextBox.Text = stringBuilder.ToString();
                return;
            }

            // iterate over the richtext box and color it
            foreach (string line in lines)
            {
                r = new Regex("([ \\t{}();.])");
                String[] tokens = r.Split(line);

                // for each found token
                foreach (string token in tokens)
                {
                    // set the token's default color and font
                    outputTextBox.SelectionColor = Color.Black;
                    outputTextBox.SelectionFont = new Font("Courier New", 10, FontStyle.Regular);

                    // check for a comment.
                    if (token == "//" || token.StartsWith("//"))
                    {
                        // find the start of the comment and then extract the whole comment
                        int index = line.IndexOf("//");
                        string comment = line.Substring(index, line.Length - index);
                        outputTextBox.SelectionColor = Color.Green;
                        outputTextBox.SelectedText = comment;
                        break;
                    }

                    // check for a comment. TODO: terminate coloring
                    if (token == "/*" || token.StartsWith("/*"))
                    {
                        // find the start of the comment and then extract the whole comment
                        int index = line.IndexOf("/*");
                        string comment = line.Substring(index, line.Length - index);
                        outputTextBox.SelectionColor = Color.Green;
                        outputTextBox.SelectedText = comment;
                        break;
                    }

                    // check for a comment. TODO: terminate coloring
                    if (token == "**" || token.StartsWith("**"))
                    {
                        // find the start of the comment and then extract the whole comment
                        int index = line.IndexOf("**");
                        string comment = line.Substring(index, line.Length - index);
                        outputTextBox.SelectionColor = Color.Green;
                        outputTextBox.SelectedText = comment;
                        break;
                    }

                    // check for a comment. TODO: terminate coloring
                    if (token == "*/" || token.StartsWith("*/"))
                    {
                        // find the start of the comment and then extract the whole comment
                        int index = line.IndexOf("*/");
                        string comment = line.Substring(index, line.Length - index);
                        outputTextBox.SelectionColor = Color.Green;
                        outputTextBox.SelectedText = comment;
                        break;
                    }

                    // check whether the token is a keyword. 
                    String[] keywords = { "byte", "char", "ushort", "short", "int", "long", "const", "using", "namespace", "class", "public", "this", "string" };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // apply alternative color and font to highlight keyword
                            outputTextBox.SelectionColor = Color.Blue;
                            break;
                        }
                    }

                    // set the token text
                    outputTextBox.SelectedText = token;
                }
            }
        }

        #endregion


        #region FONT CREATION METHODS

        private FontInfo CreateFontInfo(Font font)
        {
            // the font information
            FontInfo fontInfo = new FontInfo();

            // set name and size
            fontInfo.FontName = font.Name.Replace(" ", "") + Math.Round(font.Size);
            fontInfo.FontSize = (int)Math.Round(font.Size);

            // get the characters we need to generate from the input text, removing duplicates
            fontInfo.GeneratedChars = this.GetCharactersToGenerate();

            // save reference to the font
            fontInfo.WindowsFont = font;

            // create array to hold all bitmaps and info per character
            fontInfo.Characters = new CharacterGenerationInfo[fontInfo.GeneratedChars.Length];

            //
            // init character info
            //
            for (int charIndex = 0; charIndex < fontInfo.GeneratedChars.Length; charIndex++)
            {
                // create char info entity
                fontInfo.Characters[charIndex] = new CharacterGenerationInfo(fontInfo, fontInfo.GeneratedChars[charIndex]);
            }

            //
            // find the largest bitmap size we are going to draw
            //
            Rectangle largestBitmapSize = this.GetSizeOfLargestBitmap(fontInfo.Characters, this.outputConfig.interCharacterPixels);

            // update font height
            fontInfo.CharHeight = (ushort)largestBitmapSize.Height;

            //
            // create bitmaps per character
            //
            // iterate over characters
            for (int charIndex = 0; charIndex < fontInfo.GeneratedChars.Length; charIndex++)
            {
                // generate the original bitmap for the character
                this.ConvertCharacterToBitmap(fontInfo.GeneratedChars[charIndex], font, out fontInfo.Characters[charIndex].BitmapOriginal, largestBitmapSize);

                // save
                // fontInfo.characters[charIndex].bitmapOriginal.Save(String.Format("C:/bms/{0}.bmp", fontInfo.characters[charIndex].character));
            }

            //
            // iterate through all bitmaps and find the tightest common border
            //
            // this will contain the values of the tightest border around the characters
            BitmapBorder tightestCommonBorder = new BitmapBorder();
            this.FindTightestCommonBitmapBorder(fontInfo.Characters, tightestCommonBorder);

            //
            // iterate thruogh all bitmaps and generate the bitmap we will use as output
            // this means performing all manipulation (pad remove, flip)
            //
            // iterate over characters
            for (int charIndex = 0; charIndex < fontInfo.GeneratedChars.Length; charIndex++)
            {
                // generate output bitmap for the character
                this.ManipulateBitmap(fontInfo.Characters[charIndex].BitmapOriginal,
                                 tightestCommonBorder,
                                 out fontInfo.Characters[charIndex].BitmapToGenerate,
                                 (fontInfo.Characters[charIndex].Character == ' ' ? this.outputConfig.spaceCharacterWidth : this.outputConfig.minCharacterWidth),
                                 this.outputConfig.minCharacterHeight,
                                 this.outputConfig.interCharacterPixels,
                                 this.GetOutputRotateFlipType());
            }

            //
            // iterate through all characters and create the char array
            //
            // iterate over characters
            for (int charIndex = 0; charIndex < fontInfo.GeneratedChars.Length; charIndex++)
            {
                // check if bitmap exists
                if (fontInfo.Characters[charIndex].BitmapToGenerate != null)
                {
                    // create a ushort array from the character's bitmap
                    this.ConvertBitmapToUshortArray(fontInfo.Characters[charIndex].BitmapToGenerate, fontInfo.Characters[charIndex].BitmapArray);

                    // trim blank pixels from bottom of bitmap
                    int bitmapLength = fontInfo.Characters[charIndex].BitmapArray.Count;
                    while (bitmapLength > 1 && fontInfo.Characters[charIndex].BitmapArray[bitmapLength - 1] == 0x8000)
                    {
                        fontInfo.Characters[charIndex].BitmapArray.RemoveAt(bitmapLength - 1);
                        bitmapLength = fontInfo.Characters[charIndex].BitmapArray.Count;
                    }

                    // save the character's bitmap length
                    fontInfo.Characters[charIndex].BitmapLength = (ushort)fontInfo.Characters[charIndex].BitmapArray.Count;
                }
            }

            // update font info
            this.UpdateFontInfoFromCharacters(fontInfo);

            // return the font info
            return fontInfo;
        }

        // get the characters we need to generate
        private string GetCharactersToGenerate()
        {
            //
            // iterate through the inputted text and shove to sorted string, removing all duplicates
            //
            // sorted list for insertion/duplication removal
            SortedList<char, char> characterList = new SortedList<char, char>();

            // iterate over the characters in the textbox
            for (int charIndex = 0; charIndex < this.txtInputText.Text.Length; ++charIndex)
            {
                // get the char
                char insertionCandidateChar = this.txtInputText.Text[charIndex];

                // insert the char, if not already in the list and if not space ()
                if (characterList.ContainsKey(insertionCandidateChar) == false)
                {
                    // not in list, add
                    characterList.Add(this.txtInputText.Text[charIndex], ' ');
                }
            }

            // now output the sorted list to a string
            string characterListString = "";

            // iterate over the sorted characters to create the string
            foreach (char characterKey in characterList.Keys)
            {
                // add to string
                characterListString += characterKey;
            }

            // return the character
            return characterListString;
        }

        // get size of largest bitmap
        Rectangle GetSizeOfLargestBitmap(CharacterGenerationInfo[] charInfoArray, int interCharSpacing)
        {
            // largest rect
            Rectangle largestRect = new Rectangle(0, 0, 0, 0);

            // iterate through chars
            for (int charIdx = 0; charIdx < charInfoArray.Length; charIdx++)
            {
                // get the string of the characer
                string letterString = charInfoArray[charIdx].Character.ToString();

                // set proposed size with dimensions set to the maximum integer value.
                Size proposedSize = new Size(int.MaxValue, int.MaxValue);

                // measure the size of the character in pixels
                Size stringSize = TextRenderer.MeasureText(letterString, charInfoArray[charIdx].fontInfo.WindowsFont);

                // check if larger
                largestRect.Height = Math.Max(largestRect.Height, stringSize.Height);
                largestRect.Width = Math.Max(largestRect.Width, stringSize.Width);
            }

            // add inter-character spacing
            largestRect.Width += interCharSpacing;

            // add line spacing
        //    largestRect.Height += lineSpacing;

            // return largest
            return largestRect;
        }

        // convert a letter to bitmap
        private void ConvertCharacterToBitmap(char character, Font font, out Bitmap outputBitmap, Rectangle largestBitmap)
        {
            // get the string
            string letterString = character.ToString();

            // create bitmap, sized to the correct size
            outputBitmap = new Bitmap((int)largestBitmap.Width, (int)largestBitmap.Height);

            // create grahpics entity for drawing
            Graphics gfx = Graphics.FromImage(outputBitmap);

            // disable anti alias
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            // draw centered text
            Rectangle bitmapRect = new System.Drawing.Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height);

            // set format of string
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Near;

            // draw the character
            gfx.FillRectangle(Brushes.White, bitmapRect);
            gfx.DrawString(letterString, font, Brushes.Black, bitmapRect, drawFormat);
        }

        // iterate through the original bitmaps and find the tightest common border
        private void FindTightestCommonBitmapBorder(CharacterGenerationInfo[] charInfoArray, BitmapBorder tightestBorder)
        {
            // iterate through bitmaps
            for (int charIdx = 0; charIdx < charInfoArray.Length; ++charIdx)
            {
                // create a border
                BitmapBorder bitmapBorder = new BitmapBorder();

                // get the bitmaps border
                this.GetBitmapBorder(charInfoArray[charIdx].BitmapOriginal, bitmapBorder, 0);

                // check if we need to loosen up the tightest border
                tightestBorder.leftX = Math.Min(bitmapBorder.leftX, tightestBorder.leftX);
                tightestBorder.topY = Math.Min(bitmapBorder.topY, tightestBorder.topY);
                tightestBorder.rightX = Math.Max(bitmapBorder.rightX, tightestBorder.rightX);
                tightestBorder.bottomY = Math.Max(bitmapBorder.bottomY, tightestBorder.bottomY);
            }
        }

        // modify the original bitmap (adjust padding, flip, rotate, etc)
        private bool ManipulateBitmap(Bitmap bitmapOriginal, BitmapBorder tightestCommonBorder, out Bitmap bitmapManipulated,
                                        int minWidth, int minHeight, int interCharSpacing, 
                                        RotateFlipType flipType)
        {
            //
            // First, crop
            //
            // get bitmap border - this sets the crop rectangle to per bitmap, essentially
            BitmapBorder bitmapCropBorder = new BitmapBorder();
            if (this.GetBitmapBorder(bitmapOriginal, bitmapCropBorder, interCharSpacing) == false && minWidth == 0 && minHeight == 0)
            {
                // no data
                bitmapManipulated = null;

                // bitmap contains no data
                return false;
            }

            // check that width exceeds minimum
            if (bitmapCropBorder.rightX - bitmapCropBorder.leftX + 1 < minWidth)
            {
                // replace
                bitmapCropBorder.leftX = 0;
                bitmapCropBorder.rightX = minWidth - 1;
            }

            // check that height exceeds minimum
            if (bitmapCropBorder.bottomY - bitmapCropBorder.topY + 1 < minHeight)
            {
                // replace
                bitmapCropBorder.topY = 0;
                bitmapCropBorder.bottomY = minHeight - 1;
            }

            // crop horizontal bitmap
            switch (this.outputConfig.paddingRemovalHorizontal)
            {
                case OutputConfiguration.PaddingRemoval.None:
                    // set X to original size minus one pixel
                    bitmapCropBorder.leftX = 0;
                    bitmapCropBorder.rightX = bitmapOriginal.Width - 1;
                    break;
                case OutputConfiguration.PaddingRemoval.Fixed:
                    // crop X to size of common border
                    bitmapCropBorder.leftX = tightestCommonBorder.leftX;
                    bitmapCropBorder.rightX = tightestCommonBorder.rightX;
                    break;
                case OutputConfiguration.PaddingRemoval.Tightest:
                    break;
            }

            // crop vertical bitmap
            switch (this.outputConfig.paddingRemovalVertical)
            {
                case OutputConfiguration.PaddingRemoval.None:
                    // set Y to original size minus one pixel
                    bitmapCropBorder.topY = 0;
                    bitmapCropBorder.bottomY = bitmapOriginal.Height - 1;
                    break;
                case OutputConfiguration.PaddingRemoval.Fixed:
                    // crop Y to size of common border
                    bitmapCropBorder.topY = tightestCommonBorder.topY;
                    bitmapCropBorder.bottomY = tightestCommonBorder.bottomY;
                    break;
                case OutputConfiguration.PaddingRemoval.Tightest:
                    // crop Y to size of common border
                    bitmapCropBorder.topY = tightestCommonBorder.topY;
                    bitmapCropBorder.bottomY = tightestCommonBorder.bottomY;
                    break;
            }

            // now create the crop rectangle
            Rectangle cropRect = new Rectangle(bitmapCropBorder.leftX,
                                            bitmapCropBorder.topY,
                                            bitmapCropBorder.rightX - bitmapCropBorder.leftX + 1,
                                            bitmapCropBorder.bottomY - bitmapCropBorder.topY + 1);

            // clone the cropped bitmap into the generated one
            bitmapManipulated = bitmapOriginal.Clone(cropRect, bitmapOriginal.PixelFormat);

            // flip the cropped bitmap
            bitmapManipulated.RotateFlip(flipType);

            // bitmap contains data
            return true;
        }

        // create the ushort array
        private void ConvertBitmapToUshortArray(Bitmap bitmapToGenerate, List<ushort> ushortArray)
        {
            // create variables
            ushort ushortValue = 0x8000;
            ushort bitMask = 0x0001;
            ushort bitsRead = 0;

            // for each row
            for (int row = 0; row < bitmapToGenerate.Height; row++)
            {
                // for each column
                for (int column = 0; column < bitmapToGenerate.Width; ++column)
                {
                    // is pixel set?
                    if (bitmapToGenerate.GetPixel(column, row).ToArgb() == Color.Black.ToArgb())
                    {
                        // set the appropriate bit in the char
                        ushortValue = (ushort)(ushortValue | bitMask);
                    }

                    // adjust bitMask and increment bitsRead
                    bitMask = (ushort)(bitMask << 1);
                    bitsRead++;

                    // have we filled a char?
                    if (bitsRead == 15)
                    {
                        // add charValue to char array
                        ushortArray.Add((char)ushortValue);

                        // reset charValue, bitMask, and bitsRead
                        ushortValue = 0x8000;
                        bitMask = 0x0001;
                        bitsRead = 0;
                    }
                }
            }

            // if we have bits left, add it as is
            if (bitsRead != 0)
            {
                // add charValue to char array
                ushortArray.Add((char)ushortValue);
            }
        }

        // update font info from string
        private void UpdateFontInfoFromCharacters(FontInfo fontInfo)
        {
            // do nothing if no chars defined
            if (fontInfo.Characters.Length == 0) return;

            // total offset
            ushort bitmapOffset = 0;

            // set start char
            fontInfo.StartChar = (char)0xFFFF;
            fontInfo.EndChar = ' ';

            // iterate through letter string
            fontInfo.AvgCharWidth = 0;
            fontInfo.MaxCharWidth = 0;
            for (int charIdx = 0; charIdx < fontInfo.Characters.Length; ++charIdx)
            {
                // skip empty bitmaps
                if (fontInfo.Characters[charIdx].BitmapToGenerate == null) continue;

                // get char
                char currentChar = fontInfo.Characters[charIdx].Character;

                // is this character smaller than start char?
                if (currentChar < fontInfo.StartChar)
                {
                    fontInfo.StartChar = currentChar;
                }

                // is this character bigger than end char?
                if (currentChar > fontInfo.EndChar)
                {
                    fontInfo.EndChar = currentChar;
                }

                // populate number of rows
                this.GetAbsoluteCharacterDimensions(fontInfo.Characters[charIdx].BitmapToGenerate, ref fontInfo.Characters[charIdx].Width, ref fontInfo.Characters[charIdx].Height);

                // adjust avg char width
                fontInfo.AvgCharWidth += fontInfo.Characters[charIdx].Width;

                // adjust max char width
                if (fontInfo.Characters[charIdx].Width > fontInfo.MaxCharWidth)
                {
                    fontInfo.MaxCharWidth = fontInfo.Characters[charIdx].Width;
                }

                // set bitmap offset
                fontInfo.Characters[charIdx].BitmapOffset = bitmapOffset;

                // increment bitmap offset
                bitmapOffset += (ushort)fontInfo.Characters[charIdx].BitmapArray.Count;
            }

            // calculate avg char width
            fontInfo.AvgCharWidth = (ushort)(fontInfo.AvgCharWidth / (fontInfo.EndChar - fontInfo.StartChar + 1));
        }

        // get the bitmaps border - that is where the black parts start
        private bool GetBitmapBorder(Bitmap bitmap, BitmapBorder border, int interCharSpacing)
        {
            // search for first column (x) from the left to contain data
            for (border.leftX = 0; border.leftX < bitmap.Width; border.leftX++)
            {
                // if set pixels found, stop looking
                if (this.BitmapColumnIsEmpty(bitmap, border.leftX) == false)
                {
                    break;
                }
            }

            // search for first column (x) from the right to contain data
            for (border.rightX = bitmap.Width - 1; border.rightX >= 0; border.rightX--)
            {
                // if set pixels found, stop looking
                if (this.BitmapColumnIsEmpty(bitmap, border.rightX) == false)
                {
                    break;
                }
            }

            // set top border at 0
            border.topY = 0;

            // search for first row (y) from the bottom to contain data
            for (border.bottomY = bitmap.Height - 1; border.bottomY >= 0; border.bottomY--)
            {
                // if set pixels found, stop looking
                if (this.BitmapRowIsEmpty(bitmap, border.bottomY) == false)
                {
                    break;
                }
            }

            // check if the bitmap contains any black pixels
            if (border.rightX == -1)
            {
                // no pixels were found
                return false;
            }

            // add inter-character spacing
            border.rightX += interCharSpacing;
            return true;
        }

        // get rotate flip type according to config
        private RotateFlipType GetOutputRotateFlipType()
        {
            bool fx = this.outputConfig.flipHorizontal;
            bool fy = this.outputConfig.flipVertical;
            OutputConfiguration.Rotation rot = this.outputConfig.rotation;

            // zero degree rotation
            if (rot == OutputConfiguration.Rotation.RotateZero)
            {
                // return according to flip
                if (!fx && !fy) return RotateFlipType.RotateNoneFlipNone;
                if (fx && !fy) return RotateFlipType.RotateNoneFlipX;
                if (!fx && fy) return RotateFlipType.RotateNoneFlipY;
                if (fx && fy) return RotateFlipType.RotateNoneFlipXY;
            }

            // 90 degree rotation
            if (rot == OutputConfiguration.Rotation.RotateNinety)
            {
                // return according to flip
                if (!fx && !fy) return RotateFlipType.Rotate90FlipNone;
                if (fx && !fy) return RotateFlipType.Rotate90FlipX;
                if (!fx && fy) return RotateFlipType.Rotate90FlipY;
                if (fx && fy) return RotateFlipType.Rotate90FlipXY;
            }

            // 180 degree rotation
            if (rot == OutputConfiguration.Rotation.RotateOneEighty)
            {
                // return according to flip
                if (!fx && !fy) return RotateFlipType.Rotate180FlipNone;
                if (fx && !fy) return RotateFlipType.Rotate180FlipX;
                if (!fx && fy) return RotateFlipType.Rotate180FlipY;
                if (fx && fy) return RotateFlipType.Rotate180FlipXY;
            }

            // 270 degree rotation
            if (rot == OutputConfiguration.Rotation.RotateTwoSeventy)
            {
                // return according to flip
                if (!fx && !fy) return RotateFlipType.Rotate270FlipNone;
                if (fx && !fy) return RotateFlipType.Rotate270FlipX;
                if (!fx && fy) return RotateFlipType.Rotate270FlipY;
                if (fx && fy) return RotateFlipType.Rotate270FlipXY;
            }

            // unknown case, but just return no flip
            return RotateFlipType.RotateNoneFlipNone;
        }

        // get absolute height/width of characters
        private void GetAbsoluteCharacterDimensions(Bitmap charBitmap, ref ushort width, ref ushort height)
        {
            // check if bitmap exists, otherwise set as zero
            if (charBitmap == null)
            {
                // zero height
                width = 0;
                height = 0;
            }
            else
            {
                // get the absolute font character height. Depends on rotation
                if (this.outputConfig.rotation == OutputConfiguration.Rotation.RotateZero ||
                    this.outputConfig.rotation == OutputConfiguration.Rotation.RotateOneEighty)
                {
                    // if char is not rotated or rotated 180deg, its height is the actual height
                    height = (ushort)charBitmap.Height;
                    width = (ushort)charBitmap.Width;
                }
                else
                {
                    // if char is rotated by 90 or 270, its height is the width of the rotated bitmap
                    height = (ushort)charBitmap.Width;
                    width = (ushort)charBitmap.Height;
                }
            }
        }

        // returns whether a bitmap column is empty (empty means all is back color)
        private bool BitmapColumnIsEmpty(Bitmap bitmap, int column)
        {
            // for each row in the column
            for (int row = 0; row < bitmap.Height; ++row)
            {
                // is the pixel black?
                if (bitmap.GetPixel(column, row).ToArgb() == Color.Black.ToArgb())
                {
                    // found. column is not empty
                    return false;
                }
            }

            // column is empty
            return true;
        }

        // returns whether a bitmap row is empty (empty means all is back color)
        private bool BitmapRowIsEmpty(Bitmap bitmap, int row)
        {
            // for each column in the row
            for (int column = 0; column < bitmap.Width; ++column)
            {
                // is the pixel black?
                if (bitmap.GetPixel(column, row).ToArgb() == Color.Black.ToArgb())
                {
                    // found. column is not empty
                    return false;
                }
            }

            // column is empty
            return true;
        }

        #endregion


        #region EVENT HANDLERS

        private void Form1_Load(object sender, EventArgs e)
        {
            // set version
            this.Text = String.Format("FEZ Touch Font Generator ver {0}", ApplicationVersion);

            // set input box
            this.txtInputText.Text = Properties.Settings.Default.InputText;

            // load font
            this.fontDlgInputFont.Font = Properties.Settings.Default.InputFont;
            this.UpdateSelectedFont();

            // load configurations from file
            this.outputConfigurationManager.loadFromFile(Properties.Resources.ConfigFileName);

            // update the dropdown
            this.outputConfigurationManager.comboboxPopulate(this.outputConfiguration);

            // get saved output config index
            int lastUsedOutputConfigurationIndex = Properties.Settings.Default.OutputConfigIndex;

            // load recently used preset
            if (lastUsedOutputConfigurationIndex >= 0 &&
                lastUsedOutputConfigurationIndex < this.outputConfiguration.Items.Count)
            {
                // last used
                this.outputConfiguration.SelectedIndex = lastUsedOutputConfigurationIndex;

                // load selected configuration
                this.outputConfig = this.outputConfigurationManager.configurationGetAtIndex(lastUsedOutputConfigurationIndex);
            }
            else
            {
                // there's no saved configuration. get the working copy (default)
                this.outputConfig = this.outputConfigurationManager.workingOutputConfiguration;
            }

            // set checkbox stuff
            this.PopulateTextInsertCheckbox();

            // apply font to all appropriate places
            this.UpdateSelectedFont();
        }

        private void copySourceMenuItem_Click(object sender, EventArgs e)
        {
            // copy if any text
            if (this.outputTextDisplay.Text != "")
            {
                // copy
                Clipboard.SetText(outputTextDisplay.Text);
            }
        }

        private void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            // zero out file name
            this.dlgSaveAs.FileName = fileName;

            // try to prompt
            if (this.dlgSaveAs.ShowDialog() != DialogResult.Cancel)
            {
                // save the text
                this.outputTextDisplay.SaveFile(this.dlgSaveAs.FileName, RichTextBoxStreamType.PlainText);
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            // close self
            this.Close();
        }

        private void aboutMenuItem1_Click(object sender, EventArgs e)
        {
            // about
            AboutForm about = new AboutForm();
            about.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // show the about form
            about.Show();
        }

        private void btnFontSelect_Click(object sender, EventArgs e)
        {
            // set focus somewhere else
            this.label1.Focus();

            // open font chooser dialog
            if (this.fontDlgInputFont.ShowDialog() != DialogResult.Cancel)
            {
                this.UpdateSelectedFont();
            }
        }

        private void btnInsertText_Click(object sender, EventArgs e)
        {
            // no focus
            this.label1.Focus();

            // insert text
            ComboBoxItem selectedText = ((ComboBoxItem)this.textInsert.SelectedItem);
            this.txtInputText.Text += selectedText.value;
        }

        private void btnOutputConfig_Click(object sender, EventArgs e)
        {
            // no focus
            this.label1.Focus();

            // get it
            OutputConfigurationForm outputConfigForm = new OutputConfigurationForm(ref this.outputConfigurationManager);

            // get the oc
            int selectedConfigurationIndex = outputConfigForm.getOutputConfiguration(this.outputConfiguration.SelectedIndex);

            // update the dropdown
            this.outputConfigurationManager.comboboxPopulate(this.outputConfiguration);

            // get working configuration
            this.outputConfig = this.outputConfigurationManager.workingOutputConfiguration;

            // set selected index
            this.outputConfiguration.SelectedIndex = selectedConfigurationIndex;
        }

        private void outputConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if any configuration selected
            if (this.outputConfiguration.SelectedIndex != -1)
            {
                // get the configuration
                this.outputConfig = this.outputConfigurationManager.configurationGetAtIndex(this.outputConfiguration.SelectedIndex);
            }

            // save selected index for next time
            Properties.Settings.Default.OutputConfigIndex = this.outputConfiguration.SelectedIndex;

            // save
            Properties.Settings.Default.Save();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // set focus somewhere else
            this.label1.Focus();

            // save default input text
            Properties.Settings.Default.InputText = this.txtInputText.Text;
            Properties.Settings.Default.Save();

            // generate output text
            StringBuilder stringBuilder = new StringBuilder();
            this.GenerateFontFileOutput(this.fontDlgInputFont.Font, stringBuilder);

            // color code the strings and output to the rich textbox
            this.ApplySyntaxColoringToOutput(stringBuilder, this.outputTextDisplay);
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            // no focus
            this.label1.Focus();
        }

        #endregion


        #region MEMBER FIELDS

        public OutputConfigurationManager outputConfigurationManager = new OutputConfigurationManager();
        private OutputConfiguration outputConfig;

        #endregion
    }
}

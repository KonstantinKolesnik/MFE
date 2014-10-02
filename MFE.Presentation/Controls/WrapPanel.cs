using System;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;

namespace MFE.Presentation.Controls
{
    /// <summary>
    /// WrapPanel is used to place child UIElements at sequential positions from left to the right 
    /// and then "wrap" the lines of children from top to the bottom.
    ///
    /// All children get the layout partition of size ItemWidth x ItemHeight.
    /// </summary>
    public class WrapPanel : Panel
    {
        private struct UVSize
        {
            internal int U;
            internal int V;
            private Orientation _orientation;

            internal UVSize(Orientation orientation, int width, int height)
            {
                U = V = 0;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                U = V = 0;
                _orientation = orientation;
            }

            internal int Width
            {
                get { return (_orientation == Orientation.Horizontal ? U : V); }
                set { if (_orientation == Orientation.Horizontal) U = value; else V = value; }
            }
            internal int Height
            {
                get { return (_orientation == Orientation.Horizontal ? V : U); }
                set { if (_orientation == Orientation.Horizontal) V = value; else U = value; }
            }
        }

        #region Fields
        private int itemWidth = 0;
        private int itemHeight = 0;
        private Orientation orientation = Orientation.Horizontal;
        #endregion

        #region Properties
        /// <summary> 
        /// The ItemWidth and ItemHeight properties specify the size of all items in the WrapPanel.
        /// Note that children of WrapPanel may have their own Width/Height properties set - the ItemWidth/ItemHeight 
        /// specifies the size of "layout partition" reserved by WrapPanel for the child.
        /// If this property is not set (equal to 0) - the size of layout 
        /// partition is equal to DesiredSize of the child element.
        /// </summary>
        public int ItemWidth
        {
            get { return itemWidth; }
            set
            {
                if (itemWidth != value)
                {
                    if (!IsWidthHeightValid(value))
                        throw new ArgumentOutOfRangeException("ItemWidth");

                    itemWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary> 
        /// The ItemWidth and ItemHeight properties specify the size of all items in the WrapPanel.
        /// Note that children of WrapPanel may have their own Width/Height properties set - the ItemWidth/ItemHeight 
        /// specifies the size of "layout partition" reserved by WrapPanel for the child.
        /// If this property is not set (equal to 0) - the size of layout 
        /// partition is equal to DesiredSize of the child element.
        /// </summary>
        public int ItemHeight
        {
            get { return itemHeight; }
            set
            {
                if (itemHeight != value)
                {
                    if (!IsWidthHeightValid(value))
                        throw new ArgumentOutOfRangeException("ItemHeight");

                    itemHeight = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Specifies dimension of children positioning in absence of wrapping. 
        /// Wrapping occurs in orthogonal direction. For example, if Orientation is Horizontal,
        /// the items try to form horizontal rows first and if needed are wrapped and form vertical stack of rows. 
        /// If Orientation is Vertical, items first positioned in a vertical column, and if there is 
        /// not enough space - wrapping creates additional columns in horizontal dimension.
        /// </summary> 
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                if (value != orientation)
                {
                    orientation = value;
                    InvalidateMeasure();
                }
            }
        }
        #endregion

        public WrapPanel()
        {
        }
        public WrapPanel(Orientation orientation)
        {
            Orientation = orientation;
        }

        protected override void ArrangeOverride(int arrangeWidth, int arrangeHeight)
        {
            UVSize arrangeSize = new UVSize(orientation, arrangeWidth, arrangeHeight);
            UVSize currentLineSize = new UVSize(orientation);
            int accumulatedV = 0;

            bool itemWidthSet = itemWidth != 0;
            bool itemHeightSet = itemHeight != 0;
            bool useSetU = orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet;
            int itemSetU = orientation == Orientation.Horizontal ? itemWidth : itemHeight;

            int firstInLineIndex = 0;
            int count = Children.Count;
            for (int i = 0; i < count; i++)
            {
                UIElement child = Children[i];
                if (child == null)
                    continue;

                int desiredWidth, desiredHeight;
                child.GetDesiredSize(out desiredWidth, out desiredHeight);

                UVSize childSize = new UVSize(orientation,
                    itemWidthSet ? itemWidth : desiredWidth,
                    itemHeightSet ? itemHeight : desiredHeight);

                // does not fit on line
                if (currentLineSize.U + childSize.U > arrangeSize.U)
                {
                    // arrange previous line
                    ArrangeLine(accumulatedV, currentLineSize.V, firstInLineIndex, i /* exclusive */, useSetU, itemSetU);
                    accumulatedV += currentLineSize.V;

                    // this child is on new line
                    currentLineSize = childSize;

                    // child is bigger than available size
                    if (childSize.U > arrangeSize.U)
                    {
                        ArrangeLine(accumulatedV, childSize.V, i, i + 1, useSetU, itemSetU);
                        i++; // order of parameters evaluation is not guaranted

                        // this is the only child on line
                        accumulatedV += childSize.V;
                        currentLineSize = new UVSize(orientation);
                    }

                    firstInLineIndex = i;
                }
                else
                {
                    currentLineSize.U += childSize.U;
                    currentLineSize.V = System.Math.Max(childSize.V, currentLineSize.V);
                }
            }

            if (firstInLineIndex < count)
                ArrangeLine(accumulatedV, currentLineSize.V, firstInLineIndex, count, useSetU, itemSetU);
        }
        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            UVSize desiredSize = new UVSize(orientation);
            UVSize availableSize = new UVSize(orientation, availableWidth, availableHeight);
            UVSize currentLineSize = new UVSize(orientation);

            bool itemWidthSet = itemWidth != 0;
            bool itemHeightSet = itemHeight != 0;

            int availableChildWidth = itemWidthSet ? itemWidth : availableWidth;
            int availableChildHeight = itemHeightSet ? itemHeight : availableHeight;

            int count = Children.Count;
            for (int i = 0; i < count; i++)
            {
                UIElement child = Children[i];
                if (child == null)
                    continue;

                child.Measure(availableChildWidth, availableChildHeight);

                int desiredChildWidth, desiredChildHeight;
                child.GetDesiredSize(out desiredChildWidth, out desiredChildHeight);

                UVSize childSize = new UVSize(orientation,
                    itemWidthSet ? itemWidth : desiredChildWidth,
                    itemHeightSet ? itemHeight : desiredChildHeight);

                if (currentLineSize.U + childSize.U > availableSize.U)
                {
                    desiredSize.U = System.Math.Max(currentLineSize.U, desiredSize.U);
                    desiredSize.V += currentLineSize.V;

                    currentLineSize = childSize;
                    if (childSize.U > availableSize.U)
                    {
                        desiredSize.U = System.Math.Max(childSize.U, desiredSize.U);
                        desiredSize.V = childSize.V;
                        currentLineSize = new UVSize(orientation);
                    }
                }
                else
                {
                    currentLineSize.U += childSize.U;
                    currentLineSize.V = System.Math.Max(childSize.V, currentLineSize.V);
                }
            }

            desiredWidth = System.Math.Max(currentLineSize.U, desiredSize.U);
            desiredHeight = desiredSize.V + currentLineSize.V;
        }
        private void ArrangeLine(int v, int lineV, int indexStart, int indexEnd, bool useSetU, int itemSetU)
        {
            int u = 0;
            bool isHorizontal = orientation == Orientation.Horizontal;

            for (int i = indexStart; i < indexEnd; i++)
            {
                UIElement child = Children[i];
                if (child == null)
                    continue;

                UVSize childSize = new UVSize(orientation);
                if (isHorizontal)
                    child.GetDesiredSize(out childSize.U, out childSize.V);
                else
                    child.GetDesiredSize(out childSize.V, out childSize.U);

                int layoutSlotU = useSetU ? itemSetU : childSize.U;

                if (isHorizontal)
                    child.Arrange(u, v, layoutSlotU, lineV);
                else
                    child.Arrange(v, u, lineV, layoutSlotU);

                u += layoutSlotU;
            }
        }

        private static bool IsWidthHeightValid(object value)
        {
            int v = (int)value;
            return v >= 0;
        }
    }
}

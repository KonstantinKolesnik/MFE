using Microsoft.SPOT;
using System;
using System.Collections;

namespace MFE.Graphics.Controls
{
    public class ControlCollection2
    {
        #region Fields
        private Control owner;
        private ArrayList children = new ArrayList();
        #endregion

        #region Properties
        //public ArrayList Children
        //{
        //    get { return children; }
        //}
        public int Count
        {
            get { return children.Count; }
        }
        #endregion

        #region Constructor
        public ControlCollection2(Control owner)
        {
            this.owner = owner;
        }
        #endregion

        #region Public Methods
        public virtual void Add(Control child)
        {
            child.Parent = owner;
            int idx = children.Add(child);
            owner.OnChildrenChanged(child, null, idx);
        }
        public virtual void Clear()
        {
            children.Clear();
        }
        public virtual Control GetAt(int index)
        {
            return (Control)children[index];
        }
        public virtual void Remove(Control child)
        {
            int idx = children.IndexOf(child);
            children.Remove(child);
            child.Parent = null;
            owner.OnChildrenChanged(null, child, idx);
        }
        public virtual void RemoveAt(int index)
        {
            Control child = GetAt(index);
            children.RemoveAt(index);
            child.Parent = null;
            owner.OnChildrenChanged(null, child, index);
        }
        public virtual bool Contains(Control child)
        {
            return children.IndexOf(child) != -1;
        }
        #endregion
    }

    public class ControlCollection : ICollection
    {
        #region Fields
        internal Control[] items;
        internal int size;
        private int version;
        private Control owner;

        private const int c_defaultCapacity = 2;
        private const int c_growFactor = 2;
        #endregion

        #region Properties
        public virtual int Count
        {
            get { return size; }
        }
        public virtual bool IsSynchronized
        {
            get { return false; }
        }
        public virtual object SyncRoot
        {
            get { return this; }
        }
        /// <summary>
        /// Gets or sets the number of elements that the ControlCollection can contain.
        /// </summary>
        /// <value>
        /// The number of elements that the ControlCollection can contain.
        /// </value>
        /// <remarks>
        /// Capacity is the number of elements that the UIElementCollection is capable of storing.
        /// Count is the number of UIElements that are actually in the UIElementCollection.
        ///
        /// Capacity is always greater than or equal to Count. If Count exceeds
        /// Capacity while adding elements, the capacity of the ControlCollection is increased.
        ///
        /// By default the capacity is 0.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Capacity is set to a value that is less than Count.</exception>
        public virtual int Capacity
        {
            get { return items != null ? items.Length : 0; }
            set
            {
                int currentCapacity = items != null ? items.Length : 0;
                if (value != currentCapacity)
                {
                    if (value < size)
                        throw new ArgumentOutOfRangeException("value", "not enough capacity");

                    if (value > 0)
                    {
                        Control[] newItems = new Control[value];
                        if (size > 0)
                        {
                            Debug.Assert(items != null);
                            Array.Copy(items, 0, newItems, 0, size);
                        }

                        items = newItems;
                    }
                    else
                    {
                        Debug.Assert(value == 0, "There shouldn't be a case where value != 0.");
                        Debug.Assert(size == 0, "Size must be 0 here.");
                        items = null;
                    }
                }
            }
        }
        #endregion

        #region Constructor
        public ControlCollection(Control owner)
        {
            Debug.Assert(owner != null);
            this.owner = owner;
        }
        #endregion

        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if ((index < 0) || (array.Length - index < size))
                throw new ArgumentOutOfRangeException("index");

            Array.Copy(items, 0, array, index, size);
        }

        /// <summary>
        /// Strongly typed version of CopyTo
        /// Copies the collection into the Array.
        /// </summary>
        public virtual void CopyTo(Control[] array, int index)
        {
            CopyTo((Array)array, index);
        }

        // ----------------------------------------------------------------
        // ArrayList like operations for the UIElementCollection
        // ----------------------------------------------------------------

        /// <summary>
        /// Ensures that the capacity of this list is at least the given minimum
        /// value. If the currect capacity of the list is less than min, the
        /// capacity is increased to min.
        /// </summary>
        private void EnsureCapacity(int min)
        {
            if (Capacity < min)
                Capacity = System.Math.Max(min, (int)(Capacity * c_growFactor));
        }


        /// <summary>
        /// Indexer for the UIElementCollection. Gets or sets the UIElement stored at the
        /// zero-based index of the UIElementCollection.
        /// </summary>
        /// <remarks>This property provides the ability to access a specific UIElement in the
        /// UIElementCollection by using the following systax: <c>myUIElementCollection[index]</c>.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><c>index</c> is less than zero -or- <c>index</c> is equal to or greater than Count.</exception>
        /// <exception cref="ArgumentException">If the new child has already a parent or if the slot a the specified index is not null.</exception>
        public Control this[int index]
        {
            get
            {
                if (index < 0 || index >= size)
                    throw new ArgumentOutOfRangeException("index");

                return items[index];
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (index < 0 || index >= size)
                    throw new ArgumentOutOfRangeException("index");

                Control child = items[index];

                if (child != value)
                {
                    if ((value == null) && (child != null))
                        DisconnectChild(index);
                    else if (value != null)
                    {
                        if (value.Parent != null) // are a root node of a visual target can be set into the collection.
                            throw new System.ArgumentException("element has parent");

                        ConnectChild(index, value);
                    }

                    //_owner.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Sets the specified element at the specified index into the child
        /// collection. It also corrects the parent.
        /// Note that the function requires that _item[index] == null and it
        /// also requires that the passed in child is not connected to another UIElement.
        /// </summary>
        /// <exception cref="ArgumentException">If the new child has already a parent or if the slot a the specified index is not null.</exception>
        private void ConnectChild(int index, Control value)
        {
            // Every function that calls this function needs to call VerifyAccess to prevent
            // foreign threads from changing the tree.
            //
            // We also need to ensure that the tree is homogenous with respect
            // to the dispatchers that the elements belong to.
            //
            Debug.Assert(items[index] == null);

            value.Parent = owner;

            // The child might be dirty. Hence we need to propagate dirty information
            // from the parent and from the child.
            //Control.PropagateFlags(_owner, Control.Flags.IsSubtreeDirtyForRender);
            //Control.PropagateFlags(value, Control.Flags.IsSubtreeDirtyForRender);
            //value._flags |= Control.Flags.IsDirtyForRender;
            items[index] = value;
            version++;

            //Control.PropagateResumeLayout(value);

            // Fire notifications
            owner.OnChildrenChanged(value, null /* no removed child */, index);
        }
        /// <summary>
        /// Disconnects a child.
        /// </summary>
        private void DisconnectChild(int index)
        {
            Debug.Assert(items[index] != null);

            Control child = items[index];

            // Every function that calls this function needs to call VerifyAccess to prevent
            // foreign threads from changing the tree.

            Control oldParent = child.Parent;

            items[index] = null;

            child.Parent = null;
            //Control.PropagateFlags(_owner, Control.Flags.IsSubtreeDirtyForRender);
            version++;

            //Control.PropagateSuspendLayout(child);

            oldParent.OnChildrenChanged(null /* no child added */, child, index);
        }

        /// <summary>
        /// Appends a UIElement to the end of the UIElementCollection.
        /// </summary>
        /// <param name="visual">The UIElement to be added to the end of the UIElementCollection.</param>
        /// <returns>The UIElementCollection index at which the UIElement has been added.</returns>
        /// <remarks>Adding a null is allowed.</remarks>
        /// <exception cref="ArgumentException">If the new child has already a parent.</exception>
        public int Add(Control element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.Parent != null)
            {
                throw new System.ArgumentException("element has parent");
            }

            if ((items == null) || (size == items.Length))
                EnsureCapacity(size + 1);

            int addedPosition = size++;
            Debug.Assert(items[addedPosition] == null);

            ConnectChild(addedPosition, element);
            version++;

            // invalidate measure on parent
            //_owner.InvalidateMeasure();

            return addedPosition;
        }

        /// <summary>
        /// Returns the zero-based index of the UIElement. If the UIElement is not
        /// in the UIElementCollection -1 is returned. If null is passed to the method, the index
        /// of the first entry with null is returned. If there is no null entry -1 is returned.
        /// </summary>
        /// <param name="UIElement">The UIElement to locate in the UIElementCollection.</param>
        public int IndexOf(Control element)
        {
            if (element == null || element.Parent == owner)
            {
                for (int i = 0; i < size; i++)
                {
                    if (items[i] == element)
                        return i;
                }

                // not found, return -1
                return -1;
            }
            else
                return -1;
        }

        /// <summary>
        /// Removes the specified element from the UIElementCollection.
        /// </summary>
        /// <param name="element">The UIElement to remove from the UIElementCollection.</param>
        /// <remarks>
        /// The UIElements that follow the removed UIElements move up to occupy
        /// the vacated spot. The indexes of the UIElements that are moved are
        /// also updated.
        ///
        /// If element is null then the first null entry is removed. Note that removing
        /// a null entry is linear in the size of the collection.
        /// </remarks>
        public void Remove(Control element)
        {
            int indexToRemove = -1;

            if (element != null)
            {
                if (element.Parent != owner)
                {
                    // If the UIElement is not in this collection we silently return without
                    // failing. This is the same behavior that ArrayList implements.
                    return;
                }

                Debug.Assert(element.Parent != null);

                DisconnectChild(indexToRemove = IndexOf(element));
            }
            else
            {
                // This is the case where element == null. We then remove the first null entry.
                for (int i = 0; i < size; i++)
                    if (items[i] == null)
                    {
                        indexToRemove = i;
                        break;
                    }
            }

            if (indexToRemove != -1)
            {
                --size;

                for (int i = indexToRemove; i < size; i++)
                {
                    Control child = items[i + 1];
                    items[i] = child;
                }

                items[size] = null;
            }

            //_owner.InvalidateMeasure();
        }

        /// <summary>
        /// Determines whether a element is in the UIElementCollection.
        /// </summary>
        public bool Contains(Control element)
        {
            if (element == null)
            {
                for (int i = 0; i < size; i++)
                    if (items[i] == null)
                        return true;

                return false;
            }
            else
                return (element.Parent == owner);
        }

        /// <summary>
        /// Removes all elements from the UIElementCollection.
        /// </summary>
        /// <remarks>
        /// Count is set to zero. Capacity remains unchanged.
        /// To reset the capacity of the UIElementCollection, call TrimToSize
        /// or set the Capacity property directly.
        /// </remarks>
        public void Clear()
        {
            for (int i = 0; i < size; i++)
            {
                if (items[i] != null)
                {
                    Debug.Assert(items[i].Parent == owner);
                    DisconnectChild(i);
                }

                items[i] = null;
            }

            size = 0;
            version++;

            //_owner.InvalidateMeasure();
        }

        /// <summary>
        /// Inserts an element into the UIElementCollection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="element">The UIElement to insert. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero.
        ///
        /// -or-
        ///
        /// index is greater than Count.
        /// </exception>
        /// <remarks>
        /// If Count already equals Capacity, the capacity of the
        /// UIElementCollection is increased before the new UIElement
        /// is inserted.
        ///
        /// If index is equal to Count, value is added to the
        /// end of UIElementCollection.
        ///
        /// The UIElements that follow the insertion point move down to
        /// accommodate the new UIElement. The indexes of the UIElements that are
        /// moved are also updated.
        /// </remarks>
        public void Insert(int index, Control element)
        {
            if (index < 0 || index > size)
                throw new ArgumentOutOfRangeException("index");

            if (element == null)
                throw new ArgumentNullException("element");

            if (element.Parent != null)  // or a element target can be added.
                throw new System.ArgumentException("element has parent");

            if ((items == null) || (size == items.Length))
                EnsureCapacity(size + 1);

            for (int i = size - 1; i >= index; i--)
            {
                Control child = items[i];
                items[i + 1] = child;
            }

            items[index] = null;

            size++;
            ConnectChild(index, element);
            // Note SetUIElement that increments the version to ensure proper enumerator
            // functionality.
            //_owner.InvalidateMeasure();
        }

        /// <summary>
        /// Removes the UIElement at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than zero
        /// - or - index is equal or greater than count.</exception>
        /// <remarks>
        /// The UIElements that follow the removed UIElements move up to occupy
        /// the vacated spot. The indexes of the UIElements that are moved are
        /// also updated.
        /// </remarks>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= size)
                throw new ArgumentOutOfRangeException("index");

            Remove(items[index]);
        }

        /// <summary>
        /// Removes a range of UIElements from the UIElementCollection.
        /// </summary>
        /// <param name="index">The zero-based index of the range
        /// of elements to remove</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero.
        /// -or-
        /// count is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not denote a valid range of elements in the UIElementCollection.
        /// </exception>
        /// <remarks>
        /// The UIElements that follow the removed UIElements move up to occupy
        /// the vacated spot. The indexes of the UIElements that are moved are
        /// also updated.
        /// </remarks>
        public void RemoveRange(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (size - index < count)
                throw new ArgumentOutOfRangeException("index");

            if (count > 0)
            {
                for (int i = index; i < index + count; i++)
                    if (items[i] != null)
                    {
                        DisconnectChild(i);
                        items[i] = null;
                    }

                size -= count;
                for (int i = index; i < size; i++)
                {
                    Control child = items[i + count];
                    items[i] = child;
                    items[i + count] = null;
                }

                version++; // Incrementing version number here to be consistent with the ArrayList implementation.
            }
        }

        // ----------------------------------------------------------------
        // IEnumerable Interface
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns an enumerator that can iterate through the UIElementCollection.
        /// </summary>
        /// <returns>Enumerator that enumerates the UIElementCollection in order.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// This is a simple UIElementCollection enumerator that is based on
        /// the ArrayListEnumeratorSimple that is used for ArrayLists.
        ///
        /// The following comment is from the CLR people:
        ///   For a straightforward enumeration of the entire ArrayList,
        ///   this is faster, because it's smaller.  Benchmarks showed
        ///   this.
        /// </summary>
        public struct Enumerator : IEnumerator, ICloneable
        {
            private ControlCollection _collection;
            private int _index; // -1 means not started. -2 means that it reached the end.
            private int _version;
            private Control _currentElement;

            internal Enumerator(ControlCollection collection)
            {
                _collection = collection;
                _index = -1; // not started.
                _version = _collection.version;
                _currentElement = null;
            }

            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            public Object Clone()
            {
                return MemberwiseClone();
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            public bool MoveNext()
            {
                if (_version == _collection.version)
                {
                    if ((_index > -2) && (_index < (_collection.size - 1)))
                    {
                        _index++;
                        _currentElement = _collection[_index];
                        return true;
                    }
                    else
                    {
                        _currentElement = null;
                        _index = -2; // -2 <=> reached the end.
                        return false;
                    }
                }
                else
                {
                    throw new InvalidOperationException("collection changed");
                }
            }

            /// <summary>
            /// Gets the current UIElement.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            /// <summary>
            /// Gets the current UIElement.
            /// </summary>
            public Control Current
            {
                get
                {
                    // Disable PREsharp warning about throwing exceptions in property
                    // get methods; see Windows OS Bugs #1035349 for an explanation.

                    if (_index < 0)
                    {
                        if (_index == -1)
                        {
                            // Not started.
                            throw new InvalidOperationException("not started");
                        }
                        else
                        {
                            // Reached the end.
                            Debug.Assert(_index == -2);
                            throw new InvalidOperationException("reached end");
                        }
                    }

                    return _currentElement;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                if (_version != _collection.version)
                    throw new InvalidOperationException("collection changed");
                _index = -1; // not started.
            }
        }
    }
}

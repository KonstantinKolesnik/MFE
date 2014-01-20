using System;
using System.Collections;

namespace MFE.Net.Http
{
    public class HttpCookieCollection : ICollection
    {
        #region Fields
        private ArrayList _cookies = new ArrayList();
        private object _syncRoot = new object();
        #endregion

        public HttpCookie this[int index]
        {
            get { return Get(index); }
        }
        public HttpCookie this[string name]
        {
            get { return Get(name); }
            set { Set(name, value); }
        }

        #region Properties
        public string[] AllKeys
        {
            get { return (string[])GetKeys().ToArray(typeof(string)); }
        }
        public ArrayList Keys
        {
            get { return GetKeys(); }
        }
        #endregion

        public void Add(HttpCookie value)
        {
            AddWithoutValidate(value);
        }

        protected void AddWithoutValidate(HttpCookie value)
        {
            _cookies.Add(value);
        }

        public void Clear()
        {
            _cookies.Clear();
        }

        public HttpCookie Get(int index)
        {
            return ((HttpCookie)_cookies[index]);
        }
        public HttpCookie Get(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                HttpCookie cookie = (HttpCookie)_cookies[i];
                if (cookie.Name == name)
                {
                    return cookie;
                }
            }
            return null;
        }
        public string GetKey(int index)
        {
            return ((HttpCookie)_cookies[index]).Name;
        }
        public void Remove(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                HttpCookie cookie = (HttpCookie)_cookies[i];
                if (cookie.Name == name)
                {
                    _cookies.RemoveAt(i);
                    break;
                }
            }
        }
        public void Set(string name, HttpCookie value)
        {
            for (int i = 0; i < Count; i++)
            {
                HttpCookie cookie = (HttpCookie)_cookies[i];
                if (cookie.Name == name)
                {
                    _cookies[i] = value;
                    return;
                }
            }
            Add(value);
        }

        private ArrayList GetKeys()
        {
            ArrayList list = new ArrayList();
            foreach (HttpCookie cookie in _cookies)
                list.Add(cookie.Name);
            return list;
        }

        #region ICollection Members
        public void CopyTo(Array array, int index)
        {
            _cookies.CopyTo(array, index);
        }
        public int Count
        {
            get { return _cookies.Count; }
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public object SyncRoot
        {
            get { return _syncRoot; }
        }
        #endregion

        #region IEnumerable Members
        public IEnumerator GetEnumerator()
        {
            return new HttpCookieEnumerator(this);
        }
        #endregion
    }
}

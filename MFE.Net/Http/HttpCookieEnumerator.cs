using System.Collections;

namespace MFE.Net.Http
{
    public class HttpCookieEnumerator : IEnumerator
    {
        private HttpCookieCollection _cookies;
        private int _idx = -1;

        public HttpCookieEnumerator(HttpCookieCollection collection)
        {
            _cookies = collection;
        }

        #region IEnumerator Members
        public object Current
        {
            get
            {
                return _cookies[_idx];
            }
        }
        public bool MoveNext()
        {
            _idx++;
            if (_idx < _cookies.Count)
            {
                return true;
            }
            else
            {
                _idx = -1;
                return false;
            }

        }
        public void Reset()
        {
            _idx = -1;
        }
        #endregion
    }
}

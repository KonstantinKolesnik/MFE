using System;
using System.Collections;

namespace MFE.Net.Http
{
    public class HttpCookie
    {
        #region Fields
        private string _name;
        private string _value;
        private string _path = "/";
        private string _domain;

        private DateTime _expires;
        private bool _expiresSet = false;

        private bool _secure;
        private bool _httpOnly;
        #endregion

        #region Constructors
        public HttpCookie()
        {
        }
        public HttpCookie(string name, string value)
        {
            _name = name;
            _value = value;
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }
        public DateTime Expires
        {
            get { return _expires; }
            set
            {
                _expires = value;
                _expiresSet = true;
            }
        }
        public bool Secure
        {
            get { return _secure; }
            set { _secure = value; }
        }
        public bool HttpOnly
        {
            get { return _httpOnly; }
            set { _httpOnly = value; }
        }
        #endregion

        #region Public methods
        public static HttpCookie[] CookiesFromHeader(string httpHeader)
        {
            if (httpHeader == null)
                return null;

            ArrayList cookies = new ArrayList();

            foreach (string cookieStr in httpHeader.Split(';'))
            {
                string[] cookieParts = cookieStr.Split('=');

                if (cookieParts.Length != 2)
                    continue;

                HttpCookie cookie = new HttpCookie();
                cookie.Name = cookieParts[0];
                cookie.Value = cookieParts[1];

                cookies.Add(cookie);
            }

            HttpCookie[] res = new HttpCookie[cookies.Count];

            for (int i = 0; i < cookies.Count; i++)
            {
                res[i] = cookies[i] as HttpCookie;
            }

            return res;
        }
        public override string ToString()
        {
            // Set-Cookie: RMID=732423sdfs73242; expires=Fri, 31-Dec-2010 23:59:59 GMT; path=/; domain=.example.net; HttpOnly

            return _name + "=" + _value + "; "
                + (_expiresSet ? "expires=" +
                //#if (MF)
                _expires.ToString("ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'")
                //#else
                // _expires.ToString("r")
                //#endif

                + "; " : "")
                + (_path != null ? "path=" + _path + "; " : "")
                + (_domain != null ? "domain=" + _domain + "; " : "")
                + (_httpOnly ? "HttpOnly" : "");
        }
        #endregion
    }
}

using Konak.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HttpBrowser
{
    public enum WebDataType
    {
        TEXT,
        TEXT_HTML,
        TEXT_PLAIN,
        IMAGE,
        IMAGE_GIF,
        IMAGE_JPEG,
        IMAGE_PNG,
        AUDIO,
        MESSAGE,
        VIDEO,
        VIDEO_MPEG,
        MULTIPART,
        APPLICATION,
        APPLICATION_JSON,
        APPLICATION_POSTSCRIPT,
        APPLICATION_ODA,
        APPLICATION_OCTET_STREAM, // uninterpreted binary data
        X_TOKEN,
        UNKNOWN

    }

    public class WebBrowser
    {
        #region private properties
        private CookieContainer _cookieContainer = new CookieContainer();
        private WebHeaderCollection _webHeaders = new WebHeaderCollection();
        private byte[] _data = new byte[0];
        private object _synchRoot = new object();

        
        #endregion

        #region public events
        public event BrowserNavigationErrorDelegate BrowserNavigationErrorEvent;
        #endregion

        #region public properties
        public string Charset { get; private set; }
        public Encoding DataEncoding { get; set; }
        public HttpStatusCode ResponseStatusCode { get; private set; }
        public string ResponseServer { get; private set; }
        public Uri BaseUri { get; set; }
        public Uri NavigateUri { get; set; }
        //public string RequestResource { get; set; }
        public string Referer { get; set; }
        public WebDataType DataType { get; private set; }
        public Encoding DefaultEncoding { get { return Encoding.UTF8; } }

        #endregion

        #region constructor
        public WebBrowser()
        {
            this.DataEncoding = Encoding.UTF8;
            this.Referer = string.Empty;
            this.ResponseStatusCode = HttpStatusCode.OK;
        }
        #endregion

        #region methods

        #region RaireBrowserNavigationErrorEvent
        private void RaireBrowserNavigationErrorEvent(Exception exception)
        {
            BrowserNavigationErrorDelegate evt = BrowserNavigationErrorEvent;

            if (CH.IsEmpty(evt)) return;

            foreach(Delegate d in evt.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this, exception);
                }
                catch { }
            }

            Root.RaiseComponentErrorEvent(this, exception);
        }
        #endregion

        #region CleareCurrentPageData
        private void CleareCurrentPageData()
        {
            this._data = new byte[0];
            this.ResponseStatusCode = HttpStatusCode.OK;
            this.ResponseServer = string.Empty;
        }
        #endregion

        #region LoadCoockies
        public void LoadCoockies(List<string> coockies)
        {
            if (!CH.IsEmpty(coockies))
                foreach (string coockie in coockies)
                    _cookieContainer.SetCookies(this.BaseUri, coockie);
        }

        public void LoadCoockies(string coockies)
        {
            if (!CH.IsEmpty(coockies))
                foreach (string coockie in coockies.Split(';'))
                    _cookieContainer.SetCookies(this.BaseUri, coockie);
        }

        public void Navigate(object requestResource)
        {
            throw new NotImplementedException();
        }

        public void LoadCoockies(CookieContainer container)
        {
            if (!CH.IsEmpty(container))
                foreach (Cookie cookie in container.GetCookies(this.BaseUri))
                    this._cookieContainer.Add(cookie);
        }

        public CookieContainer GetCoockies()
        {
            return _cookieContainer;
        }

        #endregion

        #region ClearCoockies
        public void ClearCoockies()
        {
            this._cookieContainer = new CookieContainer();
        }
        #endregion

        #region SetSettings
        public void SetSettings(string baseUri, string relativeUri, string referer, string coockies)
        {
            SetSettings(new Uri(new Uri(baseUri), relativeUri), referer);
            LoadCoockies(coockies);
        }

        public void SetSettings(string baseUri, string relativeUri, string referer, List<string> coockies)
        {
            SetSettings(new Uri(new Uri(baseUri), relativeUri), referer);
            LoadCoockies(coockies);
        }

        public void SetSettings(Uri uri)
        {
            this.BaseUri = uri;
        }

        public void SetSettings(Uri uri, string referer)
        {
            SetSettings(uri);
            this.Referer = referer;
        }

        public void SetSettings(Uri uri, string referer, string coockies)
        {
            SetSettings(uri, referer);
            LoadCoockies(coockies);
        }

        public void SetSettings(Uri uri, string referer, List<string> coockies)
        {
            SetSettings(uri, referer);
            LoadCoockies(coockies);
        }
        #endregion

        #region AddHeader
        public void AddHeader(string name, string value)
        {
            if (!CH.IsEmpty(name))
                this._webHeaders.Set(name, value);
        }
        public void AddHeader(List<KeyValuePair<string, string>> headers)
        {
            if (!CH.IsEmpty(headers))
                foreach (KeyValuePair<string, string> header in headers)
                    this._webHeaders.Add(header.Key, header.Value);
        }
        public void AddHeader(WebHeaderCollection headers)
        {
            if (!CH.IsEmpty(headers))
                foreach (string header in headers)
                    this._webHeaders.Add(header);
        }
        #endregion

        #region ClearHeaders
        public void ClearHeaders()
        {
            this._webHeaders.Clear();
        }
        #endregion

        #region GetWebRequestObject

        private HttpWebRequest GetWebRequestObject()
        {
            return GetWebRequestObject(this.NavigateUri);
        }
        private HttpWebRequest GetWebRequestObject(string requestResource)
        {
            return GetWebRequestObject(this.NavigateUri = new Uri(this.BaseUri, requestResource));
        }
        private HttpWebRequest GetWebRequestObject(Uri uri)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);

            //NetworkCredential nc = new NetworkCredential("Anonymous", string.Empty);

            //req.Credentials = nc;
            req.ImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Anonymous;
            //req.PreAuthenticate = false;

            req.Accept = "*/*";
            req.UserAgent = Root.CONFIG.Settings.Browser.UserAgent;
            req.Referer = this.Referer;
            
            req.CookieContainer = this._cookieContainer;
            req.Headers = this._webHeaders;

            req.Headers.Add("Accept-Language: " + Root.CONFIG.Settings.Browser.AcceptLanguage);
            req.Headers.Add("Accept-Encoding: gzip, deflate");
            req.Headers.Add("UA-CPU: x86");

            req.KeepAlive = Root.CONFIG.Settings.Browser.KeepAlive;

            if (!req.KeepAlive)
                req.Timeout = Root.CONFIG.Settings.Browser.RequestTimeout * 1000;
            

            //req.Proxy = new WebProxy("localhost", 8888);

            //req.Timeout = Root.CONFIG.Settings.Browser.RequestTimeout * 1000;

            //if (this.useproxy)
            //{
            //    req.proxy = new webproxy(this.proxyhost, this.proxyport);
            //}
            //else
            //{
            //    req.proxy = null;
            //}

            req.ServicePoint.ConnectionLimit = 999;
            return req;
        }
        #endregion

        #region SetCharset
        private void SetCharset(string contentType)
        {
            this.Charset = "utf-8";
            string template = "charset";
            int i = contentType.IndexOf(template);

            if (i == -1) return;

            string charset = contentType.Substring(i + template.Length);

            i = charset.IndexOf(';');

            if(i > -1)
                charset = charset.Substring(0, i);
            
            this.Charset = charset.Replace('=', ' ').Trim();
        }
        #endregion

        #region SetDataType
        private void SetDataType(string contentType)
        {
            contentType = contentType.ToLower();

            // text
            if (contentType.StartsWith("text"))
            {
                SetCharset(contentType);

                if (contentType.StartsWith("text/html"))
                {
                    this.DataType = WebDataType.TEXT_HTML;
                    return;
                }
                if (contentType.StartsWith("text/plain"))
                {
                    this.DataType = WebDataType.TEXT_PLAIN;
                    return;
                }

                this.DataType = WebDataType.TEXT;
                return;
            }

            if (contentType.StartsWith("application"))
            {
                if (contentType.StartsWith("application/json"))
                {
                    this.DataType = WebDataType.APPLICATION_JSON;
                    
                    SetCharset(contentType);

                    return;
                }
                if (contentType.StartsWith("application/PostScript"))
                {
                    this.DataType = WebDataType.APPLICATION_POSTSCRIPT;
                    return;
                }
                if (contentType.StartsWith("application/oda"))
                {
                    this.DataType = WebDataType.APPLICATION_ODA;
                    return;
                }
                if (contentType.StartsWith("application/octet-stream"))
                {
                    this.DataType = WebDataType.APPLICATION_OCTET_STREAM;
                    return;
                }

                this.DataType = WebDataType.APPLICATION;
                return;
            }

            if (contentType.StartsWith("image"))
            {
                if (contentType.StartsWith("image/gif"))
                {
                    this.DataType = WebDataType.IMAGE_GIF;
                    return;
                }
                if (contentType.StartsWith("image/jpeg"))
                {
                    this.DataType = WebDataType.IMAGE_JPEG;
                    return;
                }
                if (contentType.StartsWith("image/png"))
                {
                    this.DataType = WebDataType.IMAGE_PNG;
                    return;
                }
                
                this.DataType = WebDataType.IMAGE;
                return;
            }

            if (contentType.StartsWith("audio"))
            {
                this.DataType = WebDataType.AUDIO;
                return;
            }
            if (contentType.StartsWith("message"))
            {
                this.DataType = WebDataType.MESSAGE;
                return;
            }
            if (contentType.StartsWith("video"))
            {
                if (contentType.StartsWith("video/mpeg"))
                {
                    this.DataType = WebDataType.VIDEO_MPEG;
                    return;
                }

                this.DataType = WebDataType.VIDEO;
                return;
            }
            if (contentType.StartsWith("x-token"))
            {
                this.DataType = WebDataType.X_TOKEN;
                return;
            }
            if (contentType.StartsWith("multipart"))
            {
                this.DataType = WebDataType.MULTIPART;
                return;
            }

            this.DataType = WebDataType.UNKNOWN;
        }
        #endregion

        #region ReadResponseData
        private void ReadResponseData(HttpWebResponse resp, bool keepAlive)
        {
            this.ResponseStatusCode = resp.StatusCode;
            this.ResponseServer = resp.Server;

            this.SetDataType(resp.ContentType);

            try
            {
                this.DataEncoding = CH.IsEmpty(resp.CharacterSet) ? (Encoding)this.DefaultEncoding.Clone() : Encoding.GetEncoding(resp.CharacterSet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                this.DataEncoding = (Encoding)this.DefaultEncoding.Clone();
            }

            using (Stream dataStream = Helpers.StreamHelper.GetContentEncodingReadStream(resp.ContentEncoding, resp.GetResponseStream()))
            {
                if (!keepAlive)
                    dataStream.ReadTimeout = Root.CONFIG.Settings.Browser.ResponseTimeout;

                byte[] buf = new byte[102400];
                int readBytes = 0;
                int dataLength = 0;

                while ((readBytes = dataStream.Read(buf, 0, buf.Length)) > 0)
                {
                    dataLength = this._data.Length;

                    Array.Resize<byte>(ref this._data, dataLength + readBytes);
                    Array.Copy(buf, 0, this._data, dataLength, readBytes);
                }

                dataStream.Close();
            }

        }
        #endregion 

        #region Navigate

        public bool Navigate()
        {
            return  NavigateAsync(string.Empty).Result;
        }

        public bool Navigate(string requestResource)
        {
            return NavigateAsync(requestResource).Result;
        }

        public async Task<bool> NavigateAsync(string requestResource)
        {
            HttpWebRequest req = null;
            HttpWebResponse resp = null;

            bool navigationResult = false;

            if (CH.IsEmpty(this.BaseUri))
                return false;

            this.NavigateUri = new Uri(this.BaseUri, requestResource);

            this.CleareCurrentPageData();

            try
            {
                req = this.GetWebRequestObject(requestResource);

                try
                {
                    resp = req.GetResponse() as HttpWebResponse;
                }
                catch (WebException ex)
                {
                    ReadResponseData(ex.Response as HttpWebResponse, req.KeepAlive);
                    
                    throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    throw;
                }

                ReadResponseData(resp, req.KeepAlive);

                this._cookieContainer = req.CookieContainer;

                navigationResult = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
            finally
            {
                if (resp != null) resp.Close();
            }

            return navigationResult;
        }

        #endregion

        #region MakeJsonCall
        public async Task<bool> MakeJsonCall(string formAction, string method, string jsonCall)
        {
            HttpWebRequest req = null;
            HttpWebResponse resp = null;

            bool navigationResult = false;

            if (CH.IsEmpty(this.BaseUri) || CH.IsEmpty(formAction) || CH.IsEmpty(jsonCall))
                return false;

            byte[] postData = this.DefaultEncoding.GetBytes(jsonCall);

            try
            {
                req = GetWebRequestObject(new Uri(this.BaseUri, formAction));

                req.Method = method;
                req.ContentType = "application/json";
                req.ContentLength = postData.Length;

                using (Stream reqStream = await req.GetRequestStreamAsync())
                {
                    await reqStream.WriteAsync(postData, 0, postData.Length);
                    reqStream.Close();
                }

                resp = await req.GetResponseAsync() as HttpWebResponse;
                
                this.ResponseStatusCode = resp.StatusCode;
                this.ResponseServer = resp.Server;
                this._cookieContainer = req.CookieContainer;

                this.SetDataType(resp.ContentType);

                this.DataEncoding = Encoding.GetEncoding(resp.CharacterSet);

                using (Stream dataStream = Helpers.StreamHelper.GetContentEncodingReadStream(resp.ContentEncoding, resp.GetResponseStream()))
                {
                    if (!req.KeepAlive)
                        dataStream.ReadTimeout = Root.CONFIG.Settings.Browser.ResponseTimeout;

                    byte[] buf = new byte[102400];
                    int readBytes = 0;
                    int dataLength = 0;

                    while ((readBytes = await dataStream.ReadAsync(buf, 0, buf.Length)) > 0)
                    {
                        dataLength = this._data.Length;

                        Array.Resize<byte>(ref this._data, dataLength + readBytes);
                        Array.Copy(buf, 0, this._data, dataLength, readBytes);
                    }

                    dataStream.Close();
                }

                navigationResult = true;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (resp != null) resp.Close();
            }

            return navigationResult;
        }
        #endregion

        #region GetBytes
        public byte[] GetBytes()
        {
            return _data;
        }
        #endregion

        #region GetHtml
        public string GetHtml()
        {
            return GetHtml(this.DataEncoding);
        }
        public string GetHtml(Encoding encoding)
        {
            return encoding.GetString(this._data);
        }
        #endregion

        #region GetHtmlByCharset
        public string GetHtmlByCharset()
        {
            return Encoding.GetEncoding(this.Charset).GetString(this._data);
        }
        public string GetHtmlByCharset(string charset)
        {
            return Encoding.GetEncoding(charset).GetString(this._data);
        }
        #endregion

        #region GetJsonObject
        public object GetJsonObject()
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();

            return ser.DeserializeObject(this.GetHtml());
        }
        #endregion

        #region Todo For SSL
        //private void ToDoForSSL()
        //{
        //    ServicePointManager.ServerCertificateValidationCallback += delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //    {

        //        //return true;

        //        if (sslPolicyErrors == SslPolicyErrors.None) return true;

        //        try
        //        {
        //            //chain.Reset();
        //            X509Certificate2 c = chain.ChainElements[0].Certificate;
        //            X509Chain ch = new X509Chain(true);

        //            ch.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
        //            ch.ChainPolicy.RevocationMode = X509RevocationMode.Online;

        //            X509Store stt = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);


        //            stt.Open(OpenFlags.ReadOnly);

        //            if (stt.Certificates.Count > 0)
        //                ch.ChainPolicy.ExtraStore.AddRange(stt.Certificates);

        //            stt.Close();

        //            stt = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

        //            stt.Open(OpenFlags.ReadOnly);

        //            if (stt.Certificates.Count > 0)
        //                ch.ChainPolicy.ExtraStore.AddRange(stt.Certificates);

        //            stt.Close();

        //            stt = new X509Store(StoreName.AuthRoot, StoreLocation.LocalMachine);

        //            stt.Open(OpenFlags.ReadOnly);

        //            if (stt.Certificates.Count > 0)
        //                ch.ChainPolicy.ExtraStore.AddRange(stt.Certificates);

        //            stt.Close();





        //            stt = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);


        //            stt.Open(OpenFlags.ReadOnly);

        //            if (stt.Certificates.Count > 0)
        //                ch.ChainPolicy.ExtraStore.AddRange(stt.Certificates);

        //            stt.Close();

        //            stt = new X509Store(StoreName.Root, StoreLocation.CurrentUser);

        //            stt.Open(OpenFlags.ReadOnly);

        //            if (stt.Certificates.Count > 0)
        //                ch.ChainPolicy.ExtraStore.AddRange(stt.Certificates);

        //            stt.Close();

        //            stt = new X509Store(StoreName.AuthRoot, StoreLocation.CurrentUser);

        //            stt.Open(OpenFlags.ReadOnly);

        //            if (stt.Certificates.Count > 0)
        //                ch.ChainPolicy.ExtraStore.AddRange(stt.Certificates);

        //            stt.Close();





        //            if (ch.Build(c))
        //            {
        //                System.Diagnostics.Debug.WriteLine("test");
        //            }



        //            chain.ChainElements[0].Certificate.Verify();

        //            File.WriteAllBytes(@"c:\temp\arca.cer", chain.ChainElements[0].Certificate.Export(X509ContentType.Cert));
        //            File.WriteAllBytes(@"c:\temp\t.cer", certificate.Export(X509ContentType.Cert));
        //        }
        //        catch (Exception ex)
        //        {
        //            mErrorMessage += "\r\nfile write exception:" + ex.Message + "\r\n";
        //        }

        //        mErrorMessage += "SSL Policy error: " + sslPolicyErrors.ToString();
        //        throw new Exception(mErrorMessage);
        //    };

        //}
        #endregion

        #endregion
    }
}

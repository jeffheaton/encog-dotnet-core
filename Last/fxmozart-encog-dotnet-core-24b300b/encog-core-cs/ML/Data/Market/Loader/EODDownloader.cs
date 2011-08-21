using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Encog.ML.Data.Market.FinanceDataSet;
using Encog.Util;
using Encog.Util.HTTP;

namespace Encog.ML.Data.Market.Loader
{
    class EODDownloader :IMarketLoader
    {

        /* <ticker>,<per>,<date>,<open>,<high>,<low>,<close>,<vol>
 GJH,D,20110720,9.5,9.5,9.5,9.5,500
 GJH,D,20110721,9.5,9.5,9.5,9.5,0
 GJH,D,20110722,9.5,9.5,9.45,9.45,200
    <table class="rc_t">
		<tbody><tr><td><div id="ctl00_cph1_ls1_ch1_divText">MEMBER LOGIN</div></td><td><div id="ctl00_cph1_ls1_ch1_divLink" class="hlink"></div></td><td align="right"><div id="ctl00_cph1_ls1_ch1_divIcon" class="hicon"></div></td></tr>
	</tbody></table>
</div></div></div></div>
	<div class="bbf">
		<div id="ctl00_cph1_ls1_pnlLogin" onkeypress="javascript:return WebForm_FireDefaultButton(event, 'ctl00_cph1_ls1_btnLogin')">
	
			<table width="100%" cellpadding="3" cellspacing="0">
			<tbody><tr><td colspan="2">Email/Username:</td></tr>
			<tr><td colspan="2"><input name="ctl00$cph1$ls1$txtEmail" id="ctl00_cph1_ls1_txtEmail" style="width:140px;" type="text"></td></tr>
			<tr><td colspan="2">Password:</td></tr>
			<tr><td colspan="2"><input name="ctl00$cph1$ls1$txtPassword" id="ctl00_cph1_ls1_txtPassword" style="width:140px;" type="password"></td></tr>
			<tr><td colspan="2"><input id="ctl00_cph1_ls1_chkRemember" name="ctl00$cph1$ls1$chkRemember" type="checkbox"><label for="ctl00_cph1_ls1_chkRemember">Remember Me</label> </td></tr>
			<tr><td colspan="2"><span id="ctl00_cph1_ls1_lblMessage" style="color:Red;"></span></td></tr>
			<tr><td colspan="2" align="center"><input name="ctl00$cph1$ls1$btnLogin" value="Login" id="ctl00_cph1_ls1_btnLogin" style="height:26px;width:60px;" type="submit"></td></tr>					
			<tr><td><a id="ctl00_cph1_ls1_lnkRegister" href="register.aspx">Register</a></td><td align="right"><a id="ctl00_cph1_ls1_lnkForgot" href="support/Forgot.aspx">Forgot</a></td></tr>
			</tbody></table>
         * 
         * 
         ctl00%24cph1%24ls1%24txtEmail=fxmozart@gmail.com
         ctl00%24cph1%24ls1%24txtPassword=123fx123
         ctl00%24cph1%24ls1%24btnLogin=Login
         * 
         * 
         Full request:
         * 
         ctl00_tsm_HiddenField=
__EVENTTARGET=
__EVENTARGUMENT=
__VIEWSTATE=/wEPDwUJMTAzNDI1MzE2D2QWAmYPZBYCAgMPZBYCAgcPZBYMAgcPDxYCHgtQb3N0QmFja1VybAUzaHR0cHM6Ly93d3cuZW9kZGF0YS5jb20vY2hlY2tvdXQvZGVmYXVsdC5hc3B4P3A9TUJNZGQCCQ8PFgIfAAUzaHR0cHM6Ly93d3cuZW9kZGF0YS5jb20vY2hlY2tvdXQvZGVmYXVsdC5hc3B4P3A9TVNNZGQCCw8PFgIfAAUzaHR0cHM6Ly93d3cuZW9kZGF0YS5jb20vY2hlY2tvdXQvZGVmYXVsdC5hc3B4P3A9TUdNZGQCDQ8PFgIfAAUzaHR0cHM6Ly93d3cuZW9kZGF0YS5jb20vY2hlY2tvdXQvZGVmYXVsdC5hc3B4P3A9TVBNZGQCjwIPZBYCAgMPZBYCAgcPDxYCHgRUZXh0ZWRkApECD2QWBAIBDw8WAh8BBQw4OC4xOTAuMjIuOTJkZAIDDw8WAh8BBQZGcmFuY2VkZBgBBR5fX0NvbnRyb2xzUmVxdWlyZVBvc3RCYWNrS2V5X18WAQUaY3RsMDAkY3BoMSRsczEkY2hrUmVtZW1iZXLpOEoWBYwUTCR03DulIVf8oo9vgWvS3gXVLD/Zr6G21A==
__PREVIOUSPAGE=aXe27571Y74uHu_hp0uDrATIMLj86LCicgGgZZ09geuBCx0ogbVzg4y0ttmVnGVbRRYDl5mqBjk9DGz3Wwr2yS4PoyiYSiP_gV4u9g2zzH41
__EVENTVALIDATION=/wEWDALt6 /LAwKP6ehfAviAu4APArmEpNEKArTg6rUNAoHi1qEMAqWb5iYC3Yf4hg8C7vWs0QsC3vm4gAQC7vTyxgsCmtzRjA5C0Cn1m7QsORBbDv/rw1NRwxyOtKQ6Uy7R6Oq/u2HZsw==
ctl00%24Menu1%24s1%24txtSearch=
ctl00%24cph1%24ls1%24txtEmail=fxmozart@gmail.com
ctl00%24cph1%24ls1%24txtPassword=123fx123
ctl00%24cph1%24ls1%24btnLogin=Login

         * 
         * 
         * 
         * 
         * * 
         * */
        public const string UserName = "fxmozart@gmail.com";
        public const string PassWord = "123fx123";

        /// This method builds a URL to load data from Yahoo Finance for a neural
        /// network to train with.
        /// </summary>
        /// <param name="ticker">The ticker symbol to access.</param>
        /// <param name="from">The beginning date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns>The URL to read from</returns>
        private static string BuildURL()
        {

           return GetWebContent(UserName, PassWord);

        }


        public static string GetWebContent(String username, String password)
        {
            String _retVal = "err001";
            String _urlLogin = "http://www.eoddata.com/products/default.aspx";
            String _urlSignin = "http://www.eoddata.com/products/default.aspx";
            String _respStr = "";
            String _postData = "";

            Uri _uriLogin = null;
            Uri _uriSignin = null;

            _urlLogin = "http://www.eoddata.com/products/default.aspx"; // is the login Page
            _urlSignin = "http://www.eoddata.com/products/default.aspx"; //is the link used inside the action of the form tag (Note: I suppose in this example that the form name inside the _urlLogin page is "loginForm")

           

            _uriLogin = new Uri(_urlLogin);
            _uriSignin = new Uri(_urlSignin);

            try
            {
                HttpWebRequest _wReq;
                HttpWebResponse _wResp;
                System.IO.StreamReader _sr;
                System.Text.ASCIIEncoding _enc = new System.Text.ASCIIEncoding();

                CookieContainer _cookies;

                _wReq = (HttpWebRequest)WebRequest.Create(_uriLogin);
                _wReq.CookieContainer = new CookieContainer();
                _wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

                _wResp = (HttpWebResponse)_wReq.GetResponse();
                _sr = new System.IO.StreamReader(_wResp.GetResponseStream());
                _respStr = _sr.ReadToEnd();
                _sr.Close();

                _cookies = _wReq.CookieContainer;

                _wResp.Close();
                Hashtable _otherData;
                // in an initialization routine
                _otherData = new Hashtable();
                _otherData.Add("username", new Hashtable());
                _otherData.Add("password", new Hashtable());
                //Here insert all the parameter needed for the login... eg: in my example I suppose that the loginForm has two input field named "username" and "password"
                ((Hashtable)_otherData["ctl00%24cph1%24ls1%24txtEmail"])["value"] = username;
                ((Hashtable)_otherData["ctl00%24cph1%24ls1%24txtPassword"])["value"] = password;


                //you could need  to insert other information

                _postData = "";
                foreach (string name in _otherData.Keys)
                {
                    _postData += "&" + name + "=" + ((Hashtable)_otherData[name])["value"];
                }
                _postData = _postData.Substring(1);

                byte[] _data = _enc.GetBytes(_postData);

                _wReq = (HttpWebRequest)WebRequest.Create(_uriSignin);
                _wReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                _wReq.Referer = _urlLogin;
                _wReq.KeepAlive = true;
                _wReq.Method = "POST";
                _wReq.ContentType = "application/x-www-form-urlencoded";
                _wReq.ContentLength = _data.Length;
                _wReq.CookieContainer = _cookies;
                _wReq.AllowAutoRedirect = false;
                _wReq.UserAgent = "Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";


                System.IO.Stream _outStream = _wReq.GetRequestStream();
                _outStream.Write(_data, 0, _data.Length);

                _outStream.Close();


                _wResp = (HttpWebResponse)_wReq.GetResponse();
                _sr = new System.IO.StreamReader(_wResp.GetResponseStream());
                _respStr = _sr.ReadToEnd();
                _sr.Close();

                _wResp.Close();


                // _respStr contains the page content... use this for your needed.
                _retVal = _respStr;

            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                throw ex;
                //Response.Write(e.ToString());
                //return null;
            }

            return _retVal;
        }


        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {
            BuildURL();
            return null;
        }

        public ICollection<LoadedMarketData> Load(FinanceSymbols.Instrument ticker, IList<FinanceDataTypes> dataNeeded, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public string GetFile(string file)
        {
            throw new NotImplementedException();
        }
    }
}

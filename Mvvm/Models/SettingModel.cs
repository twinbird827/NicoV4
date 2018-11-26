using NicoV4.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Models
{
    public class SettingModel : BindableBase
    {
        public static SettingModel Instance { get; private set; } = JsonConverter.Deserialize<SettingModel>(Variables.SettingPath) ?? new SettingModel();

        /// <summary>
        /// ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ
        /// </summary>
        private SettingModel()
        {

        }

        // ****************************************************************************************************
        // ﾌﾟﾛﾊﾟﾃｨ定義
        // ****************************************************************************************************

        /// <summary>
        /// ﾒｰﾙｱﾄﾞﾚｽ
        /// </summary>
        public string MailAddress
        {
            get { return _MailAddress; }
            set { SetProperty(ref _MailAddress, value); }
        }
        private string _MailAddress;

        /// <summary>
        /// ﾊﾟｽﾜｰﾄﾞ
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { SetProperty(ref _Password, value); }
        }
        private string _Password;

        /// <summary>
        /// ﾛｸﾞｲﾝ時ｸｯｷｰｺﾝﾃﾅ
        /// </summary>
        private CookieContainer Cookies { get; set; }

        // ****************************************************************************************************
        // 公開ﾒｿｯﾄﾞ定義
        // ****************************************************************************************************

        /// <summary>
        /// ﾊﾟｽﾜｰﾄﾞを取得します。
        /// </summary>
        /// <returns>複合化したﾊﾟｽﾜｰﾄﾞ</returns>
        public string GetPassword()
        {
            if (!string.IsNullOrWhiteSpace(Password))
            {
                return Encrypter.DecryptString(Password, Variables.ApplicationId);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// ﾊﾟｽﾜｰﾄﾞを設定します。
        /// </summary>
        /// <param name="password">暗号化前のﾊﾟｽﾜｰﾄﾞ</param>
        public void SetPassword(string password)
        {
            Password = Encrypter.EncryptString(password, Variables.ApplicationId);
        }

        /// <summary>
        /// ﾛｸﾞｲﾝ用ｸｯｷｰを取得します。
        /// </summary>
        /// <returns></returns>
        public async Task<CookieContainer> GetCookies()
        {
            Cookies = Cookies ?? await LoginAsync(MailAddress, GetPassword());

            if (Cookies == null)
            {
                throw new ArgumentException("ﾛｸﾞｲﾝできませんでした。");
            }

            return Cookies;
        }

        // ****************************************************************************************************
        // 内部ﾒｿｯﾄﾞ定義
        // ****************************************************************************************************

        /// <summary>
        /// ﾛｸﾞｲﾝし、ｸｯｷｰを受け取ります。
        /// </summary>
        /// <returns>ﾛｸﾞｲﾝ用CookieContainer</returns>
        private async Task<CookieContainer> LoginAsync(string mail, string password)
        {
            const string loginUrl = "https://secure.nicovideo.jp/secure/login?site=niconico";

            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                using (var handler = new HttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "next_url", string.Empty },
                    { "mail", mail },
                    { "password", password }
                });

                    await client.PostAsync(loginUrl, content);

                    return handler.CookieContainer;
                }
            }
            catch
            {
                return null;
            }
        }

        protected override void OnDisposed()
        {
            // ﾌﾟﾛﾊﾟﾃｨに設定された内容を外部ﾌｧｲﾙに保存します。
            JsonConverter.Serialize(Variables.SettingPath, this);

            MailAddress = null;
            Password = null;

            base.OnDisposed();
        }
    }
}

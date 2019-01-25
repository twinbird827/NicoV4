using NicoV4.Common;
using NicoV4.Mvvm.ComboItems;
using NicoV4.Mvvm.Views;
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
            Browser = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

            Disposed += (sender, e) =>
            {
                // ﾌﾟﾛﾊﾟﾃｨに設定された内容を外部ﾌｧｲﾙに保存します。
                JsonConverter.Serialize(Variables.SettingPath, this);

                MailAddress = null;
                Password = null;
                Cookies = null;
            };
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
        /// ﾌﾞﾗｳｻﾞ
        /// </summary>
        public string Browser
        {
            get { return _Browser; }
            set { SetProperty(ref _Browser, value); }
        }
        private string _Browser;

        /// <summary>
        /// ｻﾑﾈｲﾙ
        /// </summary>
        public ThumbnailSize Thumbnail
        {
            get { return _Thumbnail; }
            set { SetProperty(ref _Thumbnail, value); }
        }
        private ThumbnailSize _Thumbnail;

        /// <summary>
        /// ﾏｲﾘｽﾄ内ﾋﾞﾃﾞｵ検索画面のｿｰﾄ順
        /// </summary>
        public ComboboxItemModel SearchVideoByMylistSort
        {
            get { return _SearchVideoByMylistSort; }
            set { SetProperty(ref _SearchVideoByMylistSort, value); }
        }
        private ComboboxItemModel _SearchVideoByMylistSort;

        /// <summary>
        /// ﾗﾝｷﾝｸﾞ画面の対象
        /// </summary>
        public ComboboxItemModel SearchVideoByRankingTarget
        {
            get { return _SearchVideoByRankingTarget; }
            set { SetProperty(ref _SearchVideoByRankingTarget, value); }
        }
        private ComboboxItemModel _SearchVideoByRankingTarget;

        /// <summary>
        /// ﾗﾝｷﾝｸﾞ画面の期間
        /// </summary>
        public ComboboxItemModel SearchVideoByRankingPeriod
        {
            get { return _SearchVideoByRankingPeriod; }
            set { SetProperty(ref _SearchVideoByRankingPeriod, value); }
        }
        private ComboboxItemModel _SearchVideoByRankingPeriod;

        /// <summary>
        /// ﾗﾝｷﾝｸﾞ画面のｶﾃｺﾞﾘ
        /// </summary>
        public ComboboxItemModel SearchVideoByRankingCategory
        {
            get { return _SearchVideoByRankingCategory; }
            set { SetProperty(ref _SearchVideoByRankingCategory, value); }
        }
        private ComboboxItemModel _SearchVideoByRankingCategory;

        /// <summary>
        /// ﾏｲﾘｽﾄ内ﾋﾞﾃﾞｵ検索画面のｿｰﾄ順
        /// </summary>
        public ComboboxItemModel SearchVideoByWordSort
        {
            get { return _SearchVideoByWordSort; }
            set { SetProperty(ref _SearchVideoByWordSort, value); }
        }
        private ComboboxItemModel _SearchVideoByWordSort;

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞﾌｧｲﾙ名の種類
        /// </summary>
        public DownloadFileName DownloadFileName
        {
            get { return _DownloadFileName; }
            set { SetProperty(ref _DownloadFileName, value); }
        }
        private DownloadFileName _DownloadFileName;

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞﾃﾞｨﾚｸﾄﾘ
        /// </summary>
        public string DownloadDirectory
        {
            get { return _DownloadDirectory; }
            set { SetProperty(ref _DownloadDirectory, value); }
        }
        private string _DownloadDirectory;

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
    }
}

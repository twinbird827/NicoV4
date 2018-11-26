using StatefulModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Models
{
    public abstract class SearchVideoModel : BindableBase
    {
        protected SearchVideoModel() : this(false)
        {

        }

        protected SearchVideoModel(bool requiredLogin)
        {
            RequiredLogin = requiredLogin;
        }

        // ****************************************************************************************************
        // ﾌﾟﾛﾊﾟﾃｨ定義
        // ****************************************************************************************************

        private bool RequiredLogin { get; set; }

        /// <summary>
        /// ｱｲﾃﾑ構成
        /// </summary>
        public ObservableSynchronizedCollection<string> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableSynchronizedCollection<string> _Videos = new ObservableSynchronizedCollection<string>();

        // ****************************************************************************************************
        // ﾒｿｯﾄﾞ定義
        // ****************************************************************************************************

        /// <summary>
        /// 再読込を実行します。
        /// </summary>
        /// <returns></returns>
        public abstract Task Reload();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected async Task<string> GetStringAsync(string url)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                if (RequiredLogin)
                {
                    handler.CookieContainer = await SettingModel.Instance.GetCookies();
                }

                return await client.GetStringAsync(url);
            }
        }

        protected async Task<dynamic> GetJsonAsync(string url)
        {
            return JsonConverter.Parse(await GetStringAsync(url));
        }

        protected async Task<XElement> GetXmlAsync(string url)
        {
            return XDocument.Load(new StringReader(await GetStringAsync(url))).Root;
        }
    }
}

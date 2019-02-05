using NicoV4.Common;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
        public virtual ObservableSynchronizedCollection<string> Videos
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

                var txt = await client.GetStringAsync(url);
                
                //txt = txt.Replace("&copy;", "");
                //txt = txt.Replace("&nbsp;", " ");
                //txt = txt.Replace("&#x20;", " ");

                //txt = txt.Replace("&", "&amp;");

                return txt;
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

        ///// <summary>
        ///// Urlｴﾝｺｰﾄﾞ文字列から文字列に変換します。
        ///// </summary>
        ///// <param name="txt"></param>
        ///// <returns></returns>
        //private string FromUrlEncode(string txt)
        //{
        //    txt = HttpUtility.UrlDecode(txt);
        //    txt = txt.Replace("&lt;", "<");
        //    txt = txt.Replace("&gt;", ">");
        //    txt = txt.Replace("&quot;", "\"");
        //    txt = txt.Replace("&apos;", "'");
        //    txt = txt.Replace("&amp;", "&");
        //    txt = txt.Replace("&nbsp;", "\n");

        //    return txt;
        //}

        protected string UpdateVideoFromXml(XElement item, string view, string mylist, string comment)
        {
            string descriptionString;
            XElement desc;
            try
            {
                // 明細部をXDocumentで読み込むために整形
                descriptionString = item.Element("description").Value;
                descriptionString = descriptionString.Replace("&nbsp;", "&#x20;");
                //descriptionString = HttpUtility.HtmlDecode(descriptionString);
                //descriptionString = descriptionString.Replace("&", "&amp;");
                //descriptionString = descriptionString.Replace("'", "&apos;");

                // 明細部読み込み
                desc = XDocument.Load(new StringReader($"<root>{descriptionString}</root>")).Root;

                // 動画時間
                var lengthSecondsStr = (string)desc
                        .Descendants("strong")
                        .Where(x => (string)x.Attribute("class") == "nico-info-length")
                        .First();

                var video = VideoStatusModel.Instance.GetVideo(NicoDataConverter.ToId(item.Element("link").Value));

                video.Title = item.Element("title").Value;
                video.ViewCounter = NicoDataConverter.ToCounter(desc, view);
                video.MylistCounter = NicoDataConverter.ToCounter(desc, mylist);
                video.CommentCounter = NicoDataConverter.ToCounter(desc, comment);
                video.StartTime = NicoDataConverter.ToRankingDatetime(desc, "nico-info-date");
                video.ThumbnailUrl = (string)desc.Descendants("img").First().Attribute("src");
                video.LengthSeconds = NicoDataConverter.ToLengthSeconds(lengthSecondsStr);
                video.Description = (string)desc.Descendants("p").FirstOrDefault(x => (string)x.Attribute("class") == "nico-description");

                return video.VideoId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        protected XElement CreateDesc(XElement item)
        {
            var descriptionString = item.Element("description").Value;

            descriptionString = HttpUtility.HtmlDecode(descriptionString);
            descriptionString = descriptionString.Replace("&", "&amp;");
            descriptionString = descriptionString.Replace("'", "&apos;");
            return XDocument.Load(new StringReader($"<root>{descriptionString}</root>")).Root;
        }
    }
}

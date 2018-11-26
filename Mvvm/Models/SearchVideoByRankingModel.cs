using NicoV4.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WpfUtilV2.Mvvm.Service;

namespace NicoV4.Mvvm.Models
{
    public class SearchVideoByRankingModel : SearchVideoModel
    {
        public static SearchVideoByRankingModel Instance { get; private set; } = new SearchVideoByRankingModel();

        private SearchVideoByRankingModel()
        {

        }

        /// <summary>
        /// 対象
        /// </summary>
        public string Target
        {
            get { return _Target; }
            set { SetProperty(ref _Target, value); }
        }
        private string _Target = null;

        /// <summary>
        /// 期間
        /// </summary>
        public string Period
        {
            get { return _Period; }
            set { SetProperty(ref _Period, value); }
        }
        private string _Period = null;

        /// <summary>
        /// ｶﾃｺﾞﾘ
        /// </summary>
        public string Category
        {
            get { return _Category; }
            set { SetProperty(ref _Category, value); }
        }
        private string _Category = null;

        public override async Task Reload()
        {
            if (string.IsNullOrWhiteSpace(Target) || string.IsNullOrWhiteSpace(Period) || string.IsNullOrWhiteSpace(Category))
            {
                ServiceFactory.MessageService.Error("検索ワードが入力されていません。");
                return;
            }

            const string url = "http://www.nicovideo.jp/ranking/{0}/{1}/{2}?rss=2.0";

            var channel = (await GetXmlAsync(string.Format(url, Target, Period, Category)))
                    .Descendants("channel").First();

            Videos.Clear();

            foreach (var item in channel.Descendants("item"))
            {
                var desc = XDocument.Load(new StringReader("<root>" + item.Element("description").Value + "</root>")).Root;
                var lengthSecondsStr = (string)desc
                        .Descendants("strong")
                        .Where(x => (string)x.Attribute("class") == "nico-info-length")
                        .First();
                var video = VideoStatusModel.Instance.GetVideo(NicoDataConverter.ToId(item.Element("link").Value));

                video.Title = item.Element("title").Value;
                video.ViewCounter = NicoDataConverter.ToCounter(desc, "nico-info-total-view");
                video.MylistCounter = NicoDataConverter.ToCounter(desc, "nico-info-total-res");
                video.CommentCounter = NicoDataConverter.ToCounter(desc, "nico-info-total-mylist");
                video.StartTime = DateTime.Parse(channel.Element("pubDate").Value);
                video.ThumbnailUrl = (string)desc.Descendants("img").First().Attribute("src");
                video.LengthSeconds = NicoDataConverter.ToLengthSeconds(lengthSecondsStr);

                // 自身に追加
                Videos.Add(video.VideoId);
            }
        }
    }
}

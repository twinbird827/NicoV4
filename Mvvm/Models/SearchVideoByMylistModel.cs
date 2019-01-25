using NicoV4.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using WpfUtilV2.Common;

namespace NicoV4.Mvvm.Models
{
    public class SearchVideoByMylistModel : SearchVideoModel
    {
        public SearchVideoByMylistModel(string url) : this()
        {
            if (!WpfUtil.IsDesignMode())
            {
                MylistUrl = url;

                Reload().ConfigureAwait(false);
            }
        }

        public SearchVideoByMylistModel() : base(false)
        {

        }

        /// <summary>
        /// ﾏｲﾘｽﾄUrl
        /// </summary>
        public string MylistUrl
        {
            get
            {
                const string url = "http://www.nicovideo.jp/mylist/{0}?rss=2.0&numbers=1&sort={1}";

                return string.Format(url, MylistId, OrderBy);
            }
            set { MylistId = NicoDataConverter.ToId(value); }
        }

        /// <summary>
        /// ﾏｲﾘｽﾄId
        /// </summary>
        public string MylistId
        {
            get { return _MylistId; }
            set { SetProperty(ref _MylistId, value); OnPropertyChanged(nameof(MylistUrl)); }
        }
        private string _MylistId = null;

        /// <summary>
        /// ｿｰﾄ順
        /// </summary>
        public string OrderBy
        {
            get { return _OrderBy; }
            set { SetProperty(ref _OrderBy, value); }
        }
        private string _OrderBy = null;

        /// <summary>
        /// ﾀｲﾄﾙ
        /// </summary>
        public string MylistTitle
        {
            get { return _MylistTitle; }
            set { SetProperty(ref _MylistTitle, value); }
        }
        private string _MylistTitle = null;

        /// <summary>
        /// 作成者
        /// </summary>
        public string MylistCreator
        {
            get { return _MylistCreator; }
            set { SetProperty(ref _MylistCreator, value); }
        }
        private string _MylistCreator = null;

        /// <summary>
        /// ﾏｲﾘｽﾄ詳細
        /// </summary>
        public string MylistDescription
        {
            get { return _MylistDescription; }
            set { SetProperty(ref _MylistDescription, value); }
        }
        private string _MylistDescription = null;

        /// <summary>
        /// 作成者のID
        /// </summary>
        public string UserId
        {
            get { return _UserId; }
            set { SetProperty(ref _UserId, value); }
        }
        private string _UserId = null;

        /// <summary>
        /// 作成者のｻﾑﾈｲﾙUrl
        /// </summary>
        public string UserThumbnailUrl
        {
            get { return _UserThumbnailUrl; }
            set
            {
                if (SetProperty(ref _UserThumbnailUrl, value) && !string.IsNullOrWhiteSpace(value))
                {
                    NicoDataConverter.ToThumbnail(value)
                        .ContinueWith(
                            t => UserThumbnail = t.Result,
                            TaskScheduler.FromCurrentSynchronizationContext()
                    );
                }
            }
        }
        private string _UserThumbnailUrl = null;

        /// <summary>
        /// 作成者のｻﾑﾈｲﾙ
        /// </summary>
        public BitmapImage UserThumbnail
        {
            get { return _UserThumbnail; }
            set { SetProperty(ref _UserThumbnail, value); }
        }
        private BitmapImage _UserThumbnail;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime MylistDate
        {
            get { return _MylistDate; }
            set { SetProperty(ref _MylistDate, value); }
        }
        private DateTime _MylistDate = default(DateTime);

        public override async Task Reload()
        {
            var channel = (await GetXmlAsync(MylistUrl)).Descendants("channel").First();

            // ﾏｲﾘｽﾄ情報を本ｲﾝｽﾀﾝｽのﾌﾟﾛﾊﾟﾃｨに転記
            MylistTitle = channel.Element("title").Value;
            MylistCreator = channel.Element(XName.Get("creator", "http://purl.org/dc/elements/1.1/")).Value;
            MylistDate = DateTime.Parse(channel.Element("lastBuildDate").Value);
            MylistDescription = channel.Element("description").Value;

            UserId = await GetUserId();
            UserThumbnailUrl = await GetThumbnailUrl();

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
                video.ViewCounter = NicoDataConverter.ToCounter(desc, "nico-numbers-view");
                video.MylistCounter = NicoDataConverter.ToCounter(desc, "nico-numbers-mylist");
                video.CommentCounter = NicoDataConverter.ToCounter(desc, "nico-numbers-res");
                video.StartTime = NicoDataConverter.ToRankingDatetime(desc, "nico-info-date");
                video.ThumbnailUrl = (string)desc.Descendants("img").First().Attribute("src");
                video.LengthSeconds = NicoDataConverter.ToLengthSeconds(lengthSecondsStr);

                // idを追加
                Videos.Add(video.VideoId);
            }
        }

        /// <summary>
        /// ﾏｲﾘｽﾄからﾕｰｻﾞIDを取得します。
        /// </summary>
        /// <returns>ﾕｰｻﾞID</returns>
        private async Task<string> GetUserId()
        {
            const string url = "http://www.nicovideo.jp/mylist/{0}";

            var txt = await GetStringAsync(string.Format(url, MylistId));
            var id = Regex.Match(txt, "user_id: (?<user_id>[\\d]+)").Groups["user_id"].Value;
            return id;
        }

        /// <summary>
        /// ﾕｰｻﾞIDからｻﾑﾈｲﾙUrlを取得します。
        /// </summary>
        /// <returns>ｻﾑﾈｲﾙUrl</returns>
        private async Task<string> GetThumbnailUrl()
        {
            const string userIframe = "http://ext.nicovideo.jp/thumb_user/{0}";
            const string url = "https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/";

            var txt = await GetStringAsync(string.Format(userIframe, UserId));
            var thumbnail = Regex.Match(txt, url + "(?<url>[^\"]+)").Groups["url"].Value;
            return url + thumbnail;
        }

    }
}

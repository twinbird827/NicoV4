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
                video.Description = (string)desc.Descendants("p").FirstOrDefault(x => (string)x.Attribute("class") == "nico-description");

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
/*
<item>
  <title>【ゆっくり解説】世界の戦術・奇策・戦い紹介【アウステルリッツの戦い</title>
  <link>http://www.nicovideo.jp/watch/sm29712796</link>
  <guid isPermaLink="false">tag:nicovideo.jp,2016-09-25:/watch/1474729934</guid>
  <pubDate>Sun, 25 Sep 2016 07:58:54 +0900</pubDate>
  <description><![CDATA[<p class="nico-thumbnail"><img alt="【ゆっくり解説】世界の戦術・奇策・戦い紹介【アウステルリッツの戦い" src="https://tn.smilevideo.jp/smile?i=29712796.M" width="94" height="70" border="0"/></p><p class="nico-description">ザ☆新タレント登場！世界の様々な戦術や戦争をご紹介！今回もモデルになった人物がいますよ！栄えある1回目の戦いは『芸術』といわれたアウステルリッツ三帝会戦です。※動画内の肖像には年代よりイメージを優先させたものもありますマイリスト→mylist/57100985　次→sm29802171奇人・変人・偉人もどうぞ→sm29617623パート１マイリスト→mylist/55688483コミュあります→co2018467 副社長Twitter→@fukushachou9 絵：なのむら様よりBGM:煉獄庭園様より</p><p class="nico-info"><small><strong class="nico-info-length">9:53</strong>｜<strong class="nico-info-date">2016年09月25日 00：26：00</strong> 投稿</small></p><p class="nico-numbers"><small>再生：<strong class="nico-numbers-view">397,661</strong>  コメント：<strong class="nico-numbers-res">2,435</strong>  マイリスト：<strong class="nico-numbers-mylist">3,200</strong></small></p>]]></description>
</item>

    <item>
  <title>【ゆっくり解説】世界の戦術・奇策・戦い紹介【トラシメヌスの戦い】</title>
  <link>http://www.nicovideo.jp/watch/sm34377890</link>
  <guid isPermaLink="false">tag:nicovideo.jp,2018-12-26:/watch/1545802625</guid>
  <pubDate>Thu, 27 Dec 2018 00:29:17 +0900</pubDate>
  <description><![CDATA[<p class="nico-thumbnail"><img alt="【ゆっくり解説】世界の戦術・奇策・戦い紹介【トラシメヌスの戦い】" src="https://tn.smilevideo.jp/smile?i=34377890.95216.M" width="94" height="70" border="0"/></p><p class="nico-description">日曜１９時半からもっと詳しく解説生放送します→lv317597961今回は史上最大の待ち伏せ！ハンニバルの【トラシメヌス湖畔の戦い】でございますトレビアの戦い、カンナエの戦いはマイリスにあるので良かったらどうぞー！なんとyoutubeにパワーアップ版がアップしておりますよ！→https://youtu.be/zQJR0d9lu-o前→sm34119455　マイリスト→mylist/57100985　次→ しばしお待ちを世界の奇人変人sm34290569戦国武将紹介sm33200014料理動画sm32754859もうちょい詳しい解説したりします→co2018467キャラ絵：なのむら様より→@nanomuraBGM:魔王魂様より・DOVA-SYNDROME・甘茶の音楽工房画像：Jonas Buddeberg, Bernhard Jenny　　　The Cartography and Geovisualization Group at Oregon State Universityシリーズ制作者:副社長Twitter→@fukushachou9 　　(参考資料はこちら)</p><p class="nico-info"><small><strong class="nico-info-length">9:59</strong>｜<strong class="nico-info-date">2018年12月26日 20：00：00</strong> 投稿</small></p><p class="nico-numbers"><small>再生：<strong class="nico-numbers-view">85,463</strong>  コメント：<strong class="nico-numbers-res">1,522</strong>  マイリスト：<strong class="nico-numbers-mylist">853</strong></small></p>]]></description>
</item>
 * */

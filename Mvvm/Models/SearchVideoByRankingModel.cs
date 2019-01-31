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
                video.StartTime = NicoDataConverter.ToRankingDatetime(desc, "nico-info-date");
                video.ThumbnailUrl = (string)desc.Descendants("img").First().Attribute("src");
                video.LengthSeconds = NicoDataConverter.ToLengthSeconds(lengthSecondsStr);

                // 自身に追加
                Videos.Add(video.VideoId);
            }
        }
    }
}
/*
<item>
  <title>第1位：【女子2人】初めてパンの気持ちを理解する実況【I am Bread】</title>
  <link>http://www.nicovideo.jp/watch/sm34525974</link>
  <guid isPermaLink="false">tag:nicovideo.jp,2019-01-25:/watch/sm34525974</guid>
  <pubDate>Thu, 31 Jan 2019 07:06:01 +0900</pubDate>
  <description><![CDATA[
                      <p class="nico-thumbnail"><img alt="【女子2人】初めてパンの気持ちを理解する実況【I am Bread】" src="http://tn.smilevideo.jp/smile?i=34525974.59215" width="94" height="70" border="0"/></p>
                                <p class="nico-description">全パンの想いを背負いし者----------------------関西弁女子実況グループ『サイコロジカルサーカス』が、第13回実況者杯に参加します!フリー部門実況動画の部、謎部門にエントリー！再生数・コメント・マイリス数で順位が決まります！！応援よろよろ！( ˘ω˘ )ニコニ広告が可能です！よければ広告で宣伝もよろしくお願いします(強欲の壺)テーマは【初】この動画はフリー部門実況動画の部の動画です。 今回は映画風の始まりにしてみました！楽しんで見てもらえるとうれしい！(*^^*)今回のプログラム⇒mylist/63232841大会詳細⇒sm33431601パンフレット⇒mylist/55555016舞台袖⇒co3253598プレイメンバー…ノイジーワールド(紫)・馬面なおと(桃) Twitter…https://twitter.com/rojikaru2525</p>
                                <p class="nico-info"><small><strong class="nico-info-number">20,404</strong>pts.｜<strong class="nico-info-length">20:04</strong>｜<strong class="nico-info-date">2019年01月25日 18：11：01</strong> 投稿<br/><strong>合計</strong>  再生：<strong class="nico-info-total-view">729</strong>  コメント：<strong class="nico-info-total-res">59</strong>  マイリスト：<strong class="nico-info-total-mylist">9</strong><br/><strong>毎時</strong>  再生：<strong class="nico-info-hourly-view">4</strong>  コメント：<strong class="nico-info-hourly-res">0</strong>  マイリスト：<strong class="nico-info-hourly-mylist">0</strong><br/></small></p>
                  ]]></description>
</item>
 * */

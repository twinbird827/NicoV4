using NicoV4.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

namespace NicoV4.Mvvm.Models
{
    public class VideoModel : BindableBase
    {
        /// <summary>
        /// ｺﾝﾃﾝﾂId (http://nico.ms/ の後に連結することでコンテンツへのURLになります)
        /// </summary>
        public string VideoUrl
        {
            get { return "http://nico.ms/" + _VideoId; }
            set { VideoId = NicoDataConverter.ToId(value); }
        }

        /// <summary>
        /// 動画ID
        /// </summary>
        public string VideoId
        {
            get { return _VideoId; }
            set { SetProperty(ref _VideoId, value); OnPropertyChanged(nameof(VideoUrl)); }
        }
        private string _VideoId = null;

        /// <summary>
        /// ﾀｲﾄﾙ
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, HttpUtility.HtmlDecode(value)); }
        }
        private string _Title = null;

        /// <summary>
        /// ｺﾝﾃﾝﾂの説明文
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, HttpUtility.HtmlDecode(value)); }
        }
        private string _Description = null;

        /// <summary>
        /// ﾀｸﾞ (空白区切り)
        /// </summary>
        public string Tags
        {
            get { return _Tags; }
            set { SetProperty(ref _Tags, value); }
        }
        private string _Tags = null;

        /// <summary>
        /// ｶﾃｺﾞﾘﾀｸﾞ
        /// </summary>
        public string CategoryTag
        {
            get { return _CategoryTag; }
            set { SetProperty(ref _CategoryTag, value); }
        }
        private string _CategoryTag = null;

        /// <summary>
        /// 再生数
        /// </summary>
        public double ViewCounter
        {
            get { return _ViewCounter; }
            set { SetProperty(ref _ViewCounter, value); }
        }
        private double _ViewCounter = default(int);

        /// <summary>
        /// ﾏｲﾘｽﾄ数
        /// </summary>
        public double MylistCounter
        {
            get { return _MylistCounter; }
            set { SetProperty(ref _MylistCounter, value); }
        }
        private double _MylistCounter = default(int);

        /// <summary>
        /// ｺﾒﾝﾄ数
        /// </summary>
        public double CommentCounter
        {
            get { return _CommentCounter; }
            set { SetProperty(ref _CommentCounter, value); }
        }
        private double _CommentCounter = default(int);

        /// <summary>
        /// ｺﾝﾃﾝﾂの投稿時間
        /// </summary>
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { SetProperty(ref _StartTime, value); }
        }
        private DateTime _StartTime = default(DateTime);

        /// <summary>
        /// 最終ｺﾒﾝﾄ時間
        /// </summary>
        public DateTime LastCommentTime
        {
            get { return _LastCommentTime; }
            set { SetProperty(ref _LastCommentTime, value); }
        }
        private DateTime _LastCommentTime = default(DateTime);

        /// <summary>
        /// 再生時間 (秒)
        /// </summary>
        public double LengthSeconds
        {
            get { return _LengthSeconds; }
            set { SetProperty(ref _LengthSeconds, value); }
        }
        private double _LengthSeconds = default(double);

        /// <summary>
        /// ｻﾑﾈｲﾙUrl
        /// </summary>
        public string ThumbnailUrl
        {
            get { return _ThumbnailUrl; }
            set { SetProperty(ref _ThumbnailUrl, value); }
        }
        private string _ThumbnailUrl = null;

        /// <summary>
        /// ｻﾑﾈｲﾙ
        /// </summary>
        public BitmapImage Thumbnail
        {
            get
            {
                if (_Thumbnail == null && !string.IsNullOrEmpty(ThumbnailUrl))
                {
                    var urls = (new[] { ".L", ".M", "" }).Select(s => ThumbnailUrl + s);
                    NicoDataConverter.ToThumbnail(urls.ToArray())
                        .ContinueWith(
                            t => Thumbnail = t.Result,
                            TaskScheduler.FromCurrentSynchronizationContext()
                    );
                }
                return _Thumbnail;
            }
            set { SetProperty(ref _Thumbnail, value); }
        }
        private BitmapImage _Thumbnail;

        /// <summary>
        /// ｺﾐｭﾆﾃｨｱｲｺﾝのUrl
        /// </summary>
        public string CommunityIcon
        {
            get { return _CommunityIcon; }
            set { SetProperty(ref _CommunityIcon, value); }
        }
        private string _CommunityIcon = null;

        /// <summary>
        /// 最終更新時間
        /// </summary>
        public DateTime LastUpdateTime
        {
            get { return _LastUpdateTime; }
            set { SetProperty(ref _LastUpdateTime, value); }
        }
        private DateTime _LastUpdateTime = DateTime.Now;

        /// <summary>
        /// 最新ｺﾒﾝﾄ
        /// </summary>
        public string LastResBody
        {
            get { return _LastResBody; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _LastResBody, value);
                }
            }
        }
        private string _LastResBody = null;

        public async Task Open()
        {
            await Task.Delay(1).ConfigureAwait(false);

            Process.Start(SettingModel.Instance.Browser, VideoUrl);

            //MainWindowViewModel.Instance.Current = new VideoDetail2ViewModel(this);

            // SEEﾘｽﾄに追加
            VideoStatusModel.Instance.SeeVideos.Add(VideoId);

            // NEWﾘｽﾄから削除
            VideoStatusModel.Instance.NewVideos.Remove(VideoId);
        }

        public async Task Download(string path)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                client.Timeout = new TimeSpan(1, 0,0);

                // ﾛｸﾞｲﾝｸｯｷｰ設定
                handler.CookieContainer = await SettingModel.Instance.GetCookies();

                // 対象動画にｼﾞｬﾝﾌﾟ
                await client.PostAsync("http://www.nicovideo.jp/watch/" + VideoId, null);

                // 動画URL全文を取得
                var flvurl = await client.GetStringAsync("http://flapi.nicovideo.jp/api/getflv/" + VideoId);

                flvurl = Uri.UnescapeDataString(flvurl);
                flvurl = Regex.Match(flvurl, @"&url=.*").Value.Replace("&url=", "");
                var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://flapi.nicovideo.jp/api/getflv/" + VideoId));
                
                //var bytes = await client.GetByteArrayAsync(flvurl);
                //File.WriteAllBytes(path, bytes);

                ServiceFactory.MessageService.Debug(flvurl);

            }
        }
    }
}

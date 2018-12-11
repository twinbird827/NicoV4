using StatefulModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

namespace NicoV4.Mvvm.Models
{
    public class DownloadModel : BindableBase
    {
        /// <summary>
        /// ｼﾝｸﾞﾙﾄﾝｺﾝｽﾄﾗｸﾀ
        /// </summary>
        private DownloadModel()
        {
            // ﾀｲﾏｰ起動
            Timer = new DispatcherTimer(DispatcherPriority.Normal, App.Current.Dispatcher);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            Timer.Start();

        }

        public static DownloadModel Instance { get; set; } = new DownloadModel();

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞ待ちﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<VideoModel> Downloads
        {
            get { return _Downloads; }
            set { SetProperty(ref _Downloads, value); }
        }
        private SynchronizationContextCollection<VideoModel> _Downloads;

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞ中のﾃﾞｰﾀ
        /// </summary>
        public VideoModel Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value); }
        }
        private VideoModel _Current;

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞ中かどうか
        /// </summary>
        public bool IsDownloding
        {
            get { return _IsDownloding; }
            set { SetProperty(ref _IsDownloding, value); }
        }
        private bool _IsDownloding;

        /// <summary>
        /// 進捗状況を表すﾒｯｾｰｼﾞ
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }
        private string _Message;

        /// <summary>
        /// ﾀｲﾏｰ
        /// </summary>
        public DispatcherTimer Timer { get; set; }

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞ待ちﾘｽﾄに追加します。
        /// </summary>
        /// <param name="v">追加するﾃﾞｰﾀ</param>
        public void AddDownload(VideoModel v)
        {
            if (!Downloads.Any(d => d.VideoId == v.VideoId))
            {
                Downloads.Add(v);
            }
        }

        /// <summary>
        /// 処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (Current != null)
            {
                // ﾀﾞｳﾝﾛｰﾄﾞ中ﾃﾞｰﾀが存在する場合は中断
                return;
            }
            if (!Downloads.Any())
            {
                // ｽﾃｰﾀｽ完了状態に移行
                IsDownloding = false;
                Message = "";

                // ﾀﾞｳﾝﾛｰﾄﾞ待ちが存在しない場合は中断
                return;
            }

            // ﾀﾞｳﾝﾛｰﾄﾞ中に設定
            Current = Downloads.First();
            Downloads.Remove(Current);

            // ｽﾃｰﾀｽ更新
            IsDownloding = true;
            Message = string.Format("ID:{0} ﾀﾞｳﾝﾛｰﾄﾞ中 / {1} ﾌｧｲﾙ ﾀﾞｳﾝﾛｰﾄﾞ待ち ", Current.VideoId, Downloads.Count());

            // ﾀﾞｳﾝﾛｰﾄﾞ開始
            await Download(Current);
        }

        private async Task Download(VideoModel vm)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                client.Timeout = new TimeSpan(1, 0, 0);

                // ﾛｸﾞｲﾝｸｯｷｰ設定
                handler.CookieContainer = await SettingModel.Instance.GetCookies();

                // 対象動画にｼﾞｬﾝﾌﾟ
                await client.PostAsync("http://www.nicovideo.jp/watch/" + vm.VideoId, null);

                // 動画URL全文を取得
                var flvurl = await client.GetStringAsync("http://flapi.nicovideo.jp/api/getflv/" + vm.VideoId);

                flvurl = Uri.UnescapeDataString(flvurl);
                flvurl = Regex.Match(flvurl, @"&url=.*").Value.Replace("&url=", "");
                
                // TODO 逐次ﾀﾞｳﾝﾛｰﾄﾞの方法
                var bytes = await client.GetByteArrayAsync(flvurl);
                File.WriteAllBytes(CreateFilename(vm), bytes);
            }
        }

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞ先ﾊﾟｽを作成します。
        /// </summary>
        /// <param name="vm">VideoModel</param>
        /// <returns></returns>
        private string CreateFilename(VideoModel vm)
        {
            var dir = SettingModel.Instance.DownloadDirectory;

            switch (SettingModel.Instance.DownloadFileName)
            {
                case DownloadFileName.ID:
                    return Path.Combine(dir, vm.VideoId + ".mp4");
                case DownloadFileName.Title:
                    return Path.Combine(dir, vm.Title + ".mp4");
                case DownloadFileName.TitleAndID:
                    return Path.Combine(dir, vm.VideoId + "_" + vm.Title + ".mp4");
                default:
                    return Path.Combine(dir, vm.VideoId + ".mp4");
            }
        }

    }
}

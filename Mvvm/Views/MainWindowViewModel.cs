using MahApps.Metro.Controls.Dialogs;
using NicoV4.Common;
using NicoV4.Mvvm.Models;
using NicoV4.Mvvm.Views.Services;
using NicoV4.Mvvm.Views.WorkSpace;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

namespace NicoV4.Mvvm.Views
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// ｺﾝｽﾄﾗｸﾀ
        /// </summary>
        public MainWindowViewModel() : base()
        {
            if (!WpfUtil.IsDesignMode() && Instance != null)
            {
                throw new InvalidOperationException("本ViewModelは複数のｲﾝｽﾀﾝｽを作成することができません。");
            }
            Instance = this;

            Disposed += (sender, e) =>
            {
                VideoStatusModel.Instance.Dispose();
                MylistStatusModel.Instance.Dispose();
                SettingModel.Instance.Dispose();
            };

            // 初期表示ﾜｰｸｽﾍﾟｰｽの設定
            if (!string.IsNullOrWhiteSpace(SettingModel.Instance.MailAddress))
            {
                Current = new SearchVideoByRankingViewModel();
            }
            else
            {
                Current = new SettingViewModel();
            }

            // ﾒｯｾｰｼﾞｻｰﾋﾞｽの変更
            ServiceFactory.MessageService = new WpfMessageService();

            // ﾃﾝﾎﾟﾗﾘ数の監視ｲﾍﾞﾝﾄを追加
            SearchVideoByTemporaryModel.Instance.Videos.CollectionChanged += SearchVideoByTemporaryModel_OnCollectionChanged;

            // ﾀﾞｳﾝﾛｰﾄﾞ状況の監視ｲﾍﾞﾝﾄを追加
            DownloadModel.Instance.PropertyChanged += Download_OnPropertyChanged;

            // ﾀｲﾏｰ起動
            Timer = new DispatcherTimer(DispatcherPriority.Normal, App.Current.Dispatcher);
            Timer.Interval = new TimeSpan(0, 10, 0);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        // ****************************************************************************************************
        // ﾌﾟﾛﾊﾟﾃｨ定義
        // ****************************************************************************************************

        /// <summary>
        /// 本ｲﾝｽﾀﾝｽ(ｼﾝｸﾞﾙﾄﾝ)
        /// </summary>
        public static MainWindowViewModel Instance { get; private set; }

        /// <summary>
        /// ﾀﾞｲｱﾛｸﾞ表示用ｲﾝｽﾀﾝｽ
        /// </summary>
        public IDialogCoordinator DialogCoordinator { get; set; }

        /// <summary>
        /// ｶﾚﾝﾄﾜｰｸｽﾍﾟｰｽ
        /// </summary>
        public WorkSpaceViewModel Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value, true); }
        }
        private WorkSpaceViewModel _Current;

        /// <summary>
        /// ﾃﾝﾎﾟﾗﾘ数
        /// </summary>
        public int TemporaryCount
        {
            get { return _TemporaryCount; }
            set { SetProperty(ref _TemporaryCount, value); }
        }
        private int _TemporaryCount;

        /// <summary>
        /// ﾃﾝﾎﾟﾗﾘに追加した
        /// </summary>
        public bool TemporaryNewVideo
        {
            get { return _TemporaryNewVideo; }
            set { SetProperty(ref _TemporaryNewVideo, value); }
        }
        private bool _TemporaryNewVideo;

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

        // ****************************************************************************************************
        // ｺﾏﾝﾄﾞ定義
        // ****************************************************************************************************

        /// <summary>
        /// ﾒﾆｭｰ処理
        /// </summary>
        public ICommand OnClickMenu
        {
            get
            {
                return _OnClickMenu = _OnClickMenu ?? new RelayCommand<MenuItemType>(
                    t =>
                    {
                        switch (t)
                        {
                            case MenuItemType.MylistOfMe:
                                Current = new SearchVideoByRankingViewModel();
                                break;
                            case MenuItemType.MylistOfOther:
                                Current = new SearchMylistViewModel();
                                break;
                            case MenuItemType.Ranking:
                                Current = new SearchVideoByRankingViewModel();
                                break;
                            case MenuItemType.SearchByMylist:
                                Current = new SearchVideoByMylistViewModel();
                                break;
                            case MenuItemType.SearchByWord:
                                Current = new SearchVideoByWordViewModel();
                                break;
                            case MenuItemType.Setting:
                                Current = new SettingViewModel();
                                break;
                            case MenuItemType.Temporary:
                                Current = new SearchVideoByTemporaryViewModel();
                                break;
                        }
                    });
            }
        }
        public ICommand _OnClickMenu;


        // ****************************************************************************************************
        // ﾒｿｯﾄﾞ定義
        // ****************************************************************************************************

        /// <summary>
        /// ﾒｯｾｰｼﾞﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="title">ﾀｲﾄﾙ</param>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="style">ﾀﾞｲｱﾛｸﾞｽﾀｲﾙ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>MessageDialogResult</code></returns>
        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await DialogCoordinator.ShowMessageAsync(this, title, message, style, settings);
        }

        /// <summary>
        /// 入力ﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="title">ﾀｲﾄﾙ</param>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="settings">設定情報</param>
        /// <returns>入力値</returns>
        public async Task<string> ShowInputAsync(string title, string message, MetroDialogSettings settings = null)
        {
            return await DialogCoordinator.ShowInputAsync(this, title, message, settings);
        }

        /// <summary>
        /// ｶｽﾀﾑﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="dialog">ｶｽﾀﾑﾀﾞｲｱﾛｸﾞのｲﾝｽﾀﾝｽ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>Task</code></returns>
        public Task ShowMetroDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return DialogCoordinator.ShowMetroDialogAsync(this, dialog, settings);
        }

        /// <summary>
        /// ｶｽﾀﾑﾀﾞｲｱﾛｸﾞを非表示にします。
        /// </summary>
        /// <param name="dialog">ｶｽﾀﾑﾀﾞｲｱﾛｸﾞのｲﾝｽﾀﾝｽ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>Task</code></returns>
        public Task HideMetroDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return DialogCoordinator.HideMetroDialogAsync(this, dialog, settings);
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            var preload = await Task.WhenAll(MylistStatusModel.Instance.Favorites
                .Select(async favorite =>
                {
                    var mylist = await MylistStatusModel.Instance.GetMylist(favorite.Mylist);

                    var videos = mylist.Videos
                        .Select(v => VideoStatusModel.Instance.GetVideo(v))
                        .OrderBy(v => v.StartTime);

                    foreach (var video in videos)
                    {
                        if (favorite.LastConfirmDatetime < video.StartTime &&
                            !SearchVideoByTemporaryModel.Instance.Videos.Any(s => s == video.VideoId))
                        {
                            favorite.LastConfirmDatetime = video.StartTime;
                            await SearchVideoByTemporaryModel.Instance.AddVideo(video.VideoId);
                            TemporaryNewVideo = true;
                        }
                    }

                    return favorite;
                }));


            //var preload = await Task.WhenAll(
            //    MylistStatusModel.Instance.Favorites
            //    .Select(favorite => new
            //    {
            //        Mylist = MylistStatusModel.Instance.GetMylist(favorite.Mylist),
            //        Favorite = favorite
            //    })
            //    .Select(async mylist =>
            //    {
            //        await mylist.Mylist.Reload();
            //        return new
            //        {
            //            Videos = mylist.Mylist.Videos,
            //            Favorite = mylist.Favorite
            //        };
            //    })
            //);

            //var videos = preload
            //    .SelectMany(video => video)
            //    .Select(video => VideoStatusModel.Instance.GetVideo(video))
            //    .Where(video => datetime < video.StartTime)
            //    .Select(video => video.VideoId)
            //    .Where(id => !SearchVideoByTemporaryModel.Instance.Videos.Any(s => s == id));

            //// ﾋﾞﾃﾞｵを追加
            //await Task.WhenAll(
            //    videos.Select(async id =>
            //    {
            //        VideoStatusModel.Instance.NewVideos.Add(id);
            //        await SearchVideoByTemporaryModel.Instance.AddVideo(id);
            //    })
            //);

            //// ﾃﾝﾎﾟﾗﾘに追加した
            //TemporaryNewVideo = videos.Any();

            ////foreach (var id in videos)
            ////{
            ////    // ﾋﾞﾃﾞｵを追加
            ////    await SearchVideoByTemporaryModel.Instance.AddVideo(id);
            ////}

            //// 確認日時更新
            //SettingModel.Instance.LastConfirmDatetime = DateTime.Now;
        }

        private void SearchVideoByTemporaryModel_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TemporaryCount = SearchVideoByTemporaryModel.Instance.Videos.Count();
        }

        protected virtual void Download_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Message):
                    Message = DownloadModel.Instance.Message;
                    break;
                case nameof(IsDownloding):
                    IsDownloding = DownloadModel.Instance.IsDownloding;
                    break;
            }
        }


    }
}

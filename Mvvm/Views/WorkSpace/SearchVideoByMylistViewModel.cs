using NicoV4.Mvvm.ComboItems;
using NicoV4.Mvvm.Models;
using NicoV4.Mvvm.Views.ItemsControl;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Views.WorkSpace
{
    public class SearchVideoByMylistViewModel : WorkSpaceViewModel
    {
        public SearchVideoByMylistViewModel() : this(new SearchVideoByMylistModel())
        {

        }

        public SearchVideoByMylistViewModel(SearchVideoByMylistModel model) : base(model)
        {
            Source = model;
            Source.AddOnPropertyChanged(this, OnPropertyChanged);

            this.Word = Source.MylistId;
            this.MylistTitle = Source.MylistTitle;
            this.MylistCreator = Source.MylistCreator;
            this.MylistDescription = Source.MylistDescription;
            this.UserId = Source.UserId;
            this.UserThumbnail = Source.UserThumbnail;
            this.MylistDate = Source.MylistDate;

            Items = Source.Videos.ToSyncedSynchronizationContextCollection(
                id => new VideoItemViewModel(id),
                AnonymousSynchronizationContext.Current
            );

            SortItems = ComboSortMylistModel
                .Instance
                .Items
                .ToSyncedSynchronizationContextCollection(m => m, AnonymousSynchronizationContext.Current);
            SelectedSortItem = SettingModel.Instance.SearchVideoByMylistSort;

            OnSearch.Execute(null);
        }

        public SearchVideoByMylistModel Source { get; private set; }

        /// <summary>
        /// ﾒｲﾝ項目ﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<VideoItemViewModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private SynchronizationContextCollection<VideoItemViewModel> _Items;

        /// <summary>
        /// ｿｰﾄﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<ComboboxItemModel> SortItems
        {
            get { return _SortItems; }
            set { SetProperty(ref _SortItems, value); }
        }
        private SynchronizationContextCollection<ComboboxItemModel> _SortItems;

        /// <summary>
        /// 選択中のｿｰﾄ項目
        /// </summary>
        public ComboboxItemModel SelectedSortItem
        {
            get { return _SelectedSortItem; }
            set { if (SetProperty(ref _SelectedSortItem, value)) SettingModel.Instance.SearchVideoByMylistSort = value; }
        }
        private ComboboxItemModel _SelectedSortItem;

        /// <summary>
        /// 検索ﾜｰﾄﾞ
        /// </summary>
        public string Word
        {
            get { return _Word; }
            set { SetProperty(ref _Word, value); }
        }
        private string _Word = null;

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
            set { SetProperty(ref _UserThumbnailUrl, value); }
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
        private BitmapImage _UserThumbnail = null;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime MylistDate
        {
            get { return _MylistDate; }
            set { SetProperty(ref _MylistDate, value); }
        }
        private DateTime _MylistDate = default(DateTime);

        /// <summary>
        /// 作成者情報を表示するか
        /// </summary>
        public bool IsCreatorVisible
        {
            get { return _IsCreatorVisible; }
            set { SetProperty(ref _IsCreatorVisible, value); }
        }
        private bool _IsCreatorVisible = default(bool);

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MylistTitle):
                    this.MylistTitle = Source.MylistTitle;
                    break;
                case nameof(MylistCreator):
                    this.MylistCreator = Source.MylistCreator;
                    break;
                case nameof(MylistDescription):
                    this.MylistDescription = Source.MylistDescription;
                    break;
                case nameof(UserId):
                    this.UserId = Source.UserId;
                    break;
                case nameof(UserThumbnail):
                    this.UserThumbnail = Source.UserThumbnail;
                    break;
                case nameof(MylistDate):
                    this.MylistDate = Source.MylistDate;
                    break;
            }
        }

        /// <summary>
        /// 検索処理
        /// </summary>
        public ICommand OnSearch
        {
            get
            {
                return _OnSearch = _OnSearch ?? new RelayCommand(
                async _ =>
                {
                    // 入力値をﾓﾃﾞﾙにｾｯﾄ
                    Source.MylistUrl = this.Word;
                    Source.OrderBy = this.SelectedSortItem.Value;

                    // 検索実行
                    await this.Source.Reload();

                    this.MylistTitle = Source.MylistTitle;
                    this.MylistCreator = Source.MylistCreator;
                    this.MylistDescription = Source.MylistDescription;
                    this.UserId = Source.UserId;
                    this.UserThumbnail = Source.UserThumbnail;
                    this.MylistDate = Source.MylistDate;

                    // ｵｰﾅｰ情報を表示するかどうか
                    this.IsCreatorVisible = Source.Videos.Any();
                },
                _ => {
                    return !string.IsNullOrWhiteSpace(Word);
                });
            }
        }
        public ICommand _OnSearch;

        /// <summary>
        /// 追加処理
        /// </summary>
        public ICommand OnAdd
        {
            get
            {
                return _OnAdd = _OnAdd ?? new RelayCommand(
                _ =>
                {
                    MylistStatusModel.Instance.AddFavorites(Source.MylistId);
                },
                _ => {
                    return IsCreatorVisible;
                });
            }
        }
        public ICommand _OnAdd;

    }
}

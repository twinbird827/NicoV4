using NicoV4.Mvvm.ComboItems;
using NicoV4.Mvvm.Models;
using NicoV4.Mvvm.Views.ItemsControl;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Views.WorkSpace
{
    public class SearchVideoByWordViewModel : WorkSpaceViewModel
    {
        public SearchVideoByWordViewModel() : this(SearchVideoByWordModel.Instance)
        {

        }

        public SearchVideoByWordViewModel(SearchVideoByWordModel model)
        {
            Source = model;

            Items = Source.Videos.ToSyncedSynchronizationContextCollection(
                id => new VideoItemViewModel(id),
                AnonymousSynchronizationContext.Current
            );

            SortItems = ComboSortVideoModel
                .Instance
                .Items
                .ToSyncedSynchronizationContextCollection(m => m, AnonymousSynchronizationContext.Current);
            SelectedSortItem = SortItems.First();
        }

        public SearchVideoByWordModel Source { get; private set; }

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
            set { SetProperty(ref _SelectedSortItem, value); }
        }
        private ComboboxItemModel _SelectedSortItem;

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
        /// 検索ﾜｰﾄﾞ
        /// </summary>
        public string Word
        {
            get { return _Word; }
            set { SetProperty(ref _Word, value); }
        }
        private string _Word = null;

        /// <summary>
        /// ﾀｸﾞ検索 or ｷｰﾜｰﾄﾞ検索
        /// </summary>
        public bool IsTag
        {
            get { return _IsTag; }
            set { SetProperty(ref _IsTag, value); }
        }
        private bool _IsTag = true;

        /// <summary>
        /// 現在のﾍﾟｰｼﾞ位置
        /// </summary>
        public int Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value); }
        }
        private int _Current = 1;

        /// <summary>
        /// ｵﾌｾｯﾄ (取得する開始位置)
        /// </summary>
        public int Offset
        {
            get { return _Offset; }
            set { SetProperty(ref _Offset, value); }
        }
        private int _Offset = 0;

        /// <summary>
        /// ﾃﾞｰﾀ件数
        /// </summary>
        public double DataLength
        {
            get { return _DataLength; }
            set { SetProperty(ref _DataLength, value); }
        }
        private double _DataLength = 0;

        /// <summary>
        /// 検索処理
        /// </summary>
        public ICommand OnSearch
        {
            get
            {
                return _OnSearch = _OnSearch ?? new RelayCommand<bool>(
                async b =>
                {
                    // 現在位置をﾘｾｯﾄ
                    this.IsTag = b;

                    // 入力値をﾓﾃﾞﾙにｾｯﾄ
                    Source.Word = this.Word;
                    Source.Offset = 0;
                    Source.IsTag = this.IsTag;
                    Source.OrderBy = this.SelectedSortItem.Value;

                    // 検索実行
                    await this.Source.Reload();

                    // ﾃﾞｰﾀ件数をVMにｾｯﾄ
                    this.Offset = 0;
                    this.DataLength = Source.DataLength;

                },
                b => {
                    return !string.IsNullOrWhiteSpace(Word);
                });
            }
        }
        public ICommand _OnSearch;

        /// <summary>
        /// ﾍﾟｰｼﾞｬ変更処理
        /// </summary>
        public ICommand OnCurrentChanged
        {
            get
            {
                return _OnCurrentChanged = _OnCurrentChanged ?? new RelayCommand(
                async _ =>
                {
                    if (Source.Offset != this.Offset)
                    {
                        // ｵﾌｾｯﾄを更新
                        Source.Offset = this.Offset;

                        // 検索実行
                        await this.Source.Reload();
                    }
                },
                _ => {
                    return !string.IsNullOrWhiteSpace(Word);
                });
            }
        }
        public ICommand _OnCurrentChanged;

    }
}

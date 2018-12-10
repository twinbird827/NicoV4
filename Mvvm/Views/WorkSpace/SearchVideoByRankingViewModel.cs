using NicoV4.Common;
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
    public class SearchVideoByRankingViewModel : WorkSpaceViewModel
    {
        public SearchVideoByRankingViewModel() : this(SearchVideoByRankingModel.Instance)
        {

        }

        public SearchVideoByRankingViewModel(SearchVideoByRankingModel model) : base(model)
        {
            Source = model;

            Items = Source.Videos.ToSyncedSynchronizationContextCollection(
                id => new VideoItemViewModel(id),
                AnonymousSynchronizationContext.Current
            );

            ComboTargetItems = ComboRankTargetModel
                .Instance
                .Items
                .ToSyncedSynchronizationContextCollection(m => m, AnonymousSynchronizationContext.Current);

            ComboPeriodItems = ComboRankPeriodModel
                .Instance
                .Items
                .ToSyncedSynchronizationContextCollection(m => m, AnonymousSynchronizationContext.Current);

            ComboCategoryItems = ComboRankCategoryModel
                .Instance
                .Items
                .ToSyncedSynchronizationContextCollection(m => m, AnonymousSynchronizationContext.Current);

            SelectedComboTargetItem = SettingModel.Instance.SearchVideoByRankingTarget;
            SelectedComboPeriodItem = SettingModel.Instance.SearchVideoByRankingPeriod;
            SelectedComboCategoryItem = SettingModel.Instance.SearchVideoByRankingCategory;

            OnSearch.Execute(null);
        }

        public SearchVideoByRankingModel Source { get; private set; }

        /// <summary>
        /// 対象ﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<ComboboxItemModel> ComboTargetItems
        {
            get { return _ComboTargetItems; }
            set { SetProperty(ref _ComboTargetItems, value); }
        }
        private SynchronizationContextCollection<ComboboxItemModel> _ComboTargetItems;

        /// <summary>
        /// 選択中の対象項目
        /// </summary>
        public ComboboxItemModel SelectedComboTargetItem
        {
            get { return _SelectedComboTargetItem; }
            set { if (SetProperty(ref _SelectedComboTargetItem, value)) SettingModel.Instance.SearchVideoByRankingTarget = value; }
        }
        private ComboboxItemModel _SelectedComboTargetItem;

        /// <summary>
        /// 期間ﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<ComboboxItemModel> ComboPeriodItems
        {
            get { return _ComboPeriodItems; }
            set { SetProperty(ref _ComboPeriodItems, value); }
        }
        private SynchronizationContextCollection<ComboboxItemModel> _ComboPeriodItems;

        /// <summary>
        /// 選択中の期間項目
        /// </summary>
        public ComboboxItemModel SelectedComboPeriodItem
        {
            get { return _SelectedComboPeriodItem; }
            set { if (SetProperty(ref _SelectedComboPeriodItem, value)) SettingModel.Instance.SearchVideoByRankingPeriod = value; }
        }
        private ComboboxItemModel _SelectedComboPeriodItem;

        /// <summary>
        /// ｶﾃｺﾞﾘﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<ComboboxItemModel> ComboCategoryItems
        {
            get { return _ComboCategoryItems; }
            set { SetProperty(ref _ComboCategoryItems, value); }
        }
        private SynchronizationContextCollection<ComboboxItemModel> _ComboCategoryItems;

        /// <summary>
        /// 選択中のｶﾃｺﾞﾘ項目
        /// </summary>
        public ComboboxItemModel SelectedComboCategoryItem
        {
            get { return _SelectedComboCategoryItem; }
            set { if (SetProperty(ref _SelectedComboCategoryItem, value)) SettingModel.Instance.SearchVideoByRankingCategory = value; }
        }
        private ComboboxItemModel _SelectedComboCategoryItem;

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
        /// 検索処理
        /// </summary>
        public ICommand OnSearch
        {
            get
            {
                return _OnSearch = _OnSearch ?? new RelayCommand(
                async _ =>
                {
                    OnPropertyChanged(nameof(SelectedComboTargetItem));
                    OnPropertyChanged(nameof(SelectedComboPeriodItem));
                    OnPropertyChanged(nameof(SelectedComboCategoryItem));

                    // 入力値をﾓﾃﾞﾙにｾｯﾄ
                    Source.Target = this.SelectedComboTargetItem?.Value;
                    Source.Period = this.SelectedComboPeriodItem?.Value;
                    Source.Category = this.SelectedComboCategoryItem?.Value;

                    // 検索実行
                    await this.Source.Reload();
                });
            }
        }
        public ICommand _OnSearch;

    }
}

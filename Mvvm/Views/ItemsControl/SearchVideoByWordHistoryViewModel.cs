using NicoV4.Mvvm.ComboItems;
using NicoV4.Mvvm.Models;
using NicoV4.Mvvm.Views.WorkSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.CustomControls;

namespace NicoV4.Mvvm.Views.ItemsControl
{
    public class SearchVideoByWordHistoryViewModel : BindableBase, ISelectableItem
    {
        public SearchVideoByWordHistoryViewModel(SearchVideoByWordViewModel parent, SearchVideoByWordHistoryModel source)
        {
            Parent = parent;
            Source = source;

            Word = Source.Word;
            IsTag = Source.IsTag;
            OrderBy = ComboSortVideoModel.Instance.Items.First(cim => cim.Value == Source.OrderBy);

        }
        public bool IsSelected { get; set; }

        public SearchVideoByWordHistoryModel Source { get; set; }

        public SearchVideoByWordViewModel Parent { get; set; }

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
        /// ﾀｸﾞ検索 or ｷｰﾜｰﾄﾞ検索
        /// </summary>
        public bool IsWord
        {
            get { return !_IsTag; }
        }

        /// <summary>
        /// ｿｰﾄ順
        /// </summary>
        public ComboboxItemModel OrderBy
        {
            get { return _OrderBy; }
            set { SetProperty(ref _OrderBy, value); }
        }
        private ComboboxItemModel _OrderBy = null;

        /// <summary>
        /// 項目ﾀﾞﾌﾞﾙｸﾘｯｸ時ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnDoubleClick
        {
            get
            {
                return _OnDoubleClick = _OnDoubleClick ?? new RelayCommand(
                _ =>
                {
                    Parent.Word = Word;
                    Parent.SelectedSortItem = OrderBy;
                    Parent.OnSearch.Execute(IsTag);
                },
                _ =>
                {
                    return true;
                });
            }
        }
        public ICommand _OnDoubleClick;

    }
}

using NicoV4.Properties;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.ComboItems
{
    public class ComboSortMylistModel : BindableBase
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboSortMylistModel Instance { get; } = new ComboSortMylistModel();

        /// <summary>
        /// ｿｰﾄﾘｽﾄ構成
        /// </summary>
        public ObservableSynchronizedCollection<ComboboxItemModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableSynchronizedCollection<ComboboxItemModel> _Items;

        private ComboSortMylistModel()
        {
            _Items = new ObservableSynchronizedCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "0", Description = Resources.SORT_MYREG0 },
                new ComboboxItemModel() { Value = "1", Description = Resources.SORT_MYREG1 },
                new ComboboxItemModel() { Value = "2", Description = Resources.SORT_MYCOMMENT0 },
                new ComboboxItemModel() { Value = "3", Description = Resources.SORT_MYCOMMENT1 },
                new ComboboxItemModel() { Value = "4", Description = Resources.SORT_TITLE0 },
                new ComboboxItemModel() { Value = "5", Description = Resources.SORT_TITLE1 },
                new ComboboxItemModel() { Value = "6", Description = Resources.SORT_UPLOAD0 },
                new ComboboxItemModel() { Value = "7", Description = Resources.SORT_UPLOAD1 },
                new ComboboxItemModel() { Value = "8", Description = Resources.SORT_VIEWRES0 },
                new ComboboxItemModel() { Value = "9", Description = Resources.SORT_VIEWRES1 },
                new ComboboxItemModel() { Value = "10", Description = Resources.SORT_COMMENT0 },
                new ComboboxItemModel() { Value = "11", Description = Resources.SORT_COMMENT1 },
                new ComboboxItemModel() { Value = "12", Description = Resources.SORT_COMMENTRES0 },
                new ComboboxItemModel() { Value = "13", Description = Resources.SORT_COMMENTRES1 },
                new ComboboxItemModel() { Value = "14", Description = Resources.SORT_MYREGRES0 },
                new ComboboxItemModel() { Value = "15", Description = Resources.SORT_MYREGRES1 },
                new ComboboxItemModel() { Value = "16", Description = Resources.SORT_LENGTH0 },
                new ComboboxItemModel() { Value = "17", Description = Resources.SORT_LENGTH1 },
            };
        }
    }
}

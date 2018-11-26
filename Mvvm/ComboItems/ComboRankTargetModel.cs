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
    public class ComboRankTargetModel : BindableBase
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboRankTargetModel Instance { get; } = new ComboRankTargetModel();

        /// <summary>
        /// ｿｰﾄﾘｽﾄ構成
        /// </summary>
        public ObservableSynchronizedCollection<ComboboxItemModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableSynchronizedCollection<ComboboxItemModel> _Items;

        private ComboRankTargetModel()
        {
            _Items = new ObservableSynchronizedCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "fav", Description = Resources.RANK_TARGET_FAV },
                new ComboboxItemModel() { Value = "view", Description = Resources.RANK_TARGET_VIEW },
                new ComboboxItemModel() { Value = "res", Description = Resources.RANK_TARGET_RES },
                new ComboboxItemModel() { Value = "mylist", Description = Resources.RANK_TARGET_MYLIST },
            };
        }
    }
}

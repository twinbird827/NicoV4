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
    public class ComboRankPeriodModel : BindableBase
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboRankPeriodModel Instance { get; } = new ComboRankPeriodModel();

        /// <summary>
        /// ｿｰﾄﾘｽﾄ構成
        /// </summary>
        public ObservableSynchronizedCollection<ComboboxItemModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableSynchronizedCollection<ComboboxItemModel> _Items;

        private ComboRankPeriodModel()
        {
            _Items = new ObservableSynchronizedCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "hourly", Description = Resources.RANK_PERIOD_HOURLY },
                new ComboboxItemModel() { Value = "daily", Description = Resources.RANK_PERIOD_DAILY },
                new ComboboxItemModel() { Value = "weekly", Description = Resources.RANK_PERIOD_WEEKLY },
                new ComboboxItemModel() { Value = "monthly", Description = Resources.RANK_PERIOD_MONTHLY },
                new ComboboxItemModel() { Value = "total", Description = Resources.RANK_PERIOD_TOTAL },
            };
        }
    }
}

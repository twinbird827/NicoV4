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
    public class ComboThumbSizeModel : BindableBase
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboThumbSizeModel Instance { get; } = new ComboThumbSizeModel();

        /// <summary>
        /// ｿｰﾄﾘｽﾄ構成
        /// </summary>
        public ObservableSynchronizedCollection<ComboboxItemModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableSynchronizedCollection<ComboboxItemModel> _Items;

        private ComboThumbSizeModel()
        {
            _Items = new ObservableSynchronizedCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "", Description = Resources.THUMB_SIZE_S },
                new ComboboxItemModel() { Value = ".M", Description = Resources.THUMB_SIZE_M },
                new ComboboxItemModel() { Value = ".L", Description = Resources.THUMB_SIZE_L },
            };
        }
    }
}

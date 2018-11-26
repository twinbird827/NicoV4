using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.ComboItems
{
    public class ComboboxItemModel : BindableBase
    {
        /// <summary>
        /// 実際の値
        /// </summary>
        public string Value
        {
            get { return _Value; }
            set { SetProperty(ref _Value, value); }
        }
        private string _Value = null;

        /// <summary>
        /// 画面に表示する説明
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, value); }
        }
        private string _Description = null;
    }
}

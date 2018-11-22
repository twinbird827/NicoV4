using MahApps.Metro.Controls.Dialogs;
using NicoV4.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV4.Mvvm.Views.Main
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

            // TODO 初期表示ﾜｰｸｽﾍﾟｰｽの設定
            Current = null;


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
            set { SetProperty(ref _Current, value); }
        }
        private WorkSpaceViewModel _Current;

        // ****************************************************************************************************
        // ｺﾏﾝﾄﾞ定義
        // ****************************************************************************************************


    }
}

using MahApps.Metro.Controls.Dialogs;
using NicoV4.Common;
using NicoV4.Mvvm.Views.WorkSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV4.Mvvm.Views
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

        /// <summary>
        /// ﾒｯｾｰｼﾞﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="title">ﾀｲﾄﾙ</param>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="style">ﾀﾞｲｱﾛｸﾞｽﾀｲﾙ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>MessageDialogResult</code></returns>
        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await DialogCoordinator.ShowMessageAsync(this, title, message, style, settings);
        }

        /// <summary>
        /// 入力ﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="title">ﾀｲﾄﾙ</param>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="settings">設定情報</param>
        /// <returns>入力値</returns>
        public async Task<string> ShowInputAsync(string title, string message, MetroDialogSettings settings = null)
        {
            return await DialogCoordinator.ShowInputAsync(this, title, message, settings);
        }

        /// <summary>
        /// ｶｽﾀﾑﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="dialog">ｶｽﾀﾑﾀﾞｲｱﾛｸﾞのｲﾝｽﾀﾝｽ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>Task</code></returns>
        public Task ShowMetroDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return DialogCoordinator.ShowMetroDialogAsync(this, dialog, settings);
        }

        /// <summary>
        /// ｶｽﾀﾑﾀﾞｲｱﾛｸﾞを非表示にします。
        /// </summary>
        /// <param name="dialog">ｶｽﾀﾑﾀﾞｲｱﾛｸﾞのｲﾝｽﾀﾝｽ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>Task</code></returns>
        public Task HideMetroDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return DialogCoordinator.HideMetroDialogAsync(this, dialog, settings);
        }

    }
}

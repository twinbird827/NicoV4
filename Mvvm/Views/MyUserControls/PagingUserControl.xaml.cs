using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Views.MyUserControls
{
    /// <summary>
    /// PagingUserControl.xaml の相互作用ロジック
    /// </summary>
    /// <remarks>
    /// ・TODOリスト
    /// Current：表示位置（ページ数換算）
    /// Offset：表示位置（データ数）
    /// Limit：1ページに表示する件数
    /// PageLength：ページングボタンの数
    /// DataLength：データ件数
    /// 
    /// ・発生すべきイベント
    /// Current.Changed - Current位置によってデザイン再描写
    /// Initialize - Current=1として再処理 - Limit or PageLength or DataLength が変更したときに実行する (すべての変数>0であること)
    /// 
    /// </remarks>
    public partial class PagingUserControl : UserControl
    {
        #region Properties

        /// <summary>
        /// 現在の表示位置をページ数換算で取得、または設定します。
        /// </summary>
        public int Current
        {
            get { return (int)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register(
                "Current",
                typeof(int),
                typeof(PagingUserControl),
                new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(PagingUserControl.OnCurrentChanged)
                )
            );

        /// <summary>
        /// 現在の表示位置をデータ件数換算で取得、または設定します。
        /// </summary>
        public int Offset
        {
            get { return (int)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(
                "Offset",
                typeof(int),
                typeof(PagingUserControl),
                new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(PagingUserControl.OnOffsetChanged)
                )
            );

        /// <summary>
        /// 1ページに表示する件数を取得、または設定します。
        /// </summary>
        public int Limit
        {
            get { return (int)GetValue(LimitProperty); }
            set { SetValue(LimitProperty, value); }
        }

        public static readonly DependencyProperty LimitProperty =
            DependencyProperty.Register(
                "Limit",
                typeof(int),
                typeof(PagingUserControl),
                new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(PagingUserControl.OnPropertyInitialized)
                )
            );

        /// <summary>
        /// ページングボタンの数を取得、または設定します。
        /// </summary>
        public int PageLength
        {
            get { return (int)GetValue(PageLengthProperty); }
            set { SetValue(PageLengthProperty, value); }
        }

        public static readonly DependencyProperty PageLengthProperty =
            DependencyProperty.Register(
                "PageLength",
                typeof(int),
                typeof(PagingUserControl),
                new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(PagingUserControl.OnPropertyInitialized)
                )
            );

        /// <summary>
        /// データ件数を取得、または設定します。
        /// </summary>
        public int DataLength
        {
            get { return (int)GetValue(DataLengthProperty); }
            set { SetValue(DataLengthProperty, value); }
        }

        public static readonly DependencyProperty DataLengthProperty =
            DependencyProperty.Register(
                "DataLength",
                typeof(int),
                typeof(PagingUserControl),
                new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(PagingUserControl.OnPropertyInitialized)
                )
            );

        /// <summary>
        /// データ件数を取得、または設定します。
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(PagingUserControl), new PropertyMetadata());

        #endregion

        /// <summary>
        /// ﾍﾟｰｼﾞﾝｸﾞﾎﾞﾀﾝ押下時のｺﾏﾝﾄﾞ
        /// </summary>
        public ICommand NumberingCommand
        {
            get
            {
                return _NumberingCommand = _NumberingCommand ?? new RelayCommand<Button>(
                    b =>
                    {
                        Current = int.Parse(b.Content.ToString());
                    },
                    b =>
                    {
                        return int.Parse(b.Content.ToString()) * Limit <= DataLength;
                    }
                );
            }
        }
        private ICommand _NumberingCommand;

        /// <summary>
        /// 前へﾎﾞﾀﾝ押下時のｺﾏﾝﾄﾞ
        /// </summary>
        public ICommand PreviousCommand
        {
            get
            {
                return _PreviousCommand = _PreviousCommand ?? new RelayCommand(
                    _ =>
                    {
                        Current--;
                    },
                    _ =>
                    {
                        return 0 < Current;
                    }
                );
            }
        }
        private ICommand _PreviousCommand;

        /// <summary>
        /// 次へﾎﾞﾀﾝ押下時のｺﾏﾝﾄﾞ
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                return _NextCommand = _NextCommand ?? new RelayCommand(
                    _ =>
                    {
                        Current++;
                    },
                    _ =>
                    {
                        return Current < GetMaxPage();
                    }
                );
            }
        }
        private ICommand _NextCommand;

        /// <summary>
        /// 最初へﾎﾞﾀﾝ押下時のｺﾏﾝﾄﾞ
        /// </summary>
        public ICommand FirstCommand
        {
            get
            {
                return _FirstCommand = _FirstCommand ?? new RelayCommand(
                    _ =>
                    {
                        Current = 1;
                    },
                    _ =>
                    {
                        return 0 < GetMaxPage();
                    }
                );
            }
        }
        private ICommand _FirstCommand;

        /// <summary>
        /// Currentﾌﾟﾛﾊﾟﾃｨ変更時のｲﾍﾞﾝﾄ
        /// </summary>
        public event EventHandler CurrentChanged;

        /// <summary>
        /// ﾍﾟｰｼﾞﾝｸﾞの描写を変更すべきﾌﾟﾛﾊﾟﾃｨが変更されたときのｲﾍﾞﾝﾄ
        /// </summary>
        public event EventHandler PropertyInitialized;

        /// <summary>
        /// ｺﾝｽﾄﾗｸﾀ
        /// </summary>
        public PagingUserControl()
        {
            InitializeComponent();

            baseContainer.DataContext = this;
            this.PropertyInitialized += OnPropertyInitialized;
            this.CurrentChanged += OnCurrentChanged;
            this.CurrentChanged += OnUserCurrentChanged;

        }

        /// <summary>
        /// Currentﾌﾟﾛﾊﾟﾃｨ変更時の規定のｲﾍﾞﾝﾄ
        /// </summary>
        private void OnCurrentChanged(object sender, EventArgs e)
        {
            if (Current <= 0) return;

            var maxPage = GetMaxPage();

            // ﾍﾟｰｼﾞｬ開始位置を取得
            var index = (int)(Current - Math.Ceiling((PageLength - 1) / 2d));

            // 最終ﾍﾟｰｼﾞまで移動していた場合の考慮
            index = (index + PageLength - 1) < maxPage ? index : maxPage - PageLength + 1;

            // ﾍﾟｰｼﾞｬ開始位置がﾏｲﾅｽになった場合の考慮
            index = index <= 0 ? 1 : index;

            // ﾍﾟｰｼﾞｬ開始位置によってPAGE1ﾎﾞﾀﾝの活性/非活性を切り替える。
            btnFirst.IsEnabled = (1 < index);

            // ﾍﾟｰｼﾞ番号の更新
            for (int i = 0; i < PageLength; i++)
            {
                var button = GetButton(i + 1);
                button.Content = string.Format("{0}", index + i);
                button.IsEnabled = !((index + i) == Current);
            }

            // Offsetの更新
            Offset = (Current - 1) * Limit;

            // TODO PAGE x or x のﾗﾍﾞﾙ変更
            txtPageSize.Text = string.Format("PAGE {0} of {1}", Current, maxPage);

        }

        /// <summary>
        /// ﾕｰｻﾞが指定したCurrentﾌﾟﾛﾊﾟﾃｨ変更時のｲﾍﾞﾝﾄ
        /// </summary>
        private void OnUserCurrentChanged(object sender, EventArgs e)
        {
            if (Command != null && 0 < Current) Command.Execute(null);
        }

        /// <summary>
        /// ﾍﾟｰｼﾞﾝｸﾞの描写を変更すべきﾌﾟﾛﾊﾟﾃｨが変更されたときのｲﾍﾞﾝﾄ
        /// </summary>
        private void OnPropertyInitialized(object sender, EventArgs e)
        {
            var maxPage = GetMaxPage();
            var index = maxPage;
            index = index <= 0 ? 1 : index;
            index = index < PageLength ? index : PageLength;

            // 表示/非表示切替
            for (int i = 1; i <= index; i++)
            {
                GetButton(i).Visibility = Visibility.Visible;
            }
            for (int i = index + 1; i <= 10; i++)
            {
                GetButton(i).Visibility = Visibility.Collapsed;
            }

            // TODO PAGE x or x のﾗﾍﾞﾙ変更
            txtPageSize.Text = string.Format("PAGE {0} of {1}", Current, maxPage);
        }

        /// <summary>
        /// CurrentChangedｲﾍﾞﾝﾄを発行します。
        /// </summary>
        private static void OnCurrentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var instance = sender as PagingUserControl;
            if (instance != null)
            {
                instance.CurrentChanged(sender, new EventArgs());
            }
        }

        /// <summary>
        /// PropertyInitializedｲﾍﾞﾝﾄを発行します。
        /// </summary>
        private static void OnPropertyInitialized(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var instance = sender as PagingUserControl;
            if (instance != null)
            {
                instance.PropertyInitialized(sender, new EventArgs());
            }
        }

        /// <summary>
        /// OnOffsetChangedｲﾍﾞﾝﾄを発行します。
        /// </summary>
        private static void OnOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var instance = sender as PagingUserControl;
            if (instance != null)
            {
                instance.Current = instance.Offset / instance.Limit + 1;
            }
        }

        /// <summary>
        /// indexに紐付くﾎﾞﾀﾝｲﾝｽﾀﾝｽを取得します。
        /// </summary>
        /// <param name="index">ｲﾝﾃﾞｯｸｽ</param>
        /// <returns><code>Button</code></returns>
        private Button GetButton(int index)
        {
            switch (index)
            {
                case 1:
                    return btnPage1;
                case 2:
                    return btnPage2;
                case 3:
                    return btnPage3;
                case 4:
                    return btnPage4;
                case 5:
                    return btnPage5;
                case 6:
                    return btnPage6;
                case 7:
                    return btnPage7;
                case 8:
                    return btnPage8;
                case 9:
                    return btnPage9;
                case 10:
                    return btnPage10;
                default:
                    throw new ArgumentException(string.Format("Button index: ", index));
            }
        }

        /// <summary>
        /// 現在のﾘﾐｯﾄ、ﾃﾞｰﾀ件数に基づく最大ﾍﾟｰｼﾞ数を取得します。
        /// </summary>
        /// <returns></returns>
        private int GetMaxPage()
        {
            if (0 < Limit)
            {
                return (int)Math.Floor(DataLength / Limit * 1d);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 本ｵﾌﾞｼﾞｪｸﾄ読み込み時のｲﾍﾞﾝﾄ
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PropertyInitialized(this, new EventArgs());
            CurrentChanged(this, new EventArgs());
        }
    }
}

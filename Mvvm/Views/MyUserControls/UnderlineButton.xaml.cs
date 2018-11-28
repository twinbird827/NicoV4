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

namespace NicoV4.Mvvm.Views.MyUserControls
{
    /// <summary>
    /// UnderlineButton.xaml の相互作用ロジック
    /// </summary>
    public partial class UnderlineButton : UserControl
    {
        public UnderlineButton()
        {
            InitializeComponent();

            baseContainer.DataContext = this;
        }

        /// <summary>
        /// ｺﾏﾝﾄﾞを取得、または設定します。
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(UnderlineButton), new PropertyMetadata());

        /// <summary>
        /// ｺﾏﾝﾄﾞﾊﾟﾗﾒｰﾀを取得、または設定します。
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(UnderlineButton), new PropertyMetadata());

        /// <summary>
        /// 下線を表示するためのフラグ1を取得、または設定します。
        /// </summary>
        public bool ShowUnderline
        {
            get { return (bool)GetValue(ShowUnderlineProperty); }
            set { SetValue(ShowUnderlineProperty, value); }
        }

        public static readonly DependencyProperty ShowUnderlineProperty =
            DependencyProperty.Register("ShowUnderline", typeof(bool), typeof(UnderlineButton), new PropertyMetadata());

        /// <summary>
        /// 下線を表示するためのフラグ2を取得、または設定します。
        /// </summary>
        public bool ShowUnderlineCondition
        {
            get { return (bool)GetValue(ShowUnderlineConditionProperty); }
            set { SetValue(ShowUnderlineConditionProperty, value); }
        }

        public static readonly DependencyProperty ShowUnderlineConditionProperty =
            DependencyProperty.Register("ShowUnderlineCondition", typeof(bool), typeof(UnderlineButton), new PropertyMetadata());

        /// <summary>
        /// 下線を表示するためのフラグ2を取得、または設定します。
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UnderlineButton), new PropertyMetadata());

    }
}

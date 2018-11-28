using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm.Service;

namespace NicoV4.Mvvm.Views.Services
{
    public class WpfMessageService : ConsoleMessageService
    {
        public override async void Info(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            await MainWindowViewModel.Instance.ShowMessageAsync(
                "Information",
                message,
                MessageDialogStyle.Affirmative);
            base.Info(message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public override async void Error(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            await MainWindowViewModel.Instance.ShowMessageAsync(
                "Error",
                message,
                MessageDialogStyle.Affirmative);
            base.Error(message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public override async void Exception(Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            await MainWindowViewModel.Instance.ShowMessageAsync(
                "Exception",
                exception.Message,
                MessageDialogStyle.Affirmative);
            base.Exception(exception, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}

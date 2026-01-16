using System.Windows.Threading;
using WpfMvvm;
using WpfMvvm.Exceptions;
using WpfMvvm.Extensions;
using WpfMvvm.Views.Dialogs;
using WpfTestApp.View.Dialogs;

namespace GohMdlExpert.Views.Dialogs {
    public class UserDialogProviderGoh : IUserDialogProvider {
        private ExceptionFormatter? _exceptionFormatter;

        public UserDialogProviderGoh() { }

        public UserDialogProviderGoh(ExceptionFormatter exceptionFormatter) {
            _exceptionFormatter = exceptionFormatter;
        }

        public void ShowInfo(string message, string? title = null) {
            WpfApplication.Current.Synchronize(() => {
                _ = new MessageBoxWindow(message, title, DialogType.Information).ShowDialog();
            });
        }

        public void ShowWarning(string message, string? title = null) {
            WpfApplication.Current.Synchronize(() => {
                _ = new MessageBoxWindow(message, title, DialogType.Warning).ShowDialog();
            });
        }

        public void ShowError(string message, string? title = null, Exception? exception = null) {
            WpfApplication.Current.Synchronize(() => {
                string exMessage = (exception != null) ? (_exceptionFormatter?.ExceptionToString(exception) ?? exception.Message) : string.Empty;

                _ = new UserErrorDialogWindow(message, exception, title).ShowDialog();
            });
        }

        public QuestionResult? Ask(string question, string? title = null, QuestionType questionType = QuestionType.OKCancel, QuestionResult? defaultResult = null) {
            QuestionResult? result = null;

            defaultResult ??= questionType switch {
                QuestionType.OKCancel => QuestionResult.Cancel,
                QuestionType.YesNoCancel => QuestionResult.Cancel,
                QuestionType.YesNo => QuestionResult.No,
                _ => null,
            };

            WpfApplication.Current.Synchronize(() => {
                result = new MessageBoxWindow(question, title, DialogType.Question, questionType).ShowDialog();
            }, DispatcherPriority.Normal, wait: true);

            return result ?? defaultResult;
        }
    }
}

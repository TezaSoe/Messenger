using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Messenger.ViewModels
{
    class MessageBoxDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

        // Yes, YesNo, YesNoCancel
        private string _dialogType = "Yes";
        public string DialogType
        {
            get { return _dialogType; }
            set { SetProperty(ref _dialogType, value); }
        }

        private string _title = "";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public event Action<IDialogResult> RequestClose;
        bool IsDialogClosable = false;

        public MessageBoxDialogViewModel()
        {
            IsDialogClosable = true;
        }

        public virtual bool CanCloseDialog()
        {
            if (IsDialogClosable)
                return true;
            else return false;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            DialogType = parameters.GetValue<string>("dialogType");
            Title = parameters.GetValue<string>("title");
            Message = parameters.GetValue<string>("message");
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = (ButtonResult)System.Enum.Parse(typeof(ButtonResult), parameter, true);
            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
    }
}

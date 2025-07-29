using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateSystem.Models;

namespace TemplateSystem.ViewModels
{
    internal class TemplateDataEditDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "模板参数修改";

        public event Action<IDialogResult> RequestClose;

        private sys_bd_Templatedatamodel _messageTemplatedata;

        public sys_bd_Templatedatamodel MessageTemplatedata
        {
            get { return _messageTemplatedata; }
            set { SetProperty(ref _messageTemplatedata, value); }
        }



        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public TemplateDataEditDialogViewModel()
        {
            ConfirmCommand = new DelegateCommand(OnConfirm);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        private void OnCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));

        }

        private void OnConfirm()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
            {
                { "Result", MessageTemplatedata }
            }));
        }

        public bool CanCloseDialog()
        {
            return true;
            //throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {
            //throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // 从参数中读取数据
            if (parameters.ContainsKey("para"))
            {
                MessageTemplatedata = parameters.GetValue<sys_bd_Templatedatamodel>("para");
            }
        }
    }

}

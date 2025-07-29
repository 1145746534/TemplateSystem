using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using TemplateSystem.Models;


namespace TemplateSystem.ViewModels
{

    internal class MatchResultDialogViewModel : BindableBase, IDialogAware
    {
        private bool _disposed = false;

        private ObservableCollection<MatchResultModel> _matchResultDatas;

        public ObservableCollection<MatchResultModel> MatchResultDatas
        {
            get { return _matchResultDatas; }
            set { _matchResultDatas = value; }
        }

        private readonly IEventAggregator _eventAggregator;
        private SubscriptionToken _subscriptionToken;

        public MatchResultDialogViewModel(IEventAggregator eventAggregator)
        {
            MatchResultDatas = new ObservableCollection<MatchResultModel>();
            _eventAggregator = eventAggregator;

            // 订阅事件
            _subscriptionToken = _eventAggregator
                .GetEvent<WindowCommunicationEvent>()
                .Subscribe(OnMessageReceived,
                            ThreadOption.UIThread, // 确保在UI线程执行
                            keepSubscriberReferenceAlive: true);

            //EventMessage.MessageHelper.GetEvent<MatchResultDatasDisplayEvent>().Subscribe(MatchResultDatasDisplay, ThreadOption.UIThread);
            //EventMessage.MessageHelper.GetEvent<MatchResultDatasDisplayEvent>().Subscribe(MatchResultDatasDisplay);
        }
        private void OnMessageReceived(List<MatchResultModel> list)
        {
            // 处理接收到的消息
            MatchResultDatas.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                MatchResultDatas.Add(list[i]);
            }
        }
        private void MatchResultDatasDisplay(List<MatchResultModel> list)
        {
            MatchResultDatas.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                MatchResultDatas.Add(list[i]);
            }
        }

        public string Title => "匹配结果";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult());
            Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    MatchResultDatas.Clear();
                    // 取消订阅，防止内存泄漏
                    _subscriptionToken?.Dispose();
                    // Unsubscribe from events and release managed resources
                    //EventMessage.MessageHelper.GetEvent<MatchResultDatasDisplayEvent>().Unsubscribe(MatchResultDatasDisplay);
                }

                // Free unmanaged resources (if any) and set large fields to null
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}

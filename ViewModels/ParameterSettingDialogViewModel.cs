using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateSystem.Models;
using TemplateSystem.Util;

namespace TemplateSystem.ViewModels
{
    internal class SetShowEvent : PubSubEvent<bool> { }
    internal class ShapeModelParamEvent : PubSubEvent<ShapeModelParam> { }


    internal class ParameterSettingDialogViewModel : BindableBase, IDialogAware, IDisposable
    {
        private bool _disposed = false;

        public string Title => "参数设置";

        public event Action<IDialogResult> RequestClose;
        private readonly IEventAggregator _eventAggregator;

        private SubscriptionToken _subscriptionToken1;
        private SubscriptionToken _subscriptionToken2;


        private double _templateStartAngle;
        /// <summary>
        /// 制作模板时剪切制作模板区域的起始角度
        /// </summary>
        public double TemplateStartAngle
        {
            get { return _templateStartAngle; }
            set { SetProperty(ref _templateStartAngle, value); }
        }
        private double _templateEndAngle;
        /// <summary>
        /// 剪切制作模板区域的终止角度
        /// </summary>
        public double TemplateEndAngle
        {
            get { return _templateEndAngle; }
            set { SetProperty(ref _templateEndAngle, value); }
        }

        private bool _isSet;
        /// <summary>
        /// 是否启用参数
        /// </summary>
        public bool IsSet
        {
            get { return _isSet; }
            set
            {
                SetProperty(ref _isSet, value);
                SetVisibility(value);
            }
        }

        private int _contrastLow;
        /// <summary>
        /// 对比度（低）
        /// </summary>
        public int ContrastLow
        {
            get { return _contrastLow; }
            set {
                if (value>= ContrastHigh)
                {
                    ContrastHigh = value + 1;
                }
                SetProperty(ref _contrastLow, value); 
            }
        }

        private int _contrastHigh;
        /// <summary>
        /// 对比度（高）
        /// </summary>
        public int ContrastHigh
        {
            get { return _contrastHigh; }
            set {
                if (value <= ContrastLow)
                {
                    ContrastLow = value;
                }
                SetProperty(ref _contrastHigh, value); 
            }
        }
        private int _minSize;
        /// <summary>
        /// 最小组件尺寸
        /// </summary>
        public int MinSize
        {
            get { return _minSize; }
            set { SetProperty(ref _minSize, value); }
        }


        private Visibility _paramShow;
        /// <summary>
        /// 显示
        /// </summary>
        public Visibility ParamShow
        {
            get { return _paramShow; }
            set { SetProperty(ref _paramShow, value); }
        }



        public ParameterSettingDialogViewModel(IEventAggregator eventAggregator)
        {
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
            _eventAggregator = eventAggregator;
            ParamShow = Visibility.Hidden;

            // 订阅事件
            _subscriptionToken1 = _eventAggregator
                .GetEvent<SetShowEvent>()
                .Subscribe(SetParamShow,
                            ThreadOption.UIThread, // 确保在UI线程执行
                            keepSubscriberReferenceAlive: true);
            _subscriptionToken2 = _eventAggregator
                .GetEvent<ShapeModelParamEvent>()
                .Subscribe(ModelParamChange,
                            ThreadOption.UIThread, // 确保在UI线程执行
                            keepSubscriberReferenceAlive: true);

        }







        /// <summary>
        /// 确认按钮命令
        /// </summary>
        public DelegateCommand OkCommand { get; set; }
        /// <summary>
        /// 取消按钮命令
        /// </summary>
        public DelegateCommand CancelCommand { get; set; }

        private void SetParamShow(bool obj)
        {
            IsSet = obj;
           
        }
        private void SetVisibility(bool isShow)
        {
            if (isShow)
            {
                ParamShow = Visibility.Visible;
            }
            else
            {
                ParamShow = Visibility.Hidden;
            }
        }
        /// <summary>
        /// 模板出生成后反馈的参数
        /// </summary>
        /// <param name="param"></param>
        private void ModelParamChange(ShapeModelParam param)
        {
            this.ContrastLow = param.ContrastLow;
            this.ContrastHigh = param.ContrastHigh;
            this.MinSize = param.MinSize;
        }


        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult());
        }

        private void Ok()
        {
            ShapeModelParam angle = new ShapeModelParam();
            angle.TemplateStartAngle = DegreesToRadians(TemplateStartAngle);
            angle.TemplateEndAngle = DegreesToRadians(TemplateEndAngle);
            angle.IsSet = IsSet;
            if (IsSet && ContrastLow != 1 && ContrastHigh != 1 && MinSize != 1)
            {
                angle.ContrastLow = ContrastLow;
                angle.ContrastHigh = ContrastHigh;
                angle.MinSize = MinSize;
            }


            _eventAggregator.GetEvent<AngleChangeEvent>().Publish(angle);


            ////定义弹窗结果
            //IDialogResult dialogResult = new DialogResult();
            ////将新增数据添加到弹窗结果的Parameters
            //dialogResult.Parameters.Add("WheelMinThreshold", WheelMinThreshold);
            //dialogResult.Parameters.Add("WheelMinRadius", WheelMinRadius);
            //dialogResult.Parameters.Add("WindowMaxThreshold", WindowMaxThreshold);
            //dialogResult.Parameters.Add("RemoveMixArea", RemoveMixArea);
            //dialogResult.Parameters.Add("TemplateStartAngle", TemplateStartAngle);
            //dialogResult.Parameters.Add("TemplateEndAngle", TemplateEndAngle);

            //关闭弹窗并返回弹窗结果
            //RequestClose?.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

            if (parameters.ContainsKey("TemplateStartAngle"))
            {
                double radians = parameters.GetValue<double>("TemplateStartAngle");
                TemplateStartAngle = RadiansToDegrees(radians);
            }

            if (parameters.ContainsKey("TemplateEndAngle"))
            {
                double radians = parameters.GetValue<double>("TemplateEndAngle");
                TemplateEndAngle = RadiansToDegrees(radians);
            }
            if (parameters.ContainsKey("ContrastLow"))
            {
                ContrastLow = parameters.GetValue<int>("ContrastLow");
            }  
            if (parameters.ContainsKey("ContrastHigh"))
            {
                ContrastHigh = parameters.GetValue<int>("ContrastHigh");
            }  
            if (parameters.ContainsKey("MinSize"))
            {
                MinSize = parameters.GetValue<int>("MinSize");
            }


        }

        /// <summary>
        /// 弧度转角度函数
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// 角度转弧度函数
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }


        public void Dispose()
        {
            // 取消订阅，防止内存泄漏
            _subscriptionToken1?.Dispose();
            _subscriptionToken2?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateSystem.Models;
using TemplateSystem.Util;

namespace TemplateSystem.ViewModels
{
    internal class ParameterSettingDialogViewModel : BindableBase, IDialogAware
    {
        private bool _disposed = false;

        public string Title => "参数设置";

        public event Action<IDialogResult> RequestClose;

        private int _windowMaxThreshold;
        /// <summary>
        /// 制作模板时模板剔除部分最大阈值
        /// </summary>
        public int WindowMaxThreshold
        {
            get { return _windowMaxThreshold; }
            set { SetProperty(ref _windowMaxThreshold, value); }
        }

        private double _removeMixArea;
        /// <summary>
        /// 制作模板时模板剔除部分的最小面积
        /// </summary>
        public double RemoveMixArea
        {
            get { return _removeMixArea; }
            set { SetProperty(ref _removeMixArea, value); }
        }
        //TemplateEndAngle TemplateStartAngle 
        private int _wheelMinThreshold;
        /// <summary>
        /// 定位轮毂最小阈值
        /// </summary>
        public int WheelMinThreshold
        {
            get { return _wheelMinThreshold; }
            set { SetProperty(ref _wheelMinThreshold, value); }
        }
        private int _wheelMinRadius;
        /// <summary>
        /// 定位轮毂最小半径
        /// </summary>
        public int WheelMinRadius
        {
            get { return _wheelMinRadius; }
            set { SetProperty(ref _wheelMinRadius, value); }
        }

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



        /// <summary>
        /// 确认按钮命令
        /// </summary>
        public DelegateCommand OkCommand { get; set; }
        /// <summary>
        /// 取消按钮命令
        /// </summary>
        public DelegateCommand CancelCommand { get; set; }

        public ParameterSettingDialogViewModel()
        {
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult());
        }

        private void Ok()
        {
            //bool changed = false;
            //if (WheelMinThresholdValue < 0 || WheelMinThresholdValue > 100)
            //{
            //    EventMessage.SystemMessageDisplay("定位轮毂时的最小阈值输入错误，请重新输入!", Models.MessageType.Error);
            //    return;
            //}
            //else
            //{
            //    if (SystemDatas.WheelMinThreshold != WheelMinThresholdValue)
            //    {
            //        SqlAccess.SystemDatasWrite("WheelMinThreshold", WheelMinThresholdValue.ToString());
            //        SystemDatas.WheelMinThreshold = WheelMinThresholdValue;
            //        changed = true;
            //    }
            //}
            //if (WindowMaxThreshold < 0 || WindowMaxThreshold > 200)
            //{
            //    EventMessage.SystemMessageDisplay("轮毂窗口最大阈值输入错误，请重新输入!", Models.MessageType.Error);
            //    return;
            //}
            //else
            //{
            //    if (SystemDatas.WindowMaxThreshold != WindowMaxThreshold)
            //    {
            //        SystemDatas.WindowMaxThreshold = WindowMaxThreshold;
            //        SqlAccess.SystemDatasWrite("WindowMaxThreshold", WindowMaxThreshold.ToString());
            //        changed = true;
            //    }
            //}
            //if (RemoveMixAreaValue < 10 || RemoveMixAreaValue > 500)
            //{
            //    EventMessage.SystemMessageDisplay("轮毂窗口最小面积输入错误，请重新输入!", Models.MessageType.Error);
            //    return;
            //}
            //else
            //{
            //    if (SystemDatas.RemoveMixArea != RemoveMixAreaValue)
            //    {
            //        SqlAccess.SystemDatasWrite("RemoveMixArea", RemoveMixAreaValue.ToString());
            //        SystemDatas.RemoveMixArea = RemoveMixAreaValue;
            //        changed = true;
            //    }

            //}
            //if (changed)
            //{
            //    EventMessage.MessageHelper.GetEvent<ParameterSettingChangedEvent>().Publish("");
            //    EventMessage.SystemMessageDisplay("修改成功！", Models.MessageType.Success);
            //}
            //RequestClose?.Invoke(new DialogResult());

            //定义弹窗结果
            IDialogResult dialogResult = new DialogResult();
            //将新增数据添加到弹窗结果的Parameters
            dialogResult.Parameters.Add("WheelMinThreshold", WheelMinThreshold);
            dialogResult.Parameters.Add("WheelMinRadius", WheelMinRadius);
            dialogResult.Parameters.Add("WindowMaxThreshold", WindowMaxThreshold);
            dialogResult.Parameters.Add("RemoveMixArea", RemoveMixArea);
            dialogResult.Parameters.Add("TemplateStartAngle", TemplateStartAngle);
            dialogResult.Parameters.Add("TemplateEndAngle", TemplateEndAngle);

            //关闭弹窗并返回弹窗结果
            RequestClose?.Invoke(dialogResult);
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
            if (parameters.ContainsKey("WheelMinThreshold"))            
                WheelMinThreshold = parameters.GetValue<int>("WheelMinThreshold");
            if (parameters.ContainsKey("WheelMinRadius"))
                WheelMinRadius = parameters.GetValue<int>("WheelMinRadius");
            if (parameters.ContainsKey("WindowMaxThreshold"))
                WindowMaxThreshold = parameters.GetValue<int>("WindowMaxThreshold");
            if (parameters.ContainsKey("RemoveMixArea"))
                RemoveMixArea = parameters.GetValue<double>("RemoveMixArea");
            if (parameters.ContainsKey("TemplateStartAngle"))
                TemplateStartAngle = parameters.GetValue<double>("TemplateStartAngle");
            if (parameters.ContainsKey("TemplateEndAngle"))
                TemplateEndAngle = parameters.GetValue<double>("TemplateEndAngle");

          
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Unsubscribe from events and release managed resources
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

using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateSystem.Models
{
    /// <summary>
    /// 模板数据
    /// </summary>
    internal class TemplatedataModel : sys_bd_Templatedatamodel
    {
        //private readonly object _lock = new object();

        //private Task<HTuple> _loadingTask;

        public HTuple _template;

        /// <summary>
        /// 模板
        /// </summary>
        public HTuple Template
        {
            get
            {
                if (_template == null && File.Exists(TemplatePath))
                {
                    // 同步加载（）
                    Console.WriteLine($"同步加载文件：{TemplatePath}");
                    string strPath = TemplatePath.Replace(@"\", "/");
                    HOperatorSet.ReadNccModel(strPath, out HTuple modelID);
                    _template = modelID;
                }
                return _template;
            }
            set
            {
                _template?.Dispose();
                _template = value;
            }
        }


        /// <summary>
        /// 释放模板
        /// </summary>
        public void ReleaseTemplate()
        {
            if (_template != null)
            {
                _template?.Dispose();
                _template = null;
            }
        }
        /// <summary>
        /// 释放内存
        /// </summary>
        public void Dispose()
        {
            ReleaseTemplate();
        }

        /// <summary>
        /// 更新使用时间
        /// </summary>
        /// <param name="model"></param>
        public void UseTemplate()
        {
            LastUsedTime = DateTime.Now; // 更新使用时间
        }
        /// <summary>
        /// 从另一个基类对象复制所有属性值（包含模板路径）
        /// </summary>
        public void CopyPropertiesFrom(sys_bd_Templatedatamodel source)
        {
            if (source == null) return;

            // 直接复制基类属性（非绑定方式避免通知触发）
            this.Index = source.Index;
            this.WheelType = source.WheelType;
            this.UnusedDays = source.UnusedDays;
            this.WheelHeight = source.WheelHeight;
            this.WheelStyle = source.WheelStyle;
            this.InnerCircleGary = source.InnerCircleGary;
            this.CreationTime = source.CreationTime;
            this.UpdateTime = source.UpdateTime;
            this.LastUsedTime = source.LastUsedTime;
            this.TemplatePath = source.TemplatePath;
            this.TemplatePicturePath = source.TemplatePicturePath;

        }
    }
}

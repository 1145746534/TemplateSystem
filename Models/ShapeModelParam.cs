using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateSystem.Models
{
    /// <summary>
    /// 制作模板的
    /// </summary>
    internal class ShapeModelParam
    {
        /// <summary>
        /// 制作模板的起始角度
        /// </summary>
        internal double TemplateStartAngle { get; set; }

        /// <summary>
        /// 制作模板的结束角度
        /// </summary>

        internal double TemplateEndAngle { get; set; }

        /// <summary>
        /// 是否设置参数
        /// </summary>
        internal bool IsSet { get; set; }
        /// <summary>
        /// 对比度低
        /// </summary>
        internal int ContrastLow { get; set; }
        /// <summary>
        /// 对比度高
        /// </summary>
        internal int ContrastHigh { get; set; }
        /// <summary>
        /// 最小组件尺寸
        /// </summary>
        internal int MinSize { get; set; }



    }
}

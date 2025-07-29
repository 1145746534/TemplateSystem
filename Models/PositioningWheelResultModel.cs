using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateSystem.Models
{
    internal class PositioningWheelResultModel : IDisposable
    {
        /// <summary>
        /// 定位到的轮毂图像
        /// </summary>
        public HObject WheelImage { get; set; }
        /// <summary>
        /// 轮毂轮廓
        /// </summary>
        public HObject WheelContour { get; set; }
        /// <summary>
        /// 轮毂中心行坐标
        /// </summary>
        public double? CenterRow { get; set; }
        /// <summary>
        /// 轮毂中心列坐标
        /// </summary>
        public double? CenterColumn { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public double? Radius { get; set; }
        /// <summary>
        /// 内圈灰度
        /// </summary>
        public float InnerCircleMean { get; set; }

        /// <summary>
        /// 全图灰度
        /// </summary>
        public float FullFigureGary { get; set; }

        public void Dispose()
        {
            SafeHalconDispose(WheelImage);
            SafeHalconDispose(WheelContour);
            //SafeHalconDispose(CenterRow);
            //SafeHalconDispose(CenterColumn);
            //SafeHalconDispose(Radius);
        }

        private void SafeHalconDispose<T>(T obj) where T : class, IDisposable
        {
            if (obj != null)
            {
                obj?.Dispose();
                obj = null; // 关键：解除引用使GC可回收
            }
        }
    }

}

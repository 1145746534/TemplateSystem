using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateSystem.Models
{
    /// <summary>
    /// 系统数据
    /// </summary>
    internal class SystemDatas
    {
        /// <summary>
        /// 数据存储根目录
        /// </summary>
        public static string RootDirectory { get; set; }
        /// <summary>
        /// 模板图片保存路径
        /// </summary>
        public static string TemplateImagesPath { get; set; }
        /// <summary>
        /// 活跃模板保存路径
        /// </summary>
        public static string ActiveTemplatesPath { get; set; }
        /// <summary>
        /// 不活跃模板保存路径
        /// </summary>
        public static string NotActiveTemplatesPath { get; set; }
        /// <summary>
        /// 历史图片保存路径
        /// </summary>
        public static string HistoricalImagesPath { get; set; }

        /// <summary>
        /// 手动图片保存路径
        /// </summary>
        public static string HandImagesPath { get; set; }

        /// <summary>
        /// 深度学习图片路径
        /// </summary>
        public static string DeepImagesPath { get; set; }


        /// <summary>
        /// 定位轮毂最小阈值
        /// </summary>
        public static int WheelMinThreshold { get; set; }

        /// <summary>
        /// 定位轮毂最小半径
        /// </summary>
        public static int WheelMinRadius { get; set; }

        /// <summary>
        /// 轮毂最大阈值
        /// </summary>
        public static int WheelMaxThreshold { get; set; }

        /// <summary>
        /// 制作模板时轮毂窗口部分最大阈值
        /// </summary>
        public static int WindowMaxThreshold { get; set; }

        /// <summary>
        /// 制作模板时剪切制作模板区域的起始角度
        /// </summary>
        public static double TemplateStartAngle { get; set; }

        /// <summary>
        /// 剪切制作模板区域的终止角度
        /// </summary>
        public static double TemplateEndAngle { get; set; }

        /// <summary>
        /// 制作模板时模板剔除部分的最小面积
        /// </summary>
        public static double RemoveMixArea { get; set; }

        /// <summary>
        /// 匹配模板的起始角度
        /// </summary>
        public static double AngleStart { get; set; }

        /// <summary>
        /// 匹配模板的角度范围
        /// </summary>
        public static double AngleExtent { get; set; }

        /// <summary>
        /// 模板匹配的最小相似度
        /// </summary>
        public static double MinSimilarity { get; set; }

        /// <summary>
        /// 图像中定位浇口区域的半径
        /// </summary>
        public static double PositioningGateRadius { get; set; }

        /// <summary>
        /// 浇口区域轮毂最小阈值
        /// </summary>
        public static int GateOutMinThreshold { get; set; }

        /// <summary>
        /// 判断浇口存在的最小面积
        /// </summary>
        public static int GateMinArea { get; set; }

        /// <summary>
        /// 判断浇口存在的最小半径
        /// </summary>
        public static double GateMinRadius { get; set; }

        /// <summary>
        /// 浇口检测开关
        /// </summary>
        public static bool GateDetectionSwitch { get; set; }

        /// <summary>
        /// 采集图像的缩放系数，控制图像大小
        /// </summary>
        public static double ScalingCoefficient { get; set; }



        /// <summary>
        /// 系统模式 True为自动模式
        /// </summary>
        public static bool SystemModel { get; set; } = false;

        /// <summary>
        /// 自动模式模板数据更新控制
        /// </summary>
        public static bool TemplateDataUpdataControl { get; set; } = false;

        /// <summary>
        /// 自动模式下使用的模板表格数据加载
        /// </summary>
        public static bool AutoTemplateDataLoadControl { get; set; } = false;


        /// <summary>
        /// 保存识别数据的月数
        /// </summary>
        public static int SaveDataMonths { get; set; }

        /// <summary>
        /// 模板动态调整天数
        /// </summary>
        public static int TemplateAdjustDays { get; set; }
        /// <summary>
        /// 是否根据分选数据判断分选结果，热工识别机必须True
        /// </summary>
        public static bool IsScreenedResult { get; set; }
        /// <summary>
        /// 图片是否裁剪
        /// </summary>
        public static bool CroppingOrNot { get; set; } = false;

        /// <summary>
        /// 大模型最低匹配相似度
        /// </summary>
        public static double ConfidenceMatch { get; set; }
        /// <summary>
        /// 全图灰度值 - 如果小于它 直接用大模型识别 -TODO 改成从数据库中读写
        /// </summary>
        public static double MinFullFigureGary { get; set; }

        // --- 文件管理参数

        /// <summary>
        /// 保存图片的天数
        /// </summary>
        public static int SaveImageDays { get; set; }
        /// <summary>
        /// 常驻内存的模板数量
        /// </summary>
        public static int MaintainQuantity { get; set; }


        // -- 
    }
}

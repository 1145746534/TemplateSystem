using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateSystem.Models
{
    /// <summary>
    /// 模板数据模型
    /// </summary>
    internal class sys_bd_Templatedatamodel : BindableBase, ICloneable
    {
        private int _index;
        private string _wheelType;
        private int _unusedDays;
        private float _wheelHeight;
        private float _fullGary;
        private string _wheelStyle;

        private float _positionCircleRow;
        private float _positionCircleColumn;
        private float _positionCircleRadius;
        private float _circumCircleRadius;

        private float _templateAreaCenterRow;
        private float _templateAreaCenterColumn;



        /// <summary>
        /// 序号
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }
        /// <summary>
        /// 轮毂型号
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false, Length = 25)]
        public string WheelType
        {
            get { return _wheelType; }
            set { SetProperty(ref _wheelType, value); }
        }
        /// <summary>
        /// 未用天数
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public int UnusedDays
        {
            get { return _unusedDays; }
            set { SetProperty(ref _unusedDays, value); }
        }
        /// <summary>
        /// 轮毂高度
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public float WheelHeight
        {
            get { return _wheelHeight; }
            set { SetProperty(ref _wheelHeight, value); }
        }
        /// <summary>
        /// 轮毂样式 成品/半成品
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true, Length = 25)]
        public string WheelStyle
        {
            get { return _wheelStyle; }
            set { SetProperty(ref _wheelStyle, value); }
        }

        /// <summary>
        /// 定位圆Row
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public float PositionCircleRow
        {
            get { return _positionCircleRow; }
            set { SetProperty(ref _positionCircleRow, value); }
        }

        /// <summary>
        /// 定位圆Column
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public float PositionCircleColumn
        {
            get { return _positionCircleColumn; }
            set { SetProperty(ref _positionCircleColumn, value); }
        }

        /// <summary>
        /// 定位圆半径
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public float PositionCircleRadius
        {
            get { return _positionCircleRadius; }
            set { SetProperty(ref _positionCircleRadius, value); }
        }

        /// <summary>
        /// 外接圆半径
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public float CircumCircleRadius
        {
            get { return _circumCircleRadius; }
            set { SetProperty(ref _circumCircleRadius, value); }
        }

        /// <summary>
        /// 模板区域中心点Row
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public float TemplateAreaCenterRow
        {
            get { return _templateAreaCenterRow; }
            set { SetProperty(ref _templateAreaCenterRow, value); }
        }

        /// <summary>
        /// 模板区域中心点Column
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public float TemplateAreaCenterColumn
        {
            get { return _templateAreaCenterColumn; }
            set { SetProperty(ref _templateAreaCenterColumn, value); }
        }

        /// <summary>
        /// 全局灰度
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = false)]
        public float FullGary
        {
            get { return _fullGary; }
            set { SetProperty(ref _fullGary, value); }
        }

        private string _creationTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true, Length = 25)]
        public string CreationTime
        {
            get { return _creationTime; }
            set { SetProperty(ref _creationTime, value); }
        }

        private DateTime _updateTime;
        /// <summary>
        /// 更新数据时间
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true, ColumnDataType = "datetime")]
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { SetProperty(ref _updateTime, value); }
        }


        private DateTime _lastUsedTime;
        /// <summary>
        /// 上次使用时间
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true, ColumnDataType = "datetime")]
        public DateTime LastUsedTime
        {
            get { return _lastUsedTime; }
            set { SetProperty(ref _lastUsedTime, value); }
        }


        private string _templatePath;
        /// <summary>
        /// 模板保存路径
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true, Length = 255)]
        public string TemplatePath
        {
            get { return _templatePath; }
            set { SetProperty(ref _templatePath, value); }
        }

        private string _templatePicturePath;
        /// <summary>
        /// 模板图片保存路径
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true, Length = 255)]
        public string TemplatePicturePath
        {
            get { return _templatePicturePath; }
            set { SetProperty(ref _templatePicturePath, value); }
        }

        public object Clone()
        {
            return new sys_bd_Templatedatamodel
            {
                Index = this.Index,
                WheelType = this.WheelType,
                UnusedDays = this.UnusedDays,
                WheelHeight = this.WheelHeight,
                FullGary = this.FullGary,
                WheelStyle = this.WheelStyle,
                CreationTime = this.CreationTime, // string 是引用类型但不可变
                UpdateTime = this.UpdateTime,     // DateTime 是值类型
                LastUsedTime = this.LastUsedTime,
                TemplatePath = this.TemplatePath,
                TemplatePicturePath = this.TemplatePicturePath,
                PositionCircleRow = this.PositionCircleRow,
                PositionCircleColumn = this.PositionCircleColumn,
                PositionCircleRadius = this.PositionCircleRadius,
                CircumCircleRadius = this.CircumCircleRadius,
                TemplateAreaCenterRow = this.TemplateAreaCenterRow,
                TemplateAreaCenterColumn = this.TemplateAreaCenterColumn,
            };
        }
    }
}

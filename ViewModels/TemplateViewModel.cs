using HalconDotNet;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using TemplateSystem.Models;
using static TemplateSystem.Util.ImageProcessingHelper;
using static TemplateSystem.Models.SystemDatas;
using TemplateSystem.Util;
using MahApps.Metro.Controls.Dialogs;
using Prism.Events;


namespace TemplateSystem.ViewModels
{
    // 定义自定义事件
    internal class WindowCommunicationEvent : PubSubEvent<List<MatchResultModel>> { }
    internal class ScrollToIndexEvent : PubSubEvent<int> { }

    internal class AngleChangeEvent : PubSubEvent<ShapeModelParam> { }


    internal class TemplateViewModel : BindableBase, IDisposable
    {
        private bool _disposed = false;

        #region======属性字段定义======
        public string Title { get; set; } = "模板管理";

        private ObservableCollection<sys_bd_Templatedatamodel> _templateDatas;
        /// <summary>
        /// 模板数据
        /// </summary>
        public ObservableCollection<sys_bd_Templatedatamodel> TemplateDatas
        {
            get { return _templateDatas; }
            set
            {
                SetProperty(ref _templateDatas, value);
            }
        }



        private double _lineWidth = 1.0;
        /// <summary>
        /// 模板制作窗口轮廓显示线宽
        /// </summary>
        public double LineWidth
        {
            get { return _lineWidth; }
            set { SetProperty(ref _lineWidth, value); }
        }


        private sys_bd_Templatedatamodel _dataGridSelectedItem;
        /// <summary>
        /// 模板数据窗口选中的行
        /// </summary>
        public sys_bd_Templatedatamodel DataGridSelectedItem
        {
            get { return _dataGridSelectedItem; }
            set
            {
                SetProperty(ref _dataGridSelectedItem, value);
            }
        }

        private int _dataGridSelectedIndex;
        /// <summary>
        /// 模板数据窗口选中的行索引
        /// </summary>
        public int DataGridSelectedIndex
        {
            get { return _dataGridSelectedIndex; }
            set { SetProperty(ref _dataGridSelectedIndex, value); }
        }

        private string _recognitionWheelType;
        /// <summary>
        /// 识别的轮型
        /// </summary>
        public string RecognitionWheelType
        {
            get { return _recognitionWheelType; }
            set { SetProperty(ref _recognitionWheelType, value); }
        }

        private string _recognitionSimilarity;
        /// <summary>
        /// 识别的相似度
        /// </summary>
        public string RecognitionSimilarity
        {
            get { return _recognitionSimilarity; }
            set { SetProperty(ref _recognitionSimilarity, value); }
        }

        private string _recognitionConsumptionTime;
        /// <summary>
        /// 识别消耗的时间
        /// </summary>
        public string RecognitionConsumptionTime
        {
            get { return _recognitionConsumptionTime; }
            set { SetProperty(ref _recognitionConsumptionTime, value); }
        }

        private string _recognitionWay;
        /// <summary>
        /// 识别方式
        /// </summary>
        public string RecognitionWay
        {
            get { return _recognitionWay; }
            set { SetProperty(ref _recognitionWay, value); }
        }

        private string _innerCircleGary;
        /// <summary>
        /// 内圈灰度
        /// </summary>
        public string InnerCircleGary
        {
            get { return _innerCircleGary; }
            set { SetProperty(ref _innerCircleGary, value); }
        }
        private string _fullGary;
        /// <summary>
        /// 全图灰度
        /// </summary>
        public string FullGary
        {
            get { return _fullGary; }
            set { SetProperty(ref _fullGary, value); }
        }

        private string _style;
        /// <summary>
        /// 样式
        /// </summary>
        public string Style
        {
            get { return _style; }
            set { SetProperty(ref _style, value); }
        }

        private Visibility _recognitionResultDisplay;
        /// <summary>
        /// 识别结果显示
        /// </summary>
        public Visibility RecognitionResultDisplay
        {
            get { return _recognitionResultDisplay; }
            set { SetProperty(ref _recognitionResultDisplay, value); }
        }





        private string _imageGrayval;
        /// <summary>
        /// 图像灰度
        /// </summary>
        public string ImageGrayval
        {
            get { return _imageGrayval; }
            set { SetProperty(ref _imageGrayval, value); }
        }

        private string _maskContent;
        /// <summary>
        /// 掩膜显示内容
        /// </summary>
        public string MASKContent
        {
            get { return _maskContent; }
            set { SetProperty(ref _maskContent, value); }
        }
        /// <summary>
        /// 是否掩膜中
        /// </summary>
        public bool isMask = false;
        /// <summary>
        /// 是否正在擦除
        /// </summary>
        private bool _isErasing = false;
        /// <summary>
        /// 橡皮擦大小
        /// </summary>
        private double _eraserSize;
        /// <summary>
        /// 掩膜大小
        /// </summary>
        public double EraserSize
        {
            get { return _eraserSize; }
            set { SetProperty(ref _eraserSize, value); }
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
            }
        }

        private int _contrastLow;
        /// <summary>
        /// 对比度（低）
        /// </summary>
        public int ContrastLow
        {
            get { return _contrastLow; }
            set { SetProperty(ref _contrastLow, value); }
        }

        private int _contrastHigh;
        /// <summary>
        /// 对比度（高）
        /// </summary>
        public int ContrastHigh
        {
            get { return _contrastHigh; }
            set { SetProperty(ref _contrastHigh, value); }
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

        /// <summary>
        /// 弹窗服务
        /// </summary>
        readonly IDialogService _dialogService;


        /// <summary>
        /// 匹配结果弹窗是否打开
        /// </summary>
        private bool IsMatchResultDialog = false;

        /// <summary>
        /// 参数设置界面是否打开
        /// </summary>

        private bool IsParameterSettingDialog = false;

        /// <summary>
        /// 模板参数修改界面是否打开
        /// </summary>
        private bool IsTemplateDataEditDialog = false;





        #endregion

        #region 非托管内存
        // ----- 用于显示

        private HObject _displayTemplateImage;
        /// <summary>
        /// 用于显示的模板原始图像
        /// </summary>
        public HObject DisplayTemplateImage
        {
            get { return _displayTemplateImage; }
            set
            {
                // 释放旧对象
                SafeHalconDispose(_displayTemplateImage);
                SetProperty<HObject>(ref _displayTemplateImage, value);
            }
        }

        private HObject _displayTemplate;
        /// <summary>
        /// 用于显示的制作模板的图像
        /// </summary>
        public HObject DisplayTemplate
        {
            get { return _displayTemplate; }
            set
            {
                // 释放旧对象
                SafeHalconDispose(_displayTemplate);
                SetProperty<HObject>(ref _displayTemplate, value);
            }
        }

        private HObject _displayTemplateContour;
        /// <summary>
        /// 用于显示的模板轮廓
        /// </summary>
        public HObject DisplayTemplateContour
        {
            get { return _displayTemplateContour; }
            set
            {
                // 释放旧对象
                SafeHalconDispose(_displayTemplateContour);
                SetProperty<HObject>(ref _displayTemplateContour, value);
            }
        }

        private HObject _displayWheelContour;
        /// <summary>
        /// 用于显示轮毂的轮廓
        /// </summary>
        public HObject DisplayWheelContour
        {
            get { return _displayWheelContour; }
            set
            {
                // 释放旧对象
                SafeHalconDispose(_displayWheelContour);
                SetProperty(ref _displayWheelContour, value);
            }
        }


        // ----- 用于操作
        private HObject _sourceTemplateImage;

        /// <summary>
        /// 读取或采集的原始模板图像
        /// </summary>
        private HObject SourceTemplateImage
        {
            get { return _sourceTemplateImage; }
            set
            {
                SafeHalconDispose(_sourceTemplateImage);
                SetProperty(ref _sourceTemplateImage, value);
            }
        }

        private HObject _ho_Region_Removeds;
        /// <summary>
        /// 掩膜区域
        /// </summary>
        private HObject ho_Region_Removeds
        {
            get { return _ho_Region_Removeds; }
            set
            {
                SafeHalconDispose(_ho_Region_Removeds);
                SetProperty(ref _ho_Region_Removeds, value);
            }
        }

        private HObject _InPoseWheelImage;
        /// <summary>
        /// 定位轮毂获取到的轮毂图像
        /// </summary>
        private HObject InPoseWheelImage
        {
            get { return _InPoseWheelImage; }
            set
            {
                SafeHalconDispose(_InPoseWheelImage);
                SetProperty(ref _InPoseWheelImage, value);
            }
        }

        private HObject _templateImage = new HObject();
        /// <summary>
        ///用于制作和保存的模板图像
        /// </summary>
        private HObject TemplateImage
        {
            get { return _templateImage; }
            set
            {
                SafeHalconDispose(_templateImage);
                SetProperty(ref _templateImage, value);
            }
        }

        /// <summary>
        /// 模板
        /// </summary>
        HTuple hv_ModelID;

        /// <summary>
        /// 模板区域
        /// </summary>
        HObject circleSector;

        /// <summary>
        /// 圆中心Column
        /// </summary>
        double circleCol;
        /// <summary>
        /// 圆中心Row
        /// </summary>
        double circleRow;
        /// <summary>
        /// 圆半径
        /// </summary>
        double circleRadius;
        /// <summary>
        /// 模板中心Row
        /// </summary>
        double TemplateCenterRow;
        /// <summary>
        /// 模板中心Column
        /// </summary>
        double TemplateCenterColumn;

        private List<TemplatedataModel> templateDataList = new List<TemplatedataModel>();
        #endregion

        #region======命令定义======
        /// <summary>
        /// 模板制作按钮命令
        /// </summary>
        public DelegateCommand<string> TemplateBtnCommand { get; set; }

        /// <summary>
        /// 模板数据窗口鼠标左键按下命令
        /// </summary>
        public ICommand MouseLeftButtonDownCommand { get; set; }
        /// <summary>
        /// 模板窗口鼠标移动命令
        /// </summary>
        public ICommand HWindowMouseMoveCommand { get; set; }
        /// <summary>
        /// 模板窗口鼠标按下命令
        /// </summary>
        public ICommand OnHalconMouseDownCommand { get; set; }
        /// <summary>
        /// 模板窗口鼠标抬起命令
        /// </summary>
        public ICommand OnHalconMouseUpCommand { get; set; }

        #endregion



        //匹配相似度结果显示
        List<MatchResultModel> matchResultModels = new List<MatchResultModel>();

        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IEventAggregator _eventAggregator;

        public TemplateViewModel(IDialogService dialogService, IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator)
        {
            InitialPara();

            this._dialogCoordinator = dialogCoordinator;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            TemplateBtnCommand = new DelegateCommand<string>(TemplateButtonCommand);
            MouseLeftButtonDownCommand = new DelegateCommand<object>(MouseLeftButtonDown);
            HWindowMouseMoveCommand = new DelegateCommand<object>(HWindowMouseMove);
            OnHalconMouseDownCommand = new DelegateCommand<object>(OnHalconMouseDown);
            OnHalconMouseUpCommand = new DelegateCommand<object>(OnHalconMouseUp);

            // 初始化命令（带 Halcon 事件参数）
            DisplayTemplateImage = new HObject();
            DisplayTemplate = new HObject();
            DisplayWheelContour = new HObject();
            DisplayTemplateContour = new HObject();

            TemplateDatas = new ObservableCollection<sys_bd_Templatedatamodel>();


            RecognitionResultDisplay = Visibility.Collapsed;
            MASKContent = "图像掩膜";
            EraserSize = 11;

            _eventAggregator.GetEvent<AngleChangeEvent>().Subscribe(AngleChange);

            LoadedTemplateDatas();
        }



        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitialPara()
        {
            var sDB = new SqlAccess().SystemDataAccess;
            List<sys_bd_systemsettingsdatamodel> systemDatas = sDB.Queryable<sys_bd_systemsettingsdatamodel>().ToList();
            //定位
            WheelMinThreshold = int.Parse(GetPara(systemDatas, "WheelMinThreshold", "22"));
            WheelMinRadius = int.Parse(GetPara(systemDatas, "WheelMinRadius", "120"));
            //制作模板时的参数 
            WindowMaxThreshold = int.Parse(GetPara(systemDatas, "WindowMaxThreshold", "100"));
            RemoveMixArea = double.Parse(GetPara(systemDatas, "RemoveMixArea", "60"));
            TemplateStartAngle = double.Parse(GetPara(systemDatas, "TemplateStartAngle", "3.1415"));
            TemplateEndAngle = double.Parse(GetPara(systemDatas, "TemplateEndAngle", "4.7123"));

            // 一般不需要修改下列参数
            AngleStart = double.Parse(GetPara(systemDatas, "AngleStart", "-1.57"));
            AngleExtent = double.Parse(GetPara(systemDatas, "AngleExtent", "1.57"));

            TemplateImagesPath = GetPara(systemDatas, "TemplateImagesPath", "D:\\VisualDatas\\TemplateImages");
            ActiveTemplatesPath = GetPara(systemDatas, "ActiveTemplatesPath", "D:\\VisualDatas\\ActiveTemplate");
            HistoricalImagesPath = GetPara(systemDatas, "HistoricalImagesPath", "D:\\VisualDatas\\HistoricalImages");

        }

        private string GetPara(List<sys_bd_systemsettingsdatamodel> systemDatas, string name, string defaultValue)
        {
            try
            {
                string value = systemDatas.First(x => x.Name == name).Value;
                return value;
            }
            catch (Exception ex)
            {
                SqlAccess.SystemDatasInsertable(name, defaultValue);
                return defaultValue;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Unsubscribe from events and release managed resources
                }

                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        #region  模板窗口显示

        /// <summary>
        /// 模板数据窗口鼠标左键按下
        /// </summary>
        /// <param name="obj"></param>
        private void MouseLeftButtonDown(object obj)
        {
            DataGrid dataGrid = (DataGrid)obj;
            if (dataGrid != null && dataGrid.Items.Count > 0 && dataGrid.CurrentItem != null)
            {
                //获取选中的行索引
                int rowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
                //获取选中的列索引
                int columnIndex = dataGrid.CurrentCell.Column.DisplayIndex;
                //强制分选设置
                if (columnIndex == 4)
                {
                    DataGridSelectedItem = TemplateDatas[DataGridSelectedIndex];
                    //var sDB = new SqlAccess().SystemDataAccess;
                    //sDB.Updateable(DataGridSelectedItem).ExecuteCommand();
                    //sDB.Close(); sDB.Dispose();
                }
            }
        }

        /// <summary>
        /// 模板窗口显示
        /// </summary>
        /// <param name="templateImage">模板图像</param>
        /// <param name="templateContour">模板轮廓</param>
        /// <param name="inGateContour">浇口轮廓</param>
        private void TemplateWindowDisplay(HObject sourceImage, HObject templateImage, HObject wheelContour, HObject templateContour, HObject gateContour)
        {

            DisplayTemplateImage = CloneImageSafely(sourceImage);

            DisplayTemplate = CloneImageSafely(templateImage);
            if (wheelContour != null)
            {
                LineWidth = 4.0;
                DisplayWheelContour = CloneImageSafely(wheelContour);
            }
            if (templateContour != null)
            {
                LineWidth = 3.0;
                DisplayTemplateContour = CloneImageSafely(templateContour);
            }
            if (gateContour != null)
            {
                LineWidth = 3.0;
                //DisplayInGateContour = gateContour;
            }
            //显示完后可以立即清除内存
            SafeHalconDispose(DisplayTemplateImage);
            SafeHalconDispose(DisplayTemplate);
            SafeHalconDispose(DisplayWheelContour);
            SafeHalconDispose(DisplayTemplateContour);
        }

        #endregion

        #region 加载 增加 删除 插入 修改 查找 模板参数 显示 整理数据




        /// <summary>
        /// 软件启动时加载数据
        /// </summary>
        private void LoadedTemplateDatas()
        {
            var db = new SqlAccess().SystemDataAccess;
            List<sys_bd_Templatedatamodel> Datas = db.Queryable<sys_bd_Templatedatamodel>().ToList();
            //Datas.ForEach((temp) => temp.UpdateTime = DateTime.Now);
            DisplayTemplateDatas(Datas);
            db.Close(); db.Dispose();
            try
            {
                for (int i = 0; i < Datas.Count; i++)
                {
                    sys_bd_Templatedatamodel item = Datas[i];
                    TemplatedataModel model = new TemplatedataModel();
                    TimeSpan difference = DateTime.Now - item.LastUsedTime;
                    item.UnusedDays = (int)difference.TotalDays;
                    model.CopyPropertiesFrom(item);
                    templateDataList.Add(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常：{ex.ToString()}");
            }

        }

        /// <summary>
        /// 根据名称获取模板数据 供生成模板区域使用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HTuple GetHTupleByName(string name)
        {
            TemplatedataModel templatedata = templateDataList.Find((TemplatedataModel x) => x.WheelType == name);
            return templatedata.Template;
        }

        /// <summary>
        /// 删除使用区模型
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        private void DeleteModel(string name)
        {
            int index = templateDataList.FindIndex((TemplatedataModel x) => x.WheelType == name);
            if (index != -1)
            {
                templateDataList[index].ReleaseTemplate();
                templateDataList.RemoveAt(index);
            }

        }

        /// <summary>
        /// 更新使用区 直接卸载内存 等下一次使用去加载模板上来
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        private void UpdateTemplate(string name)
        {
            int index = templateDataList.FindIndex((TemplatedataModel x) => x.WheelType == name);
            templateDataList[index].ReleaseTemplate();

        }

        private void AddTemplate(sys_bd_Templatedatamodel item)
        {
            TemplatedataModel model = new TemplatedataModel();
            model.CopyPropertiesFrom(item);
            templateDataList.Add(model);
        }

        /// <summary>
        /// 插入整个数据
        /// </summary>
        /// <param name="newDatas"></param>
        private void InsertTemplateDatas(List<sys_bd_Templatedatamodel> newDatas)
        {
            var db = new SqlAccess().SystemDataAccess;
            db.DbMaintenance.TruncateTable<sys_bd_Templatedatamodel>();
            db.Insertable(newDatas).ExecuteCommand();
            db.Close(); db.Dispose();
        }

        /// <summary>
        /// 修改单个模板参数
        /// </summary>
        /// <param name="newModel"></param>
        private void UpdataTemplateData(sys_bd_Templatedatamodel newModel)
        {
            var db = new SqlAccess().SystemDataAccess;
            db.Updateable(newModel).ExecuteCommand();
            db.Close(); db.Dispose();
        }

        /// <summary>
        /// 查找指定 WheelType 的第一条记录
        /// </summary>
        /// <param name="targetType">要查找的 WheelType</param>
        /// <returns>找到的 Templatedata，未找到时返回null</returns>
        public sys_bd_Templatedatamodel GetTemplatedataForType(string targetType)
        {
            var db = new SqlAccess().SystemDataAccess;
            sys_bd_Templatedatamodel foundItem = db.Queryable<sys_bd_Templatedatamodel>()
                                            .Where(w => w.WheelType == targetType).First();
            db.Close(); db.Dispose();
            // 检查是否找到
            if (foundItem != null)
            {
                return foundItem;
            }

            return null;
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="Datas"></param>
        private void DisplayTemplateDatas(List<sys_bd_Templatedatamodel> Datas)
        {
            //此处必须使用Invoke
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                TemplateDatas.Clear();
                TemplateDatas = new ObservableCollection<sys_bd_Templatedatamodel>(Datas);
            }));
        }

        /// <summary>
        /// 整理TemplateDatas数据并保存数据库
        /// </summary>
        private void OrganizeTemplateDatas()
        {
            //内部数据重新整理 数据根据轮型还有轮毂样式排序
            var datas = TemplateDatas.OrderBy(x => x.WheelType).ThenBy(x => x.WheelStyle).ToList();
            for (int i = 0; i < datas.Count; i++)
            {
                datas[i].Index = i + 1;
            }
            //更新窗口显示
            TemplateDatas.Clear();
            TemplateDatas.AddRange(datas);
            //修改数据库
            InsertTemplateDatas(datas);
        }



        #endregion

        #region   操作模板
        /// <summary>
        /// 模板管理按钮命令响应
        /// </summary>
        /// <param name="obj"></param>
        private void TemplateButtonCommand(string obj)
        {
            if (obj == "添加模板") AddTemplate();
            else if (obj == "参数设置") ParameterSetting();
            else if (obj == "图像掩膜") MASKClick(obj);
            else if (obj == "关闭掩膜") MASKClick(obj);
            else if (obj == "识别测试") RecognitionTest();
            else if (obj == "读取图片") ReadImage();
            else if (obj == "制作模板") PreviewTemplate1();
            else if (obj == "保存") SaveTemplate1();
            else if (obj == "显示模板") DisplayTemplates1();
            else if (obj == "删除模板") DelTemplate();
            else if (obj == "匹配结果") MatchResult();
            else if (obj == "数据校准") Calibration();

            //else if (obj == "模板检查") TemplateExamine();
            else return;
        }

       


        /// <summary>
        /// 添加模板
        /// </summary>
        private void AddTemplate()
        {
            RecognitionResultDisplay = Visibility.Collapsed;
            //GateDetectionVisibility = Visibility.Collapsed;
            //弹出窗口带返回结果写法
            //_dialogService.ShowDialog("WheelTypeSetting", new Action<IDialogResult>(AddWheelResult));
            int ID = TemplateDatas.Count;
            var parameters = new DialogParameters
            {
                { "templateDatas", TemplateDatas }
            };
            _dialogService.ShowDialog("WheelTypeSetting", parameters,
                new Action<IDialogResult>((IDialogResult result) =>
                {
                    if (result.Parameters.Count != 0)
                    {
                        if (result.Parameters.GetValue<string>("set") == "add_OK")
                        {
                            //插入数据库
                            InsertTemplateDatas(TemplateDatas.ToList());
                            int index = Convert.ToInt32(result.Parameters.GetValue<string>("Index"));
                            DataGridSelectedItem = TemplateDatas[index];
                            DataGridSelectedIndex = index;
                            _eventAggregator.GetEvent<ScrollToIndexEvent>().Publish(index);

                        }

                    }
                }));


        }

        /// <summary>
        /// 参数设置
        /// </summary>
        private void ParameterSetting()
        {
            RecognitionResultDisplay = Visibility.Collapsed;
            if (!IsParameterSettingDialog)
            {
                IsParameterSettingDialog = true;

                var parameters = new DialogParameters
                {
                    { "TemplateStartAngle", TemplateStartAngle },
                    { "TemplateEndAngle", TemplateEndAngle },
                    {"ContrastLow",this.ContrastLow},
                    {"ContrastHigh",this.ContrastHigh},
                    {"MinSize",this.MinSize}
                 };
                _dialogService.Show("ParameterSetting", parameters,
                    new Action<IDialogResult>((IDialogResult result) =>
                    {

                        IDialogParameters paraResult = result.Parameters;
                        IsParameterSettingDialog = false;
                        this.IsSet = false;
                        this.ContrastLow = 1;
                        this.ContrastHigh = 1;
                        this.MinSize = 1;
                        if (paraResult.Count != 0)
                        {

                            //WheelMinThreshold = int.Parse(DialogParametersSet(paraResult, "WheelMinThreshold"));
                            //WheelMinRadius = int.Parse(DialogParametersSet(paraResult, "WheelMinRadius"));
                            //WindowMaxThreshold = int.Parse(DialogParametersSet(paraResult, "WindowMaxThreshold"));
                            //RemoveMixArea = double.Parse(DialogParametersSet(paraResult, "RemoveMixArea"));
                            //TemplateStartAngle = double.Parse(DialogParametersSet(paraResult, "TemplateStartAngle"));
                            //TemplateEndAngle = double.Parse(DialogParametersSet(paraResult, "TemplateEndAngle"));
                            //发送指令刷新参数
                            //pipeClientUtility.SendMessage("参数刷新");
                        }
                    }));
            }
        }

        /// <summary>
        /// 掩膜
        /// </summary>
        /// <param name="obj"></param>
        private void MASKClick(string obj)
        {
            if ("图像掩膜" == obj)
            {
                if (InPoseWheelImage != null && InPoseWheelImage.IsInitialized())
                {
                    HObject temp = null; // 创建一个临时变量
                    HOperatorSet.GenEmptyObj(out temp);
                    ho_Region_Removeds = temp; // 然后将临时变量赋值给属性
                    isMask = true;
                    MASKContent = "关闭掩膜";
                }
                //else
                //EventMessage.SystemMessageDisplay("无模板图像", MessageType.Error);


            }
            else
            {
                isMask = false;
                MASKContent = "图像掩膜";
                if (InPoseWheelImage != null && ho_Region_Removeds != null
                    && InPoseWheelImage.IsInitialized() && ho_Region_Removeds.IsInitialized())
                {
                    //HOperatorSet.Difference(TemplateImage, ho_Region_Removeds, out HObject regionDifference); //差集操作（获取未被选中的区域）
                    //HOperatorSet.ReduceDomain(TemplateImage, regionDifference, out HObject TemplateImage1);
                    //SafeHalconDispose(TemplateImage);
                    //SafeHalconDispose(regionDifference);
                    //SafeHalconDispose(ho_Region_Removeds);

                    //TemplateImage = TemplateImage1;
                }

            }
        }

        private void OnHalconMouseDown(object obj)
        {
            var data = obj as HSmartWindowControlWPF.HMouseEventArgsWPF;
            if (data != null && data.Button.ToString() == "Right")
            {
                //Console.WriteLine($"按钮：{data.Button}");
                if (isMask) //掩膜模式
                {
                    _isErasing = true;
                }
            }
        }

        private void OnHalconMouseUp(object obj)
        {
            var data = obj as HSmartWindowControlWPF.HMouseEventArgsWPF;
            if (data != null && data.Button.ToString() == "Right")
                _isErasing = false;
        }

        /// <summary>
        /// 模板窗口鼠标移动
        /// </summary>
        /// <param name="obj"></param>
        private void HWindowMouseMove(object obj)
        {
            var data = obj as HSmartWindowControlWPF.HMouseEventArgsWPF;
            if (data.Button.ToString() == "Right" && _isErasing)
            {
                if (InPoseWheelImage != null && InPoseWheelImage.IsInitialized())
                {

                    CoverUp1(InPoseWheelImage, data.Row, data.Column);
                    TemplateWindowDisplay(null, InPoseWheelImage, null, null, null);


                }
            }
            else
            {

                if (SourceTemplateImage != null && SourceTemplateImage.IsInitialized())
                {
                    try
                    {
                        int row = (int)data.Row;
                        int column = (int)data.Column;
                        HOperatorSet.GetGrayval(SourceTemplateImage, row, column, out HTuple grayval);
                        ImageGrayval = "行: " + row.ToString() + " 列: " + column.ToString() + " 灰度值: " + grayval;
                    }
                    catch { }
                }
            }
        }

        public void CoverUp1(HObject image, double row, double column)
        {
            HObject ho_EraserRegion = null;
            HObject regionDifference = null;
            HObject TemplateImage1 = null;
            HTuple hRow = null;
            HTuple hColumn = null;
            HTuple hEraserSize = null;

            try
            {
                // 创建临时对象
                hRow = new HTuple(row);
                hColumn = new HTuple(column);
                hEraserSize = new HTuple(EraserSize);

                // 生成橡皮擦区域
                HOperatorSet.GenCircle(out ho_EraserRegion, hRow, hColumn, hEraserSize);

                // 执行图像处理
                HOperatorSet.Difference(image, ho_EraserRegion, out regionDifference);
                HOperatorSet.ReduceDomain(image, regionDifference, out TemplateImage1);

                // 先保存旧图像引用
                HObject oldImage = InPoseWheelImage;

                // 更新模板图像
                InPoseWheelImage = TemplateImage1.Clone();

                // 安全释放旧图像
                SafeDisposeHObject(ref oldImage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CoverUp:{ex}");
            }
            finally
            {
                // 确保所有临时对象都被释放
                SafeDisposeHObject(ref ho_EraserRegion);
                SafeDisposeHObject(ref regionDifference);
                SafeDisposeHObject(ref TemplateImage1);
                SafeDisposeHTuple(ref hRow);
                SafeDisposeHTuple(ref hColumn);
                SafeDisposeHTuple(ref hEraserSize);
            }
        }

        /// <summary>
        /// 识别测试
        /// </summary>
        private async void RecognitionTest()
        {



            if (templateDataList.Count == 0)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"无模板数据,请先录入模板！");
                //EventMessage.SystemMessageDisplay("无模板数据，请先录入模板!", MessageType.Warning);
                return;
            }
            if (SourceTemplateImage == null || !SourceTemplateImage.IsInitialized())
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"无图像数据，请先执行采集图像或读取图片！");
                //EventMessage.SystemMessageDisplay("无图像数据，请先执行采集图像或读取图片!", MessageType.Warning);
                return;
            }

            RecognitionWheelType = "NG";
            RecognitionSimilarity = "0";
            DateTime startTime = DateTime.Now;


            //存储识别结果
            RecognitionResultModel recognitionResult = new RecognitionResultModel();
            List<RecognitionResultModel> list = new List<RecognitionResultModel>();
            recognitionResult = WheelRecognitionAlgorithm1(SourceTemplateImage, templateDataList, AngleStart, AngleExtent, MinSimilarity, out list);



            FullGary = recognitionResult.FullFigureGary.ToString();
            Style = recognitionResult.WheelStyle;

            DateTime endTime = DateTime.Now;
            HObject templateContour = null;
            HObject contoursAffineTrans = null;
            HObject wheelContour = null;
            if (recognitionResult.RecognitionWheelType != "NG")
            {
                //定位到识别的轮型
                int index = TemplateDatas
                .Select((item, idx) => new { Item = item, Index = idx })
                .FirstOrDefault(x => x.Item.WheelType == recognitionResult.RecognitionWheelType)?.Index ?? -1;
                if (index != -1)
                {
                    DataGridSelectedItem = TemplateDatas[index];
                    DataGridSelectedIndex = index;
                    _eventAggregator.GetEvent<ScrollToIndexEvent>().Publish(index);
                }

                //识别成功后 把这个轮形上次使用时间刷新
                //string _type = recognitionResult.RecognitionWheelType;
                //var results = templateDataList
                //    .Where(t => t.WheelType != null &&
                //                t.WheelType == _type);
                //foreach (TemplatedataModel item in results)
                //    item.UseTemplate();

                //显示的轮廓
                templateContour = recognitionResult.RecognitionContour.Clone();
                RecognitionWheelType = recognitionResult.RecognitionWheelType;
                RecognitionSimilarity = recognitionResult.Similarity.ToString();

                if (recognitionResult.HomMat2D != null)
                {
                    HOperatorSet.GenCircleContourXld(out wheelContour, recognitionResult.CenterRow,
                            recognitionResult.CenterColumn, recognitionResult.Radius, 0, (new HTuple(360)).TupleRad(), "positive", 1.0);
                    HOperatorSet.AffineTransContourXld(wheelContour, out contoursAffineTrans, recognitionResult.HomMat2D);

                }
            }

            //显示区域
            //HOperatorSet.GetDomain(SourceTemplateImage, out HObject imageDomain);
            ////外接圆
            //HOperatorSet.SmallestCircle(imageDomain, out HTuple row, out HTuple column, out HTuple radius);
            //HOperatorSet.GenCircleContourXld(out HObject CircleCir, row, column, radius, 0, (new HTuple(360)).TupleRad(), "positive", 1.0);

            TemplateWindowDisplay(SourceTemplateImage, null, contoursAffineTrans, templateContour, null);
            //匹配相似度结果显示          
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Dispose();
                //最后识别时间更新
                MatchResultModel data = new MatchResultModel
                {
                    Index = i + 1,
                    WheelType = list[i].RecognitionWheelType,
                    Similarity = list[i].Similarity.ToString(),
                    FullFigureGary = list[i].FullFigureGary.ToString(),
                    InnerCircleGary = list[i].InnerCircleGary.ToString()
                };
                matchResultModels.Add(data);
            }
            _eventAggregator.GetEvent<WindowCommunicationEvent>().Publish(matchResultModels);

            matchResultModels.Clear();
            recognitionResult.Dispose();
            list.Clear();
            recognitionResult = null;
            SafeHalconDispose(templateContour);
            SafeHalconDispose(wheelContour);
            SafeHalconDispose(contoursAffineTrans);

            TimeSpan consumeTime = endTime.Subtract(startTime);
            RecognitionConsumptionTime = Convert.ToString(Convert.ToInt32(consumeTime.TotalMilliseconds)) + " ms"; ;
            RecognitionResultDisplay = Visibility.Visible;



        }

        /// <summary>
        /// 读取图像
        /// </summary>
        private void ReadImage()
        {
            RecognitionResultDisplay = Visibility.Collapsed;
            var path = HistoricalImagesPath + @"\" + DateTime.Now.Month.ToString() + @"月\" + DateTime.Now.Day.ToString() + @"日";
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = HistoricalImagesPath,
                Title = "请选择要制作模板的图片，当日图像存储路径为：" + path,
                Filter = "TIF文件|*.tif|JPEG文件|*.jpg|BMP文件|*.bmp|PNG文件|*.png|所有文件(*.*)|*.*",//文件筛选器设定
                FilterIndex = 1,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var File_Name = openFileDialog.FileName;
                    SafeHalconDispose(SourceTemplateImage);
                    HOperatorSet.ReadImage(out HObject image, File_Name);
                    SourceTemplateImage = image;

                    TemplateWindowDisplay(SourceTemplateImage, null, null, null, null);
                }
                catch (Exception ex)
                {
                    //EventMessage.SystemMessageDisplay(ex.Message, MessageType.Error);
                }
            }
        }

        /// <summary>
        /// 手动定位轮毂
        /// </summary>
        public async void ManuslPositionHub(double row, double col, double radius)
        {
            if (SourceTemplateImage == null || !SourceTemplateImage.IsInitialized())
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"请先执行读取图片！");
                //EventMessage.SystemMessageDisplay("请先执行读取图片或采集图像!", MessageType.Warning);
                return;
            }
            try
            {

                PositioningWheelResultModel pResult = ManuslPositioningWheel(SourceTemplateImage, row, col, radius);



                InPoseWheelImage = CloneImageSafely(pResult.WheelImage);
                circleRow = row;
                circleCol = col;
                circleRadius = radius;
                TemplateWindowDisplay(SourceTemplateImage, null, pResult.WheelContour, null, null);
                pResult.WheelContour.Dispose();
                FullGary = pResult.FullFigureGary.ToString();
                pResult.Dispose();
                pResult = null;

            }
            catch (Exception ex)
            {
                //EventMessage.SystemMessageDisplay(ex.Message, MessageType.Error);
                return;
            }
        }


        /// <summary>
        /// 制作模板
        /// </summary>
        private async void PreviewTemplate1()
        {


            if (InPoseWheelImage == null || !InPoseWheelImage.IsInitialized())
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"请先执行定位轮毂！").ContinueWith(t => Console.WriteLine(t.Result));

                return;
            }

            //扇形区域
            HOperatorSet.GenCircleSector(out circleSector, circleRow, circleCol, circleRadius, TemplateStartAngle, TemplateEndAngle);
            //剪切制作模板的区域
            HOperatorSet.ReduceDomain(InPoseWheelImage, circleSector, out HObject reduced);

            HObject ho_ModelContours, ho_TransContours;

            HTuple hv_ModelRegionArea = new HTuple();
            HTuple hv_RefRow = new HTuple(), hv_RefColumn = new HTuple();
            HTuple hv_HomMat2D = new HTuple();


            HOperatorSet.CreateGenericShapeModel(out hv_ModelID);
            HOperatorSet.SetGenericShapeModelParam(hv_ModelID, "metric", "use_polarity");
            if (IsSet && ContrastLow != 1 && ContrastHigh != 1 && MinSize != 1)
            {
                HOperatorSet.SetGenericShapeModelParam(hv_ModelID, "contrast_high", ContrastHigh);
                HOperatorSet.SetGenericShapeModelParam(hv_ModelID, "contrast_low", ContrastLow);
                HOperatorSet.SetGenericShapeModelParam(hv_ModelID, "min_size", MinSize);
            }


            HOperatorSet.TrainGenericShapeModel(reduced, hv_ModelID);


            //显示轮毂线条
            HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);
            HOperatorSet.AreaCenter(circleSector, out hv_ModelRegionArea, out hv_RefRow, out hv_RefColumn);
            HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RefRow, hv_RefColumn, 0, out hv_HomMat2D);
            HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_TransContours, hv_HomMat2D);
            TemplateWindowDisplay(InPoseWheelImage, null, null, ho_TransContours, null);

            //记录信息
            TemplateCenterRow = hv_RefRow.D;
            TemplateCenterColumn = hv_RefColumn.D;

            //数据参数反馈
            HOperatorSet.GetGenericShapeModelParam(hv_ModelID, "contrast_high", out HTuple hv_contrast_high);
            HOperatorSet.GetGenericShapeModelParam(hv_ModelID, "contrast_low", out HTuple hv_contrast_low);
            HOperatorSet.GetGenericShapeModelParam(hv_ModelID, "min_size", out HTuple hv_min_size);
            _eventAggregator.GetEvent<ShapeModelParamEvent>().Publish(new ShapeModelParam()
            {
                ContrastHigh = hv_contrast_high.I,
                ContrastLow = hv_contrast_low.I,
                MinSize = hv_min_size.I
            });
            this.ContrastHigh = hv_contrast_high.I;
            this.ContrastLow = hv_contrast_low.I;
            this.MinSize = hv_min_size.I;




            SafeDisposeHObject(ref ho_ModelContours);
            SafeDisposeHObject(ref ho_TransContours);

            SafeDisposeHTuple(ref hv_ModelRegionArea);
            SafeDisposeHTuple(ref hv_RefRow);
            SafeDisposeHTuple(ref hv_RefColumn);
            SafeDisposeHTuple(ref hv_HomMat2D);

            SafeDisposeHTuple(ref hv_contrast_high);
            SafeDisposeHTuple(ref hv_contrast_low);
            SafeDisposeHTuple(ref hv_min_size);


        }

        /// <summary>
        /// 保存
        /// </summary>
        private async void SaveTemplate1()
        {
            if (DataGridSelectedItem == null)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"请先选择轮型！");
                //EventMessage.SystemMessageDisplay("请先选择轮型!", MessageType.Warning);
                return;
            }
            if (hv_ModelID == null || hv_ModelID.Length == 0)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"请先执行制作模板！");

                return;
            }
            MessageDialogResult dialogResult = await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"您选择的轮型是！：《 {DataGridSelectedItem.WheelType} 》，请确定轮型选择正确后，再点击确认！", MessageDialogStyle.AffirmativeAndNegative);

            bool result = dialogResult == MessageDialogResult.Affirmative ? true : false; //WMessageBox.Show("保存确认", "您选择的轮型是：《" + DataGridSelectedItem.WheelType + "》，请确定轮型选择正确后，再点击确认！");
            if (result)
            {
                try
                {
                    //hv_ModelID
                    bool isUpdate = false;
                    string tPath = TemplateImagesPath.Replace(@"\", "/") + @"/" + DataGridSelectedItem.WheelType + ".tif"; //直接覆盖保存
                    //Halcon中的路径                                                                                                                                                                                                        
                    //string aPath = ActiveTemplatesPath.Replace(@"\", "/") + @"/" + DataGridSelectedItem.WheelType + ".ncm";
                    string aPath = ActiveTemplatesPath.Replace(@"\", "/") + @"/" + DataGridSelectedItem.WheelType + ".shm";
                    string existPath = aPath.Replace("/", @"\");
                    string bParh = TemplateImagesPath.Replace(@"\", "/") + @"/" + DataGridSelectedItem.WheelType + ".hobj";
                    if (File.Exists(existPath))
                    {
                        isUpdate = true;
                    }
                    HOperatorSet.WriteShapeModel(hv_ModelID, aPath);
                    HOperatorSet.WriteRegion(circleSector, bParh);
                    HOperatorSet.WriteImage(InPoseWheelImage, "tiff", 0, tPath);
                    // 在后台线程执行耗时操作
                    //await Task.Run(async () =>
                    //{

                    //    // 异步保存模板文件
                    //    await Task.WhenAll(
                    //        Task.Run(() => HOperatorSet.WriteShapeModel(hv_ModelID, aPath)),
                    //        Task.Run(() => HOperatorSet.WriteRegion(circleSector, bParh)),
                    //        Task.Run(() => HOperatorSet.WriteImage(InPoseWheelImage, "tiff", 0, tPath)
                    //        )
                    //    );
                    //});


                    if (isUpdate)
                    {
                        DataGridSelectedItem.UpdateTime = DateTime.Now;
                    }
                    else
                    {
                        DataGridSelectedItem.CreationTime = DateTime.Now.ToString("yy-MM-dd HH:mm");
                        DataGridSelectedItem.UpdateTime = DateTime.Now;
                    }
                    DataGridSelectedItem.FullGary = float.Parse(FullGary);
                    DataGridSelectedItem.TemplatePath = aPath;
                    DataGridSelectedItem.TemplatePicturePath = tPath;
                    DataGridSelectedItem.LastUsedTime = DateTime.Now;
                    DataGridSelectedItem.PositionCircleRow = (float)circleRow;
                    DataGridSelectedItem.PositionCircleColumn = (float)circleCol;
                    DataGridSelectedItem.PositionCircleRadius = (float)circleRadius;
                    DataGridSelectedItem.CircumCircleRadius = (float)circleRadius;
                    DataGridSelectedItem.TemplateAreaCenterRow = (float)TemplateCenterRow;
                    DataGridSelectedItem.TemplateAreaCenterColumn = (float)TemplateCenterColumn;
                    if (isUpdate) //更新
                    {
                        UpdateTemplate(DataGridSelectedItem.WheelType);
                    }
                    else
                    {
                        //新增
                        AddTemplate(DataGridSelectedItem);
                    }
                    UpdataTemplateData(DataGridSelectedItem); // 数据层更新
                    //恢复参数默认值
                    _eventAggregator.GetEvent<SetShowEvent>().Publish(false);
                    this.IsSet = false;
                    this.ContrastHigh = 1;
                    this.ContrastLow = 1;
                    this.MinSize = 1;
                    _eventAggregator.GetEvent<ShapeModelParamEvent>().Publish(new ShapeModelParam()
                    {
                        ContrastHigh = this.ContrastHigh,
                        ContrastLow = this.ContrastLow,
                        MinSize = this.MinSize
                    });
                }
                finally
                {
                    // 确保资源释放
                    SafeHalconDispose(hv_ModelID);
                    SafeHalconDispose(circleSector);
                    SafeHalconDispose(SourceTemplateImage);
                    SafeHalconDispose(InPoseWheelImage);
                    SafeHalconDispose(TemplateImage);
                    SafeHalconDispose(ho_Region_Removeds);

                }

                //EventMessage.MessageHelper.GetEvent<TemplateClearEvent>().Publish(string.Empty);
                //EventMessage.SystemMessageDisplay("模板保存成功，型号是：" + DataGridSelectedItem.WheelType, MessageType.Success);
                //EventMessage.MessageDisplay("模板保存成功，型号是：" + DataGridSelectedItem.WheelType, true, true);
                pipeClientUtility.SendMessage("传统视觉模板刷新");

            }

        }

        /// <summary>
        /// 显示模板
        /// </summary>
        private async void DisplayTemplates1()
        {
            //GetAllImageTemplateCenter();
            // GetAllImageSmallestCircle();


            if (DataGridSelectedItem == null)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"请先选择轮型！").ContinueWith(t => Console.WriteLine(t.Result));
                //EventMessage.SystemMessageDisplay("请先选择轮型!", MessageType.Warning);
                return;
            }
            string aPath = ActiveTemplatesPath.Replace(@"\", "/") + @"/" + DataGridSelectedItem.WheelType + ".shm";
            string bParh = TemplateImagesPath.Replace(@"\", "/") + @"/" + DataGridSelectedItem.WheelType + ".hobj";
            string strPath = DataGridSelectedItem.TemplatePicturePath;



            if (File.Exists(strPath))
            {
                HOperatorSet.ReadImage(out HObject Image, strPath);
                TemplateImage = Image.Clone();
                //显示区域
                HOperatorSet.GetDomain(TemplateImage, out HObject imageDomain);
                //外接圆
                HOperatorSet.SmallestCircle(imageDomain, out HTuple row, out HTuple column, out HTuple radius);

                //HOperatorSet.InnerCircle(imageDomain, out HTuple row, out HTuple column, out HTuple radius);
                HOperatorSet.GenCircleContourXld(out HObject wheelContour, row, column, radius, 0, (new HTuple(360)).TupleRad(), "positive", 1.0);

                HTuple hv_ModelID;
                HObject ho_ModelContours;
                HOperatorSet.ReadShapeModel(aPath, out hv_ModelID);
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);

                HOperatorSet.AreaCenterXld(ho_ModelContours, out HTuple area, out row, out column, out HTuple point);
                Console.WriteLine($"中心row:{row.D} col:{column.D}");
                HObject ho_TransContours = new HObject();
                //读取模板 与 模板区域
                if (File.Exists(aPath) && File.Exists(bParh))
                {
                    HTuple hv_ModelRegionArea = new HTuple();
                    HTuple hv_RefRow = new HTuple(), hv_RefColumn = new HTuple();
                    HTuple hv_HomMat2D = new HTuple();



                    HObject ho_circleSector;


                    HOperatorSet.ReadRegion(out ho_circleSector, bParh);

                    //显示轮毂线条
                    HOperatorSet.AreaCenter(ho_circleSector, out hv_ModelRegionArea, out hv_RefRow, out hv_RefColumn);
                    HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RefRow, hv_RefColumn, 0, out hv_HomMat2D);

                    HTuple matchRow, matchCol;


                    HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_TransContours, hv_HomMat2D);
                    SafeHalconDispose(hv_RefRow);
                    SafeHalconDispose(hv_RefColumn);
                    SafeHalconDispose(hv_HomMat2D);


                    SafeHalconDispose(ho_circleSector);

                }
                SafeHalconDispose(ho_ModelContours);
                SafeHalconDispose(hv_ModelID);

                //TemplateWindowDisplay(InPoseWheelImage, null, null, ho_TransContours, null);

                //InnerCircleGary = DataGridSelectedItem.InnerCircleGary.ToString();
                TemplateWindowDisplay(Image, null, wheelContour, ho_TransContours, null);
                SafeHalconDispose(Image);
                SafeHalconDispose(ho_TransContours);


            }
            else
            {
                TemplateWindowDisplay(null, null, null, null, null);

                //EventMessage.SystemMessageDisplay("轮型" + DataGridSelectedItem.WheelType + "无模板图像，请先录入模板!", MessageType.Warning);
            }




        }

        /// <summary>
        /// 删除模板
        /// </summary>
        private async void DelTemplate()
        {
            if (TemplateDatas.Count == 0)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"无模板数据，请先录入模板！").ContinueWith(t => Console.WriteLine(t.Result));
                return;
            }
            if (DataGridSelectedItem == null)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"请先选择轮型！").ContinueWith(t => Console.WriteLine(t.Result));
                return;
            }
            MessageDialogResult dialogResult = await this._dialogCoordinator.ShowMessageAsync(this, "删除确认", $"确定删除模板 {DataGridSelectedItem.WheelType} 吗？", MessageDialogStyle.AffirmativeAndNegative);

            bool result = dialogResult == MessageDialogResult.Affirmative ? true : false; //WMessageBox.Show("删除确认", "确定删除模板：" + DataGridSelectedItem.WheelType + " 吗？");
            if (result)
            {



                if (File.Exists(DataGridSelectedItem.TemplatePicturePath))
                    File.Delete(DataGridSelectedItem.TemplatePicturePath);
                //删除轮型模板
                if (File.Exists(DataGridSelectedItem.TemplatePath))
                    File.Delete(DataGridSelectedItem.TemplatePath);

                //删除样式               
                string bParh = TemplateImagesPath.Replace(@"\", "/") + @"/" + DataGridSelectedItem.WheelType + ".hobj";
                if (File.Exists(bParh))
                    File.Delete(bParh);

                int deleteIndex = DataGridSelectedItem.Index - 1; //被删除项
                string wheelType = DataGridSelectedItem.WheelType;

                DeleteModel(wheelType);
                TemplateDatas.RemoveAt(deleteIndex);

                OrganizeTemplateDatas();


                //光标显示
                if (TemplateDatas.Count - 1 >= deleteIndex)
                {
                    DataGridSelectedItem = TemplateDatas[deleteIndex];
                    DataGridSelectedIndex = deleteIndex;
                    //EventMessage.MessageHelper.GetEvent<TemplateDataEditEvent>().Publish(DataGridSelectedItem);
                }
                else if (TemplateDatas.Count - 1 < deleteIndex && deleteIndex != 0)
                {
                    DataGridSelectedItem = TemplateDatas[deleteIndex - 1];
                    DataGridSelectedIndex = deleteIndex - 1;
                    //EventMessage.MessageHelper.GetEvent<TemplateDataEditEvent>().Publish(DataGridSelectedItem);
                }
                else
                {
                    DataGridSelectedItem = null;
                    DataGridSelectedIndex = -1;
                }


                await this._dialogCoordinator.ShowMessageAsync(this, "提示", $"模板删除成功，轮型是:{wheelType}").ContinueWith(t => Console.WriteLine(t.Result));
                pipeClientUtility.SendMessage("传统视觉模板刷新");

            }
        }


        /// <summary>
        /// 匹配结果
        /// </summary>
        private void MatchResult()
        {

            if (!IsMatchResultDialog)
            {
                IsMatchResultDialog = true;
                _dialogService.Show("MatchResult", dialogResult =>
                {
                    IsMatchResultDialog = false;
                });
            }

        }

        private void Calibration()
        {
            if (DataGridSelectedItem != null)
            {
                string strPath = DataGridSelectedItem.TemplatePicturePath;
                if (File.Exists(strPath))
                {
                    HOperatorSet.ReadImage(out HObject Image, strPath);
                    SourceTemplateImage = Image.Clone();
                    RecognitionTest();
                    TemplatedataModel target = templateDataList.FirstOrDefault(t => t.WheelType == RecognitionWheelType);
                    Console.WriteLine($"数据校准 - hv_RefRow:{target.TemplateAreaCenterRow} hv_RefColumn:{target.TemplateAreaCenterColumn}");

                }

            }
        }


        public async void GetAllImageTemplateCenter()
        {
            foreach (sys_bd_Templatedatamodel item in TemplateDatas)
            {
                //if (item.TemplateAreaCenterRow == 0 && item.TemplateAreaCenterColumn == 0)
                {
                    string strPath = item.TemplatePicturePath;
                    Console.WriteLine($"下标：{item.Index} 地址：{strPath}");


                    if (File.Exists(strPath))
                    {
                        HOperatorSet.ReadImage(out HObject Image, strPath);
                        SourceTemplateImage = Image.Clone();



                        RecognitionTest();
                        TemplatedataModel target = templateDataList.FirstOrDefault(t => t.WheelType == RecognitionWheelType);
                        item.TemplateAreaCenterRow = target.TemplateAreaCenterRow;
                        item.TemplateAreaCenterColumn = target.TemplateAreaCenterColumn;
                        Console.WriteLine($"结果参数 - hv_RefRow3:{item.TemplateAreaCenterRow} hv_RefColumn3:{item.TemplateAreaCenterColumn}");


                    }
                    else
                    {
                        TemplateWindowDisplay(null, null, null, null, null);

                    }
                    await Task.Delay(500);
                }


            }
            //OrganizeTemplateDatas();

        }

        /// <summary>
        /// 获取所有图像的手动定位外接圆
        /// </summary>
        public async void GetAllImageSmallestCircle()
        {


            foreach (sys_bd_Templatedatamodel item in TemplateDatas)
            {

                await Task.Delay(200);
                DataGridSelectedItem = item;
                DataGridSelectedIndex = item.Index;
                _eventAggregator.GetEvent<ScrollToIndexEvent>().Publish(item.Index);
                string strPath = item.TemplatePicturePath;
                if (File.Exists(strPath))
                {


                    HOperatorSet.ReadImage(out HObject Image, strPath);
                    TemplateImage = Image.Clone();
                    //显示区域
                    HOperatorSet.GetDomain(TemplateImage, out HObject imageDomain);
                    //外接圆
                    HOperatorSet.SmallestCircle(imageDomain, out HTuple row, out HTuple column, out HTuple radius);
                    HOperatorSet.GenCircleContourXld(out HObject wheelContour, row, column, radius, 0, (new HTuple(360)).TupleRad(), "positive", 1.0);
                    Console.WriteLine($"外接圆 - row:{row} column:{column} radius:{radius}");
                    item.PositionCircleRow = (float)row.D;
                    item.PositionCircleColumn = (float)column.D;
                    item.PositionCircleRadius = (float)radius.D;
                    item.CircumCircleRadius = (float)radius.D;
                    string aPath = ActiveTemplatesPath.Replace(@"\", "/") + @"/" + item.WheelType + ".shm";
                    string bParh = TemplateImagesPath.Replace(@"\", "/") + @"/" + item.WheelType + ".hobj";

                    HTuple hv_ModelID;
                    HObject ho_ModelContours;
                    HOperatorSet.ReadShapeModel(aPath, out hv_ModelID);
                    HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);

                    //HOperatorSet.AreaCenterXld(ho_ModelContours, out HTuple area, out row, out column, out HTuple point);
                    //Console.WriteLine($"中心row:{row.D} col:{column.D}");
                    HObject ho_TransContours = new HObject();
                    //读取模板 与 模板区域
                    if (File.Exists(aPath) && File.Exists(bParh))
                    {
                        HTuple hv_ModelRegionArea = new HTuple();
                        HTuple hv_RefRow = new HTuple(), hv_RefColumn = new HTuple();
                        HTuple hv_HomMat2D = new HTuple();



                        HObject ho_circleSector;


                        HOperatorSet.ReadRegion(out ho_circleSector, bParh);

                        //显示轮毂线条
                        HOperatorSet.AreaCenter(ho_circleSector, out hv_ModelRegionArea, out hv_RefRow, out hv_RefColumn);
                        HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RefRow, hv_RefColumn, 0, out hv_HomMat2D);
                        Console.WriteLine($"转换参数 - hv_RefRow:{hv_RefRow} hv_RefColumn:{hv_RefColumn}");
                        item.TemplateAreaCenterRow = (float)hv_RefRow.D;
                        item.TemplateAreaCenterColumn = (float)hv_RefColumn.D;
                        HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_TransContours, hv_HomMat2D);
                        Console.WriteLine($"hv_HomMat2D:{hv_HomMat2D.ToString()}");


                        //-----验证----

                        //// 计算模板轮廓的中心
                        HObject ho_ModelRegion;
                        HOperatorSet.GenRegionContourXld(ho_ModelContours, out ho_ModelRegion, "filled");
                        HTuple hv_AreaModel, hv_RowModel, hv_ColModel;
                        HOperatorSet.AreaCenter(ho_ModelRegion, out hv_AreaModel, out hv_RowModel, out hv_ColModel);

                        // 计算变换后轮廓的中心
                        HObject ho_TransRegion;
                        HOperatorSet.GenRegionContourXld(ho_TransContours, out ho_TransRegion, "filled");
                        HTuple hv_AreaTrans, hv_RowTrans, hv_ColTrans;
                        HOperatorSet.AreaCenter(ho_TransRegion, out hv_AreaTrans, out hv_RowTrans, out hv_ColTrans);

                        // 计算平移量

                        HTuple hv_RefRow1 = hv_RowTrans[hv_RowTrans.Length - 1].D - hv_RowModel[hv_RowModel.Length - 1].D;
                        HTuple hv_RefColumn1 = hv_ColTrans[hv_ColTrans.Length - 1].D - hv_ColModel[hv_ColModel.Length - 1].D;
                        Console.WriteLine($"验证参数 - hv_RefRow1:{hv_RefRow1} hv_RefColumn1:{hv_RefColumn1}");


                        //-----验证-----




                        SafeHalconDispose(hv_RefRow);
                        SafeHalconDispose(hv_RefColumn);
                        SafeHalconDispose(hv_HomMat2D);


                        SafeHalconDispose(ho_circleSector);

                    }





                    TemplateWindowDisplay(TemplateImage, null, wheelContour, ho_TransContours, null);
                    SafeHalconDispose(Image);


                }
                else
                {
                    TemplateWindowDisplay(null, null, null, null, null);

                }

            }


            //OrganizeTemplateDatas();

        }

        /// <summary>
        /// 所有图像的平均灰度值
        /// </summary>
        public void GetAllImageGray()
        {

            foreach (sys_bd_Templatedatamodel item in TemplateDatas)
            {
                string strPath = item.TemplatePicturePath;
                HOperatorSet.ReadImage(out HObject Image, strPath);
                HObject rgbImage = GrayTransRGB(Image);
                HObject grayImage = RGBTransGray(rgbImage);
                float fullGray = (float)GetIntensity(grayImage);
                item.FullGary = fullGray;
                Console.WriteLine($"{item.WheelType} 图片全局灰度值：{fullGray}");
                SafeHalconDispose(Image);
                SafeHalconDispose(rgbImage);
                SafeHalconDispose(grayImage);
            }


        }


        /// <summary>
        /// 参数修改后更新参数与保存
        /// </summary>
        /// <param name="angle"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AngleChange(ShapeModelParam angle)
        {
            this.IsSet = angle.IsSet;
            this.ContrastLow = angle.ContrastLow;
            this.ContrastHigh = angle.ContrastHigh;
            this.MinSize = angle.MinSize;

            TemplateStartAngle = angle.TemplateStartAngle;
            TemplateEndAngle = angle.TemplateEndAngle;

            //写入数据库
            SqlAccess.SystemDatasUpdateable("TemplateStartAngle", TemplateStartAngle.ToString());
            SqlAccess.SystemDatasUpdateable("TemplateEndAngle", TemplateEndAngle.ToString());
        }

        //private string DialogParametersSet(IDialogParameters paraResult, string name)
        //{
        //    string value = paraResult.GetValue<string>(name);
        //    if (value != null)
        //    {
        //        SqlAccess.SystemDatasUpDialogParameterseable(name, value);
        //    }
        //    return value;
        //}








        /// <summary>
        /// 打开修改模板参数界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsTemplateDataEditDialog)
            {
                IsTemplateDataEditDialog = true;
                var parameters = new DialogParameters
                {
                        { "para", DataGridSelectedItem.Clone()}
                };
                _dialogService.Show("TemplateDataEdit", parameters, result =>
                {
                    // 处理结果
                    if (result.Parameters.Count != 0 && result.Parameters.ContainsKey("Result"))
                    {

                        sys_bd_Templatedatamodel returnItem = result.Parameters.GetValue<sys_bd_Templatedatamodel>("Result");
                        //界面显示
                        DataGridSelectedItem.WheelHeight = returnItem.WheelHeight;
                        DataGridSelectedItem.WheelStyle = returnItem.WheelStyle;
                        DataGridSelectedItem.FullGary = returnItem.FullGary;
                        DataGridSelectedItem.TemplateAreaCenterRow = returnItem.TemplateAreaCenterRow;
                        DataGridSelectedItem.TemplateAreaCenterColumn = returnItem.TemplateAreaCenterColumn;
                        //数据保存
                        UpdataTemplateData(DataGridSelectedItem);
                        TemplatedataModel target = templateDataList.FirstOrDefault(t => t.WheelType == returnItem.WheelType);
                        target.WheelHeight = returnItem.WheelHeight;
                        target.WheelStyle = returnItem.WheelStyle;
                        target.FullGary = returnItem.FullGary;
                        target.TemplateAreaCenterRow = returnItem.TemplateAreaCenterRow;
                        target.TemplateAreaCenterColumn = returnItem.TemplateAreaCenterColumn;
                    }
                    IsTemplateDataEditDialog = false;
                });
            }

        }


        #endregion


    }



}

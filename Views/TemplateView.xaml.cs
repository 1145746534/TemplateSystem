using HalconDotNet;
using Prism.Events;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using TemplateSystem.Models;
using TemplateSystem.ViewModels;

namespace TemplateSystem.Views
{
    /// <summary>
    /// TemplateManagementViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateView : UserControl
    {

        private readonly IEventAggregator _eventAggregator;

        public TemplateView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            //订阅模板数据编辑的消息
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ScrollToIndexEvent>().Subscribe(ScrollToIndex);
        }

        private void TemplateDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var viewModel = this.DataContext as TemplateViewModel;
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid != null && dataGrid.Items.Count > 0 && dataGrid.CurrentItem != null)
            {
                viewModel.DataGridSelectedItem = dataGrid.CurrentItem as sys_bd_Templatedatamodel;
                viewModel.DataGridSelectedIndex = viewModel.DataGridSelectedItem.Index - 1;

                ContextMenu contextMenu = new ContextMenu();
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "修改";
                menuItem.FontSize = 14;
                menuItem.Click += viewModel.MenuItem_Click;
                contextMenu.Items.Add(menuItem);
                contextMenu.IsOpen = true;
            }
        }

        private void ScrollToIndex(int index)
        {
           
            if (TemplateDataGrid.Items.Count > index)
            {
                TemplateDataGrid.ScrollIntoView(TemplateDataGrid.Items[index]);
            }
        }

        HTuple hv_DrawID;
        HTuple hv_row;
        HTuple hv_column;
        HTuple hv_radius;

        private void btn_Circle_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string content = string.Empty;
            if (btn_Circle.Content.ToString() == "画圆")
            {
                StartDrawCircle();
                content = "结束画圆";
            }
            if (btn_Circle.Content.ToString() == "结束画圆")
            {
                EndDrawCircle();
                content = "画圆";
            }
            btn_Circle.Content = content;
        }

        public void StartDrawCircle()
        {
            HOperatorSet.CreateDrawingObjectCircle(800, 1000, 80, out hv_DrawID);
            HOperatorSet.AttachDrawingObjectToWindow(halconWPF.HalconWindow, hv_DrawID);
            // 



        }

        /// <summary>
        /// 结束画圆
        /// </summary>
        public void EndDrawCircle()
        {
            HOperatorSet.GetDrawingObjectParams(hv_DrawID, "row", out hv_row);
            HOperatorSet.GetDrawingObjectParams(hv_DrawID, "column", out hv_column);
            HOperatorSet.GetDrawingObjectParams(hv_DrawID, "radius", out hv_radius);
            Console.WriteLine($"画圆的行：{hv_row} 列：{hv_column} 半径{hv_radius}");
            var viewModel = this.DataContext as TemplateViewModel;
            viewModel.ManuslPositionHub(hv_row.D, hv_column.D, hv_radius.D);
            if (hv_DrawID != null)
            {
                HOperatorSet.DetachDrawingObjectFromWindow(halconWPF.HalconWindow, hv_DrawID);
                hv_DrawID?.Dispose();

            }           
            hv_row.Dispose();
            hv_column.Dispose();
            hv_radius.Dispose();
        }

    }
}

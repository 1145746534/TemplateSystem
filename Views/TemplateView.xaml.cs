using HalconDotNet;
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


        public TemplateView()
        {
            InitializeComponent();
            //订阅模板数据编辑的消息
            
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

    }
}

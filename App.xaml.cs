using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;
using Prism.Unity;
using TemplateSystem.Views;


namespace TemplateSystem
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App  : PrismApplication
    {
        // 用于进程间通信的Mutex名称（需唯一）
        private  string MutexName = "123123123123Q111";

        private static Mutex _appMutex;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //根据任务栏应用程序显示的名称找窗口的名称
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private const int SW_RESTORE = 9;
        private const int SW_MAXIMIZE = 3;

        protected override void OnStartup(StartupEventArgs e)
        {

            try
            {
                // 尝试创建 Mutex
                _appMutex = new Mutex(true, MutexName, out bool createdNew);

                if (!createdNew)
                {
                    IntPtr findPtr = FindWindow(null, "模板修改");
                    if (findPtr.ToInt32() != 0)
                    {
                        ShowWindow(findPtr, SW_RESTORE); //将窗口还原，如果不用此方法，缩小的窗口不能激活
                        SetForegroundWindow(findPtr);//将指定的窗口选中(激活)
                    }
                    //ActivateExistingInstance();
                    Current.Shutdown();
                    return;
                }
            }
            catch (Exception ex)
            {
                // 记录错误但继续启动
                Debug.WriteLine($"Mutex 创建失败: {ex.Message}");
            }

            base.OnStartup(e);

        }

       

        private void ActivateExistingInstance()
        {
            try
            {
                // 获取当前进程名称（不含扩展名）
                var currentProcessName = Process.GetCurrentProcess().ProcessName;

                // 查找同名的所有进程
                var processes = Process.GetProcessesByName(currentProcessName);

                // 排除当前进程（新启动的实例）
                var existingProcess = processes.FirstOrDefault(p => p.Id != Process.GetCurrentProcess().Id);

                if (existingProcess != null)
                {
                    // 获取主窗口句柄
                    IntPtr mainWindowHandle = existingProcess.MainWindowHandle;

                    if (mainWindowHandle != IntPtr.Zero)
                    {
                        // 最大化窗口
                        ShowWindow(mainWindowHandle, SW_MAXIMIZE);

                        // 激活窗口到前台
                        SetForegroundWindow(mainWindowHandle);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"激活实例失败: {ex.Message}");
            }
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Views.MainWindow>();
            //throw new NotImplementedException();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.Register<Views.MainWindow>();
            //throw new NotImplementedException();
            containerRegistry.RegisterForNavigation<TemplateView>();

            //containerRegistry.RegisterDialog<TemplateView>("Template");
            containerRegistry.RegisterDialog<WheelTypeSettingDialog>("WheelTypeSetting");
            containerRegistry.RegisterDialog<ParameterSettingDialog>("ParameterSetting");
            containerRegistry.RegisterDialog<TemplateDataEditDialog>("TemplateDataEdit");
            containerRegistry.RegisterDialog<MatchResultDialog>("MatchResult");


            //注册弹窗窗口，这句代码会将框架内的默认弹窗窗口替换掉
            containerRegistry.RegisterDialogWindow<MetroDialog>();

            containerRegistry.RegisterInstance<IDialogCoordinator>(DialogCoordinator.Instance);

        }


    }


}

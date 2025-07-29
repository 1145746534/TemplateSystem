using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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

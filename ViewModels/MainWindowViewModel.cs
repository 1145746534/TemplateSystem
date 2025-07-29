using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateSystem.Views;

namespace TemplateSystem.ViewModels
{
    internal class MainWindowViewModel
    {
        private RegionManager _regionManager;
        public MainWindowViewModel(RegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("DisplayRegion1", typeof(TemplateView));

        }
    }
}

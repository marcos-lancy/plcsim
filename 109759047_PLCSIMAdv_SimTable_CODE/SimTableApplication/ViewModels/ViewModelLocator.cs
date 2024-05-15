/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:SimTableApplication"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using SimTableApplication.Services.Implementation;
using SimTableApplication.Services.Interfaces;
using SimTableApplication.ViewModels.DialogViewModels;

namespace SimTableApplication.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
                     
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AddNewProjectDialogViewModel>();
            SimpleIoc.Default.Register<AddNewPlcDialogViewModel>();
            SimpleIoc.Default.Register<MvvmDialogs.IDialogService>(() => new MvvmDialogs.DialogService());
            SimpleIoc.Default.Register<IBackgroundTaskService>(() => new BackgroundTaskService());
            
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        } 


        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<MainViewModel>();            
            SimpleIoc.Default.Unregister<AddNewProjectDialogViewModel>();
            SimpleIoc.Default.Unregister<AddNewPlcDialogViewModel>();
            SimpleIoc.Default.Unregister<MvvmDialogs.IDialogService>();
            SimpleIoc.Default.Unregister<IBackgroundTaskService>();


        }
    }
}
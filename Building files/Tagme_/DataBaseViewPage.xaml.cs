using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tagme_
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataBaseViewPage : Page
    {
        public DataBaseViewPage()
        {
            this.InitializeComponent();

            Loaded += DataBaseViewPage_Loaded;
        }

        //variables
        public static class Variable
        {
            //ID, Name
            public static List<(string, string)> RouteList = new List<(string, string)>();
        }

        private void DataBaseViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetDataBaseCoverImageOnPropertyPagePanel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Initialize database
            if (Frame.BackStack[Frame.BackStack.Count - 1].SourcePageType.ToString() == "Tagme_.DataBaseListPage")
            {
                InitializeRouteBar(); 
            }

            //Forward animation
            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DataBaseViewPageOptionBarConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(OperationPanel);
            }
            anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DataBaseViewPageOptionBarSortAppBarButtonConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(OptionBarSortAppBarButton);
            }
            anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DataBaseViewPageOptionBarViewModeAppBarButtonConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(OptionBarViewModeAppBarButton);
            }
            anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DataBaseViewPageOptionsCommandBarSeparator1ConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(OptionBarCommandBarSeparator1);
            }
            anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DataBaseViewPageOptionBarCreateDataBaseAppBarButtonConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(OptionBarAddItemAppBarButton);
            }
            anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DataBaseViewPageOptionBarMultiSelectAppBarButtonConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(OptionBarMultiSelectAppBarButton);
            }
            anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DataBaseViewPageOptionSelectedDataBaseCoverConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(OptionBarCurrentDataBaseDetailPageButtonImage);
            }

            //Back animation
            anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackDataBaseViewPageOptionBarCurrentDataBaseDetailPageButtonConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(OptionBarCurrentDataBaseDetailPageButtonImage);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackDataBaseViewPageOptionBarConnectedAnimation", OperationPanel);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
                animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackDataBaseViewPageOptionBarSortAppBarButtonConnectedAnimation", OptionBarSortAppBarButton);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
                animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackDataBaseViewPageOptionBarViewModeAppBarButtonConnectedAnimation", OptionBarViewModeAppBarButton);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
                animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackDataBaseViewPageOptionsCommandBarSeparator1ConnectedAnimation", OptionBarCommandBarSeparator1);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
                //animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DataBaseViewPageOptionSelectedDataBaseCoverConnectedAnimation", OptionBarCurrentDataBasePropertyPageButtonImage);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
                try
                {
                    animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackDataBaseViewPageOptionBarCreateDataBaseAppBarButtonConnectedAnimation", OptionBarAddItemAppBarButton);
                    animation.Configuration = new DirectConnectedAnimationConfiguration();
                }
                catch { }
                try
                {
                    animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackDataBaseViewPageOptionBarMultiSelectAppBarButtonConnectedAnimation", OptionBarMultiSelectAppBarButton);
                    animation.Configuration = new DirectConnectedAnimationConfiguration();
                }catch(Exception ex) { }



            }
        }


        //Functions
        private void SetDataBaseCoverImageOnPropertyPagePanel()
        {
            if (File.Exists(Tagme_CoreUWP.CoreRunningData.Tagme_DataBase.UsingDataBasePath))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={Tagme_CoreUWP.CoreRunningData.Tagme_DataBase.UsingDataBasePath}"))
                {
                    db.Open();

                    //Get image
                    SqliteCommand selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.DataBaseCover.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                    selectCommand.Connection = db;
                    OptionBarCurrentDataBaseDetailPageButtonImage.Source = ShimizuCoreUWP.TypeService.ByteToBitmapImage((byte[])selectCommand.ExecuteScalar());

                    db.Close();
                }
            }
        }

        private void InitializeRouteBar()
        {
            Variable.RouteList = new List<(string, string)>() {("0","Root")};
            List<string> RouteListForShow = new List<string>();
            foreach ((string,string) item in Variable.RouteList)
            {
                RouteListForShow.Add(item.Item2);
            }
            RouteBar.ItemsSource = RouteListForShow;
        }

        private void OptionBarCurrentDataBaseDetailPageButton_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to DataBaseDetailPage
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DataBaseViewPageOptionBarCurrentDataBaseDetailPageButtonConnectedAnimation", OptionBarCurrentDataBaseDetailPageButton);
            Frame.Navigate(typeof(DataBaseDetailPage));
        }

        private void OptionBarAddItemAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddItemPage));
        }
    }
}

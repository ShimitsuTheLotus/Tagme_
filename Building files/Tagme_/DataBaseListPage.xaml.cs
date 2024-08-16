using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Tagme_.Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item;
using System.Text;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tagme_
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataBaseListPage : Page
    {
        public DataBaseListPage()
        {
            this.InitializeComponent();

            Loaded += DataBaseListPage_Loaded;

            //Initialize
            InitializeShadows();
            InitializeStatusPanel();
            InitializeUIPositions();
            InitializeDataBaseListView();

            //Repeating Tasks
            KeepUpdateStatusBarDataBaseStorageInfo();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackDataBaseViewPageOptionBarConnectedAnimation");
            if (animation != null)
            {
                animation.TryStart(OptionBarRelativePanel);
            }
            animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackDataBaseViewPageOptionBarSortAppBarButtonConnectedAnimation");
            if (animation != null)
            {
                animation.TryStart(OptionBarSortAppBarButton);
            }
            animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackDataBaseViewPageOptionBarViewModeAppBarButtonConnectedAnimation");
            if (animation != null)
            {
                animation.TryStart(OptionBarViewModeAppBarButton);
            }
            animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackDataBaseViewPageOptionsCommandBarSeparator1ConnectedAnimation");
            if (animation != null)
            {
                animation.TryStart(OptionsCommandBarSeparator1);
            }
            animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackDataBaseViewPageOptionBarCreateDataBaseAppBarButtonConnectedAnimation");
            if (animation != null)
            {
                animation.TryStart(OptionBarCreateDataBaseAppBarButton);
            }
            animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackDataBaseViewPageOptionBarMultiSelectAppBarButtonConnectedAnimation");
            if (animation != null)
            {
                animation.TryStart(OptionBarMultiSelectAppBarButton);
            }


            animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackCreateDataBasePagePageConnectedAnimation");
            if (animation != null)
            {
                animation.TryStart(OptionBarCreateDataBaseAppBarButton);
            }
        }

        //Functions
        //Loaded
        private void DataBaseListPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> dblist = new List<string>();
            Tagme_CoreUWP.InfoManager.GetDataBasePathList(ref dblist);
            Tagme_CoreUWP.InfoManager.RemoveDataBasePath(pathsList: Tagme_CoreUWP.Tagme_DataBaseOperation.CheckIfAllDataBaseInListExist(dblist));
        }

        //Debug

        /// <summary>
        /// Starting debug.
        /// </summary>
        public void DebugStart()
        {
            Tagme_CoreUWP.Debug.IsDebug = true;

            //UI change
            OptionsCommandBarDebugPart.Visibility = Visibility.Visible;
            ShowDebugInfoBar();

            //Tasks
            KeepShadowExisting();
        }

        /// <summary>
        /// Stopping debug.
        /// </summary>
        public void DebugStop()
        {
            Tagme_CoreUWP.Debug.IsDebug = false;

            //UI recover
            OptionsCommandBarDebugPart.Visibility = Visibility.Collapsed;
            HideDebugInfoBar();
        }

        //UI movements
        /// <summary>
        /// Show debug info bar.
        /// </summary>
        public void ShowDebugInfoBar()
        {
            DebugInfoPanel.Margin = new Thickness(8, 8, 8, 8);
            DataBaseListView.Margin = new Thickness(8, 8, DebugInfoPanel.Margin.Right + DebugInfoPanel.ActualWidth + 8, 8);
        }

        /// <summary>
        /// Hide debug info bar.
        /// </summary>
        public void HideDebugInfoBar()
        {
            DebugInfoPanel.Margin = new Thickness(8, 8, MainPagePanel.Margin.Right + 8 - 1280 - DebugInfoPanel.ActualWidth, 8);
            DataBaseListView.Margin = new Thickness(8, 8, MainPagePanel.Margin.Right + 8, 8);
        }


        //Notice push
        /// <summary>
        /// Push the information.
        /// </summary>
        /// <param name="noticeTitle">The title of a push</param>
        /// <param name="description">The description of the info.</param>
        /// <param name="severity">The severity of the info.</param>
        public void PushNotice(string noticeTitle, string description = "", Tagme_CoreUWP.Struct.PushInfoSeverity severity = Tagme_CoreUWP.Struct.PushInfoSeverity.Informational)
        {

            //options

        }


        /// <summary>
        /// Initialize the shadow of the UI elements.
        /// </summary>
        private void InitializeShadows()
        {
            try
            {
                PageSharedShadow.Receivers.Clear();
                OptionBarRelativePanel.Translation += new Vector3(0, 0, 16);
                BrowseStatusRelativePanel.Translation += new Vector3(0, 0, 16);
                DatabaseStorageStatusRelativePanel.Translation += new Vector3(0, 0, 16);
                Tagme_DebugStatusPanel.Translation += new Vector3(0, 0, 16);
                SearchDatabaseAutoSuggestBox.Translation += new Vector3(0, 0, 16);
                DebugInfoPanel.Translation += new Vector3(0, 0, 64);
            }
            catch (Exception ex)
            {
                PushNotice("Error", ex.ToString(), Tagme_CoreUWP.Struct.PushInfoSeverity.Error);
            }
        }

        /// <summary>
        /// InitializeStatusPanel
        /// </summary>
        private void InitializeStatusPanel()
        {
            BrowseSortStatusIcon.Rotation = 90;
        }

        /// <summary>
        /// Initialize the ListView of database list.
        /// </summary>
        private void InitializeDataBaseListView()
        {
            DataBaseListView.Items.Clear();
            Tagme_CustomizedCore.DataBaseListViewSource.Clear();

            //Get databases that exists.
            List<string> ExistDataBasePathsList = Tagme_CoreUWP.Tagme_DataBaseOperation.GetExistDataBasesList();

            //try
            {
                foreach (string dbpath in ExistDataBasePathsList)
                {
                    string dbTitle = "";
                    string dbDescription = "";
                    byte[] dbCover = null;
                    string createdTime = "";
                    string modifiedTime = "";
                    long dbFileSize = 0;
                    string subitemCount = "";

                    if (File.Exists(dbpath)) {
                        using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
                        {
                            
                            db.Open();

                            //Get dbTitle
                            SqliteCommand selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.DataBaseName.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                            selectCommand.Connection = db;
                            SqliteDataReader reader = selectCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                dbTitle = reader.GetString(0);
                            }
                            //Get dbDiscribe
                            selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.Description.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                            selectCommand.Connection = db;
                            reader = selectCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                dbDescription = reader.GetString(0);
                            }
                            //Get dbCover
                            selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.DataBaseCover.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                            selectCommand.Connection = db;
                            dbCover = (byte[])selectCommand.ExecuteScalar();
                            //Get createdTime
                            selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.CreatedTimeStamp.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                            selectCommand.Connection = db;
                            reader = selectCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                createdTime = reader.GetString(0);
                            }
                            //Get modifiedTime
                            selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.LastModifiedTimeStamp.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                            selectCommand.Connection = db;
                            reader = selectCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                modifiedTime = reader.GetString(0);
                            }
                            //Get dbFileSize
                            //DirectoryInfo directoryInfo = new DirectoryInfo(dbpath);
                            FileInfo fileInfoList = new FileInfo(dbpath);
                            dbFileSize = fileInfoList.Length;

                            //Get subitemCount
                            selectCommand = new SqliteCommand($"SELECT Count(*) FROM {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Name}");
                            selectCommand.Connection = db;
                            reader = selectCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                subitemCount = reader.GetString(0);
                            }


                            Tagme_CustomizedCore.DataBaseListViewSource.Add(new Tagme_CustomizedCore.Template.DataBaseListViewTemplate
                            {
                                DataBasePath = dbpath,
                                DataBaseTitle = dbTitle,
                                DataBaseDescription = dbDescription,
                                DataBaseCover = ShimizuCoreUWP.TypeService.ByteToBitmapImage(dbCover),
                                DataBaseCreatedTime = ShimizuCoreUWP.UnitConvertion.SecondUnixTimeStampToDateTime(long.Parse(createdTime)).ToString(),
                                DataBaseModifiedTime = ShimizuCoreUWP.UnitConvertion.SecondUnixTimeStampToDateTime(long.Parse(modifiedTime)).ToString(),
                                DataBaseFileSize = ShimizuCoreUWP.UnitConvertion.FitByte(dbFileSize),
                                DataBaseAllSubItemCount = subitemCount,
                            });
                            db.Close();
                        }
                    }
                }
            }
            //catch (Exception ex) { }

            //For effect preview
            /*
            Tagme_CustomizedCore.DataBaseListViewSource.Add(new Tagme_CustomizedCore.Template.DataBaseListViewTemplate
            {
                DataBasePath = "path",
                DataBaseTitle = "dbTitle",
                DataBaseCover = "Assets/StoreLogo.png",
                DataBaseCreatedTime = "0",
                DataBaseModifiedTime = "0",
                DataBaseFileSize = "0",
                DataBaseAllSubItemCount = "0",
            });*/

            DataBaseListView.ItemsSource = null;
            DataBaseListView.ItemsSource = Tagme_CustomizedCore.DataBaseListViewSource;
            ApplyDataBaseListViewChildShadow();

        }

        private async void ApplyDataBaseListViewChildShadow()
        {
            await Task.Delay(500);
            foreach (var item in DataBaseListView.Items)
            {
                ListViewItem item2 = DataBaseListView.ContainerFromItem(item) as ListViewItem;
                if (item2 != null)
                {
                    item2.Background = null;
                    RelativePanel relativePanel = ShimizuCoreUWP.UIXAML.FindElementByName(item2, "DataBaseListViewTemplateBackground") as RelativePanel;
                    relativePanel.Translation += new Vector3(0, 0, 32);
                }
            }
        }

        /// <summary>
        /// Initialize the position of UI elements
        /// </summary>
        private void InitializeUIPositions()
        {
            HideDebugInfoBar();
        }

        //Repeating tasks
        /// <summary>
        /// Get database storage usage every few seconds and update the info on the status bar.
        /// </summary>
        private async void KeepUpdateStatusBarDataBaseStorageInfo()
        {
            while (true)
            {
                //options

                await Task.Delay(4000);
            }
        }

        /// <summary>
        /// Keep the shadow of controls existing.
        /// </summary>
        private async void KeepShadowExisting()
        {
            while (true)
            {
                InitializeShadows();
                await Task.Delay(4000);
            }
        }

        private void SearchDatabaseAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void SearchDatabaseAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        /// <summary>
        /// The button in debug panel clicked, then switch the debug mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DebugIOButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Tagme_CoreUWP.Debug.IsDebug)
            {
                DebugIOButton.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(0x6A, 0xA9, 0xA9, 0xFF));
                DebugStart();
            }
            else
            {
                DebugIOButton.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
                DebugStop();
            }
        }

        private void DataBaseListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataBaseListView.SelectedItems.Count > 0)
            {
                Tagme_CoreUWP.CoreRunningData.Tagme_DataBase.UsingDataBasePath = Tagme_CustomizedCore.DataBaseListViewSource[DataBaseListView.SelectedIndex].DataBasePath;
                Tagme_CoreUWP.CoreRunningData.Tagme_DataBase.UsingDataBase = Tagme_CustomizedCore.DataBaseListViewSource[DataBaseListView.SelectedIndex].DataBaseTitle;

                //ConnectedAnimation
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DataBaseViewPageOptionBarConnectedAnimation", OptionBarRelativePanel);
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DataBaseViewPageOptionBarSortAppBarButtonConnectedAnimation", OptionBarSortAppBarButton);
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DataBaseViewPageOptionBarViewModeAppBarButtonConnectedAnimation", OptionBarViewModeAppBarButton);
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DataBaseViewPageOptionsCommandBarSeparator1ConnectedAnimation", OptionsCommandBarSeparator1);
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DataBaseViewPageOptionBarCreateDataBaseAppBarButtonConnectedAnimation", OptionBarCreateDataBaseAppBarButton);
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DataBaseViewPageOptionBarMultiSelectAppBarButtonConnectedAnimation", OptionBarMultiSelectAppBarButton);
                if (DataBaseListView.ContainerFromItem(e.ClickedItem) is ListViewItem container)
                {
                    Tagme_CustomizedCore.Template.DataBaseListViewTemplate item = container.Content as Tagme_CustomizedCore.Template.DataBaseListViewTemplate;
                    DataBaseListView.PrepareConnectedAnimation("DataBaseViewPageOptionSelectedDataBaseCoverConnectedAnimation", item, "DataBaseCoverImage");
                    Frame.Navigate(typeof(DataBaseViewPage), item, new SuppressNavigationTransitionInfo());
                }
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("CreateDataBasePagePageConnectedAnimation", OptionBarCreateDataBaseAppBarButton);

            //Add database page
            Frame.Navigate(typeof(CreateDataBasePage));
        }
    }
}

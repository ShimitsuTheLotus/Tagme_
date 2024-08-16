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
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tagme_
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataBaseDetailPage : Page
    {
        public DataBaseDetailPage()
        {
            this.InitializeComponent();

            Loaded += DataBaseDetailPage_Loaded;
        }

        private void DataBaseDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetBasicProperties();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DataBaseViewPageOptionBarCurrentDataBaseDetailPageButtonConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(DataBaseCover);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackDataBaseViewPageOptionBarCurrentDataBaseDetailPageButtonConnectedAnimation", DataBaseCover);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
            }
        }


        private void GetBasicProperties()
        {
            string dbpath = Tagme_CoreUWP.CoreRunningData.Tagme_DataBase.UsingDataBasePath;
            if (File.Exists(dbpath))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    //Get image
                    SqliteCommand selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.DataBaseCover.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                    selectCommand.Connection = db;
                    DataBaseCover.Source = ShimizuCoreUWP.TypeService.ByteToBitmapImage((byte[])selectCommand.ExecuteScalar());

                    selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.CreatedTimeStamp.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                    selectCommand.Connection = db;
                    SqliteDataReader reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        CreatedTime.Text = ShimizuCoreUWP.UnitConvertion.SecondUnixTimeStampToDateTime(long.Parse(reader.GetString(0))).ToString();
                    }

                    selectCommand = new SqliteCommand($"SELECT {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.LastModifiedTimeStamp.Name} FROM {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}");
                    selectCommand.Connection = db;
                    reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        ModifiedTime.Text = ShimizuCoreUWP.UnitConvertion.SecondUnixTimeStampToDateTime(long.Parse(reader.GetString(0))).ToString();
                    }

                    selectCommand = new SqliteCommand($"SELECT Count(*) FROM {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Name}");
                    selectCommand.Connection = db;
                    reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        SubitemCount.Text = reader.GetString(0);
                    }
                    
                    FileInfo fileInfoList = new FileInfo(dbpath);
                    Storage.Text = ShimizuCoreUWP.UnitConvertion.FitByte(fileInfoList.Length) + " (" + fileInfoList.Length.ToString("N0") + "bytes" + ")";
                    

                    db.Close();
                }
            }
            DataBaseTitle.Text = Tagme_CoreUWP.CoreRunningData.Tagme_DataBase.UsingDataBase;
            DataBasePath.Text = Tagme_CoreUWP.CoreRunningData.Tagme_DataBase.UsingDataBasePath;
            
        }
    }
}

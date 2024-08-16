using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tagme_
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateDataBasePage : Page
    {
        static class CreateDataBaseProperties
        {
            //Database name will use the value of the textblock
            public static BitmapImage coverSource = new BitmapImage();
            public static string savePath = string.Empty;
            public static byte[] coverImage = null;
        }

        public CreateDataBasePage()
        {
            this.InitializeComponent();

            Loaded += CreateDataBasePage_Loaded;
            SizeChanged += CreateDataBasePage_SizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("CreateDataBasePagePageConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(BasePanel);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackCreateDataBasePagePageConnectedAnimation", BasePanel);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
            }
        }

        //Loaded
        private async void CreateDataBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeShadow();

            //Initialize Property list
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            PropertyList_DataBaseCoverSourcePath.Text = resourceLoader.GetString("CreateDataBasePage/PropertyList/DataBaseCoverSourcePath/Text") + " [" + resourceLoader.GetString("CreateDataBasePage/CS/PropertyList/Default/Text") + "]";
            PropertyList_Tagme_DataBaseVersion.Text += " " + Tagme_CoreUWP.Tagme_DataBaseConst.Tagme_DataBaseVersion;

            //Initialize CreateDataBaseProperties
            CreateDataBaseProperties.coverSource.DecodePixelWidth = 1024;
            CreateDataBaseProperties.coverSource.UriSource = new Uri(DataBaseCoverImage.BaseUri, "Assets\\Square150x150Logo.scale-200.png");

            //Initialize database cover
            BitmapImage bitmapImage = DataBaseCoverImage.Source as BitmapImage;
            RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromUri(bitmapImage.UriSource);
            var streamContent = await stream.OpenReadAsync();
            byte[] buffer = new byte[streamContent.Size];
            await streamContent.ReadAsync(buffer.AsBuffer(), (uint)streamContent.Size, InputStreamOptions.None);
            CreateDataBaseProperties.coverImage = buffer;
        }

        //SizeChanged
        private void CreateDataBasePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //throw new NotImplementedException();
            FitLayout();
        }

        private void FitLayout()
        {
            //Reorder flyout
            if (DataBaseCoverImage.ActualWidth / BasePanel.ActualWidth < 0.4)
            {
                EditInfoPanel.Orientation = Orientation.Horizontal;
                DataBaseNameTextBox.Width = BasePanel.ActualWidth 
                    - DataBaseCoverPanel.ActualWidth 
                    - DataBaseCoverPanel.Margin.Left 
                    - DataBaseCoverPanel.Margin.Right
                    - MovableSettingsPanel.Margin.Left
                    - MovableSettingsPanel.Margin.Right
                    - DataBaseNameTextBox.Margin.Left
                    - DataBaseNameTextBox.Margin.Right;
            }
            else
            {
                EditInfoPanel.Orientation = Orientation.Vertical;
                DataBaseNameTextBox.Width = BasePanel.ActualWidth - 2 * DataBaseNameTextBox.Margin.Left;
            }
        }

        //Initialization
        //Initialize UI shadow
        public void InitializeShadow()
        {
            DataBaseCoverPanel.Translation += new System.Numerics.Vector3(0, 0, 16);
            CreateButtonPanel.Translation += new System.Numerics.Vector3(0, 0, 64);
        }


        //Functions
        //Remind naming the database(NOT file name!).
        private void RemindNamingDataBase()
        {
            //Update the property list
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            TextBlock originalTextBlock = (TextBlock)FindName("PropertyList_DataBaseName");
            originalTextBlock.Text = resourceLoader.GetString("CreateDataBasePage/PropertyList/DataBaseName/Text") + DataBaseNameTextBox.Text;

            if (DataBaseNameTextBox.Text != string.Empty)
            {
                //Database name is Not Empty
                StackPanel stackPanel = (StackPanel)FindName("PropertyList_DataBaseNamePanel");
                TextBlock newTextBlock = new TextBlock();
                newTextBlock.Name = "PropertyList_DataBaseName";
                newTextBlock.Text = originalTextBlock.Text;
                newTextBlock.Style = originalTextBlock.Style;
                int index = stackPanel.Children.IndexOf(originalTextBlock);
                if (index != -1)
                {
                    stackPanel.Children.Remove(originalTextBlock);
                    stackPanel.Children.Insert(index, newTextBlock);
                }

                //Delete NoEmptyDataBaseNameReminder
                while (true)
                {
                    stackPanel = (StackPanel)FindName("MovableSettingsPanel");
                    TextBlock textBlock = (TextBlock)FindName("NoEmptyDataBaseNameReminder");
                    index = stackPanel.Children.IndexOf(textBlock);
                    if (index != -1)
                    {
                        stackPanel.Children.RemoveAt(index);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                //Empty
                originalTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);

                //Add NoEmptyDataBaseNameReminder
                StackPanel stackPanel = (StackPanel)FindName("MovableSettingsPanel");
                TextBlock textBlock = new TextBlock();
                textBlock.Name = "NoEmptyDataBaseNameReminder";
                textBlock.Text = resourceLoader.GetString("CreateDataBasePage/CS/InputInfo/NoEmptyDataBaseName/Text");
                SolidColorBrush solidColorBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                textBlock.Foreground = solidColorBrush;
                textBlock.Margin = new Thickness(16, 0, 8, 8);
                int index = stackPanel.Children.IndexOf(DataBaseNameTextBox);
                if (index != -1 &&
                    index + 1 < stackPanel.Children.Count)
                {
                    if (stackPanel.Children[index + 1].GetType() == typeof(TextBlock))
                    {
                        TextBlock tb = stackPanel.Children[index + 1] as TextBlock;
                        if (tb.Text != resourceLoader.GetString("CreateDataBasePage/CS/InputInfo/NoEmptyDataBaseName/Text"))
                        {
                            stackPanel.Children.Insert(index + 1, textBlock);
                        }
                    }
                }
            }
        }

        //Remind Choosing Path to create


        //UI events

        private void DataBaseNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RemindNamingDataBase();
        }

        //Main buttons
        private void CancelCreateDataBaseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void CreateDataBaseButton_Click(object sender, RoutedEventArgs e)
        {
            RemindNamingDataBase();
            //Basicc check passed, now create database and initialize
            if (DataBaseNameTextBox.Text != string.Empty)
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.FileTypeChoices.Add("Tagme_ database file", new List<string>() { ".tdb" });
                savePicker.SuggestedFileName = "Tagme_ database";
                StorageFile file = await savePicker.PickSaveFileAsync();

                //Database created
                if (file != null)
                {
                    //Create database
                    Tagme_CoreUWP.Tagme_DataBaseOperation.InitializeTagme_DataBase(file.Path, DataBaseNameTextBox.Text, DataBaseDescriptionTextBox.Text, CreateDataBaseProperties.coverImage);
                }
            }
            Frame.GoBack();
        }


        //Change database settings
        private async void DataBaseCoverChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            foreach (string item in Tagme_CoreUWP.Const.SupportedImageExtendNameList)
            {
                picker.FileTypeFilter.Add(item);
            }

            Windows.Storage.StorageFile imagesource = await picker.PickSingleFileAsync();
            Windows.Storage.StorageFile image;
            if (imagesource != null)
            {
                image = await imagesource.CopyAsync(ApplicationData.Current.LocalCacheFolder, imagesource.Name, NameCollisionOption.ReplaceExisting);
                //Change sample cover image source
                CreateDataBaseProperties.coverImage = await ShimizuCoreUWP.TypeService.BitmapImageFileToByte(image);
                using (IRandomAccessStream fileStream = await image.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    CreateDataBaseProperties.coverSource.DecodePixelWidth = 1024;
                    await CreateDataBaseProperties.coverSource.SetSourceAsync(fileStream);

                }
                DataBaseCoverImage.Source = CreateDataBaseProperties.coverSource;

                /*if (image != null)
                {
                    await image.deleteasync(storagedeleteoption.permanentdelete);
                }*/



                //Change path in propertylist
                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

                PropertyList_DataBaseCoverSourcePath.Text = resourceLoader.GetString("CreateDataBasePage/PropertyList/DataBaseCoverSourcePath/Text") + " " + image.Path;
            }
        }

        private void DataBaseCoverUseDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            CreateDataBaseProperties.coverSource.DecodePixelWidth = 1024;
            CreateDataBaseProperties.coverSource.UriSource = new Uri(DataBaseCoverImage.BaseUri, "Assets\\Square150x150Logo.scale-200.png");
            DataBaseCoverImage.Source = CreateDataBaseProperties.coverSource;

            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            PropertyList_DataBaseCoverSourcePath.Text = resourceLoader.GetString("CreateDataBasePage/PropertyList/DataBaseCoverSourcePath/Text") + " [" + resourceLoader.GetString("CreateDataBasePage/CS/PropertyList/Default/Text") + "]";
        }

    }
}

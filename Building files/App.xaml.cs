using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using static Tagme_.Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item;

namespace Tagme_
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }

    //Classes
    /// <summary>
    /// Shimizu services that could be used to variety of programs.
    /// These functions must not ref any variety or functions of a certain program. 
    /// Compatibility Caution: Windows UWP
    /// Core version: 0.0.0
    /// </summary>
    public class ShimizuCoreUWP
    {
        /// <summary>
        /// The constant values that could be changed by developers or users.
        /// If it will be changed by the users, the developer should save the customized values by themselves and initialize the values each time the app runs to apply the user settings.
        /// </summary>
        public class CustomizableConsts
        {
            /// <summary>
            /// If file size is over this value, the core will suggest to open file with stream.
            /// Measurement:Byte
            /// The default value 1048576 Byte is 2 MiB.
            /// </summary>
            //Compatibility Caution: Int64
            public static Int64 thresholdSizeOfFileOpenMode = 1048576;
        }

        public class Struct
        {
            /// <summary>
            /// The status used when checking if file is exist or opening file.
            /// </summary>
            public enum FileGetStatus
            {
                Unknown,
                PathIsEmpty,
                NotExist,
                NotExistAndFailedToCreate,
                JustCreated,
                Exist,
                ExistButFailedToOpen
            }

            /// <summary>
            /// The mode for opening file.
            /// FileNotExist: The target file is not exist.
            /// AllIn: Use it for small file, usually text files.
            /// Stream: Use stream to read file.
            /// Auto: ShimizuCore will determine which file open mode to use automatically.
            /// </summary>
            public enum SuggestedFileOpenMode
            {
                FileNotExist,
                AllIn,
                Stream,
                Auto
            }
        }

        /// <summary>
        /// Services for types, such as changing a type into another type.
        /// </summary>
        public static class TypeService
        {
            //Turn a byte[] type into BitmapImage type.
            public static BitmapImage ByteToBitmapImage(byte[] bytes)
            {
                BitmapImage img = new BitmapImage();
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(bytes);
                        writer.StoreAsync().GetResults();
                    }
                    stream.Seek(0);
                    img.SetSource(stream);
                }
                return img;
            }

            //Turn a BitmapImage into byte[] type
            public static async Task<byte[]> BitmapImageFileToByte(StorageFile bitmapImageFile)
            {
                if (bitmapImageFile == null) return null;
                RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromFile(bitmapImageFile);
                var streamContent = await stream.OpenReadAsync();
                byte[] buffer = new byte[streamContent.Size];
                await streamContent.ReadAsync(buffer.AsBuffer(), (uint)streamContent.Size, InputStreamOptions.None);

                return buffer;
            }
        }

        /// <summary>
        /// Convert the units, usually storage
        /// </summary>
        public static class UnitConvertion
        {
            //Change the storage unit to normal unit
            public static string FitByte(long byteNum, bool thousandsSeparator = true)
            {
                if (byteNum < Math.Pow(1024, 1))
                {
                    return byteNum.ToString(thousandsSeparator ? "N0" : "") + "B";
                }
                else if (byteNum < Math.Pow(1024, 2))
                {
                    return (byteNum/ Math.Pow(1024, 1)).ToString(thousandsSeparator ? "N0" : "") + "KiB";
                }
                else if (byteNum < Math.Pow(1024, 3))
                {
                    return (byteNum / Math.Pow(1024, 2)).ToString(thousandsSeparator ? "N0" : "") + "MiB";
                }
                else if (byteNum < Math.Pow(1024, 4))
                {
                    return (byteNum / Math.Pow(1024, 3)).ToString(thousandsSeparator ? "N0" : "") + "GiB";
                }
                else if (byteNum < Math.Pow(1024, 5))
                {
                    return (byteNum / Math.Pow(1024, 4)).ToString(thousandsSeparator ? "N0" : "") + "TiB";
                }
                else if (byteNum < Math.Pow(1024, 6))
                {
                    return (byteNum / Math.Pow(1024, 5)).ToString(thousandsSeparator ? "N0" : "") + "PiB";
                }
                else if (byteNum < Math.Pow(1024, 7))
                {
                    return (byteNum / Math.Pow(1024, 6)).ToString(thousandsSeparator ? "N0" : "") + "EiB";
                }
                else if (byteNum < Math.Pow(1024, 8))
                {
                    return (byteNum / Math.Pow(1024, 7)).ToString(thousandsSeparator ? "N0" : "") + "ZiB";
                }
                else if (byteNum < Math.Pow(1024, 9))
                {
                    return (byteNum / Math.Pow(1024, 8)).ToString(thousandsSeparator ? "N0" : "") + "YiB";
                }
                else
                {
                    return (byteNum / Math.Pow(1024, 9)).ToString(thousandsSeparator ? "N0" : "") + "BiB";
                }
            }

            public static DateTime SecondUnixTimeStampToDateTime(Int64 timestamp)
            {
                Int64 begtime = timestamp * 10000000;
                DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
                Int64 ticks_1970 = dt_1970.Ticks;
                Int64 time_ticks = ticks_1970 + begtime;
                DateTime dt = new DateTime(time_ticks);
                return dt;
            }
        }

        /// <summary>
        /// Options about files.
        /// </summary>
        public class File
        {
            /// <summary>
            /// Check if the path exists
            /// </summary>
            /// <param name="path">File path</param>
            /// <param name="createIfNotExist">When the value is true, the core will try to create a file. Plus, if the directory of the file not exist, the core will also try to create it.</param>
            /// <returns>The status of the target file.</returns>
            public Struct.FileGetStatus AccessFileChecker(string path, bool createIfNotExist)
            {
                if (System.IO.File.Exists(path))
                {
                    return Struct.FileGetStatus.Exist;
                }
                //Direction is null or empty string.
                if (path == null || path == string.Empty)
                {
                    return Struct.FileGetStatus.PathIsEmpty;
                }
                //Directory not exists.
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                {
                    if (createIfNotExist)
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                        }
                        catch
                        {
                            return Struct.FileGetStatus.NotExistAndFailedToCreate;
                        }
                    }
                    else
                    {
                        return Struct.FileGetStatus.NotExist;
                    }
                }

                //Directory failed to be created.
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                {
                    return Struct.FileGetStatus.NotExistAndFailedToCreate;
                }
                else
                {
                    try
                    {
                        System.IO.File.Create(System.IO.Path.GetDirectoryName(path));
                    }
                    catch
                    {
                        return Struct.FileGetStatus.NotExistAndFailedToCreate;
                    }
                }
                //File failed to be created.
                if (!System.IO.File.Exists(path))
                {
                    return Struct.FileGetStatus.NotExistAndFailedToCreate;
                }

                //File now exists.
                return Struct.FileGetStatus.JustCreated;
            }

            /// <summary>
            /// Make sure a file exists and suggest the file open method.
            /// </summary>
            public Struct.SuggestedFileOpenMode SuggestOpenFileMethod(string path)
            {
                var x = new ShimizuCoreUWP.File();
                //File exists
                if (x.AccessFileChecker(path, false) != Struct.FileGetStatus.PathIsEmpty && x.AccessFileChecker(path, false) != Struct.FileGetStatus.NotExist)
                {
                    if (new FileInfo(path).Length <= CustomizableConsts.thresholdSizeOfFileOpenMode)
                    {
                        return Struct.SuggestedFileOpenMode.AllIn;
                    }
                    else
                    {
                        return Struct.SuggestedFileOpenMode.Stream;
                    }
                }
                else
                {
                    return Struct.SuggestedFileOpenMode.FileNotExist;
                }
            }
        }

        public class UIXAML
        {
            //From Web, we added comment to make it easier to understand.
            /// <summary>
            /// Get a UI element from the visual tree. Then you can operate against the element.
            /// </summary>
            /// <param name="startNode">The parent of the target element waiting to be find</param>
            /// <param name="name">The x:Name property of the target element</param>
            /// <returns></returns>
            public static FrameworkElement FindElementByName(DependencyObject startNode, string name)
            {
                int count = VisualTreeHelper.GetChildrenCount(startNode);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(startNode, i);

                    if (child is FrameworkElement fe && fe.Name == name)
                    {
                        return fe;
                    }

                    var result = FindElementByName(child, name);
                    if (result != null)
                    {
                        return result;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// Tagme_ core that has the main functions of a Tagme_ Program
    /// Compatibility Caution: Windows UWP
    /// Tagme_ core should not ref anything not in itself except Shimizu core.
    /// Requirements: NuGet Package - Microsoft.Data.Sqlite
    /// </summary>
    public class Tagme_CoreUWP
    {
        /// <summary>
        /// The consts that Tagme_ uses.
        /// </summary>
        public class Const
        {
            /// <summary>
            /// The folder of Tagme_ info.
            /// </summary>
            public static StorageFolder Tagme_InfoPath = ApplicationData.Current.LocalFolder;
            /// <summary>
            /// The database path of a database that records info of Tagme_ core needs, including paths of all Tagme_ databases.
            /// </summary>
            public static string CoreInfoDataBasePath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "dbPathsDB.db");
            public static List<string> SupportedImageExtendNameList = new List<string>() { ".jpeg", ".jpg", ".jpe", ".jfif", ".jif", ".png", ".bmp", ".gif", ".tif", ".jxr", ".wdp", ".ico" };
        }

        //Struct
        public class Struct
        {
            /// <summary>
            /// The severity of a pushed info.
            /// </summary>
            public enum PushInfoSeverity
            {
                Informational,
                Success,
                Warning,
                Error
            }

            /// <summary>
            /// Use it as a function return when the function is for creating a Tagme_ database.
            /// </summary>
            public enum DataBaseCreateFailedReason
            {
                Unknown,//Failed cause of unknown error
                Success,//File created successfully, in fact it's not an error
                NameUsed//The file using the name already exists
            }

            public enum DataBaseUpdateFailedReason
            {
                Unknown,
                Success,
            }

            public enum DataBaseDeleteFailedReason
            {
                Unknown,
                Success,
            }
        }

        public class Services
        {

        }

        /// <summary>
        /// The data Tagme_ needs for running.
        /// </summary>
        public class CoreRunningData
        {
            /// <summary>
            /// Operates about cache(mostly are clearing cache)
            /// </summary>
            public class Cache
            {
                /// <summary>
                /// Clear all files in cache, not including the folders
                /// </summary>
                public void ClearAllCache()
                {
                    ClearCoverSettingsCache();
                }

                public void ClearCoverSettingsCache()
                {

                }

                public async Task<StorageFile> CopySettingCoverToCache(StorageFile image)
                {
                    if (image == null) return null;
                    StorageFile copiedFile = await image.CopyAsync(ApplicationData.Current.LocalCacheFolder, image.Name, NameCollisionOption.ReplaceExisting);
                    copiedFile = await image.CopyAsync(ApplicationData.Current.LocalCacheFolder, image.Name, NameCollisionOption.GenerateUniqueName);
                    return copiedFile;
                }
            }

            /// <summary>
            /// Lists of Tagme_ databases.
            /// </summary>
            public class Tagme_DataBasesList
            {
                /// <summary>
                /// The paths of all databases.
                /// </summary>
                public static List<string> dataBasePaths = new List<string>();
                /// <summary>
                /// The paths that Tagme_ database files exist.
                /// </summary>
                public static List<string> dataBasePathsExistFile = new List<string>();
            }
            /// <summary>
            /// The data about Single Tagme_ database.
            /// </summary>
            public class Tagme_DataBase
            {
                public static string UsingDataBasePath;
                public static string UsingDataBase;
                /// <summary>
                /// The data needed to simulate folders.
                /// </summary>
                public class SimulatedFolder
                {
                    /// <summary>
                    /// A stack of opened folders
                    /// The last element is the ID of the folder that is being used.
                    /// </summary>
                    public static Stack<string> UsingFolderIDStack;
                }
            }
        }

        /// <summary>
        /// Functions for getting and setting info.
        /// </summary>
        public static class InfoManager
        {
            /// <summary>
            /// Initialize the database for Tagme_ core info.
            /// Please check if the dbpath exists a database file before using this function.
            /// Plus, never use it while the db is already in using. Use it after a db.Close()
            /// </summary>
            /// <param name="dbpath">The path of string</param>
            public static void InitializeInfoDataBase(string dbpath = "default")
            {
                if (dbpath == "default") dbpath = Const.CoreInfoDataBasePath;

                using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    SqliteCommand insertCommand = new SqliteCommand("CREATE TABLE IF NOT EXISTS SETTINGS(" +
                        "NAME TEXT, " +
                        "VALUE TEXT)", db);
                    insertCommand.ExecuteNonQuery();
                    insertCommand = new SqliteCommand("CREATE TABLE IF NOT EXISTS DATABASES(" +
                        "PATH TEXT)", db);
                    insertCommand.ExecuteNonQuery();

                    db.Close();
                }
            }

            /// <summary>
            /// Get info of a list of database path.
            /// Not checked if the databases are exist.
            /// </summary>
            /// <param name="getDataBasePathList">The list that will be filled with database paths.</param>
            public static void GetDataBasePathList(ref List<string> getDataBasePathList, string dbpath = "default")
            {
                getDataBasePathList.Clear();
                if (dbpath == "default") dbpath = Const.CoreInfoDataBasePath;
                using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    bool isTableExists = false;
                    var x = new ShimizuCoreUWP.File();
                    if (x.AccessFileChecker(Const.CoreInfoDataBasePath, true) == ShimizuCoreUWP.Struct.FileGetStatus.Exist)
                    {

                        SqliteCommand selectCommand = new SqliteCommand("SELECT EXISTS (SELECT name FROM sqlite_schema WHERE type='table' AND name='DATABASES');");
                        selectCommand.Connection = db;
                        SqliteDataReader selectDataReader = selectCommand.ExecuteReader();
                        while (selectDataReader.Read())
                        {
                            if (selectDataReader.GetString(0) != "0")
                            {
                                isTableExists = true;
                            }
                        }
                        db.Close();
                        InitializeInfoDataBase();

                        db.Open();
                        selectCommand = new SqliteCommand("SELECT PATH FROM DATABASES");
                        selectCommand.Connection = db;
                        selectDataReader = selectCommand.ExecuteReader();
                        while (selectDataReader.Read())
                        {
                            getDataBasePathList.Add(selectDataReader.GetString(0));
                        }
                    }
                    else if (x.AccessFileChecker(Const.CoreInfoDataBasePath, true) == ShimizuCoreUWP.Struct.FileGetStatus.JustCreated)
                    {
                        InitializeInfoDataBase();
                        return;
                    }
                    else
                    {
                        //Tagme_ info database not exists and failed to create.
                    }
                    db.Close();
                }
                return;
            }

            /// <summary>
            /// Log the database path or paths in the Tagme_ info database.
            /// can only use path or pathsList as the parameter, use string for single path, use list for paths
            /// </summary>
            /// <param name="path">The path waiting to be logged in Tagme_ info database</param>
            /// <param name="pathsList">When </param>
            /// <returns>Is logging success?</returns>
            public static bool LogDataBasePath(string path = "default", List<string> pathsList = null)
            {
                if (path == "default" && pathsList == null)
                {
                    //No parameter.
                    return false;
                }
                else if (path != "default" && pathsList == null)
                {
                    pathsList = pathsList ?? new List<string>();
                    pathsList.Add(path);
                }
                else if (path != "default" && pathsList != null)
                {
                    //Too much parameters
                    return false;
                }


                var x = new ShimizuCoreUWP.File();
                ShimizuCoreUWP.Struct.FileGetStatus accessStatus = x.AccessFileChecker(path, true);
                if (accessStatus == ShimizuCoreUWP.Struct.FileGetStatus.Exist)
                {
                    //Do nothing
                }
                //The db was not exist but now just created a new one, now it need to be initialized.
                else if (accessStatus == ShimizuCoreUWP.Struct.FileGetStatus.JustCreated)
                {
                    InitializeInfoDataBase() ;
                }
                //Failed to create
                else
                {
                    return false;
                }

                using (SqliteConnection db = new SqliteConnection($"Filename={Const.CoreInfoDataBasePath}"))
                {
                    db.Open();

                    List<string> existPaths = new List<string>();
                    GetDataBasePathList(ref existPaths);

                    foreach (string insertPath in pathsList) 
                    {
                        if(existPaths.Contains(insertPath))
                            continue;
                        SqliteCommand createCommand = new SqliteCommand();
                        createCommand.Connection = db;
                        createCommand.CommandText = "CREATE TABLE IF NOT EXISTS DATABASES(PATH TEXT)";
                        createCommand.ExecuteNonQuery();
                        SqliteCommand insertCommand = new SqliteCommand();
                        insertCommand.Connection = db;
                        insertCommand.CommandText = "INSERT INTO DATABASES VALUES(@P4TH)";
                        insertCommand.Parameters.Clear();
                        //insertCommand.Parameters.AddWithValue("@TABLE", "DATABASES");
                        insertCommand.Parameters.AddWithValue("@P4TH", insertPath);
                        insertCommand.ExecuteReader();
                    }

                    db.Close();
                }

                return true;
            }

            /// <summary>
            /// Remove a provided Tagme_ database path or paths in Tagme_ info database.
            /// Can only use one parameter at once.
            /// </summary>
            /// <param name="path">The path waiting to be removed from Tagme_ info database.</param>
            /// <param name="pathsList">The paths waiting to be removed from Tagme_ info database.</param>
            public static void RemoveDataBasePath(string path = "default", List<string> pathsList = null)
            {
                if (path == "default" && pathsList == null)
                {
                    //No parameter
                    return;
                }
                else if (path != "default" && pathsList != null)
                {
                    //Too much parameters
                    return;
                }

                if (path != "default")
                {
                    pathsList = new List<string>();
                }

                using (SqliteConnection db = new SqliteConnection($"Filename={Const.CoreInfoDataBasePath}"))
                {
                    db.Open();
                    foreach (string deletePath in pathsList)
                    {
                        SqliteCommand deleteCommand = new SqliteCommand();
                        deleteCommand.Connection = db;
                        deleteCommand.CommandText = "DELETE FROM DATABASES WHERE PATH = @P4R4M3T3R";
                        deleteCommand.Parameters.Clear();
                        deleteCommand.Parameters.AddWithValue("@P4R4M3T3R", deletePath);
                        deleteCommand.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }
        }

        /// <summary>
        /// The string value of a table or a column of Tagme_ database
        /// </summary>
        public static class Tagme_DataBaseConst
        {
            /// <summary>
            /// The version of Tagme_ database.
            /// </summary>
            public static string Tagme_DataBaseVersion = "1";
            /// <summary>
            /// A table with basic database info.
            /// </summary>
            public static class BasicDataBaseInfo
            {
                public static string Name = "BasicDataBaseInfo";
                public static class Item
                {
                    public static class DataBaseName
                    {
                        public static string Name = "DataBaseName";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Description
                    {
                        public static string Name="Description";
                        public static string SQLiteType = "TEXT";
                    }

                    public static class DataBaseCover
                    {
                        public static string Name = "DataBaseCover";
                        public static string SQLiteType = "BLOB";
                    }
                    public static class CreatedTimeStamp
                    {
                        public static string Name = "CreatedTimeStamp";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class LastModifiedTimeStamp
                    {
                        public static string Name = "LastModifiedTimeStamp";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class LastViewTimeStamp
                    {
                        public static string Name = "LastViewTimeStamp";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Tagme_DataBaseVersion
                    {
                        public static string Name = "Tagme_DataBaseVersion"; 
                        public static string SQLiteType = "TEXT";
                    }
                }
            }

            /// <summary>
            /// A table for tags.
            /// </summary>
            public static class TagMapping
            {
                public static string Name = "TagMapping";
                public static class Item
                {
                    public static class TagMapID
                    {
                        public static string Name = "TagMapID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class TagID
                    {
                        public static string Name = "TagID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Tag
                    {
                        public static string Name = "Tag";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class TagDescription
                    {
                        public static string Name = "TagDescription";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class TagParentID
                    {
                        public static string Name = "TagParentID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class RelatedTagIDs
                    {
                        public static string Name = "RelatedIDs";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class CreatedTimeStamp
                    {
                        public static string Name = "CreatedTimeStamp";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ModifiedTimeStamp
                    {
                        public static string Name = "ModifiedTimeStamp";
                        public static string SQLiteType = "TEXT";
                    }
                }
            }

            /// <summary>
            /// The root of item.
            /// </summary>
            public static class ItemIndexRoot
            {
                public static string Name = "ItemIndexRoot";
                public static class Item
                {
                    public static class ItemID
                    {
                        public static string Name = "ItemID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ItemParentID
                    {
                        public static string Name = "ItemParentID";
                        public static string SQLiteType = "TEXT";
                    }

                    public static class ContentType
                    {
                        public static string Name = "ContentType";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Title
                    {
                        public static string Name = "Title";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Description
                    {
                        public static string Name = "Description ";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ItemSourceMap
                    {
                        public static string Name = "ItemSourceMap ";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class PropertyMap
                    {
                        public static string Name = "PropertyMap";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class CreatedTimeStamp
                    {
                        public static string Name = "CreatedTimeStamp";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ModifiedTimeStamp
                    {
                        public static string Name = "ModifiedTimeStamp ";
                        public static string SQLiteType = "TEXT";
                    }
                }
            }

            /// <summary>
            /// The file in item, each item can only contains only one ItemSource.
            /// </summary>
            public static class ItemSource
            {
                public static string Name = "ItemSource";
                public static class Item
                {
                    public static class ID
                    {
                        public static string Name = "ID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class FileName
                    {
                        public static string Name = "FileName";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class IsChain
                    {
                        public static string Name = "IsChain";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ChainID
                    {
                        public static string Name = "ChainID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Content
                    {
                        public static string Name = "Content";
                        public static string SQLiteType = "BLOB";
                    }
                }
            }

            /// <summary>
            /// ItemProperty
            /// </summary>
            public static class ItemProperty
            {
                public static string Name = "ItemProperty";
                public static class Item
                {
                    public static class ID
                    {
                        public static string Name = "ID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ParentID
                    {
                        public static string Name = "ParentID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Property
                    {
                        public static string Name = "Property";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Value
                    {
                        public static string Name = "Value";
                        public static string SQLiteType = "TEXT";
                    }
                }
            }

            /// <summary>
            /// Property template
            /// </summary>
            public static class ItemPropertyTemplate
            {
                public static string Name = "ItemPropertyTemplate";
                public static class Item
                {
                    public static class TemplateID
                    {
                        public static string Name = "TemplateID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class TemplateName
                    {
                        public static string Name = "TemplateName";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ID
                    {
                        public static string Name = "ID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ParentID
                    {
                        public static string Name = "ParentID";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Property
                    {
                        public static string Name = "Property";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class Value
                    {
                        public static string Name = "Value";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class CreatedTimeStamp
                    {
                        public static string Name = "CreatedTimeStamp";
                        public static string SQLiteType = "TEXT";
                    }
                    public static class ModifiedTimeStamp
                    {
                        public static string Name = "ModifiedTimeStamp";
                        public static string SQLiteType = "TEXT";
                    }
                }
            }
        }

        /// <summary>
        /// The operations of Tagme_ Databases.
        /// </summary>
        public static class Tagme_DataBaseOperation
        {
            /// <summary>
            /// Check if a database exists.
            /// </summary>
            public static bool CheckIfDataBaseExists(string dataBasePath)
            {
                var x = new ShimizuCoreUWP.File();
                if (x.AccessFileChecker(dataBasePath, false) == ShimizuCoreUWP.Struct.FileGetStatus.Exist)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Check all the databases in list if they're exist.
            /// It will return a list of databases that doesn't exist.
            /// </summary>
            /// <param name="targetPathList">A list filled with database paths that waiting to check if the databases are exist.</param>
            /// <returns>A list of paths that doesn't refer to any file.</returns>
            public static List<string> CheckIfAllDataBaseInListExist(List<string> targetPathList)
            {
                List<string> dataBaseNotExistList = new List<string>();

                foreach (string path in targetPathList)
                {
                    if (!CheckIfDataBaseExists(path))
                    {
                        dataBaseNotExistList.Add(path);
                    }
                }

                return dataBaseNotExistList;
            }

            /// <summary>
            /// Get a list of the databases that is exist and is logged in the Tagme_ info database.
            /// </summary>
            /// <returns>The list of exist database.</returns>
            public static List<string> GetExistDataBasesList()
            {
                List<string> dataBaseList = new List<string>();
                try
                {
                    using (SqliteConnection db = new SqliteConnection($"Filename={Const.CoreInfoDataBasePath}"))
                    {
                        db.Open();

                        SqliteCommand selectCommand = new SqliteCommand($"SELECT PATH FROM DATABASES", db);
                        SqliteDataReader query = selectCommand.ExecuteReader();
                        while (query.Read())
                        {
                            dataBaseList.Add(query.GetString(0));
                        }


                        db.Close();
                    }
                }
                catch { }
                return dataBaseList;
            }


            /// <summary>
            /// Create a database at a specific path.
            /// </summary>
            /// <param name="createPath">The path that database will be created</param>
            /// <param name="dataBaseFileName">The name of the database file, it will be used as file name.</param>
            /// <param name="dataBaseName">The name of the database, it will be logged in the database rather than used as the file name.</param>
            /// <param name="cover">The cover image of the database</param>
            /// <returns>If succeeded, it will return success, or it will return the reason of failure</returns>
            public static async Task<Tagme_CoreUWP.Struct.DataBaseCreateFailedReason> CreateAndInitializeTagme_DataBase(string createPath,string dataBaseFileName, string databaseDescription, string dataBaseName, byte[] cover)
            {
                try
                {
                    //CreateFile
                    if (!Directory.Exists(createPath))
                    {
                        Directory.CreateDirectory(createPath);
                    }
                    StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(createPath);
                    if(Directory.Exists(System.IO.Path.Combine(createPath, dataBaseName))) 
                    {
                        return Struct.DataBaseCreateFailedReason.NameUsed;
                    }
                    StorageFile storageFile = await storageFolder.CreateFileAsync(dataBaseName + ".tdb", CreationCollisionOption.GenerateUniqueName);
                    if (storageFile == null) { return Struct.DataBaseCreateFailedReason.Unknown; }
                    InitializeTagme_DataBase(databasePath: storageFile.Path, databaseDescription: databaseDescription, dataBaseName: dataBaseName, cover: cover);

                }
                catch 
                {
                    return Struct.DataBaseCreateFailedReason.Unknown;
                }

                //When created successfully
                return Struct.DataBaseCreateFailedReason.Success;
            }

            public static void InitializeTagme_DataBase(string databasePath, string dataBaseName, string databaseDescription, byte[] cover)
            {
                //Log database path.
                Tagme_CoreUWP.InfoManager.LogDataBasePath(path: databasePath);

                //Insert tables
                string dbpath = databasePath;
                using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
                {
                    try
                    {
                        db.Open();

                        SqliteCommand createCommand = new SqliteCommand();
                        createCommand.Connection = db;
                        createCommand.CommandText = $"CREATE TABLE IF NOT EXISTS {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name}(" +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.DataBaseName.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.DataBaseName.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.Description.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.Description.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.DataBaseCover.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.DataBaseCover.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.CreatedTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.CreatedTimeStamp.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.LastModifiedTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.LastModifiedTimeStamp.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.LastViewTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.LastViewTimeStamp.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.Tagme_DataBaseVersion.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Item.Tagme_DataBaseVersion.SQLiteType})";
                        createCommand.ExecuteNonQuery();

                        createCommand = new SqliteCommand();
                        createCommand.Connection = db;
                        createCommand.CommandText = $"CREATE TABLE IF NOT EXISTS {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Name}(" +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.TagMapID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.TagMapID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.TagID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.TagID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.Tag.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.Tag.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.TagDescription.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.TagDescription.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.TagParentID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.TagParentID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.RelatedTagIDs.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.RelatedTagIDs.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.CreatedTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.CreatedTimeStamp.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.ModifiedTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.TagMapping.Item.ModifiedTimeStamp.SQLiteType})";
                        createCommand.ExecuteNonQuery();

                        createCommand = new SqliteCommand();
                        createCommand.Connection = db;
                        createCommand.CommandText = $"CREATE TABLE IF NOT EXISTS {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Name}(" +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ItemID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ItemID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ItemParentID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ItemParentID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ContentType.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ContentType.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.Title.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.Title.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.Description.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.Description.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ItemSourceMap.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ItemSourceMap.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.PropertyMap.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.PropertyMap.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.CreatedTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.CreatedTimeStamp.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ModifiedTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemIndexRoot.Item.ModifiedTimeStamp.SQLiteType})";
                        createCommand.ExecuteNonQuery();

                        createCommand = new SqliteCommand();
                        createCommand.Connection = db;
                        createCommand.CommandText = $"CREATE TABLE IF NOT EXISTS {Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Name}(" +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.ID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.ID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.FileName.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.FileName.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.IsChain.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.IsChain.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.ChainID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.ChainID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.Content.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemSource.Item.Content.SQLiteType})";
                        createCommand.ExecuteNonQuery();

                        createCommand = new SqliteCommand();
                        createCommand.Connection = db;
                        createCommand.CommandText = $"CREATE TABLE IF NOT EXISTS {Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Name}(" +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Item.ID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Item.ID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Item.ParentID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Item.ParentID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Item.Property.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Item.Property.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Item.Value.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemProperty.Item.Value.SQLiteType})";
                        createCommand.ExecuteNonQuery();

                        createCommand = new SqliteCommand();
                        createCommand.Connection = db;
                        createCommand.CommandText = $"CREATE TABLE IF NOT EXISTS {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Name}(" +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.TemplateID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.TemplateID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.TemplateName.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.TemplateName.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.ID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.ID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.ParentID.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.ParentID.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.Property.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.Property.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.Value.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.Value.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.CreatedTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.CreatedTimeStamp.SQLiteType}," +
                            $"{Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.ModifiedTimeStamp.Name} {Tagme_CoreUWP.Tagme_DataBaseConst.ItemPropertyTemplate.Item.ModifiedTimeStamp.SQLiteType})";
                        createCommand.ExecuteNonQuery();

                        string secondUnixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

                        //Insert basic database info
                        SqliteCommand insertCommand = new SqliteCommand();
                        insertCommand.Connection = db;
                        insertCommand.CommandText = $"INSERT INTO {Tagme_CoreUWP.Tagme_DataBaseConst.BasicDataBaseInfo.Name} " +
                            $"VALUES(@DataBaseName, @DataBaseDescription, @DataBaseCover, @CreatedTimeStamp, @LastModifiedTimeStamp, @LastViewTimeStamp, @Tagme_DataBaseVersion)";
                        insertCommand.Parameters.Clear();
                        insertCommand.Parameters.AddWithValue("@DataBaseName", dataBaseName);
                        insertCommand.Parameters.AddWithValue("@DataBaseDescription", databaseDescription);
                        insertCommand.Parameters.AddWithValue("@DataBaseCover", cover);
                        insertCommand.Parameters.AddWithValue("@CreatedTimeStamp", secondUnixTimestamp);
                        insertCommand.Parameters.AddWithValue("@LastModifiedTimeStamp", secondUnixTimestamp);
                        insertCommand.Parameters.AddWithValue("@LastViewTimeStamp", secondUnixTimestamp);
                        insertCommand.Parameters.AddWithValue("@Tagme_DataBaseVersion", Tagme_CoreUWP.Tagme_DataBaseConst.Tagme_DataBaseVersion);
                        insertCommand.ExecuteNonQuery();

                        db.Close();
                    }
                    catch (Exception ex)
                    {
                        var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                        MainPage.MainPagePointer.ShowGlobalInfoContentDialog(resourceLoader.GetString("Global/CS/Error/BroadFileSystemAccessNotEnabled/DialogTitleText"),
                            "[0x1] " + "Global.CS.Error.BroadFileSystemAccessNotEnabled\n" +
                            resourceLoader.GetString("Global/CS/Error/BroadFileSystemAccessNotEnabled/DialogContent"),
                            resourceLoader.GetString("Global/CS/Error/BroadFileSystemAccessNotEnabled/DialogPrimaryKeyText"),
                            resourceLoader.GetString("Global/CS/Error/BroadFileSystemAccessNotEnabled/DialogCloseKeyText"));
                    }
                }
            }

            public static void SortDataBase()
            {

            }

            /// <summary>
            /// Open a folder in a Tagme_ database file.
            /// </summary>
            /// <param name="FolderID">The ID of the folder being opened</param>
            public static void OpenFolder(string FolderID)
            {
                CoreRunningData.Tagme_DataBase.SimulatedFolder.UsingFolderIDStack.Push(FolderID);
            }

            /// <summary>
            /// Go back to previous level of folder.
            /// Compatibility Caution: Int64
            /// </summary>
            public static void GoBack(Int64 times = 1)
            {
                for (Int64 i = times; i >= 0; i--)
                {
                    try
                    {
                        CoreRunningData.Tagme_DataBase.SimulatedFolder.UsingFolderIDStack.Pop();
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// The items related with debugging.
        /// </summary>
        public class Debug
        {
            /// <summary>
            /// Whether if Tagme_ in debug mode.
            /// </summary>
            public static bool IsDebug = false;
        }
    }

    /// <summary>
    /// Part of the data and other things that Tagme_ needs while running which not included in the Tagme_Core cause it's not a necessary, such as customized UI.
    /// It's not in the Tagme_Core cause it could be customized frequently and not always necessary to Tagme_.
    /// </summary>
    public static class Tagme_CustomizedCore
    {
        public class Template
        {
            public class DataBaseListViewTemplate
            {
                public string DataBasePath { get; set; }
                public string DataBaseTitle { get; set; }
                public string DataBaseDescription { get; set; }
                public object DataBaseCover { get; set; }
                public string DataBaseCreatedTime { get; set; }
                public string DataBaseModifiedTime { get;set; }
                public string DataBaseFileSize { get; set; }
                public string DataBaseAllSubItemCount { get; set; }
            }

            public class TagInputSuggestionTemplate
            {
                /// <summary>
                /// The name of the tag, this will be stored in database.
                /// This name uses 2 spaces to refer to properties. For display, the program will use highlighted "."
                /// </summary>
                public string TagName { get; set; }
                /// <summary>
                /// The ID of a stored Tag.
                /// </summary>
                public string ID { get; set; }
                /// <summary>
                /// The color used to let user know whether the tag is used or not
                /// </summary>
                public SolidColorBrush SignColor { get; set; } = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            }
        }
        public static List<Template.DataBaseListViewTemplate> DataBaseListViewSource = new List<Template.DataBaseListViewTemplate>();
        /// <summary>
        /// The customized data for Tagme_ UI or something else running.
        /// </summary>
        public static class CustomizedRunningData
        {

        }
    }
}

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
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace PDF_Creator
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();         
            TimeSpan tperiod = TimeSpan.FromSeconds(0.1);
            ThreadPoolTimer tTimer = ThreadPoolTimer.CreatePeriodicTimer(
                async (source) =>
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        () =>
                        {

                            myPage.Width = Window.Current.Bounds.Width;
                            grid_main.Width = Window.Current.Bounds.Width;
                            Klasse klasse = DataManager.Instance.Klasse;
                            if(klasse != null)
                            {
                                generateKlass();
                            }
                        }
                        );
                },
                    tperiod);
        }


        private void generateKlass()
        {
            Grid klassenGrid = new Grid();
            klassenGrid.HorizontalAlignment = HorizontalAlignment.Center;
            klassenGrid.VerticalAlignment = VerticalAlignment.Center;
            for (int i = 1; i <= KLASSENGRID_COL_COUNT; ++i)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(KLASSENGRID_COL_WIDTH);
                klassenGrid.ColumnDefinitions.Add(columnDefinition);
            }
            for (int i = 1; i <= KLASSENGRID_ROW_COUNT; ++i)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(KLASSENGRID_ROW_HEIGHT);
                klassenGrid.RowDefinitions.Add(rowDefinition);
            }
            Klasse klasse = DataManager.Instance.Klasse;
            int studentsCount = klasse.Students.Count;
            int cur = 0, col = 0, row = 0;
            while (cur < studentsCount)
            {
                TextBlock txt = new TextBlock();
                txt.FontSize = 12;
                if (col == 0 && row == 0)
                {
                    txt.Text = "Klasse: " + klasse.Name;
                    txt.FontWeight = FontWeights.Bold;

                }
                else if (col == 1 && row == 0)
                {
                    txt.Text = "Klassenleiter: " + klasse.Leiter;
                    txt.FontWeight = FontWeights.Bold;
                }
                else if (!(col == 2 && row == 0))
                {
                    txt.Text = klasse.Students[cur++];
                }

                klassenGrid.Children.Add(txt);
                Grid.SetColumn(txt, col);
                Grid.SetRow(txt, row);

                if (++row == KLASSENGRID_ROW_COUNT)
                {
                    row = 0;
                    col++;
                }
            }

            ScrollViewer sv = new ScrollViewer();
            sv.Content = klassenGrid;
            klass_border.Child = sv;
        }

        private async void BTN_change_school_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                // Application now has read/write access to the picked file
                BitmapImage img = new BitmapImage();
                img = await LoadImage(file);
                IMG_class.Source = img;
            }
            else
            {
            }
        }

        private async void BTN_school_source_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                // Application now has read/write access to the picked file
                BitmapImage img = new BitmapImage();
                img = await LoadImage(file);
                IMG_school.Source = img;
            }
            else
            {
            }
        }

        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

            bitmapImage.SetSource(stream);

            return bitmapImage;

        }

        
        private void BTN_open_Click(object sender, RoutedEventArgs e)

        {
            FileManager.readCSV();            
        }

        public void klasse_Changed(Klasse klasse)
        {
            if (klasse != null)
            {
                BTN_print.Visibility = Visibility.Visible;
                BTN_save.Visibility = Visibility.Visible;
            }
            else
            {
                BTN_print.Visibility = Visibility.Collapsed;
                BTN_save.Visibility = Visibility.Collapsed;
            }
        }
    }
}

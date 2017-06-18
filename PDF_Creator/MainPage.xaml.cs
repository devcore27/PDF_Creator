using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

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
                        }
                    );
                },
                tperiod
            );
        }

        private void generateKlass(Grid place)
        {
            Grid klassenGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            for (int i = 1; i <= KLASSENGRID_COL_COUNT; ++i)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition()
                {
                    Width = new GridLength(KLASSENGRID_COL_WIDTH)
                };
                klassenGrid.ColumnDefinitions.Add(columnDefinition);
            }
            for (int i = 1; i <= KLASSENGRID_ROW_COUNT; ++i)
            {
                RowDefinition rowDefinition = new RowDefinition()
                {
                    Height = new GridLength(KLASSENGRID_ROW_HEIGHT)
                };
                klassenGrid.RowDefinitions.Add(rowDefinition);
            }
            Klasse klasse = DataManager.Instance.KlasseAt(klassenCombo.SelectedIndex);
            int studentsCount = klasse.StudentsCount();
            int cur = 0, col = 0, row = 0;
            while (cur < studentsCount)
            {
                TextBlock txt = new TextBlock()
                {
                    FontSize = 14
                };
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
                    txt.Text = klasse.StudentAt(cur++);
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
            place.Children.Add(klassenGrid);
        }

        private void UpdateKlassenGrid(Klasse klasse)
        {
            klassenGrid.Children.Clear();
            if (klasse == null) return;
            if (klassenGrid.ColumnDefinitions.Count == 0)
                for (int i = 1; i <= KLASSENGRID_COL_COUNT; ++i)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition()
                    {
                        Width = new GridLength(KLASSENGRID_COL_WIDTH)
                    };
                    klassenGrid.ColumnDefinitions.Add(columnDefinition);
                }
            if (klassenGrid.RowDefinitions.Count == 0)
                for (int i = 1; i <= KLASSENGRID_ROW_COUNT; ++i)
                {
                    RowDefinition rowDefinition = new RowDefinition()
                    {
                        Height = new GridLength(KLASSENGRID_ROW_HEIGHT)
                    };
                    klassenGrid.RowDefinitions.Add(rowDefinition);
                }
            int studentsCount = klasse.StudentsCount();
            int cur = 0, col = 0, row = 0;
            while (cur < studentsCount)
            {
                TextBlock txt = new TextBlock()
                {
                    FontSize = 12
                };
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
                    txt.Text = klasse.StudentAt(cur++);
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
        }

        private async void BTN_change_school_Click(object sender, RoutedEventArgs e)
        {
            IMG_class.Visibility = Visibility.Visible;
            var picker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
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
            var picker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary,
            };
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
            FileManager.ReadCSV();
        }

        public void KlassenChanged(Klasse klasse)
        {
            if (!DataManager.Instance.IsEmpty())
            {
                BTN_print.Visibility = Visibility.Visible;
                //BTN_save.Visibility = Visibility.Visible;
                klassenCombo.Items.Add(klasse.Name);
                if (klassenCombo.SelectedIndex < 0)
                    klassenCombo.SelectedIndex = 0;
                COB_mode.IsEnabled = true;
            }
            else
            {
                BTN_print.Visibility = Visibility.Collapsed;
               //BTN_save.Visibility = Visibility.Collapsed;
                klassenCombo.Items.Clear();
                COB_mode.IsEnabled = false;
            }
        }

        Grid g = new Grid();

        private void COB_mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (COB_mode.SelectedIndex == 1)
            {
                GRID_prev.Visibility = Visibility.Collapsed;
                Grid.SetColumn(g, 2);
                Grid.SetColumnSpan(g, 11);
                Grid.SetRow(g, 2);
                Grid.SetRowSpan(g, 3);
                g.BorderBrush = new SolidColorBrush(Colors.Black);
                g.BorderThickness = new Thickness(2);
                generateKlass(g);
                grid_main.Children.Add(g);
                g.Visibility = Visibility.Visible;
            }
            else if (COB_mode.SelectedIndex == 0)
            {
                g.Visibility = Visibility.Collapsed;
                grid_main.Children.Remove(g);
                g.Children.Clear();
                if (GRID_prev != null)
                {
                    GRID_prev.Visibility = Visibility.Visible;
                }
                else
                {
                    //BTN_save.Visibility = Visibility.Collapsed;
                    //klassenCombo.Items.Clear();
                }
            }
        }

        private void KlassenCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Klasse klasse = DataManager.Instance.KlasseAt(klassenCombo.SelectedIndex);
            UpdateKlassenGrid(klasse);
        }
    }
}

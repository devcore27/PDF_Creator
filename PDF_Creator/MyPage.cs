using System;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Printing;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Shapes;

namespace PDF_Creator
{
    public sealed partial class MainPage : Page
    {
        private static int DEFAULT_PADDING = 10;
        private static int PAGE_PADDING = 30;
        private static int GRID_WIDTH = 1100 - 2 * PAGE_PADDING;
        private static int GRID_HEIGHT = 820 - 2 * PAGE_PADDING;
        private static int GRID_LEFT_COL_WIDTH = GRID_WIDTH * 17 / 40;
        private static int GRID_RIGHT_COL_WIDTH = GRID_WIDTH * 23 / 40;
        private static int GRID_ROW_HEIGHT = GRID_HEIGHT / 2;
        private static int KLASSENGRID_COL_COUNT = 2;
        private static int KLASSENGRID_ROW_COUNT = 14;
        private static int KLASSENGRID_COL_WIDTH = (GRID_LEFT_COL_WIDTH - 2 * DEFAULT_PADDING) / KLASSENGRID_COL_COUNT;
        private static int KLASSENGRID_ROW_HEIGHT = (GRID_ROW_HEIGHT - 2 * DEFAULT_PADDING) / KLASSENGRID_ROW_COUNT;

        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;
        private Grid grid;

        #region Register for printing

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register for PrintTaskRequested event
            printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;

            // Build a PrintDocument and register for callbacks
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;
            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
        }

        #endregion

        #region Showing the print dialog

        private async void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            InitPage();

            if (PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI
                    await PrintManager.ShowPrintUIAsync();
                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can't proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }
            else
            {
                // Printing is not supported on this device
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing not supported",
                    Content = "\nSorry, printing is not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }

        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Create the PrintTask.
            // Defines the title and delegate for PrintTaskSourceRequested
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            // Handle PrintTask.Completed to catch failed print jobs
            printTask.Completed += PrintTaskCompleted;

            printTask.Options.Orientation = PrintOrientation.Landscape;
            printTask.Options.MediaSize = PrintMediaSize.IsoA4;
        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            // Set the document source.
            args.SetSource(printDocSource);
        }

        #endregion

        #region Print preview

        private void Paginate(object sender, PaginateEventArgs e)
        {
            // As I only want to print one Rectangle, so I set the count to 1
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // Provide a UIElement as the print preview.
            this.Width = GRID_WIDTH;
            try
            {
                printDoc.SetPreviewPage(e.PageNumber, grid);
            }
            catch
            {
                ShowCD();
            }
        }

        private async void ShowCD()
        {
            ContentDialog noPrintingDialog = new ContentDialog()
            {
                Title = "Dokument cann ncot size error",
                Content = "\nSorry, can not reach size.",
                PrimaryButtonText = "OK"
            };
            await noPrintingDialog.ShowAsync();
        }

        #endregion

        #region Add pages to send to the printer

        private void AddPages(object sender, AddPagesEventArgs e)
        {
            printDoc.AddPage(grid);

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        #endregion

        #region Print task completed

        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, failed to print.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });
            }
        }

        #endregion

        #region Init Page

        private void InitPage()
        {
            grid = new Grid();
            RelativePanel rp = new RelativePanel();


            Image schoolImg = new Image()
            {
                Source = IMG_school.Source,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            StackPanel logoStack = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };
            //logoStack.HorizontalAlignment = HorizontalAlignment.Center;
            //logoStack.VerticalAlignment = VerticalAlignment.Center;

            TextBlock congratsText = new TextBlock()
            {
                Text = "Herzlichen Glückwunsch\nvom Lehrerkollegium\ndes Beruflichen Schulzentrums für Technik I",
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                FontSize = 22,
                FontWeight = FontWeights.Bold
            };
            Image logoImg = new Image()
            {
                Source = IMG_logo.Source,
                Height = 150
            };
            TextBlock zumText = new TextBlock()
            {
                Text = "zum",
                FontSize = 26,
                FontWeight = FontWeights.Bold
            };
            TextBlock causeText = new TextBlock()
            {
                Text = TBL_logo_title.Text,
                FontSize = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold
            };
            logoStack.Children.Add(congratsText);
            logoStack.Children.Add(logoImg);
            logoStack.Children.Add(zumText);
            logoStack.Children.Add(causeText);


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

            Image klassenFoto = new Image()
            {
                Source = IMG_class.Source,
                Height = GRID_ROW_HEIGHT
            };
            //klassenFoto.HorizontalAlignment = HorizontalAlignment.Center;
            //klassenFoto.VerticalAlignment = VerticalAlignment.Center;

            Rectangle rect = new Rectangle();
            rect.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
            rect.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
            rect.SetValue(RelativePanel.LeftOfProperty, logoStack);
            rect.SetValue(RelativePanel.RightOfProperty, schoolImg);
            rect.SetValue(RelativePanel.LeftOfProperty, klassenFoto);
            rect.SetValue(RelativePanel.RightOfProperty, klassenGrid);
            rect.Width = 30;

            schoolImg.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
            schoolImg.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
            schoolImg.SetValue(RelativePanel.AlignTopWithProperty, logoStack);
            schoolImg.SetValue(RelativePanel.AlignBottomWithProperty, klassenFoto);
            schoolImg.SetValue(RelativePanel.AlignLeftWithProperty, klassenGrid);
            schoolImg.SetValue(RelativePanel.AlignRightWithProperty, klassenGrid);
            schoolImg.VerticalAlignment = VerticalAlignment.Top;
            schoolImg.Margin = new Thickness(0, 0, 30, 0);


            logoStack.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
            logoStack.SetValue(RelativePanel.AlignRightWithPanelProperty, true);

            logoStack.Margin = new Thickness(0);
            if (IMG_class.Visibility == Visibility.Collapsed)
            {
            }
            else
            {
                logoStack.SetValue(RelativePanel.AlignLeftWithProperty, klassenFoto);
                logoStack.SetValue(RelativePanel.AlignRightWithProperty, klassenFoto);
            }

            klassenGrid.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
            klassenGrid.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
            klassenGrid.Margin = new Thickness(0, 0, 30, 0);


            klassenFoto.SetValue(RelativePanel.AlignRightWithPanelProperty, true);
            klassenFoto.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
            klassenFoto.Margin = new Thickness(0);

            rp.Margin = new Thickness(30);


            rp.Children.Add(rect);
            rp.Children.Add(schoolImg);
            rp.Children.Add(logoStack);
            rp.Children.Add(klassenGrid);
            rp.Children.Add(klassenFoto);

            grid.Children.Add(rp);

        }

        #endregion
    }
}

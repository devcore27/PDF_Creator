using System;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Printing;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;

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
        private static int KLASSENGRID_COL_COUNT = 3;
        private static int KLASSENGRID_ROW_COUNT = 11;
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
            initPage();

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
            printDoc.SetPreviewPage(e.PageNumber, grid);
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

        private void initPage()
        {
            grid = new Grid();

            // Create column definitions.
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(GRID_LEFT_COL_WIDTH);
            columnDefinition2.Width = new GridLength(GRID_RIGHT_COL_WIDTH);

            // Create row definitions.
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition1.Height = new GridLength(GRID_ROW_HEIGHT);
            rowDefinition2.Height = new GridLength(GRID_ROW_HEIGHT);

            // Attach definitions to grid.
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);
            grid.RowDefinitions.Add(rowDefinition1);
            grid.RowDefinitions.Add(rowDefinition2);

            Image schoolImg = new Image();
            schoolImg.Source = IMG_school.Source;
            schoolImg.Height = GRID_ROW_HEIGHT - 2 * DEFAULT_PADDING;            
            schoolImg.HorizontalAlignment = HorizontalAlignment.Center;
            schoolImg.VerticalAlignment = VerticalAlignment.Center;

            StackPanel logoStack = new StackPanel();
            logoStack.Orientation = Orientation.Vertical;
            logoStack.HorizontalAlignment = HorizontalAlignment.Center;
            logoStack.VerticalAlignment = VerticalAlignment.Center;
            TextBlock congratsText = new TextBlock();
            congratsText.Text = "Herzlichen Glückwunsch\nvom Lehrerkollegium\ndes Beruflichen Schulzentrums für Technik I";
            congratsText.HorizontalAlignment = HorizontalAlignment.Center;
            congratsText.TextAlignment = TextAlignment.Center;
            congratsText.FontSize = 22;
            congratsText.FontWeight = FontWeights.Bold;
            Image logoImg = new Image();
            logoImg.Source = IMG_logo.Source;
            logoImg.Height = 140;            
            TextBlock zumText = new TextBlock();
            zumText.Text = "zum";
            zumText.FontSize = 26;
            zumText.FontWeight = FontWeights.Bold;
            TextBlock causeText = new TextBlock();
            causeText.Text = "Berufsabschluss";
            causeText.FontSize = 40;
            causeText.HorizontalAlignment = HorizontalAlignment.Center;
            causeText.FontWeight = FontWeights.Bold;
            logoStack.Children.Add(congratsText);
            logoStack.Children.Add(logoImg);
            logoStack.Children.Add(zumText);
            logoStack.Children.Add(causeText);

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

            Image klassenFoto = new Image();
            klassenFoto.Source = IMG_class.Source;
            klassenFoto.Height = GRID_ROW_HEIGHT;
            klassenFoto.HorizontalAlignment = HorizontalAlignment.Center;
            klassenFoto.VerticalAlignment = VerticalAlignment.Center;
            klassenFoto.Margin = new Thickness(DEFAULT_PADDING, 0, 0, 0);

            grid.Children.Add(schoolImg);
            grid.Children.Add(logoStack);
            grid.Children.Add(klassenGrid);
            grid.Children.Add(klassenFoto);
            Grid.SetColumn(schoolImg, 0);
            Grid.SetRow(schoolImg, 0);
            Grid.SetColumn(logoStack, 1);
            Grid.SetRow(logoStack, 0);
            Grid.SetColumn(klassenGrid, 0);
            Grid.SetRow(klassenGrid, 1);
            Grid.SetColumn(klassenFoto, 1);
            Grid.SetRow(klassenFoto, 1);

            grid.Padding = new Thickness(PAGE_PADDING);
        }

        #endregion
    }
}

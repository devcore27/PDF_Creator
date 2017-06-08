﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace PDF_Creator
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BTN_change_school_Click(object sender, RoutedEventArgs e)
        {
           // IMG_school.Source  = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/StoreLogo.png", UriKind.Absolute) };
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

        private async void BTN_preview_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog1 cdm = new ContentDialog1();
            //cdm.SecondaryButtonClick += navigate_to_settings;
            await cdm.ShowAsync();
            //this.Frame.Navigate(typeof(PreviewPage), null);
            
        }
    

    }
}

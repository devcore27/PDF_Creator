using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

namespace PDF_Creator
{
    static class FileManager
    {
        public async static void readCSV()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".csv");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                localSettings.Values["is_csv_file_selectet"] = "true";
                try
                {
                    using (var inputStream = await file.OpenReadAsync())
                    using (var readStream = inputStream.AsStreamForRead())
                    using (var reader = new StreamReader(readStream))
                    {
                        string name = reader.ReadLine();
                        string leiter = reader.ReadLine();
                        DataManager.Instance.Klasse = new Klasse(name, leiter);
                        while (!reader.EndOfStream)
                        {
                            DataManager.Instance.Klasse.addStudent(reader.ReadLine());
                        }
                    }
                }
                catch (Exception ex)
                {
                    var dialog = new MessageDialog("Error: Could not read file from disk. Original error: " + ex.Message); await dialog.ShowAsync();
                }
            }            
        }

        public async static void writeCSV()
        {
            Klasse klasse = DataManager.Instance.Klasse;
            if (klasse == null) return;

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("CSV-Datei", new List<string>() { ".csv" });
            savePicker.SuggestedFileName = klasse.Name;

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                try
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    await FileIO.WriteTextAsync(file, klasse.Name + '\n');
                    await FileIO.AppendTextAsync(file, klasse.Leiter + '\n');
                    await FileIO.AppendLinesAsync(file, klasse.Students);

                }
                catch (Exception ex)
                {
                    var dialog = new MessageDialog("Error: Could not write file to disk. Original error: " + ex.Message); await dialog.ShowAsync();
                }
            }
        }
    }
}

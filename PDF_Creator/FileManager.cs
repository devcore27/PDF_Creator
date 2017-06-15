using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

namespace PDF_Creator
{
    static class FileManager
    {
        public async static void ReadCSV()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".csv");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                DataManager.Instance.Clear();
                try
                {
                    using (var inputStream = await file.OpenReadAsync())
                    using (var readStream = inputStream.AsStreamForRead())
                    using (var reader = new StreamReader(readStream))
                    {
                        Dictionary<string, Klasse> dict = new Dictionary<string, Klasse>();

                        // skip legend
                        reader.ReadLine();

                        while (!reader.EndOfStream)
                        {
                            string[] data = reader.ReadLine().Split(';');
                            string klassenName = data[2];
                            if (!dict.ContainsKey(klassenName))
                            {
                                string leiter = (data[4][0] == 'm' ? "Herr " : "Frau ") + data[3];
                                dict.Add(klassenName, new Klasse(klassenName, leiter));
                            }
                            dict[klassenName].AddStudent(data[1], data[0]);                            
                        }

                        List<string> keysList = new List<string>();
                        foreach (var klasse in dict)
                            keysList.Add(klasse.Key);
                        keysList.Sort();

                        foreach (var key in keysList)
                            DataManager.Instance.AddKlasse(dict[key]);
                    }
                }
                catch (Exception ex)
                {
                    var dialog = new MessageDialog("Error: Could not read file from disk. Original error: " + ex.Message); await dialog.ShowAsync();
                }
            }
        }

        public async static void WriteCSV()
        {   
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("CSV-Datei", new List<string>() { ".csv" });            
            savePicker.SuggestedFileName = "klassenliste";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                try
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    await FileIO.WriteTextAsync(file, "Name;Vorname;Klasse;Klassenlehr;Geschlecht" + '\n');

                    for (int i = 0; i < DataManager.Instance.Count(); ++i)
                    {
                        Klasse klasse = DataManager.Instance.KlasseAt(i);
                        for (int j = 0; j != klasse.StudentsCount(); ++j)
                        {
                            string[] name = klasse.StudentAt(j).Split(',');
                            string entry = name[0] + ';' +
                                           name[1].Substring(1) + ';' +
                                           klasse.Name + ';' +
                                           klasse.Leiter.Substring(5) + ';' +
                                           (klasse.Leiter[0] == 'H' ? "männlich" : "weiblich");
                            await FileIO.AppendTextAsync(file, entry + '\n');
                        }                            
                    }
                    

                }
                catch (Exception ex)
                {
                    var dialog = new MessageDialog("Error: Could not write file to disk. Original error: " + ex.Message); await dialog.ShowAsync();
                }
            }
        }
    }
}

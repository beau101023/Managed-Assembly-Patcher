using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.IO;
using System.Collections.ObjectModel;
using DiffMatchPatch;

namespace MAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string baseFile = null;
        static string modFile = null;

        static string baseFile1 = null;

        ObservableCollection<FileInfo> fileList = new ObservableCollection<FileInfo>();

        public MainWindow()
        {
            InitializeComponent();

            ModList.DisplayMemberPath = "Name";
            ModList.ItemsSource = fileList;
        }

        private string GetFilePathFromDialog(string defaultExt, string filter)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = defaultExt;
            dlg.Filter = filter;


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                return filename;
            }
            // Else
            return null;
        }

        private string CreateFileFromDialog(string defaultExt, string filter)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = defaultExt;
            dlg.Filter = filter;


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                return filename;
            }
            // Else
            return null;
        }

        private void BaseSelect_Click(object sender, RoutedEventArgs e)
        {
            string temp = GetFilePathFromDialog(".exe", "EXE Files (*.exe)|*.exe|DLL Files (*.dll)|*.dll|All Files|*.*");

            if(temp == null)
            {
                baseTextBox.Content = "Select valid file";
                return;
            }
            else
            {
                baseFile = temp;
                baseTextBox.Content = "";
            }
        }

        private void ModSelect_Click(object sender, RoutedEventArgs e)
        {
            string temp = GetFilePathFromDialog(".exe", "EXE Files (*.exe)|*.exe|DLL Files (*.dll)|*.dll|All Files|*.*");

            if (temp == null)
            {
                modTextBox.Content = "Select valid file";
                return;
            }
            else
            {
                modFile = temp;
                modTextBox.Content = "";
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (baseFile == null || modFile == null)
            {
                createTextBox.Content = "Please choose a valid file for base and modified files.";
                return;
            }

            if(baseFile == modFile)
            {
                createTextBox.Content = "Please select two different files.";
                return;
            }

            if (ModeSelector.SelectedItem == null)
            {
                createTextBox.Content = "Please select an analysis mode.";
                return;
            }

            if (ModeSelector.SelectedItem == RawMode)
            {
                string temp = CreateFileFromDialog(".diffmod", "Diffmod Files (*.diffmod)|*.diffmod|All Files|*.*");

                if (temp == null)
                {
                    createTextBox.Content = "Select valid file";
                    return;
                }

                AnalysisResults r = Analyzer.RawAnalyze(baseFile, modFile);

                if (r.status != AnalysisResults.AnalysisStatus.Success)
                {
                    if(r.status == AnalysisResults.AnalysisStatus.FilesAreEqual)
                    {
                        createTextBox.Content = "The files you have selected appear to have the same contents.";
                    }
                    if (r.status == AnalysisResults.AnalysisStatus.FilesSamePath)
                    {
                        createTextBox.Content = "Please select two different files.";
                    }
                    if (r.status == AnalysisResults.AnalysisStatus.PatchError)
                    {
                        createTextBox.Content = "Something went horribly wrong, please try again.";
                    }
                    return;
                }

                createTextBox.Content = "Success! Diffmod can be found at " + temp;

                File.WriteAllText(temp, r.editScript);
            }

            if(ModeSelector.SelectedItem == DNMode)
            {
                
            }
        }

        private void AddDiffmod_Click(object sender, RoutedEventArgs e)
        {
            string temp = GetFilePathFromDialog(".diffmod", "Diffmod Files (*.diffmod)|*.diffmod|All Files (*.*)|*.*");

            if(temp == null)
            {
                return;
            }

            FileInfo f = new FileInfo(temp);

            fileList.Add(f);
        }

        private void RemoveDiffmod_Click(object sender, RoutedEventArgs e)
        {
            if (ModList.SelectedItem != null)
            {
                fileList.RemoveAt(ModList.SelectedIndex);
            }
        }

        private void SelectBaseFile_Click(object sender, RoutedEventArgs e)
        {
            baseFile1 = GetFilePathFromDialog(".exe", "EXE Files (*.exe)|*.exe|DLL Files (*.dll)|*.dll|All Files|*.*");
        }

        private void MoveItemUp_Click(object sender, RoutedEventArgs e)
        {
            if (ModList.SelectedIndex != -1 && ModList.SelectedIndex != 0)
            {
                int oldIndex = ModList.SelectedIndex;

                FileInfo f = fileList[oldIndex];

                fileList.RemoveAt(ModList.SelectedIndex);

                fileList.Insert(oldIndex - 1, f);
            }
        }

        private void MoveItemDown_Click(object sender, RoutedEventArgs e)
        {
            if (ModList.SelectedIndex != -1 && ModList.SelectedIndex != fileList.Count - 1)
            {
                int oldIndex = ModList.SelectedIndex;

                FileInfo f = fileList[oldIndex];

                fileList.RemoveAt(ModList.SelectedIndex);

                fileList.Insert(oldIndex + 1, f);
            }
        }

        private void ExportModdedFile_Click(object sender, RoutedEventArgs e)
        {
            if(baseFile1 == null)
            {
                return;
            }
            if(fileList.Count == 0)
            {
                return;
            }
            if(fileList.Count == 1)
            {
                string exportFile = CreateFileFromDialog(".exe", "EXE Files (*.exe)|*.exe|DLL Files (*.dll)|*.dll|All Files|*.*");

                if(exportFile == null)
                {
                    return;
                }

                string modFileContent = File.ReadAllText(fileList[0].FullName);

                List<Patch> patches = Patcher.ParseStringToPatches(modFileContent);

                PatchResults p = Patcher.ApplyPatches(baseFile1, patches);

                if(p.status == PatchResults.PatchStatus.Error)
                {
                    ErrorDisplay.Content = "Something is really wrong, please Contact the Developer";
                }
                else if(p.status == PatchResults.PatchStatus.Failure)
                {
                    ErrorDisplay.Content = "Patch failure. The current patch files were probably not intended to modify the selected base file.";
                }
                else if(p.status == PatchResults.PatchStatus.PartialPatch)
                {
                    ErrorDisplay.Content = "File only partially patched, may cause unexpected behaviour.";
                }
                else if(p.status == PatchResults.PatchStatus.Success)
                {
                    File.WriteAllText(exportFile, p.patchedText);
                    ErrorDisplay.Content = "Patched file can be found at" + exportFile;
                }
            }
            if(fileList.Count > 1)
            {
                string exportFile = CreateFileFromDialog(".exe", "EXE Files (*.exe)|*.exe|DLL Files (*.dll)|*.dll|All Files|*.*");

                if (exportFile == null)
                {
                    return;
                }

                List<FileInfo> modFiles = new List<FileInfo>(fileList);

                List<List<Patch>> patchLists = new List<List<Patch>>();

                foreach(FileInfo f in modFiles)
                {
                    patchLists.Add(Patcher.ParseStringToPatches(File.ReadAllText(f.FullName)));
                }

                List<Patch> patches = Patcher.CombinePatches(patchLists);

                PatchResults p = Patcher.ApplyPatches(baseFile1, patches);

                if (p.status == PatchResults.PatchStatus.Error)
                {
                    ErrorDisplay.Content = "Something is really wrong, please Contact the Developer";
                }
                else if (p.status == PatchResults.PatchStatus.Failure)
                {
                    ErrorDisplay.Content = "Patch failure. The current patch files were probably not intended to modify the selected base file.";
                }
                else if (p.status == PatchResults.PatchStatus.PartialPatch)
                {
                    ErrorDisplay.Content = "File only partially patched, may cause unexpected behaviour.";
                }
                else if (p.status == PatchResults.PatchStatus.Success)
                {
                    File.WriteAllText(exportFile, p.patchedText);
                    ErrorDisplay.Content = "Patched file can be found at" + exportFile;
                }
            }
            return;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace MAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string baseFile = "";
        static string modFile = "";

        public MainWindow()
        {
            InitializeComponent();
        }
        private string getFilePathFromDialog()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


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

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            var filepath = getFilePathFromDialog();
            file1Dir.Text = filepath;
            baseFile = filepath;
        }

        private void Test2_Click(object sender, RoutedEventArgs e)
        {
            var filepath = getFilePathFromDialog();
            file2Dir.Text = filepath;
            modFile = filepath;
        }

        private void makeResult_Click(object sender, RoutedEventArgs e)
        {
            AnalysisResults result = Analyzer.RawAnalyze(baseFile, modFile);

            if(result.status != AnalysisResults.AnalysisStatus.Success)
            {
                if (result.status == AnalysisResults.AnalysisStatus.FilesAreEqual)
                {
                    Result.Text = "Error: files are equal!";
                    return;
                }
                else
                {
                    // !Error!
                    Result.Text = "Error: PatchError";
                    return;
                }
            }

            string moddedFile = System.IO.Path.GetFileNameWithoutExtension(baseFile) + "_modded" + System.IO.Path.GetExtension(baseFile);

            File.Copy(baseFile, moddedFile);

            Analyzer.ApplyPatches(moddedFile, result.editScript);
        }
    }
}

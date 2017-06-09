using System;
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
using System.Collections.ObjectModel;

namespace MAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string baseFile = null;
        static string modFile = null;

        ObservableCollection<FileInfo> _fileList = new ObservableCollection<FileInfo>();

        public MainWindow()
        {
            InitializeComponent();

            listBox1.DisplayMemberPath = "Name";
            listBox1.ItemsSource = _fileList;

            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(s_PreviewMouseLeftButtonDown)));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(listbox1_Drop)));
            listBox1.ItemContainerStyle = itemContainerStyle;
        }

        void s_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        void listbox1_Drop(object sender, DragEventArgs e)
        {
            FileInfo droppedData = e.Data.GetData(typeof(FileInfo)) as FileInfo;
            FileInfo target = ((ListBoxItem)(sender)).DataContext as FileInfo;

            int removedIdx = listBox1.Items.IndexOf(droppedData);
            int targetIdx = listBox1.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                _fileList.Insert(targetIdx + 1, droppedData);
                _fileList.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (_fileList.Count + 1 > remIdx)
                {
                    _fileList.Insert(targetIdx, droppedData);
                    _fileList.RemoveAt(remIdx);
                }
            }
        }

        private string GetFilePathFromDialog()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".exe";
            dlg.Filter = "DLL Files (*.dll)|*.dll|EXE Files (*.exe)|*.exe|All Files|*.*";


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
    }
}

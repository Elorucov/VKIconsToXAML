using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace VKIconsToXAML {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Multiselect = true;
            sfd.AddExtension = true;
            sfd.DefaultExt = ".svg";
            sfd.Filter = "Scalable vector graphics file (*.svg)|*.svg";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (sfd.ShowDialog() == true) {
                Parse(sfd.FileNames);
            }
        }


        private void Parse(string[] fileNames) {
            progressBar.Visibility = Visibility.Visible;
            fileButton.Visibility = Visibility.Collapsed;
            progressBar.Maximum = fileNames.Length;
            progressBar.Value = 0;
            files.Inlines.Clear();
            Task.Run(() => {
                foreach (string fileName in fileNames) {
                    using (VKIconSVGParser p = new VKIconSVGParser()) {
                        Tuple<bool, string, string> res = p.Parse(fileName);
                        string r = res.Item1 ? "OK" : "FAIL";
                        Dispatcher.Invoke(() => {
                            files.Inlines.Add(new Run {
                                Text = $"{fileName} — {r}",
                                Foreground = new SolidColorBrush(res.Item1 ? Colors.Black : Colors.Red)
                            });
                            files.Inlines.Add(new LineBreak());

                            code.Text += res.Item2 + "\n";
                            code2.Text += res.Item3 + "\n";
                            progressBar.Value++;
                            if(progressBar.Value == fileNames.Length) {
                                progressBar.Visibility = Visibility.Collapsed;
                                fileButton.Visibility = Visibility.Visible;
                            }
                        });
                    }
                }
            });
            
        }
    }
}

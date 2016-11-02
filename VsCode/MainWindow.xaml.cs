using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Newtonsoft.Json;
using VsCode.Annotations;

namespace VsCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel) (this.DataContext)).CopyToClipboard();
        }

        private void Insert(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)(this.DataContext)).PasteFromClipboard();
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _title;
        private string _prefix;
        private string _description;
        private string _input;

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Output));
            }
        }

        public string Prefix
        {
            get { return _prefix; }
            set
            {
                if (value == _prefix) return;
                _prefix = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Output));
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Output));
            }
        }

        public string Input
        {
            get { return _input; }
            set
            {
                if (value == _input) return;
                _input = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Output));
            }
        }

        public string Output
        {
            get
            {
                if (Input == null)
                    return string.Empty;

                using (var reader = new StringReader(Input))
                {
                    var body = new List<string>();
                    while (reader.Peek() > 0)
                    {
                        body.Add(reader.ReadLine());
                    }

                    return JsonConvert.SerializeObject(new
                        {
                            PrutteMaskine = new
                            {
                                prefix = Prefix,
                                body,
                                description = Description
                            }
                        }, Formatting.Indented)
                        .Replace("PrutteMaskine", this.Title)
                        .TrimStart('{')
                        .TrimEnd('}');
                }
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void CopyToClipboard()
        {
            Clipboard.SetText(Output);
        }

        public void PasteFromClipboard()
        {
            this.Input = Clipboard.GetText(TextDataFormat.UnicodeText);
        }
    }
}
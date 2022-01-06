using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.Editors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MultiselectComboBox
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void OnInputSubmitted(object sender, ComboBoxInputSubmittedEventArgs e)
        {
            var emailString = e.Text;
            bool isEmail = Regex.IsMatch(emailString, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            // If given input is a valid email format
            if (isEmail)
            {
                // Create new person for selection
                Person newPerson = new Person();
                newPerson.EmailID = emailString;
                int atIndex = emailString.LastIndexOf('@');
                int dotIndex = emailString.IndexOf('.');
                string name = emailString.Substring(0, atIndex);
                if (name.Contains('.'))
                {
                    newPerson.FirstName = name.Substring(0, dotIndex);
                    newPerson.LastName = name.Substring(dotIndex + 1);
                }
                else
                {
                    newPerson.FirstName = name;
                }
                e.Item = newPerson;
            }

            // If email is id invalid, show error dialog.
            else
            {
                var dialog = new ContentDialog();
                dialog.CloseButtonText = "Close";
                dialog.Content = "Invalid email id!";
                dialog.Title = "Error";
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }
    }
}

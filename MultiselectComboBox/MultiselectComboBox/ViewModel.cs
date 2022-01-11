using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Core;
using Syncfusion.UI.Xaml.Editors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MultiselectComboBox
{
    public class ViewModel : NotificationObject, IComboBoxFilterBehavior
    {
        public ViewModel()
        {
            Persons = new ObservableCollection<Person>();
            Persons.Add(createNewPerson("Anne", "Dodsworth", "anne.dodsworth@mail.com"));
            Persons.Add(createNewPerson("Andrew", "Fuller", "andrew.fuller@mail.com"));
            Persons.Add(createNewPerson("Emilio", "Alvaro", "emilio.alvaro@mail.com"));
            Persons.Add(createNewPerson("Janet", "Leverling", "janet.leverling@mail.com"));
            Persons.Add(createNewPerson("Laura", "Callahan", "laura.callahan@mail.com"));
            Persons.Add(createNewPerson("Margaret", "Peacock", "margaret.peacock@mail.com"));
            Persons.Add(createNewPerson("Michael", "Suyama", "michael.suyama@mail.com"));
            Persons.Add(createNewPerson("Nancy", "Davolio", "nancy.davolio@mail.com"));
            Persons.Add(createNewPerson("Robert", "King", "robert.king@mail.com"));
            Persons.Add(createNewPerson("Steven", "Buchanan", "steven.buchanan@mail.com"));

            InputSubmitted = new DelegateCommand(OnInputSubmitted, CanExe);

        }

        private bool CanExe(object obj)
        {
            return true;
        }

        private ObservableCollection<Person> persons;

        public ObservableCollection<Person> Persons
        {
            get { return persons; }
            set
            {
                if (persons != value)
                {
                    persons = value;
                    RaisePropertyChanged(nameof(Persons));
                }
            }
        }

        public ICommand InputSubmitted { get; set; }

        private Person createNewPerson(string firstName, string lastName, string eMail)
        {
            return new Person
            {
                FirstName = firstName,
                LastName = lastName,
                EmailID = eMail
            };
        }
        public List<int> GetMatchingIndexes(SfComboBox source, ComboBoxFilterInfo filterInfo)
        {
            var items = Persons;
            var ignoreCase = StringComparison.InvariantCultureIgnoreCase;

            // get person details whose name or email id contains the given input.
            var filteredItems = items.Where(
                                    person => person.FirstName.Contains(filterInfo.Text, ignoreCase) ||
                                    person.LastName.Contains(filterInfo.Text, ignoreCase) ||
                                    person.EmailID.Contains(filterInfo.Text, ignoreCase));

            // find the indices of the item to be shown in the drop-down.
            var indices = filteredItems.Select<Person, int>(p => items.IndexOf(p)).ToList();

            return indices;
        }

        public List<int> GetMatchingIndexes2(SfComboBox source, ComboBoxFilterInfo filterInfo)
        {
            var items = Persons;
            var ignoreCase = StringComparison.InvariantCultureIgnoreCase;

            // avoid showing already selected items in dropdown.
            var unselectedItems = Persons.Except(source.SelectedItems).OfType<Person>();

            // get person details whos name or email id stats with the given input.
            var firstPriority = unselectedItems.Where(
                                    person => person.FirstName.StartsWith(filterInfo.Text, ignoreCase) ||
                                    person.LastName.StartsWith(filterInfo.Text, ignoreCase) ||
                                    person.EmailID.StartsWith(filterInfo.Text, ignoreCase));

            // append person whosh name or email id contains the given input.
            var secondPriority = unselectedItems.Except(firstPriority).Where(
                                    person => person.FirstName.Contains(filterInfo.Text, ignoreCase) ||
                                    person.LastName.Contains(filterInfo.Text, ignoreCase) ||
                                    person.EmailID.Contains(filterInfo.Text, ignoreCase));

            // find the indices of the item to be shown in the dropdown.
            var indices = firstPriority.Union(secondPriority).Select<Person, int>(p => items.IndexOf(p)).ToList();

            return indices;
        }

        public void OnInputSubmitted(object e)
        {
            var args = e as ComboBoxInputSubmittedEventArgs;
            var emailString = args.Text;
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
                args.Item = newPerson;
            }

            // If email is id invalid, show error dialog.
            else
            {
                var dialog = new ContentDialog();
                dialog.CloseButtonText = "Close";
                dialog.Content = "Invalid email id!";
                dialog.Title = "Error";
                // dialog.XamlRoot = this.Content.XamlRoot;
                // await dialog.ShowAsync();
            }
        }
    }
}

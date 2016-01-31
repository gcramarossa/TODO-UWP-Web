/*
 * Created by Giuseppe Cramarossa
 * January 2016
 * This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License. 
 * To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/.
 */

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Html;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// Il modello di elemento per la pagina vuota è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace TODO_UWP_A4D
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Contains the last list box item selected 
        /// </summary>
        private ListBoxItem _lastSelectedListBox;
        private string _websiteContent = "";
        public MainPage()
        {
            this.InitializeComponent();
            CreateUIDFile();
        }

        /// <summary>
        /// Create a file that contains an user id file
        /// </summary>
        private async void CreateUIDFile()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile uidFile = await storageFolder.CreateFileAsync("uid.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
            while (!uidFile.IsAvailable)
            {
                // do nothing until the file is not available
            }

            string content = await Windows.Storage.FileIO.ReadTextAsync(uidFile);
            if (content == "")
            {
                uidbox.Text = _generateUID(25);
                await Windows.Storage.FileIO.WriteTextAsync(uidFile, uidbox.Text);
            }
            else
            {
                uidbox.Text = content;
            }

        }

        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="maxValues">the number of the values</param>
        /// <returns></returns>
        private string _generateUID(byte maxValues)
        {

            char[] letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            Random randomNumber = new Random(); 
            string uid = "" + randomNumber.Next(0, letters.Length - 1);
            for (byte i = 1; i < maxValues; i++)
            {
                uid += letters[randomNumber.Next(0, letters.Length - 1)];
            }
            return uid;
        }

        /// <summary>
        /// Generated when an item in the listbox is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout menu = new MenuFlyout();
            MenuFlyoutItem markAsCompleted = new MenuFlyoutItem();
            ListBoxItem newTaskList = (ListBoxItem)sender;
            _lastSelectedListBox = newTaskList;
            if (((sender as ListBoxItem).Background as SolidColorBrush).Color == Colors.Green)
            {
                markAsCompleted.Text = "Mark as incomplete";
                markAsCompleted.Click += MenuMarkIncomplete;
            }
            else
            {
                markAsCompleted.Text = "Mark as completed";
                markAsCompleted.Click += MenuMarkAsCompleted;
            }
            MenuFlyoutItem menuRemove = new MenuFlyoutItem();
            menu.Items.Add(markAsCompleted);
            menuRemove.Text = "Remove";
            menuRemove.Click += MenuRemove;
            menu.Items.Add(menuRemove);
            menu.ShowAt((FrameworkElement)sender);
 
        }

        /// <summary>
        /// Remove a task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuRemove(object sender, RoutedEventArgs e)
        {
            listView.Items.RemoveAt(listView.Items.IndexOf(_lastSelectedListBox));
            website.Source = new Uri(websiteURI.Text + "api/removeTask.php?taskID=" + _lastSelectedListBox.Name + "&userID=" + uidbox.Text);
        }

        /// <summary>
        /// Mark a task as completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuMarkAsCompleted(object sender, RoutedEventArgs e)
        {
            _lastSelectedListBox.Background = new SolidColorBrush(Colors.Green);
            website.Source = new Uri(websiteURI.Text + "api/editStatusTask.php?taskID=" + _lastSelectedListBox.Name + "&userID=" + uidbox.Text + "&status=1");
        }
        
        /// <summary>
        /// Mark a task as incomplete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuMarkIncomplete(object sender, RoutedEventArgs e)
        {
            _lastSelectedListBox.Background = new SolidColorBrush(Colors.Transparent);
            website.Source = new Uri(websiteURI.Text + "api/editStatusTask.php?taskID=" + _lastSelectedListBox.Name + "&userID=" + uidbox.Text + "&status=0");
        }

        /// <summary>
        /// Generated when the "add task" button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = new ListBoxItem();
            string value;
            textTask.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out value);
            value = value.Trim();
            if (value != "")
            {
                website.Source = new Uri(websiteURI.Text + "api/saveTask.php?task=" + value + "&userID=" + uidbox.Text);
                string oldID = _websiteContent;
                
                // Wait until the website content is updated
                do
                {
                    _websiteContent = await website.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
                    
                    /*
                        The first time this app is launched, _websiteContent is filled with the content
                        of the index page.
                        For this reason, wait until the websitecontent is updated with the id of the task
                    */

                    while (!Regex.Match(_websiteContent, @"<html><head></head><body>([0-9]+)</body></html>").Success)
                    {
                        _websiteContent = await website.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
                    }
                } while (_websiteContent == oldID);
                lbi.Name = HtmlUtilities.ConvertToText(_websiteContent);
                lbi.Content = value;
                lbi.Width = Window.Current.Bounds.Width;
                lbi.Tapped += ListBoxItem_Tapped;
                listView.Items.Add(lbi);
            }
            textTask.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");
        }

        /// <summary>
        /// Once a website is inserted. the listview is filled with the tasks created by the user
        /// </summary>
        private async void _fillList()
        {

            website.Source = new Uri(websiteURI.Text + "api/readTasks.php?userID=" + uidbox.Text);
            await Task.Delay(2000);
            _websiteContent = await website.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
            _websiteContent = HtmlUtilities.ConvertToText(_websiteContent);
            string[] tasks = _websiteContent.Split(new string[] { "#ENDTASK#" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var task in tasks)
            {
                string[] row = task.Split(':');
                ListBoxItem newTaskList = new ListBoxItem();
                newTaskList.Name = row[0];
                newTaskList.Content = row[2];
                newTaskList.Width = Window.Current.Bounds.Width;
                newTaskList.Background = new SolidColorBrush(Colors.Transparent);
                if (row[1] == "1")
                {
                    newTaskList.Background = new SolidColorBrush(Colors.Green);
                }
                newTaskList.Tapped += ListBoxItem_Tapped;
                listView.Items.Add(newTaskList);
            }
        }

        /// <summary>
        /// Generated when the "launch program" is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (websiteURI.Text == "")
            {
                Windows.UI.Popups.MessageDialog showWarning = new Windows.UI.Popups.MessageDialog("Please enter the program website before clicking the button");
                showWarning.Options = Windows.UI.Popups.MessageDialogOptions.None;
                await showWarning.ShowAsync();
            }
            else
            {
                website.Source = new Uri(websiteURI.Text + "api/readTasks.php?userID=" + uidbox.Text);
                _fillList();
                websiteURI.IsReadOnly = true;
                confirmButton.IsEnabled = false;
                applicationPivot.Visibility = Visibility.Visible;
            }

        }
    }
}

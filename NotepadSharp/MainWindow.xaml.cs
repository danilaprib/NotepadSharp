using Microsoft.Win32;
using NotepadSharp.Services;
using NotepadSharp.Services.ExpressionParser;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.VisualBasic.FileIO; // for moving files to Recycle Bin
using static System.Net.WebRequestMethods;
using NotepadSharp.Commands;



namespace NotepadSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TerminalService _terminalService;
        Lex _lex;
        TokenParser _tokenParser;

        private const string NUMBER_REGEX = @"[0-9]+\.?[0-9]*";

        

        public MainWindow()
        {
            InitializeComponent();

            string curDir = current_directory.Text;

            output.Text = $"{curDir}\n\n";
            text_panel.Text = "";

            _terminalService = new TerminalService();
            _lex = new Lex();
            _tokenParser = new TokenParser();

        }


        private void open_button_Click(object sender, RoutedEventArgs a)
        {
            string fileName = file_name.Text;

            string? text = _terminalService.openFile(fileName);            

            if (text != null)
            {

                save_button.IsEnabled = true;
                save_as_button.IsEnabled = true;
                exit_button.IsEnabled = true;
                save_and_exit_button.IsEnabled = true;

                text_panel.IsReadOnly = false;
                text_panel.Text = text;
                text_panel.Focus();

                output.Text += $"{fileName} opened\n";
            }
            else
            {
                output.Text += $"Unable to open {fileName}\n";
            }

            output.ScrollToEnd();
        }

        private void list_button_Click(object sender, RoutedEventArgs e)
        {
            string curDir = current_directory.Text;

            List<string> contents = _terminalService.listContent(curDir);


            if (contents.Count != 0)
            {
                foreach (var c in contents)
                {
                    output.Text += c;
                }
            }


            output.ScrollToEnd();
        }

        private async void save_button_ClickAsync(object sender, RoutedEventArgs e)
        {

            string fileName = file_name.Text;
            string? text = text_panel.Text;
            await _terminalService.saveFile(fileName, text);


            output.Text += $"{fileName} saved\n\n";
            output.ScrollToEnd();
        }

        private void exit_button_Click(object sender, RoutedEventArgs e)
        {

            string fileName = file_name.Text;
            text_panel.Text = "";

            save_button.IsEnabled = false;
            save_as_button.IsEnabled = false;
            exit_button.IsEnabled = false;
            save_and_exit_button.IsEnabled = false;

            output.Text += $"{fileName} exited\n\n";
            output.ScrollToEnd();
        }

        private async void save_and_exit_button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string fileName = file_name.Text;
            string? text = text_panel.Text;
            await _terminalService.saveFile(fileName, text);

            text_panel.Text = "";

            save_button.IsEnabled = false;
            save_as_button.IsEnabled = false;
            exit_button.IsEnabled = false;
            save_and_exit_button.IsEnabled = false;


            output.Text += $"{fileName} saved and exited\n\n";
            output.ScrollToEnd();
        }

        private async void save_as_button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = file_name.Text;
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();
            
            if (result == true)
            {
                
                string filename = dialog.FileName;

                await _terminalService.saveFile(filename, text_panel.Text);

                output.Text += $"Saved as {filename}\n\n";
                output.ScrollToEnd();
            }
        }

        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            string fileName = file_name.Text;
            bool res = _terminalService.deleteFile(fileName);

            if (res)
            {
                output.Text += $"{fileName} moved to Recycle Bin\n";
            }
            else
            {
                output.Text += $"Unable to move {fileName} to Recycle Bin\n";
            }

            output.ScrollToEnd();
        }

        private void perma_delete_button_Click(object sender, RoutedEventArgs e)
        {
            string fileName = file_name.Text;
            bool res = _terminalService.deleteFile(fileName);

            if (res)
            {
                output.Text += $"{fileName} deleted\n";
            }
            else
            {
                output.Text += $"Unable to delete {fileName}\n";
            }

            output.ScrollToEnd();
        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {
            output.Text = string.Empty;
            output.Text = current_directory.Text + "\n\n";
            output.ScrollToEnd();
        }

        private void settingsThemeSubItem_Click(object sender, RoutedEventArgs e)
        {
            //current_directory.Style = Style = System.Windows.Style   "{DynamicResource lightThemeTextBlock}"
            //Window.Style

            //if (current_directory.Style == Window.Style.Resources.Keys)
            //ResourceDictionary rd = 
            //Window.St
        }

        private void create_button_Click(object sender, RoutedEventArgs e)
        {
            string file = file_name.Text;
            bool wasCreated = _terminalService.createFile(file);

            if (wasCreated)
            {
                output.Text += $"{file} created\n";

            }
            else
            {
                output.Text += $"Attempting to create {file}...\n";
                output.Text += $"ERROR: {file} already exists\n";
            }

            output.ScrollToEnd();
        }

        private void calc_insert_button_Click(object sender, RoutedEventArgs e)
        {
            string expression = calc_panel.Text;

            if (expression.Length != 0)
            {
                text_panel.Text += $" {expression} ";
            }
        }

        private void equal_button_Click(object sender, RoutedEventArgs e)
        {
            string expression = calc_panel.Text;

            if (expression == "") return;

            try
            {
                var tokens = _lex.GenerateTokens(expression);

                //if (tokens.Count == 0)
                //{
                //    Console.WriteLine("NOTHING TO PARSE: EXPRESSION IS EMPTY OR INCORRECT");
                //    return Array.Empty<char>();
                //}

                double expressionResult = _tokenParser.ParseTokens(tokens);



                calc_panel.Text = expressionResult.ToString();
            }
            catch (Exception ex)
            {
                output.Text += ex.ToString() + '\n';
            }

            output.ScrollToEnd();
        }

        private void calc_panel_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // check if text in panel is a number
            string expression = calc_panel.Text;
            var match = Regex.Match(expression, NUMBER_REGEX);


            // ensure text in panel is a number and not a word
            if (!match.Success)
            {
                calc_panel.Text = null;
            }
        }

        private void converter_convert_button_Click(object sender, RoutedEventArgs e)
        {

            // format to 2 digits after point and stuff like that

            // use dictionaries here to adjust conversion formulas 

            if (converter_from_panel.Text != null && converter_from_panel.Text != "")
            {
                double from = Convert.ToDouble(converter_from_panel.Text);
                double to = from * 2.54; // use #define like INCH_TO_CM instead of hardcoded multiplier
                converter_to_panel.Text = to.ToString();
            }
        }

        private void converter_clear_button_Click(object sender, RoutedEventArgs e)
        {
            converter_from_panel.Text = null;
            converter_to_panel.Text = null;
        }

        private void converter_from_panel_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // check if text in panel is a number
            string fromText = converter_from_panel.Text;
            var match = Regex.Match(fromText, NUMBER_REGEX);
            

            // ensure text in panel is a number and not a word
            if (!match.Success)
            {
                converter_from_panel.Text = null;
            }
        }



        private void converter_to_panel_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // check if text in panel is a number
            string fromText = converter_from_panel.Text;
            var match = Regex.Match(fromText, NUMBER_REGEX);


            // ensure text in panel is a number and not a word
            if (!match.Success)
            {
                converter_from_panel.Text = null;
            }
        }

        private void converter_from_insert_button_Click(object sender, EventArgs a)
        {

            // something like (sender as TextBox) to bind one event for these 2 buttons




            // will insert units too in the future
            text_panel.Text += converter_from_panel.Text;
        }
        
        private void converter_to_insert_button_Click(object sender, EventArgs a)
        {

            text_panel.Text += converter_to_panel.Text;
        }

        private void text_formatter_italic_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            text_panel.FontStyle = FontStyles.Italic;
        }

        private void text_formatter_bold_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            text_panel.FontWeight = FontWeights.Bold;

        }

        private void text_formatter_bold_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            text_panel.FontWeight = FontWeights.Normal;
        }

        private void text_formatter_italic_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            text_panel.FontStyle = FontStyles.Normal;
        }

        private void open_file_item_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = file_name.Text;
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                file_name.Text = filename;
            }
        }

        private void exit_file_item_Click(object sender, RoutedEventArgs e)
        {
            if (text_panel.Text != "") // add INotifyPropertyChanged
            {
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage image = MessageBoxImage.Question;
                string text = "Do you want to save changes in a file?";
                string title = "You have unsaved changes";

                MessageBoxResult result = MessageBox.Show(text, title, button, image);
                

                if (result == MessageBoxResult.Yes)
                {
                    save_as_button_Click(this, null);
                }
                else if (result == MessageBoxResult.No)
                {
                    System.Windows.Application.Current.Shutdown();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void calc_button_Click(object? sender, RoutedEventArgs args)
        {
            // adds button content to calculator panel


            //calc_panel.Text += sender.Content;
            Type type = sender.GetType();

            if (type == typeof(System.Windows.Controls.Button))
            {
                PropertyInfo prop = type.GetProperty("Content");
                var cont = prop.GetValue(sender);
                if (cont != null)
                {
                    calc_panel.Text += cont;
                }
            }
        }

        //private void CommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        //{

        //}

        //private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        //{
        //    System.Windows.Application.Current.Shutdown();
        //}

        private void calc_clear_everything_button_Click(object sender, RoutedEventArgs e)
        {
            calc_panel.Text = string.Empty;

            output.Text += $"Calculator cleared\n\n";
            output.ScrollToEnd();
        }

        private void calc_clear_button_Click(object sender, RoutedEventArgs e)
        {
            string exp = calc_panel.Text;

            if (exp.Length != 0)
            {
                StringBuilder sb = new(exp);

                calc_panel.Text = sb.ToString().Substring(0, sb.Length - 1); // too much memory usage
            }
        }

        private void terminal_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string terminalText = terminal.Text;

            //terminal.Text = terminalText;
            //string exp = calc_panel.Text;



            if (terminalText != string.Empty && terminalText != null && terminalText[terminalText.Length - 1] == '\n')
            {

                if (terminalText.Length != 0)
                {
                    // removes newline character to not execute command again when pressing Backspace so newline becomes the last char again
                    // this way it wont trigger command third time

                    StringBuilder sb = new(terminalText);

                    terminal.Text = sb.ToString().Substring(0, sb.Length - 1);
                    terminal.Select(terminal.Text.Length - 1, 0);   // reset the cursor to the end
                }


                // second commit




                string[] args = terminalText.Split(' ');
                

                var res = _terminalService.executeCommand(args);

                output.Text += $"Command exited with code: {res.ExitCode}\n";
                output.Text += $"Command result: {res.Result}\n";
            }

            output.ScrollToEnd();
        }
    }
}
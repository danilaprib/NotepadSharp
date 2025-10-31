using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Text;
using System.Windows;


namespace NotepadSharp.Services
{
    public class TerminalService
    {



        // -------------------- INPUT ----------------


        // returns some output + exit code
        // should be either an async method or run on a new thread
        public (string Result, int ExitCode) executeCommand(string[] args)
        {
            int exitCode = 0;


            // loop through built-in commands
            //if (args[0] == "ls")
            //{

            //} else if (args[0] == "cd")
            //{

            //}

            StringBuilder resBuilder = new StringBuilder();
            resBuilder.AppendLine();
            foreach (string arg in args) { resBuilder.AppendLine(arg); }

            string result = resBuilder.ToString();

            return (result, exitCode);
        }







        // -------------------- OUTPUT ----------------

        public List<string> listContent(string dir)
        {
            if (!Directory.Exists(dir))
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage image = MessageBoxImage.Error;
                string text = "this directory doesnt exist";
                string title = "Unable to find directory";

                var result = MessageBox.Show(text, title, button, image);
                return new List<string>();
            }

            var dirs = Directory.GetDirectories(dir);
            var files = Directory.GetFiles(dir);


            List<string> res = new List<string>();

            res.Add(dir + '\n');

            res.Add($"----Directories: {dirs.Length}\n");
            foreach (var d in dirs) res.Add($"{d}\n");

            res.Add($"----Files: {files.Length}\n");
            foreach (var f in files) res.Add($"{f}\n");

            res.Add("\n");
            return res;
        }


        public string? openFile(string file)
        {
            if (!File.Exists(file))
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage image = MessageBoxImage.Error;
                string text = "this file doesnt exist";
                string title = "Unable to find file";

                var result = MessageBox.Show(text, title, button, image);
                return null;
            }

            string res = File.ReadAllText(file);

            return res;
        }



        public bool createFile(string file)
        {
            bool wasCreated = false;

            if (!File.Exists(file))
            {
                using (FileStream fs = File.Create(file))
                {
                    wasCreated = true;
                }

            }
            else
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage image = MessageBoxImage.Error;
                string text = $"{file} already exists";
                string title = "Unable to create already existing file";

                var result = MessageBox.Show(text, title, button, image);
            }
            return wasCreated;
        }

        public async Task saveFile(string file, string text)
        {
            await File.WriteAllTextAsync(file, text);
        }


        public bool deleteFile(string file)
        {
            bool wasMovedToRecycleBin = false;

            if (System.IO.File.Exists(file))
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                wasMovedToRecycleBin = true;
            }
            else
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage image = MessageBoxImage.Error;
                string messageBoxTitle = "Unable to move to Recycle Bin";
                string messageBoxText = $"Can't move {file} to Recycle Bin";

                MessageBox.Show(messageBoxText, messageBoxTitle, button, image);
            }

            return wasMovedToRecycleBin;
        }

        public bool permaDeleteFile(string file)
        {
            bool wasDeleted = false;

            if (!File.Exists(file))
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage image = MessageBoxImage.Error;
                string text = $"{file} does not exist";
                string title = "Unable to delete non-existent file";

                var result = MessageBox.Show(text, title, button, image);


                return wasDeleted;

            }

            File.Delete(file);
            wasDeleted = true;
            return wasDeleted;
        }
    }
}

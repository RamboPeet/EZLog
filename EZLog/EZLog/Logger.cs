using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZLog
{
    public static class Logger
    {
        #region Private Constants

        private const long DEFAULT_FILE_SIZE = 8388608; // 8 MB
        private const string DEFAULT_DIR_NAME = "Logs";

        #endregion

        #region Private Static Variables

        private static object semaphore = new object();

        #endregion

        #region Public Static Events

        public static event EventHandler<EventArgs> LogErrorEvent;

        #endregion

        #region Private Static Methods

        private static long GetFileSize(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    FileInfo fInfo = new FileInfo(filePath);
                    return fInfo.Length;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        private static string CheckFolderStructure(string rootPath, bool monthNameFolders = false)
        {
            string yearFolder = default(string);
            string dayFolder = default(string);
            string monthFolder = default(string);
            string folderPath = default(string);

            try
            {
                // If statement to check if the root folder exists, else create's the folder.
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                yearFolder = DateTime.Now.Year.ToString();
                dayFolder = DateTime.Now.Day.ToString().PadLeft(2, '0');

                // If statement to check if the month folders should be the name, or number.
                if (monthNameFolders)
                {
                    monthFolder = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
                }
                else
                {
                    monthFolder = DateTime.Now.Month.ToString().PadLeft(2, '0');
                }

                // Sets the default folder path structure. (\MonthNumber\DayNumber)
                folderPath = Path.Combine(rootPath, yearFolder, monthFolder, dayFolder);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                return folderPath;
            }
            catch (Exception ex)
            {
                // TODO:
                throw ex;
            }
        }

        private static void CheckFile(string filePath, LogLevel level, string message = null, Exception ex = null)
        {
            if (File.Exists(filePath))
            {
                if (GetFileSize(filePath) >= DEFAULT_FILE_SIZE)
                {
                    // Gets the file count.
                    int fileCount = Directory.GetFiles(Path.GetDirectoryName(filePath)).Length;

                    // Renames the full log's extension. 
                    File.Move(filePath, Path.Combine(filePath, Path.ChangeExtension(filePath, "log" + fileCount.ToString())));

                    // Create the new empty log file.
                    File.Create(filePath);
                }
            }
            else
            {
                // Create the new empty log file.
                File.Create(filePath);
            }

            // Writes to the log.
            WriteLog(filePath, level, message, ex);
        }

        private static void WriteLog(string filePath, LogLevel level, string message = null, Exception ex = null)
        {
            string entry = default(string);
            string dateTime = default(string);

            try
            {
                dateTime = string.Format("[{0}-{1}-{2} {3}:{4}:{5}]", DateTime.Now.Year, DateTime.Now.Month.ToString().PadLeft(2, '0'),
                    DateTime.Now.Day.ToString().PadLeft(2, '0'), DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                    DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0'));


                if (message != null && ex == null)
                {
                    entry = string.Format("{0} ({1}) - {2}", dateTime, level.ToString(), message);
                }
                else if (message == null && ex != null)
                {
                    // TODO: Test what needs to be written to the log with an Exception
                    entry = string.Format("{0} ({1}) - {2}", dateTime, level.ToString(), ex.StackTrace);
                }
                else
                {
                    // TODO: Test what needs to be written to the log with an Exception
                    entry = string.Format("{0} ({1}) - {2} - {3}", dateTime, level.ToString(), message, ex);
                }

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(entry);
                }
            }
            catch (Exception exc)
            {
                // TODO:
                throw exc;
            }
        }

        #endregion

        #region Public Static Methods

        public static void Log(LogLevel level, string message = null, Exception ex = null, string logFolderPath = null)
        {
            string path = default(string);

            try
            {
                // Locks the following code snippet so that only one thread can work on it at a time.
                lock (semaphore)
                {
                    if (logFolderPath == null)
                    {
                        // TODO: Custom Log settings for month folder names.
                        path = CheckFolderStructure(Path.Combine(Environment.CurrentDirectory, DEFAULT_DIR_NAME), true);
                    }
                    else
                    {
                        // TODO: Custom Log settings for month folder names.
                        path = CheckFolderStructure(logFolderPath, true);
                    }

                    if (path != null)
                    {
                        // Sets the filename, and new path.
                        string fileName = string.Format("{0}-{1}-{2} -- Log.log", DateTime.Now.Year,
                            DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'));
                        path = Path.Combine(path, fileName);

                        // Checks if the file needs to renamed, and then writes to the log file.
                        CheckFile(path, level, message, ex);
                    }
                }
            }
            catch (UnauthorizedAccessException accessEx)
            {
                throw accessEx;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion

        #region Private Static Event Trigger Methods

        private static void LogErrorMethod(EventArgs args)
        {
            if (LogErrorEvent != null)
            {
                LogErrorEvent(null, args);
            }
        }

        #endregion
    }
}

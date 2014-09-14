// 
// PipeWrench - automate the transformation of text using "stackable" text filters
// Copyright (c) 2014  Barry Block 
// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version. 
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE.  See the GNU General Public License for more details. 
// 
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
using System;
using System.IO;

namespace Firefly.PipeWrench
{
   /// <summary>
   /// Implements a logging object.
   /// </summary>
   public class Logger : ILogger
   {
      public string LogPath { get; set; }
         // Folder where log files are kept.
      public string Editor { get; set; }
         // Editor application used to view the log's contents.
      public bool Enabled { get; set; }
         // Enables logging.

      public Logger(string logPath)
      {
         this.LogPath = logPath;
         Enabled = false;
      }

      /// <summary>
      /// Writes the given message to the current log file.
      /// </summary>
      public void Write(string message)
      {
         if (Enabled)
         {
            string thePath = FilePath(DateTime.Now);

            if (!File.Exists(thePath))
            {
               File.WriteAllText(thePath, DateTime.Now.ToString("HH:mm:ss") + " " +
               message + Environment.NewLine);
            }
            else
            {
               File.AppendAllText(thePath, DateTime.Now.ToString("HH:mm:ss") + " " +
               message + Environment.NewLine);
            }
         }
      }

      /// <summary>
      /// Writes text to log.
      /// </summary>
      public void WriteText(string text, string heading, string prefix)
      {
         if (Enabled)
         {
            if ((heading != null) && (heading != string.Empty))
            {
               Write(heading);
            }

            string[] lines = text.Split(new string[] { System.Environment.NewLine },
            StringSplitOptions.None);
            int lineNo = 0;

            foreach (string line in lines)
            {
               lineNo++;
               string tempStr = prefix.Replace("[]", lineNo.ToString().PadLeft(5, ' '));
               Write(tempStr + line);
            }
         }
      }

      /// <summary>
      /// Opens current log file.
      /// </summary>
      public void Open()
      {
         System.Diagnostics.Process.Start(Editor, FilePath(DateTime.Now));
      }

      /// <summary>
      /// Returns the given log's path.
      /// </summary>
      public string FilePath(DateTime logDate)
      {
         if (LogPath != null)
         {
            return LogPath + System.IO.Path.DirectorySeparatorChar +
            logDate.ToString("yyyyMMdd") + ".txt";
         }
         else
         {
            return logDate.ToString("yyyyMMdd") + ".txt";
         }
      }
   }
}

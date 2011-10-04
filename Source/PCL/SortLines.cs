using System;
using System.Diagnostics;
using System.IO;

namespace Firefly.Pyper
{
   /// <summary>
   /// Sorts the input text.
   /// </summary>
   public sealed class SortLines : FilterPlugin
   {
      private void ExecShellCmd(string cmd, string cmdArgs)
      {
         try
         {
            ProcessStartInfo ps = new ProcessStartInfo(cmd, cmdArgs);
            ps.UseShellExecute = false;
            ps.RedirectStandardOutput = true;
            ps.RedirectStandardError = true;
            ps.CreateNoWindow = true;

            // Start the process:

            using (Process p = Process.Start(ps))
            {
               // Create temp file for writing:

               string tempFileName = Path.GetTempFileName();
               StreamWriter sw = new StreamWriter(tempFileName);

               // Write standard output from sort command to it:

               string line;

               while ((line = p.StandardOutput.ReadLine()) != null)
               {
                  sw.WriteLine(line);
               }

               // Close the temp file and replace the host's output text file reference with it:

               sw.Close();
               ((Filter) Host).OutText = tempFileName;

               string errors = p.StandardError.ReadToEnd();

               // Wait for the process to exit (must come AFTER StandardOutput
               // is "empty" to avoid deadlock due to the intermediate kernel
               // pipe being full):

               p.WaitForExit();

               // Output any errors:

               if (errors != string.Empty) throw new Exception(errors);
            }
         }

         catch (Exception ex)
         {
            throw new PyperEngineException("Sort error: " + ex.Message);
         }
      }

      private void SortFile(string filePath, int charPos, bool isReverse)
      {
         // Set the reverse string accordingly:

         string revStr = string.Empty;

         if (isReverse)
         {
            revStr = ((Engine) Eng).SortRevCmd;
         }

         // Sort the file using the system sort utility:

         string command = ((Engine) Eng).SortExec;
         string cmdLine = ((Engine) Eng).SortCmdLine;
         cmdLine = cmdLine.Replace("[CharPos]", charPos.ToString());
         cmdLine = cmdLine.Replace("[Rev]", revStr);
         cmdLine = cmdLine.Replace("[File]", "\"" + filePath + "\"");
         ExecShellCmd(command, cmdLine);
      }

      public override void Execute()
      {
         int charPos = CmdLine.GetIntSwitch("/P", 1);
         bool reverseSort = CmdLine.GetBooleanSwitch("/R");

         CheckIntRange(charPos, 1, int.MaxValue, "Char. position", CmdLine.GetSwitchPos("/P"));

         // Copy the input text to the output text object:

         File.Copy(((Filter) Host).InText, ((Filter) Host).OutText, true);

         // Sort it:

         SortFile(((Filter) Host).OutText, charPos, reverseSort);
      }

      public SortLines(IFilter host) : base(host)
      {
         Template = "/Pn /R";
      }
   }
}

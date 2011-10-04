using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// JoinLines <string>
   /// Joins each line of the input text.
   /// </summary>
   public sealed class JoinLines : FilterPlugin
   {
      public override void Execute()
      {
         int noOfLinesToJoin = 0;

         if (CmdLine.ArgCount > 0)
         {
            noOfLinesToJoin = (int) CmdLine.GetArg(0).Value;
            CheckIntRange(noOfLinesToJoin, 1, int.MaxValue, "No. of lines", CmdLine.GetArg(0).CharPos);
         }

         bool paragraphJoin = CmdLine.GetBooleanSwitch("/P");
         string line;
         string newLine = string.Empty;

         Open();

         try
         {
            if (noOfLinesToJoin > 0)
            {
               // Joining a specified number of lines.

               int count = 1;

               while (!EndOfText)
               {
                  line = ReadLine();

                  if (count <= noOfLinesToJoin)
                  {
                     // Append the current line to the end of newLine:

                     newLine += line;

                     if ((count == noOfLinesToJoin) || EndOfText)
                     {
                        // Add newLine to the output queue:

                        WriteText(newLine);
                        count = 1;
                        newLine = string.Empty;
                     }
                     else
                        count++;
                  }
               }
            }
            else
            {
               if (!paragraphJoin)
               {
                  // Joining all lines, (stripping newlines).

                  while (!EndOfText)
                  {
                     newLine += ReadLine();
                  }

                  Write(newLine); // This fixes special case where joining 0 input lines was resulting in 1 line.
               }
               else
               {
                  // Joining all lines, (Paragraph Join).

                  while (!EndOfText)
                  {
                     do
                     {
                        line = ReadLine();

                        if (line.Trim() != string.Empty)
                        {
                           newLine += line;
                        }
                     }
                     while ((line.Trim() != string.Empty) && !EndOfText);

                     if (line.Trim() == string.Empty) newLine += System.Environment.NewLine;

                     WriteText(newLine);
                     newLine = string.Empty;
                  }
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public JoinLines(IFilter host) : base(host)
      {
         Template = "[n] /P";
      }
   }
}

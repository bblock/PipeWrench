using System;

namespace Firefly.Pyper
{
   public abstract class JustCharsPlugin : FilterPlugin
   {
      public override abstract void Execute();

      /// <summary>
      /// Justifies each line of text.  Note that only leading/trailing spaces
      /// are considered during justification. Tabs are not considered.
      /// </summary>
      protected void Execute(bool justifyingLeft)
      {
         int begPos = 0;
         int endPos = 0;
         bool rangeIsGiven = (CmdLine.ArgCount == 2);

         if (rangeIsGiven)
         {
            begPos = (int) CmdLine.GetArg(0).Value;
            endPos = (int) CmdLine.GetArg(1).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Begin char. position", CmdLine.GetArg(0).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "End char. position", CmdLine.GetArg(1).CharPos);

            if (begPos >= endPos)
            {
               // Oops.

               ThrowException("End char. position must be > begin char. position.", CmdLine.GetArg(1).CharPos);
            }
         }

         Open();

         try
         {
            while (!EndOfText)
            {
               int i;
               int justLen;

               string line = ReadLine();

               if (!rangeIsGiven)
               {
                  // Justifying entire line.

                  if (line.Length > 1)
                  {
                     begPos = 1;
                     endPos = line.Length;

                     if (justifyingLeft)
                     {
                        // Justifying left.

                        i = 0;
                        justLen = line.Length;

                        while ((line[0] == ' ') && (i < justLen))
                        {
                           line = line.Substring(1) + ' ';
                           i++;
                        }
                     }
                     else
                     {
                        // Justifying right.

                        i = 0;
                        justLen = line.Length;

                        while ((line[justLen-1] == ' ') && (i < justLen))
                        {
                           line = ' ' + line.Substring(0, justLen-1);
                           i++;
                        }
                     }
                  }
               }
               else
               {
                  // Justifying just a portion of the line according to the
                  // range supplied.  Pad line to end position with blanks:

                  line = line.PadRight(endPos, ' ');

                  if (justifyingLeft)
                  {
                     // Justifying left.

                     i = 0;
                     justLen = endPos - begPos + 1;

                     while ((line[begPos-1] == ' ') && (i < justLen))
                     {
                        line = line.Substring(0, begPos-1) + line.Substring(begPos, justLen-1) + ' ' +
                        line.Substring(endPos);
                        i++;
                     }
                  }
                  else
                  {
                     // Justifying right.

                     i = 0;
                     justLen = endPos - begPos + 1;

                     while ((line[endPos-1] == ' ') && (i < justLen))
                     {
                        line = line.Substring(0, begPos-1) + ' ' + line.Substring(begPos-1, justLen-1) +
                        line.Substring(endPos);
                        i++;
                     }
                  }
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public JustCharsPlugin(IFilter host) : base(host) {}
   }
}

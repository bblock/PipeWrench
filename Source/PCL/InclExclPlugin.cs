using System;

namespace Firefly.Pyper
{
   public abstract class InclExclPlugin : FilterPlugin
   {
      public override abstract void Execute();

      protected void Execute(bool excludingLines)
      {
         string newSource;
         bool found;

         string matchStr = (string) CmdLine.GetArg(0).Value;
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         bool isRegEx = CmdLine.GetBooleanSwitch("/R");

         if (matchStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetArg(0).CharPos);

         int begPos = 0;
         int endPos = 0;

         bool rangeGiven = (CmdLine.ArgCount > 1);

         if (rangeGiven)
         {
            begPos = (int) CmdLine.GetArg(1).Value;
            endPos = (int) CmdLine.GetArg(2).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Begin char. position", CmdLine.GetArg(1).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "End char. position", CmdLine.GetArg(2).CharPos);
         }

         if (begPos > endPos)
         {
            // Oops.

            ThrowException("End char. position must be >= begin char. position.", CmdLine.GetArg(2).CharPos);
         }

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (rangeGiven)
               {
                  if (line.Length >= endPos)
                  {
                     newSource = line.Substring(begPos-1, endPos-begPos+1);
                     found = StringMatched(matchStr, newSource, ignoringCase, isRegEx);
                  }
                  else
                  {
                     // Range extends past end of line.

                     found = false;
                  }
               }
               else
                  found = StringMatched(matchStr, line, ignoringCase, isRegEx);

               if ((!found && excludingLines) || (found && !excludingLines))
               {
                  WriteText(line);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public InclExclPlugin(IFilter host) : base(host) {}
   }
}

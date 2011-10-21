// 
// Pyper - automate the transformation of text using "stackable" text filters
// Copyright (C) 2011  Barry Block 
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

namespace Firefly.Pyper
{
   /// <summary>
   /// Removes extraneous blanks from the input text.
   /// </summary>
   public sealed class DelExtraBlanks : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string Source = ReadLine();
               int i = 0;

               while (i < Source.Length)
               {
                  while ((i < Source.Length) && (Source[i] != ' '))
                  {
                     i++;
                  }

                  if (i < Source.Length)
                  {
                     int BegPos = i;

                     while ((i < Source.Length) && (Source[i] == ' '))
                     {
                        i++;
                     }

                     int NoOfChars = i - BegPos - 1;
                     if (NoOfChars > 0)
                     {
                        Source = Source.Remove(BegPos, NoOfChars);
                        i = BegPos + 1;
                     }
                  }
               }

               WriteText(Source);
            }
         }

         finally
         {
            Close();
         }
      }

      public DelExtraBlanks(IFilter host) : base(host) {}
   }
}

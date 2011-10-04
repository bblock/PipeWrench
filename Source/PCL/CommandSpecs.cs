using System;
using System.Collections;
using System.Collections.Generic;

namespace Firefly.Pyper
{
   /// <summary>
   /// Implements a list of CommandSpec objects.
   /// </summary>
   public class CommandSpecs : IEnumerable
   {
      private List<CommandSpec> iItems;

      public int Count
      {
         get { return iItems.Count; }
      }

      public CommandSpec this[int i]
      {
         get { return (CommandSpec) iItems[i]; }
         set { iItems[i] = value; }
      }

      public IEnumerator GetEnumerator()
      {
         return iItems.GetEnumerator();
      }

      public CommandSpecs()
      {
         iItems = new List<CommandSpec>();
      }

      public void Add(CommandSpec Spec)
      {
         iItems.Add(Spec);
      }

      public int IndexOf(string Name)
      {
         int i = iItems.Count - 1;

         while (i > -1)
         {
            if (Name.ToUpper() == iItems[i].Name.ToUpper())
            {
               break;
            }
            else
               i--;
         }

         return i;
      }

      public void AddCoreSpec(string typeName, string iconName, string template, string prompt,
      string shortDesc, string assyPath)
      {
         string[] parts = typeName.Split(new char[] {'.'}, StringSplitOptions.None);
         string name = parts[parts.Length-1];
         CommandSpec Spec = new CommandSpec(name, typeName, iconName,
         template, shortDesc, prompt, assyPath, true);
         iItems.Add(Spec);
      }

      public void AddNonCoreSpec(string typeName, string iconName, string template, string prompt,
      string shortDesc, string assyPath)
      {
         string[] parts = typeName.Split(new char[] {'.'}, StringSplitOptions.None);
         string name = parts[parts.Length-1];
         CommandSpec Spec = new CommandSpec(name, typeName, iconName,
         template, shortDesc, prompt, assyPath, false);
         iItems.Add(Spec);
      }

      /// <summary>
      /// Sorts the list.
      /// </summary>
      public void Sort()
      {
         Comparison<CommandSpec> theDelegate =
         delegate(CommandSpec spec1, CommandSpec spec2)
         {
            return spec1.CompareTo(spec2);
         };

         iItems.Sort(theDelegate);
      }
   }
}

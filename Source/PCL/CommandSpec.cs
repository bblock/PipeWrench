using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Implements a Pyper filter/command specification.
   /// </summary>
   public class CommandSpec
   {
      private string name;
      private string typeName;
      private string iconName;
      private string template;
      private string shortDesc;
      private string prompt;
      private string assyPath;
      private bool isCore;

      public string Name
      {
         get { return name; }
      }

      public bool IsCore
      {
         get { return isCore; }
      }

      public string AssyPath
      {
         get { return assyPath; }
      }

      public string TypeName
      {
         get { return typeName; }
      }

      public string IconName
      {
         get { return iconName; }
      }

      public string Template
      {
         get { return template; }
      }

      public string ShortDesc
      {
         get { return shortDesc; }
      }

      public string Prompt
      {
         get { return prompt; }
      }

      public CommandSpec(string name, string typeName,
      string iconName, string template, string shortDesc,
      string prompt, string assyPath, bool isCore)
      {
         this.name = name;
         this.typeName = typeName;
         this.iconName = iconName;
         this.template = template;
         this.shortDesc = shortDesc;
         this.prompt = prompt;
         this.assyPath = assyPath;
         this.isCore = isCore;
      }

      /// <summary>
      /// Used to sort specifications.
      /// </summary>
      public int CompareTo(CommandSpec spec)
      {
         return this.Name.CompareTo(spec.Name);
      }
   }
}

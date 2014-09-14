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

namespace Firefly.PipeWrench
{
   /// <summary>
   /// Implements a PipeWrench filter/command specification.
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

using System;
using Gtk;

namespace Firefly.Pyper
{
   class MainClass
   {
      public static void Main (string[] args)
      {
         Application.Init ();
         MainWindow win = new MainWindow(args);
         win.Show ();
         Application.Run ();
      }
   }
}

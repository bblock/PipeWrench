This pipe translates all "slash-star" comments (/* ... */) found in the input C# source code to the "slash-slash" variety, (// ...).  The first sample source file uses spaces to indent the lines of code whereas the second uses tabs.  Run against either "Sample.cs" or "Sample2.cs".  

In Linux, to run this demo from a terminal, do the following:

   - Open a terminal.
   - Change to this folder.
   - Run these two shell commands:

     cat "Sample.cs" | pcl "call './Translate Slash-Star Comments.pip'"

     cat "Sample2.cs" | pcl "call './Translate Slash-Star Comments.pip'"

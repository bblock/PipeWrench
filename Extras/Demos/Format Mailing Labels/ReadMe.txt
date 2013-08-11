This demo formats a list of name/adress records so that they can be printed as mailing labels.

The first pipe, "One Column.pip" formats the labels for label stock that's one label wide. The second pipe, "Three Columns.pip" formats the labels for stock that's three labels wide.

Both pipes can be run against "Sample Text.txt".

In Linux, to run this demo from a terminal, do the following:

   - Open a terminal.
   - Change to this folder.
   - Run these two shell commands:

     cat "Sample Text.txt" | pcl "call './One-Wide Labels.pip'"

     cat "Sample Text.txt" | pcl "call './Three-Wide Labels.pip'"

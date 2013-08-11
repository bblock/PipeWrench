This pipe creates a word index from a text document. For it to work, each page of text must end with a page number string, "Page n" where "n" is a whole number. Run against "Sample Text.txt". 

In Linux, to run this demo from a terminal, do the following:

   - Open a terminal.
   - Change to this folder.
   - Run this shell command:

     cat "Sample Text.txt" | pcl "call './Create An Index.pip'"

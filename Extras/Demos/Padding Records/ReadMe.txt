This pipe pads each record of a dataset to the same # of fields as the record with the most fields. It does this by appending commas to those records that are shorter. Note that the logic here assumes no field is longer than 12 (15 - marker length) but this can easily be extended. Run against "Sample Text.txt". 

In Linux, to run this demo from a terminal, do the following:

   - Open a terminal.
   - Change to this folder.
   - Run this shell command:

     cat "Sample Text.txt" | pcl "call './Pad Records.pip'"

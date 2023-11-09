 using System;
 using System.IO;
 using System.Diagnostics;

namespace TmpFile.CRUD
{
    class TmpFileCRUD
    {
        public static string CreateTmpFile(string extension)
        {
            string fileName = string.Empty;
            try{
                // Get the full name of the newly created Temporary file. 
                // Note that the GetTempFileName() method actually creates
                // a 0-byte file and returns the name of the created file.
                // *Experienced: if you add the GetTempPath() to other strings,
                // the 0-byte file is NOT created.*
                fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + extension;

                // Creation of file
                var sw = new StreamWriter(fileName);
                sw.Close();

                // Create a FileInfo object to set the file's attributes
                FileInfo fileInfo = new FileInfo(fileName);

                // Set the Attribute property of this file to Temporary. 
                // Although this is not completely necessary, the .NET Framework is able 
                // to optimize the use of Temporary files by keeping them cached in memory.
                // https://docs.microsoft.com/pt-br/dotnet/api/system.io.fileattributes?view=net-6.0
                fileInfo.Attributes = FileAttributes.Temporary;

                return fileName;
            }
            catch (Exception ex){
                return $"{ex}";
            }
        }

        public static string UpdateTmpFile(string tmpFile, string content)
        {
            try
            {
                // Write to the temp file.
                StreamWriter streamWriter = new StreamWriter(tmpFile);
                streamWriter.WriteLine(content);
                streamWriter.Close();

                return "TEMP file updated.";
            }
            catch (Exception ex)
            {
                return "Error writing to TEMP file: " + ex.Message;
            }
        }

        public static string DeleteTmpFile(string tmpFile)
        {
            try
            {
                // Delete the temp file (if it exists)
                var fileInfo = new FileInfo(tmpFile);
                fileInfo.Delete();
                return "TEMP file deleted.";
            }
            catch (Exception ex)
            {
                return "Error deleteing TEMP file: " + ex.Message;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FolderOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Indicate source path: ");
            var sourceDirectoryPath = Console.ReadLine();
            
            if (Directory.Exists(sourceDirectoryPath))
            {
                Console.Write("Indicate destination path: ");
                var targetDirectoryPath = Console.ReadLine();

                var files = Directory.GetFiles(sourceDirectoryPath, "*.txt", SearchOption.TopDirectoryOnly);

                string sourceFilePath, destinationFilePath;
                DateTime fileCreateDate = new DateTime();
                DateTime fileModificationDate = new DateTime();

                foreach (var file in files)
                {
                    sourceFilePath = Path.Combine(sourceDirectoryPath, Path.GetFileName(file));

                    fileCreateDate = File.GetCreationTime(sourceDirectoryPath);
                    fileModificationDate = File.GetLastWriteTime(sourceDirectoryPath);

                    //create destination path
                    destinationFilePath = Path.Combine(targetDirectoryPath, fileCreateDate.Year.ToString(), fileCreateDate.ToString("MM"));

                    if (!Directory.Exists(destinationFilePath))
                    {
                        DirectoryInfo targetDI = Directory.CreateDirectory(destinationFilePath);
                    }

                    destinationFilePath = Path.Combine(destinationFilePath, Path.GetFileName(file));

                    if (!Directory.Exists(destinationFilePath))
                    {

                    }
                    File.Copy(sourceFilePath, destinationFilePath, false);
                }
            }

            Console.WriteLine("Path does not exist");
            return;
        }
    }
}

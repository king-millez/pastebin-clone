using System;
using System.Net;
using System.IO;

namespace pastebin_clone
{
    class Program
    {
        static void Main(string[] args)
        {
            const string url = "https://pastebin.com/raw/"; //Pastebin's raw file directory. Files in this directory are untouched text files which are downloaded by the program.
            bool debugMode = false;
            string outputFile = null;
            string inputURL = null;
            string fullPath = null;
            bool customName = false;
            int pos = Array.IndexOf(args, "-i");

            if (args.Length == 0 || pos <= -1) //Checks to see if the user has entered the "-i" argument in the console.
            {
                Console.WriteLine("Usage:\npastebin-clone.exe [options] <URL>\n\n-i: Put the Paste you'd like to download here.\n\n-o: Output file destination.\n\n-d: Run in debug mode.");
            }

            for (int i = 0; i < args.Length; i++) //Checks each argument entered by the user.
            {
                switch(args[i])
                {
                    case "-d":
                        debugMode = true;
                        break;

                    case "-o":
                        outputFile = args[i + 1];
                        try
                        {
                            fullPath = Path.GetFullPath(outputFile);
                            if(fullPath.Length >= 4)
                            {
                                if(fullPath.Substring(fullPath.Length - 4) == ".txt") //Checks if the output path the user entered ends with ".txt" to determine whether to write to a custom named file.
                                {
                                    customName = true;
                                }
                            }
                        }

                        catch
                        {
                            Console.WriteLine("Please enter a valid path.");
                            return;
                        }
                        break;

                    case "-i":
                        inputURL = args[i + 1];
                        if(!inputURL.ToUpper().Contains("PASTEBIN"))
                        {
                            Console.WriteLine("Enter a valid Pastebin URL.");
                            return;
                        }
                        break;
                }
            }

            if(debugMode == true && outputFile != null)
            {
                Console.WriteLine("Output file: " + outputFile);
            }

            #region Download
            string response = "";
            if(inputURL != null)
            {;
                HttpWebRequest get = (HttpWebRequest)WebRequest.Create(url + inputURL.Substring(inputURL.Length - 8)); //Creates a get request targeting "pastebin.com/raw/<paste id>"
                get.Method = "GET";

                WebResponse rp = get.GetResponse();
                using (Stream dataStream = rp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    response = reader.ReadToEnd();
                }
            }

            if(inputURL != null)
            {
                if (outputFile == null)
                {
                    File.WriteAllText(inputURL.Substring(inputURL.Length - 8) + ".txt", response);
                    Console.WriteLine("Output written to " + inputURL.Substring(inputURL.Length - 8) + ".txt");
                }
                else
                {
                    if (customName == true)
                    {
                        File.WriteAllText(fullPath, response);
                        Console.WriteLine("Output written to " + fullPath);
                    }
                    else
                    {
                        if (fullPath.Substring(fullPath.Length - 1) != "/" || fullPath.Substring(fullPath.Length - 1) != "\\")
                        {
                            fullPath = fullPath + "/";
                        }
                        File.WriteAllText(fullPath + inputURL.Substring(inputURL.Length - 8) + ".txt", response);
                        Console.WriteLine("Output written to " + fullPath + inputURL.Substring(inputURL.Length - 8) + ".txt");
                    }
                }
            }
            #endregion
        }
    }
}

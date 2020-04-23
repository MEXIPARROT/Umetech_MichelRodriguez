//Michel Rodriguez
//michel.rdrgz@gmail.com
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using System.Globalization;
using System.Net.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;
using System.IO;
using System.Runtime.CompilerServices;
using ConsoleLib;//

    class Program
    {
        private const string clientId = "fd5bcb47-6ac9-490d-a44a-2210c556f551";
        private const string aadInstance = "https://login.microsoftonline.com/{0}";
        private const string tenant = "a2ef1a59-e845-4e49-8b0b-920c3485dca2";
        private const string resource = "https://graph.windows.net";
        private const string appKey = "-94A]@vxOu:2gu1RPU?]XZpBVc1yMyrs";
        private const string userID = "03c3d376-9ed3-48f1-b75d-fda94e40b3b3";

        static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        private static HttpClient httpClient = new HttpClient();
        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var OneDriveClient = new OneDriveClient(clientId, tenant, appKey);

            var Root = await OneDriveClient.GetRootID(); //Recieves root, then use Root.Name to pass as parameters to other functions
            await OneDriveClient.ListDriveItems(userID, Root.Name); //Lists Number of DriveItems in Onedrive and displays name and ID of each one
            await OneDriveClient.Rename(userID, "01YZM7SMS4YDYZL3PMYVBZHAW62L5JXYKY", "newNAMEmate 2"); //second parameter is a DriveItem ID, change the 3rd parameter string to change specific file's name
            await OneDriveClient.CreateFolder(userID, Root.Name, "tempMichel"); 
            await OneDriveClient.Delete(userID, "01YZM7SMS6VB5K72VG3RFLS3G34HFGBNUE");//"01YZM7SMTICJTXLJWD3RE244SEPKHMZLJ3"); //2nd parameter is ID of a driveItem, the two strings currently commented out don't work, 
            //ID string currently in DELETE will delete if program is to be ran.
            
            string FilePath = "";
            string path = FilePath;//"D:\\LessThan4MB.txt";//D:\\MoreThan5MB.txt"; //Actual File Location in your hard drive //This could be in main or this function but probably best in main            

            byte[] data = System.IO.File.ReadAllBytes(path);  //Stores all data into byte array by name of "data" then "PUT to the root folder

            Stream stream = new MemoryStream(data);

            await OneDriveClient.UploadLess4mb(userID, Root.Name, stream); //uploads file in my physical PC, so edit strings in function so it may work!
            //await OneDriveClient.UploadMore5mb(userID, Root.Name); //Couldn't figure out probably involves byte arrays again and I'm barely familiar with those, trying though.
            Console.ReadLine(); //program doesn't close as soon as it's complete.
        }
    }

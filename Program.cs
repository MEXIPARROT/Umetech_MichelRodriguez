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

//pass for Michel_temp user: Kuza9677 //I don't remember what this was for, leaving in case needed.

namespace ConsoleApp1
{
    class OneDriveClient
    {
        public const string userPrincipalName = "michel.rdrgz@gmail.com";
        public GraphServiceClient _client { get; set; }
        public OneDriveClient(string appClientID, string tenantID, string clientSecret)
        {
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(appClientID)
                .WithTenantId(tenantID)
                .WithClientSecret(clientSecret)
                .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            _client = new GraphServiceClient(authProvider);
        }
        

        public async Task<string> CreateFolder(string userID, string folderID, string FolderName) //Function to create folder, requires userID from Azure, "root", and name of this new folder
        {
            try
            {
                var driveItem = new DriveItem //create a driveItem with these attributes
                {
                    Name = FolderName,
                    Folder = new Microsoft.Graph.Folder
                    {
                    },
                    AdditionalData = new Dictionary<string, object>()
                {
                    {"@microsoft.graph.conflictBehavior","rename"}
                }
                };

                var added = await _client.Users[userID].Drive.Items[folderID].Children //users[] is azure user! not user in onedrive! //This API call will then Add the DriveItem to be created on OneDrive.
                    .Request()
                    .AddAsync(driveItem);
                Console.WriteLine("CreateFolder function says... adding: "); //check values of what was submitted
                Console.WriteLine(added.Name);
                Console.WriteLine(added.Id);

                return added.Id; //return string of ID of DriveItem uploaded
            }
            catch (Exception ex) //If error catch and throw error this way instead
            {
                Console.WriteLine(ex.ToString());
                return "";
            }

        }
        public async Task<DriveItem> GetRootID()//returns DriveItem object (not string), so I may access OneDrive more easily.
        {
            Console.WriteLine("...GetRootID...");
            var root = await _client.Drive.Root
                .Request()
                .GetAsync();
            Console.WriteLine("Finished GetRootID()");
            Console.WriteLine(root); //Pass root like this because I can then "root.Id" and "root.Name" in Main
            Console.WriteLine(root.Name);
            Console.WriteLine(root.Id);
            Console.WriteLine("\n");
            return root;
        }

        public async Task<IDriveItemChildrenCollectionPage> ListDriveItems(string userID, string FolderID) //Function to list all items in OneDrive
        {
            var items = await _client.Users[userID].Drive.Items[FolderID].Children
                .Request()
                .GetAsync();
            Console.WriteLine("items.Count"); //check how many will be printed (how many items in OneDrive)
            foreach (var item in items) //neatly print all DriveItems with Name and IDs
            {
                Console.WriteLine(item.Name);
                Console.WriteLine(item.Id);
                Console.WriteLine("------------------");
            }
            return items;
            
        }

        public async Task<string> Rename(string userID, string DriveItemID, string newname) //Function to rename a specific file/folder
        {
            var driveItem = new DriveItem //create DriveItem with only element is name
            {
                Name = newname
            };

            await _client.Users[userID].Drive.Items[DriveItemID] //API call to Update file/folder in OneDrive
                .Request()
                .UpdateAsync(driveItem);
            Console.WriteLine("Rename is done check onedrive"); //could print ListDriveItems
            return "done"; //may be empty
        }

        public async Task<string> Delete(string userID, string DriveItemID) //Function to delete a specific file/folder in OneDrive
        {
            await _client.Users[userID].Drive.Items[DriveItemID] //API call to Delete specific DriveItem in OneDrive
                .Request()
                .DeleteAsync();
            Console.WriteLine("Delete function says... Deleted DriveItem with the following ID: ");
            Console.WriteLine(DriveItemID);

            return "done";
        }

        public async Task<string> UploadLess4mb(string userID, string FolderID) //expect DriveItem?
        {
            string path = "D:\\LessThan4MB.txt";//D:\\MoreThan5MB.txt"; //Actual File Location in your hard drive //This could be in main or this function but probably best in main
            byte[] data = System.IO.File.ReadAllBytes(path);  //Stores all data into byte array by name of "data" then "PUT to the root folder

            Stream stream = new MemoryStream(data);

            var item = await _client.Users[userID].Drive.Items[FolderID] //api call to upload a file (limited to max 4MB files, throws error if too big)
                        .ItemWithPath("LessThan4MB.txt")
                        .Content
                        .Request()
                        .PutAsync<DriveItem>(stream);
            return "done";
        }
        //ALL WORK BELOW ARE ATTEMPTS TO UPLOAD MORE THAN 5MB


        //public async Task<string> UploadMore5mb(string userID, string FolderID)
        //{
        //    var maxChunkSize = 320 * 1024;// 320 KB
        //    Console.WriteLine("before");
        //    string path = "D:\\MoreThan5MB.txt";
        //    var content = new MemoryStream();
        //    //byte[] data = File

        //    var temp = await _client.Users[userID].Drive.Items[FolderID]
        //        .CreateUploadSession()
        //        .Request()
        //       .PostAsync();
        //    //var temp2 =             
        //    var webReq = (HttpWebRequest)WebRequest.Create(temp.UploadUrl);
        //    WebResponse response = await webReq.GetResponseAsync();
        //    Stream responsStream = response.GetResponseStream();
        //    await responsStream.WriteAsync(content);
        //    return content.ToArray();

            //await temp.UploadUrl
                
        //   Console.WriteLine("after");
            //            Console.WriteLine("after");
         //   Console.WriteLine(temp.UploadUrl);
         //   Console.WriteLine(temp.ExpirationDateTime);
         //   Console.WriteLine(temp.NextExpectedRanges);

         //   return "done";
        //}
    
    
            //var file = path.ToArray();
            //Stream file = @"D:\LessThan4BM.txt";
            //MemoryStream ms = new MemoryStream();// "LessThan4MB.txt".ToArray());//file);
            //FileStream file = new FileStream("D:\\LessThan4MB.txt", FileMode.Open, FileAccess.Read);
            //byte[] bytes = new byte[file.Length];
            //file.Read(bytes, 0, (int)file.Length);
            //ms.Write(bytes, 0, (int)file.Length);

            //Console.WriteLine("Before");
            ///Console.WriteLine(file);
            
            //var item = await _client.Users[userID].Drive.Items[FolderID]//"01YZM7SMVOQ7YVNBXPZFFKNQAU5OB3XA3K"].Content
            //        .ItemWithPath("D:\\LessThan4MB.txt")
            //        .CreateUploadSession()
            //        .Request()
            //        .PostAsync();
            //Console.WriteLine("done printing");
            /*
            await _client.Users[userID].Drive.Items[FolderID]//.Children
                .CreateUploadSession()
                .Request()
                //.AddAsync();
                .PutAsync();
                */

            //return "done";// item;
            //

        

    }
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
            Console.WriteLine("Main says: Rootname is:");
            Console.WriteLine(Root.Name);
            Console.WriteLine(Root.Id);
            Console.WriteLine("\n");
            await OneDriveClient.ListDriveItems(userID, Root.Name); //Lists Number of DriveItems in Onedrive and displays name and ID of each one
            Console.WriteLine("Main says: After ListDrive");
            Console.WriteLine("\n");
            await OneDriveClient.Rename(userID, "01YZM7SMS4YDYZL3PMYVBZHAW62L5JXYKY", "newNAMEmate 2"); //second parameter is a DriveItem ID, change the 3rd parameter string to change specific file's name
            await OneDriveClient.CreateFolder(userID, Root.Name, "tempMichel"); 
            await OneDriveClient.Delete(userID, "01YZM7SMS6VB5K72VG3RFLS3G34HFGBNUE");//"01YZM7SMTICJTXLJWD3RE244SEPKHMZLJ3"); //2nd parameter is ID of a driveItem, the two strings currently commented out don't work, 
            //ID string currently in DELETE will delete if program is to be ran.

            await OneDriveClient.UploadLess4mb(userID, Root.Name); //uploads file in my physical PC, so edit strings in function so it may work!
            //await OneDriveClient.UploadMore5mb(userID, Root.Name); //Couldn't figure out probably involves byte arrays again and I'm barely familiar with those, trying though.
            Console.ReadLine(); //program doesn't close as soon as it's complete.
        }
    }
}
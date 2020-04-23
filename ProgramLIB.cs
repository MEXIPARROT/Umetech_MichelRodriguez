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

//pass for Michel_temp user: Kuza9677 //I don't remember what this was for, leaving in case needed.

namespace ConsoleLib
{
    public class OneDriveClient
    {
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
            var root = await _client.Drive.Root
                .Request()
                .GetAsync();
            return root;
        }

        public async Task<IDriveItemChildrenCollectionPage> ListDriveItems(string userID, string FolderID) //Function to list all items in OneDrive
        {
            var items = await _client.Users[userID].Drive.Items[FolderID].Children
                .Request()
                .GetAsync();
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
            return "done"; //may be empty
        }

        public async Task<string> Delete(string userID, string DriveItemID) //Function to delete a specific file/folder in OneDrive
        {
            await _client.Users[userID].Drive.Items[DriveItemID] //API call to Delete specific DriveItem in OneDrive
                .Request()
                .DeleteAsync();
            return "done";
        }

        public async Task<string> UploadLess4mb(string userID, string FolderID, Stream stream) //expect DriveItem?
        {
            var item = await _client.Users[userID].Drive.Items[FolderID] //api call to upload a file (limited to max 4MB files, throws error if too big)
                        .ItemWithPath("LessThan4MB.txt")
                        .Content
                        .Request()
                        .PutAsync<DriveItem>(stream);
            return "done";
        }
    }
}
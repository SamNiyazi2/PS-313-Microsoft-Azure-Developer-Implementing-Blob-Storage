using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WiredBrainCoffee.Storage
{
    public class CoffeeVideoStorage : ICoffeeVideoStorage
    {
        private readonly string blobStorageConnectionString;
        private readonly string blobContainerName = "coffeevideos";

        private readonly string _metadataKeyTitle = "title";
        private readonly string _metadataKeyDescription = "description";


        public CoffeeVideoStorage(string blobStorageConnectionString)
        {
            this.blobStorageConnectionString = blobStorageConnectionString;
        }

        // 05/19/2021 05:34 am - SSN - [20210519-0529] - [002] - M03-05 - Show the blob URI of the uploaded blob
        // public async Task UploadVideoAsync(byte[] videoByteArray, string blobName)
        public async Task<CloudBlockBlob> UploadVideoAsync(byte[] videoByteArray, string blobName, string blobTitle, string blobDescription)
        {
            // 05/19/2021 12:06 am - SSN - [20210518-2359] - [001] - M03-03 - Upload a blob to a container 

            CloudBlockBlob cloudBlockBlob = await getCloudBlockBlobAsync(blobName);

            SetMetadata(cloudBlockBlob, _metadataKeyTitle, blobTitle);
            SetMetadata(cloudBlockBlob, _metadataKeyDescription, blobDescription);

            cloudBlockBlob.Properties.ContentType = "video/mp4";

            await cloudBlockBlob.UploadFromByteArrayAsync(videoByteArray, 0, videoByteArray.Length);

            return cloudBlockBlob;

        }


        // public async Task OverwriteVideoAsync(CloudBlockBlob cloudBlockBlob, byte[] videooByteArray )
        public async Task OverwriteVideoAsync(CloudBlockBlob cloudBlockBlob, byte[] videooByteArray)
        {
            AccessCondition accessCondition = new AccessCondition
            {
                IfMatchETag = cloudBlockBlob.Properties.ETag
            };
            BlobRequestOptions blobRequestOptions = null;
            OperationContext operationContext = null;
            await cloudBlockBlob.UploadFromByteArrayAsync(videooByteArray, 0, videooByteArray.Length, accessCondition, blobRequestOptions, operationContext);
        }



        public async Task<bool> CheckIfBlobExistsAsync(string blobName)
        {
            // 05/19/2021 05:57 am - SSN - [20210519-0548] - [001] - M03-06 - Check if a blob exists

            CloudBlockBlob cloudBlockBlob = await getCloudBlockBlobAsync(blobName);

            return await cloudBlockBlob.ExistsAsync();
        }


        public async Task<IEnumerable<CloudBlockBlob>> ListVideoBlobsAsync(string prefix = null)
        {
            var cloudBlockBlobs = new List<CloudBlockBlob>();
            var cloudBlobContainer = await getCloudBlobContainerAsync();

            BlobContinuationToken token = null;
            bool useFlagBlobListing = true;
            int maxResults = 2;
            BlobRequestOptions blobRequestOptions = null;
            OperationContext operationContext = null;


            do
            {
                // Option 1
                //var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(null);

                // Option 2
                //var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(null, useFlagBlobListing, BlobListingDetails.None, 
                //                                maxResults, token, blobRequestOptions, operationContext);

                // Option 3
                // var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix, token);

                // Option 4
                var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(null, useFlagBlobListing, BlobListingDetails.Metadata,
                                                maxResults, token, blobRequestOptions, operationContext);

                cloudBlockBlobs.AddRange(blobResultSegment.Results.OfType<CloudBlockBlob>());
                token = blobResultSegment.ContinuationToken;

            } while (token != null);


            return cloudBlockBlobs;

        }


        private async Task<CloudBlockBlob> getCloudBlockBlobAsync(string blobName)
        {
            var cloudBlobContainer = await getCloudBlobContainerAsync();
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            return cloudBlockBlob;
        }

        ///////////////////////////////////    GetCoffeeVideosContainerAsync

        private async Task<CloudBlobContainer> getCloudBlobContainerAsync()
        {

            if (string.IsNullOrWhiteSpace(blobStorageConnectionString))
            {
                throw new Exception("Calling with null or empty storage connection string [20210519-0558]");
            }

            var cloudStorageAccount_2 = CloudStorageAccount.Parse(blobStorageConnectionString);

            var cloudBlobClient = cloudStorageAccount_2.CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

            // For no public access
            // await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            //await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, null, null);
            // Equivelent to:
            await cloudBlobContainer.CreateIfNotExistsAsync();

            return cloudBlobContainer;

        }


        public async Task DownloadVideoAsync(CloudBlockBlob cloudBlockBlob, Stream targetStream)
        {
            await cloudBlockBlob.DownloadToStreamAsync(targetStream);
        }

        public async Task DeleteVideoAsync(CloudBlockBlob cloudBlockBlob)
        {
            AccessCondition accessCondition = new AccessCondition
            {
                IfMatchETag = cloudBlockBlob.Properties.ETag
            };

            DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None;
            BlobRequestOptions blobRequestOptions = null;
            OperationContext operationContext = null; ;

            await cloudBlockBlob.DeleteAsync(deleteSnapshotsOption, accessCondition, blobRequestOptions, operationContext);
        }


        // 05/20/2021 06:14 am - SSN - [20210520-0607] - [001] - M05-06 - Configure soft delete
        // Not tested. For the record
        public async Task UndeleteVideoAsync(string prefix)
        {

            BlobContinuationToken token = null;

            var deletedCloudBlockBlobs = new List<CloudBlockBlob>();

            var cloudBlobContainer = await getCloudBlobContainerAsync();


            do
            {
                var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix, true, BlobListingDetails.Deleted, null, token, null, null);

                deletedCloudBlockBlobs.AddRange(blobResultSegment.Results.OfType<CloudBlockBlob>().Where(c => c.IsDeleted));

                token = blobResultSegment.ContinuationToken;

            } while (token != null);


            foreach (var cloudBlockBlob in deletedCloudBlockBlobs)
            {
                await cloudBlockBlob.UndeleteAsync();
            }


        }


        public (string title, string description) GetBlobMetadata(CloudBlockBlob cloudBlockBlob)
        {
            return (cloudBlockBlob.Metadata.ContainsKey(_metadataKeyTitle)
                     ? cloudBlockBlob.Metadata[_metadataKeyTitle]
                     : ""
                  , cloudBlockBlob.Metadata.ContainsKey(_metadataKeyDescription)
                     ? cloudBlockBlob.Metadata[_metadataKeyDescription]
                     : "");
        }



        private static void SetMetadata(CloudBlockBlob cloudBlockBlob, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (cloudBlockBlob.Metadata.ContainsKey(key))
                {
                    cloudBlockBlob.Metadata.Remove(key);
                }
            }
            else
            {
                cloudBlockBlob.Metadata[key] = value;
            }
        }


        public async Task UpdateMetadataAsync(CloudBlockBlob cloudBlockBlob, string title, string description)
        {
            SetMetadata(cloudBlockBlob, _metadataKeyTitle, title);
            SetMetadata(cloudBlockBlob, _metadataKeyDescription, description);

            AccessCondition accessCondition = new AccessCondition
            {
                IfMatchETag = cloudBlockBlob.Properties.ETag
            };

            await cloudBlockBlob.SetMetadataAsync(accessCondition, null, null);
        }

        public async Task ReloadMetadataAsync(CloudBlockBlob cloudBlockBlob)
        {
            await cloudBlockBlob.FetchAttributesAsync();
        }


        private static async Task setBlobPermission(CloudBlobContainer cloudBlobContainer, BlobContainerPublicAccessType accessType)
        {
            var blobContainerPermission = new BlobContainerPermissions
            {
                PublicAccess = accessType
            };
            await cloudBlobContainer.SetPermissionsAsync(blobContainerPermission);

        }

        public string GetBlobUriWithSasToken(CloudBlockBlob cloudBlockBlob)
        {
            var sharedAccessBlobPolicy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(2)
            };

            var sasToken = cloudBlockBlob.GetSharedAccessSignature(sharedAccessBlobPolicy);

            return cloudBlockBlob.Uri + sasToken;
        }







        public async Task<string> AcquireOneMinuteLeaseAsync(CloudBlockBlob cloudBlockBlob)
        {
            var accessCondition = new AccessCondition
            {
                IfMatchETag = cloudBlockBlob.Properties.ETag
            };

            return await cloudBlockBlob.AcquireLeaseAsync(TimeSpan.FromMinutes(1), null, accessCondition, null, null);
        }

        public async Task RenewLeaseAsync(CloudBlockBlob cloudBlockBlob, string leaseId)
        {
            var accessCondition = new AccessCondition
            {
                LeaseId = leaseId
            };

            await cloudBlockBlob.RenewLeaseAsync(accessCondition);
        }

        public async Task ReleaseLeaseAsync(CloudBlockBlob cloudBlockBlob, string leaseId)
        {
            var accessCondition = new AccessCondition
            {
                LeaseId = leaseId
            };

            await cloudBlockBlob.ReleaseLeaseAsync(accessCondition);
        }

        public async Task<string> LoadLeaseInfoAsync(CloudBlockBlob cloudBlockBlob)
        {
            await cloudBlockBlob.FetchAttributesAsync();

            return $"Lease state: {cloudBlockBlob.Properties.LeaseState}\n" +
              $"Lease status: {cloudBlockBlob.Properties.LeaseStatus}\n" +
              $"Lease duration: {cloudBlockBlob.Properties.LeaseDuration}";
        }




    }
}

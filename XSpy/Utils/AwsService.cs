using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace XSpy.Utils
{
    public class AwsService
    {
        private IConfiguration Configuration;

        public AwsService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty);
            sbText.Replace(" ", String.Empty);
            var cleanString = sbText.ToString();

            return cleanString.Substring(cleanString.IndexOf(',') + 1);
        }

        private byte[] GetImageBytes(string base64)
        {
            return Convert.FromBase64String(FixBase64ForImage(base64));
        }

        private async Task<string> ProcessUpload(string filePath, string path, string fileNameInS3)
        {
            var chain = new CredentialProfileStoreChain();
            if (!chain.TryGetAWSCredentials("shared_profile", out var awsCredentials))
                return null;

            using (var client = new AmazonS3Client(awsCredentials, RegionEndpoint.USEast2))
            {
                using (var utility = new TransferUtility(client))
                {
                    var amazonSettings = Configuration.GetSection("AmazonSettings");

                    var request = new TransferUtilityUploadRequest
                    {
                        BucketName = amazonSettings["BucketName"] + path,
                        Key = fileNameInS3,
                        FilePath = filePath,
                        ContentType = "image/png"
                    };


                    await utility.UploadAsync(request);

                    await client.PutACLAsync(new PutACLRequest
                    {
                        BucketName = amazonSettings["BucketName"] + path,
                        Key = fileNameInS3,
                        CannedACL = S3CannedACL.PublicRead
                    }).ConfigureAwait(false);


                    return amazonSettings["BucketUrl"] + path + "/" + fileNameInS3;
                }
            }
        }

        public async Task<string> TryUpload(IFormFile formFile, string path,
            string fileNameInS3)
        {
            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return await ProcessUpload(filePath, path, fileNameInS3);
        }

        public async Task<string> TryUpload(string base64File, string path,
            string fileNameInS3)
        {
            var localFilePath = Path.GetTempFileName();

            await File.WriteAllBytesAsync(localFilePath, GetImageBytes(base64File));

            return await ProcessUpload(localFilePath, path, fileNameInS3);
        }

        public async Task<string> TryUpload(byte[] fileBytes, string path,
            string fileNameInS3)
        {
            var localFilePath = Path.GetTempFileName();

            await File.WriteAllBytesAsync(localFilePath, fileBytes);

            return await ProcessUpload(localFilePath, path, fileNameInS3);
        }
        
        public async Task<string> UploadFile(IFormFile file, string path,
            string fileNameInS3)
        {
            var localFilePath = Path.GetTempFileName();

            await using (var stream = new FileStream(localFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var chain = new CredentialProfileStoreChain();
            if (!chain.TryGetAWSCredentials("shared_profile", out var awsCredentials)) return null;
            using (var client = new AmazonS3Client(awsCredentials, RegionEndpoint.USEast1))
            {
                using (var utility = new TransferUtility(client))
                {
                    var amazonSettings = Configuration.GetSection("AmazonSettings");
                    var request = new TransferUtilityUploadRequest
                    {
                        BucketName = amazonSettings["BucketName"] + path,
                        Key = fileNameInS3,
                        FilePath = localFilePath,
                        ContentType = file.ContentType,
                    };

                    await utility.UploadAsync(request);

                    await client.PutACLAsync(new PutACLRequest
                    {
                        BucketName = amazonSettings["BucketName"] + path,
                        Key = fileNameInS3,
                        CannedACL = S3CannedACL.PublicRead
                    }).ConfigureAwait(false);

                    return amazonSettings["BucketUrl"] + path + "/" + fileNameInS3;
                }
            }
        }
    }

}
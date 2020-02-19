using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class S3Handler : MonoBehaviour
{
    public string IdentityPoolId = "";
    string CognitoIdentityRegion = RegionEndpoint.EUWest2.SystemName;
    private RegionEndpoint _CognitoIdentityRegion
    {
        get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
    }
    string S3Region = RegionEndpoint.EUWest2.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }
    string S3BucketName = null;

    public Action<float> OnDownloadProgressUpdate;
    public Action OnDownloadVideoComplete;
    public Action OnDownloadFailed;
    public Action OnReadyForServerCalls;

    public bool Ready { get; private set; }
    public bool PopulateComplete { get; private set; }

    ServerFileData versionFileData;
    public Dictionary<string, ServerFileData> thumbnailData = new Dictionary<string, ServerFileData>();

    void Start()
    {
        if (Application.isEditor)
        {
            print(Application.persistentDataPath);
        }

#if STAGING
        S3BucketName = "dynamicvideoappstaging";
#else
        S3BucketName = "holomev3";
#endif
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        Ready = true;
        OnReadyForServerCalls?.Invoke();
        Amazon.Runtime.Internal.UnityMainThreadDispatcher.DownloadProgress += x => OnDownloadProgressUpdate?.Invoke(x);
    }

    #region private members

    private IAmazonS3 _s3Client;
    private AWSCredentials _credentials;

    private AWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
            return _credentials;
        }
    }

    private IAmazonS3 Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(Credentials, _S3Region);
            }
            //test comment
            return _s3Client;
        }
    }

    #endregion

    public void PopulateData(Action<Dictionary<string, ServerFileData>> OnVideoDataReceived)
    {
        var request = new ListObjectsRequest() { BucketName = S3BucketName };

        try
        {
            Client.ListObjectsAsync(request, (responseObject) =>
            {
                if (responseObject.Exception == null)
                {
                    //PopulateVideoDictionary(responseObject.Response.S3Objects, OnVideoDataReceived);

                    Dictionary<string, ServerFileData> videoDataList = new Dictionary<string, ServerFileData>();

                    List<S3Object> s3Objects = responseObject.Response.S3Objects;

                    //print("Count = " + responseObject.Response.S3Objects.Count);
                    for (int i = 0; i < s3Objects.Count; i++)
                    {
                        var o = s3Objects[i];
                        try
                        {
                            if (o.Key == HelperFunctions.versionFile)//Needs to be changed so this class takes a generic type list to add data too if 
                            {
                                versionFileData = new ServerFileData(o.Key, o.ETag, o.LastModified);
                                continue;
                            }

                            if (HelperFunctions.IsVideoThumbnailData(o.Key) && !thumbnailData.ContainsKey(GetKey(o.ETag)))
                            {
                                thumbnailData.Add(o.Key, new ServerFileData(o.Key, o.ETag, o.LastModified));
                                continue;
                            }

                            if (videoDataList.ContainsKey(GetKey(o.ETag)))
                            {
                                Debug.LogError($"Dictionary video = {videoDataList[GetKey(o.ETag)].FileName} attempted to add {o.Key}");
                            }
                            else
                            {
                                videoDataList.Add(GetKey(o.ETag), new ServerFileData(o.Key, o.ETag, o.LastModified));
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("ERROR OCCURRED - " + "Filename = " + o.Key + " Exception = " + e.ToString());
                        }
                        //print("i = " + i);
                    }

                    PopulateComplete = true;
                    OnVideoDataReceived?.Invoke(videoDataList);
                }
                else
                {
                    Debug.LogError("Got Exception \n" + responseObject.Exception);
                    //GetOfflineFiles(OnReceived, videoDataList);
                }
            });

        }
        catch (Exception e)
        {
            print("ERROR " + e.Message);
            //GetOfflineFiles(OnReceived, videoDataList);
        }
    }

    private string GetKey(string eTag)
    {
        return eTag.Substring(1, 4);
    }

    //public bool S3Exists(string fileKey)
    //{
    //    Amazon.Util.ProfileManager.RegisterProfile(“s3”, “XXX”,”XXXX”);
    //    AWSCredentials credentials = new StoredProfileAWSCredentials(“s3”);
    //    IAmazonS3 s3Client = new AmazonS3Client(credentials, RegionEndpoint.EUWest1);
    //    var s3FileInfo = new Amazon.S3.IO.S3FileInfo(s3Client, “XXXX”, fileKey);
    //    return s3FileInfo.Exists;
    //}

    //private static void GetOfflineFiles(Action<Dictionary<string, VideoFileData>> OnReceived, Dictionary<string, VideoFileData> videoDataList)
    //{
    //    var downloadedFiles = Directory.GetFiles(Application.persistentDataPath + "/");

    //    foreach (string videoName in downloadedFiles)
    //    {
    //        videoDataList.Add(new VideoFileData(Path.GetFileName(videoName),  , File.GetCreationTime(videoName)));
    //    }
    //    OnReceived?.Invoke(videoDataList);
    //}

    /// <summary>
    /// This function will delete local files which have been removed from the server to save device storage space
    /// </summary>
    /// <param name="videoDataList"></param>
    public void DeleteNonExistantFiles(List<ServerFileData> videoDataList)
    {
        var downloadedFiles = Directory.GetFiles(HelperFunctions.PersistentDir());

        foreach (string videoName in downloadedFiles)
        {
            bool videoFound = false;
            foreach (ServerFileData data in videoDataList)
            {
                if (data.FileName == Path.GetFileName(videoName))
                {
                    videoFound = true;
                    break;
                }
            }

            if (!videoFound)
            {
                File.Delete(videoName);
            }
        }
    }

    public bool IsOutOfDate(ServerFileData serverFileData)
    {
        if (HelperFunctions.DoesFileExist(serverFileData.FileName))
        {
            DateTime localFileCreationTime = File.GetCreationTime(HelperFunctions.PersistentDir() + serverFileData.FileName);

            if (DateTime.Compare(localFileCreationTime, serverFileData.LastModified) < 0)
            {
                return true;
            }
        }
        return false;
    }

    public void DownloadVersionText(Action onComplete)
    {
        if (!Ready)
        {
            Debug.LogError("Tried downloading version before ready");
            return;
        }

        if (HelperFunctions.DoesFileExist(HelperFunctions.versionFile))
        {
            if (!IsOutOfDate(versionFileData))
            {
                onComplete?.Invoke();
                return;
            }
        }

        Client.GetObjectAsync(S3BucketName, HelperFunctions.versionFile, (responseObj) =>
        {
            if (responseObj.Exception != null)
            {
                Debug.LogError(responseObj.Exception.Message);
                OnDownloadFailed?.Invoke();
            }
            else
            {
                var response = responseObj.Response;
                if (response.ResponseStream != null)
                {
                    try
                    {
                        versionFileData = new ServerFileData(response.Key, response.ETag, response.LastModified);

                        if (HelperFunctions.DoesFileExist(HelperFunctions.versionFile))
                        {
                            File.Delete(HelperFunctions.PersistentDir() + (HelperFunctions.versionFile));
                        }

                        using (var fs = System.IO.File.Create(HelperFunctions.PersistentDir() + (HelperFunctions.versionFile)))
                        {
                            byte[] buffer = new byte[81920];
                            int count;
                            while ((count = response.ResponseStream.Read(buffer, 0, buffer.Length)) != 0)
                                fs.Write(buffer, 0, count);
                            fs.Flush();
                        }

                        File.SetCreationTime(HelperFunctions.PersistentDir() + (HelperFunctions.versionFile), DateTime.Now);
                        onComplete?.Invoke();
                    }
                    catch (Exception e)
                    {
                        print(e.Message);
                    }
                }
                else
                {
                    Debug.LogError("Response Stream was null");
                }
            }
        });
    }

    public void DownloadVideo(string fileName, string saveAsName = "", Action OnDownloadCompleteOneOff = null)
    {
        Client.GetObjectAsync(S3BucketName, fileName, (responseObj) =>
        {
            if (responseObj.Exception != null)
            {
                Debug.LogError(responseObj.Exception.Message);
                OnDownloadFailed?.Invoke();
            }
            else
            {
                var response = responseObj.Response;
                if (response.ResponseStream != null)
                {
                    try
                    {
                        WriteFile(saveAsName == "" ? fileName : saveAsName, response);

                        OnDownloadVideoComplete?.Invoke();
                        OnDownloadCompleteOneOff?.Invoke();
                    }
                    catch (Exception e)
                    {
                        print(e.Message);
                    }
                }
                else
                {
                    Debug.LogError("Response Stream was null");
                }
            }
        });
    }

    public void DownloadGeneric(string fileName, Action OnDownloadCompleteOneOff = null)
    {
        Client.GetObjectAsync(S3BucketName, fileName, (responseObj) =>
        {
            if (responseObj.Exception != null)
            {
                Debug.LogError(responseObj.Exception.Message + " File = " + fileName);
                OnDownloadFailed?.Invoke();
            }
            else
            {
                var response = responseObj.Response;
                if (response.ResponseStream != null)
                {
                    try
                    {
                        WriteFile(fileName, response);
                        OnDownloadCompleteOneOff?.Invoke();
                    }
                    catch (Exception e)
                    {
                        print(e.Message);
                    }
                }
                else
                {
                    Debug.LogError("Response Stream was null");
                }
            }
        });
    }

    private static void WriteFile(string fileName, GetObjectResponse response)
    {
        if (HelperFunctions.DoesFileExist(fileName))
        {
            File.Delete(HelperFunctions.PersistentDir() + fileName);
        }

        using (var fs = System.IO.File.Create(HelperFunctions.PersistentDir() + fileName))
        {
            byte[] buffer = new byte[81920];
            int count;
            while ((count = response.ResponseStream.Read(buffer, 0, buffer.Length)) != 0)
                fs.Write(buffer, 0, count);
            fs.Flush();
        }

        File.SetCreationTime(HelperFunctions.PersistentDir() + fileName, DateTime.Now);
        //print(File.GetCreationTime(HelperFunctions.PersistentDir()  + fileName));
    }

    public void UploadFile()
    {
        string fileName = GetFileHelper();
        var stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

        var request = new PostObjectRequest()
        {
            Region = _S3Region,
            Bucket = S3BucketName,
            Key = fileName,
            InputStream = stream,
            CannedACL = S3CannedACL.Private
        };

        Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log("Upload Successful");
            }
            else
            {
                Debug.LogError($"Upload failed {responseObj.Response.HttpStatusCode.ToString()}");
            }
        });
    }
    private string GetFileHelper()
    {
        var fileName = "UPLOADTEST";

        if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName))
        {
            var streamReader = File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);
            streamReader.WriteLine("This is a sample s3 file uploaded from unity s3 sample");
            streamReader.Close();
        }
        return fileName;
    }

}

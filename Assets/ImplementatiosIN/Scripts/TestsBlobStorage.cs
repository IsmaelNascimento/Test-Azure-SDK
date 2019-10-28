using UnityEngine;
using Microsoft.WindowsAzure.Storage.Blob;

public class TestsBlobStorage : MonoBehaviour
{
    #region Methods Create

    [ContextMenu("CreateContainer")]
    private async void CreateContainer()
    {
        await BlobStorageManager.Instance.CreateContainerAsync("", () => print("Starting creation new container..."), () => print("Created container"));
    }

    [ContextMenu("UploadFileOrUpdateFile")]
    private async void UploadFileOrUpdateFile()
    {
        var nameFileWithExtesion = "photoReward.png";
        await BlobStorageManager.Instance.UploadOrUpdateBlockBlobAsync("viva-picnic-tests", Application.streamingAssetsPath, "ImageReward/", nameFileWithExtesion, () => print("Start upload..."), () => print("Upload done"));
    }

    #endregion

    #region Methods Read

    [ContextMenu("GetListBlobsInContainerAsync")]
    private async void GetListBlobsInContainerAsync()
    {
        var listBlobs = await BlobStorageManager.Instance.GetListBlobsInContainerAsync("", () => print("Start getting list..."), () => print("List get done"));

        foreach (IListBlobItem blob in listBlobs)
        {
            var nameBlob = blob.Uri.ToString().Split('/')[blob.Uri.ToString().Split('/').Length - 1];
            Debug.Log(string.Format("Name: {0} - Url: {1}", nameBlob, blob.Uri));
        }
    }

    [ContextMenu("GetListBlobsInDirectory")]
    private void GetListBlobsInDirectory()
    {
        var listBlobs = BlobStorageManager.Instance.GetListBlobsInDirectory("", "", () => print("Starting getting list..."), () => print("List get done"));

        foreach (IListBlobItem blob in listBlobs)
        {
            var nameBlob = blob.Uri.ToString().Split('/')[blob.Uri.ToString().Split('/').Length - 1];
            Debug.Log(string.Format("Name: {0} - Url: {1}", nameBlob, blob.Uri));
        }
    }

    [ContextMenu("DownloadBlockBlobAsync")]
    private async void DownloadBlockBlobAsync()
    {
        await BlobStorageManager.Instance.DownloadBlobAsync("", "", "", () => print("Start download..."), () => print("Download done"));
    }

    #endregion

    #region Methods Update

    // Call method UploadFileOrUpdateFile for update any type of file

    [ContextMenu("UpdateText")]
    private async void UpdateText()
    {
        var pathFileTxt = Application.streamingAssetsPath;
        await BlobStorageManager.Instance.UpdateFileTxtBlockBlobAsync("", pathFileTxt, "", "", "", () => print("Start update txt..."), () => print("txt Updated"));
    }

    #endregion

    #region Methods  Delete
    
    [ContextMenu("DeleteBlob")]
    private async void DeleteBlob()
    {
        await BlobStorageManager.Instance.DeleteBlockBlobAsync("", "", () => print("Start delete..."), () => print("Blob deleted"));
    }

    [ContextMenu("DeleteBlobInDirectory")]
    private async void DeleteBlobInDirectory()
    {
        await BlobStorageManager.Instance.DeleteBlockBlobInDirectoryAsync("", "", "", () => print("Start delete..."), () => print("Blob deleted"));
    }

    [ContextMenu("DeleteFolderbAsync")]
    private async void DeleteFolderbAsync()
    {
        await BlobStorageManager.Instance.DeleteFolderBlobAsync("", "", () => print("Start delete..."), () => print("Folder deleted"));
    }

    [ContextMenu("DeleteContainer")]
    private async void DeleteContainer()
    {
        await BlobStorageManager.Instance.DeleteContainerAsync("", () => print("Start deleted..."), () => print("Container deleted."));
    }

    #endregion
}
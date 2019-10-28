using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;

public class ShowInformation : MonoBehaviour
{
    public Text textShowDisplay;
    public string setLicense;

    // Imagem onde vai aparecer a foto do premio
    public Image imageProfile;

    // InputField onde atualiza senha da crianca para entrar no APP
    public InputField inputPassword;

    // Canvas que sera desabilitado na hora da foto
    public GameObject canvas;

    #region Verificacoes do APP
    
    #region Verifica se ADM tem a licensa correta para entrar no app

    [ContextMenu("VerifyLicense")]
    private void VerifyLicense()
    {
        StartCoroutine(LoadLicense());
    }

    private IEnumerator LoadLicense()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://unitytests.blob.core.windows.net/viva-picnic-tests/License/license.txt"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                if (setLicense.Equals(www.downloadHandler.text))
                    print("License valid");
                else
                    print("License invalid");
            }
        }
    }

    #endregion

    #endregion

    #region ADM

    #region Captura foto e enviar para o servidor

    [ContextMenu("TakeScreenshot")]
    public void TakeScreenshot()
    {
        StartCoroutine(TakeScreenshot_Coroutine());
    }

    public IEnumerator TakeScreenshot_Coroutine()
    {
        canvas.SetActive(false);
        yield return new WaitForEndOfFrame();
        try
        {
            var pathScreenshot = Application.streamingAssetsPath + "/photoReward.png";
            ScreenCapture.CaptureScreenshot(pathScreenshot);
        }
        catch (Exception e)
        {
            textShowDisplay.text = e.ToString();
        }
        yield return new WaitForEndOfFrame();
        canvas.SetActive(true);
        UploadAsync();
    }

    private async void UploadAsync()
    {
        await BlobStorageManager.Instance.UploadOrUpdateBlockBlobInDirectoryAsync("viva-picnic-tests", "ImageReward/", Application.streamingAssetsPath, "photoReward.png", () =>
        {
            textShowDisplay.text = ("Upload image...");
        },
        () =>
        {
            textShowDisplay.text = ("Upload complete");
            print("Upload image complete");
            StartCoroutine(LoadImageOnline());
        },
        (e) =>
        {
            textShowDisplay.text = e.ToString();
        });
    }

    #endregion

    #region Atualiza a senha da criança para entrar no app

    private string GetStreamingAssetsPath()
    {
        string path;
#if UNITY_EDITOR
    path = Application.dataPath + "/StreamingAssets";
#elif UNITY_ANDROID
     //path = "jar:file://"+ Application.dataPath + "!/assets/";
     path = "jar:file:"+ Application.dataPath + "!/assets/";
#elif UNITY_IOS
     path = "file:" + Application.dataPath + "/Raw";
#else
     //Desktop (Mac OS or Windows)
     path = Application.dataPath + "/StreamingAssets";
#endif

        return path;
    }

    public async void UpdatePasswordUser()
    {
        var pathFileInContainer = "User/";
        await BlobStorageManager.Instance.UpdateFileTxtBlockBlobAsync("viva-picnic-tests", GetStreamingAssetsPath(), pathFileInContainer, "passwordForUser.txt", inputPassword.text, () => 
        {
            textShowDisplay.text = "Start update...";
        }, 
        () => 
        {
            textShowDisplay.text = GetStreamingAssetsPath() + "-/- " + "Password updated";
        },
        (e) =>
        {
            textShowDisplay.text = GetStreamingAssetsPath() + " -/- " + e.ToString();
        });
    }

    #endregion

    #endregion

    #region Crianca

    #region Verifica se a criança pode entrar no dispositivo com o password colocado

    public void VerifyPassword()
    {
        StartCoroutine(LoadPassword());
    }

    private IEnumerator LoadPassword()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://unitytests.blob.core.windows.net/viva-picnic-tests/User/passwordForUser.txt"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                textShowDisplay.text = www.error;
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                if (inputPassword.text == (www.downloadHandler.text))
                    textShowDisplay.text = (www.downloadHandler.text + " - Password valid");
                else
                    textShowDisplay.text = (www.downloadHandler.text + " - Password invalid");
            }
        }
    }

    #endregion
    
    #region Carrega imagem do premio para a criança

    public void Start()
    {
        textShowDisplay.text = GetStreamingAssetsPath();
        StartCoroutine(LoadImageOnline());
    }

    private IEnumerator LoadImageOnline()
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://unitytests.blob.core.windows.net/viva-picnic-tests/ImageReward/photoReward.png"))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
            {
                var texture2D = DownloadHandlerTexture.GetContent(request);
                var newSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));
                imageProfile.sprite = newSprite;
                //textShowDisplay.text = ("Load image done");
            }
        }
    }

    #endregion

    #endregion
}
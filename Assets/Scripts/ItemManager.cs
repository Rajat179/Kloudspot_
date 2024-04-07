using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public string id;
    public string url;
    public int width;
    public int height;

    public void SetItemDataInUI(string id, string url) {

        Debug.Log($"URL   {id} {url}");
        string substr = url.Substring(url.Length - 3);
        Debug.Log(id);

        if (substr == "gif")
        {
            StartCoroutine(GetGifAndAssign(url, this.gameObject.GetComponent<RawImage>()));
        }
        else
        {
            StartCoroutine(GetTextureAndAssign(url, this.gameObject.GetComponent<RawImage>()));
        }
    }

    // DOWNLOAD & ASSIGN TEXTURE TO RAW IMAGE AND PLAY
    IEnumerator GetTextureAndAssign(string item, RawImage rawImage)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture;
            rawImage.texture = myTexture;
            rawImage.rectTransform.sizeDelta = new Vector2(200, 200);
            rawImage.gameObject.SetActive(true);
        }
    }

    //DOWNLOAD & ASSIGN GIF TO RAW IMAGE & PLAY
    private IEnumerator GetGifAndAssign(string item, RawImage rawImage)
    {
        rawImage.gameObject.SetActive(true);
        rawImage.enabled = false;
        yield return StartCoroutine(rawImage.GetComponent<UniGifImage>().SetGifFromUrlCoroutine(url));
       // rawImage.rectTransform.sizeDelta = new Vector2(item.width, item.height);
        rawImage.enabled = true;
    }
}

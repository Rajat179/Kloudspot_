using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;
using TMPro;
using System.Linq;

public class API_Handler : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string id;
        public string url;
        public int width;
        public int height;
    }

    [SerializeField]
    Transform contentParent;
    [SerializeField]
    GameObject ImagePrefab;

    [SerializeField]
    string apiEndpoint;

    public List<Item> itemsListRef;

    [SerializeField]
    bool EnabledAPI;
    bool isFirstPageLoaded;

    [SerializeField]
    Button GetTexturesButton;

    [SerializeField]
    GameObject ContentParentObject;

    [SerializeField]
    Button nextBtn;

    [SerializeField]
    RawImage[] rawImages;

    [SerializeField]
    List<GameObject> inScenePrefabList;

    private void Awake()
    {
        StartCoroutine(GetJSON());
    }

    // DOWNLOAD THE JSON USING API CALL
    IEnumerator GetJSON()
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(apiEndpoint);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {
            string json = unityWebRequest.downloadHandler.text;
            SpawnPrefabs(json);
            LoadItems(json);
            isFirstPageLoaded = true;

        }
    }

   public void NextButtonClicked()
    {
        nextBtn.interactable = false;
        StartCoroutine(GetJSON());
        Invoke("SetBtnTrue", 3f);
    }

    void SpawnPrefabs(string json)
    {
        List<Item> ItemList = JsonConvert.DeserializeObject<List<Item>>(json);
        for (int i = 0; i < 10; i++)
        {
            if (!isFirstPageLoaded)
            {
                GameObject listObjectsUI = Instantiate(ImagePrefab, contentParent.position, Quaternion.identity);
                listObjectsUI.transform.SetParent(contentParent);
                listObjectsUI.transform.localScale = new Vector3(1, 1, 1);
                inScenePrefabList.Add(listObjectsUI);
            }
            itemsListRef.Add(ItemList[i]);
            AssignTexturesFromItemData(ItemList, i);
        }
    }

    // DESERIALISE JSON OBJECT & PUT ALL THE ITEM IN ITEMS LIST & DOWNLOAD & ASSIGN TEXTURES
    void LoadItems(string json)
    {
        List<Item> ItemList = JsonConvert.DeserializeObject<List<Item>>(json);
        for (int i = 0; i < ItemList.Count; i++)
        {
            itemsListRef.Add(ItemList[i]);
            AssignTexturesFromItemData(ItemList, i);
        }
    }

    #region Put Data on UI

    // ASSIGN URL & CHECK DOWNLOAD ITEM TYPE FROM THE JSON ITEM
    private void AssignTexturesFromItemData(List<Item> ItemList, int i)
    {
        string url = ItemList[i].url;
        string substr = url.Substring(url.Length - 3);
        Debug.Log(ItemList[i].id);
        if (substr == "gif")
        {
            StartCoroutine(GetGifAndAssign(ItemList[i], inScenePrefabList[i].GetComponent<RawImage>()));
        }
        else
        {
            StartCoroutine(GetTextureAndAssign(ItemList[i], inScenePrefabList[i].GetComponent<RawImage>()));
        }
    }

    // DOWNLOAD & ASSIGN TEXTURE TO RAW IMAGE AND PLAY
    IEnumerator GetTextureAndAssign(Item item, RawImage rawImage)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(item.url);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture;
            rawImage.texture = myTexture;
            rawImage.rectTransform.sizeDelta = new Vector2(400, 400);
            rawImage.gameObject.SetActive(true);
        }

    }

    //DOWNLOAD & ASSIGN GIF TO RAW IMAGE & PLAY
    private IEnumerator GetGifAndAssign(Item item, RawImage rawImage)
    {
        rawImage.gameObject.SetActive(true);
        rawImage.enabled = false;
        yield return StartCoroutine(rawImage.GetComponent<UniGifImage>().SetGifFromUrlCoroutine(item.url));
        rawImage.rectTransform.sizeDelta = new Vector2(400,400);
        rawImage.enabled = true;
    }

    void SetBtnTrue() { nextBtn.interactable = true; }
    #endregion
}

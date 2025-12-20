using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopController : MonoBehaviour
{
    public KeyCode openKey = KeyCode.M;
    public GameObject shopPanel;
    public GameObject itemCardPrefab;
    public Transform itemsContainer;
    public Text moneyText;

    public MonoBehaviour cameraScript;
    public MonoBehaviour playerScript;

    public List<ShopItem> itemsToSell = new List<ShopItem>();
    public int playerMoney = 1000;

    private bool isShopOpen = false;

    [System.Serializable]
    public class ShopItem
    {
        public GameObject itemToShow;
        public GameObject itemToHide;
        public string itemName = "Товар";
        public int price = 100;
    }

    void Start()
    {
        shopPanel.SetActive(false);

        if (moneyText != null)
            moneyText.text = playerMoney.ToString();

        foreach (ShopItem item in itemsToSell)
        {
            if (item.itemToShow != null)
                item.itemToShow.SetActive(false);

            if (item.itemToHide != null)
                item.itemToHide.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            if (isShopOpen)
                CloseShop();
            else
                OpenShop();
        }
    }

    void OpenShop()
    {
        isShopOpen = true;
        shopPanel.SetActive(true);

        if (cameraScript != null) cameraScript.enabled = false;
        if (playerScript != null) playerScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        CreateItems();
    }

    void CloseShop()
    {
        isShopOpen = false;
        shopPanel.SetActive(false);

        if (cameraScript != null) cameraScript.enabled = true;
        if (playerScript != null) playerScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    void CreateItems()
    {
        // Удаляем старые
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаем новые
        for (int i = 0; i < itemsToSell.Count; i++)
        {
            GameObject card = Instantiate(itemCardPrefab, itemsContainer);

            // Находим ItemName и ItemPrice по именам
            Transform nameObj = card.transform.Find("ItemName");
            Transform priceObj = card.transform.Find("ItemPrice");

            // Настраиваем ItemName - ВСЕГДА ставим название товара
            if (nameObj != null)
            {
                Text nameText = nameObj.GetComponent<Text>();
                if (nameText != null)
                {
                    nameText.text = itemsToSell[i].itemName;
                }
            }

            // Настраиваем ItemPrice
            if (priceObj != null)
            {
                Text priceText = priceObj.GetComponent<Text>();
                if (priceText != null)
                {
                    bool isBought = (itemsToSell[i].itemToShow != null &&
                                   itemsToSell[i].itemToShow.activeSelf);

                    if (isBought)
                    {
                        priceText.text = "Куплено";
                        priceText.color = Color.gray;
                    }
                    else
                    {
                        priceText.text = itemsToSell[i].price + " $";
                        priceText.color = playerMoney >= itemsToSell[i].price ?
                                        Color.green : Color.red;
                    }
                }
            }

            // Кнопку настраиваем - ВАЖНО: меняем только текст на самой кнопке, а не ItemName
            Button buyButton = card.GetComponentInChildren<Button>();
            if (buyButton != null)
            {
                // Находим текст кнопки, который НЕ является ItemName или ItemPrice
                // Предполагаем, что текст кнопки - это Text (TMP) который в корне кнопки
                Text buttonText = null;

                // Ищем Text компоненты внутри кнопки
                Text[] allTextsInButton = buyButton.GetComponentsInChildren<Text>(true);
                foreach (Text text in allTextsInButton)
                {
                    // Пропускаем ItemName и ItemPrice
                    if (text.gameObject.name == "ItemName" || text.gameObject.name == "ItemPrice")
                        continue;

                    // Если это текст, который находится прямо в кнопке или в непосредственном ребенке
                    if (text.transform.parent == buyButton.transform ||
                        text.transform == buyButton.transform)
                    {
                        buttonText = text;
                        break;
                    }
                }

                // Если не нашли - ищем любой текст, кроме ItemName/ItemPrice
                if (buttonText == null)
                {
                    foreach (Text text in allTextsInButton)
                    {
                        if (text.gameObject.name != "ItemName" && text.gameObject.name != "ItemPrice")
                        {
                            buttonText = text;
                            break;
                        }
                    }
                }

                bool isBought = (itemsToSell[i].itemToShow != null &&
                               itemsToSell[i].itemToShow.activeSelf);

                if (buttonText != null)
                    buttonText.text = isBought ? "Куплено" : "Купить";

                buyButton.interactable = !isBought && playerMoney >= itemsToSell[i].price;

                // Обработчик
                int index = i;
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() => BuyItem(index));
            }

            // Позиционируем карточку
            MoveCardToTop(card, i);
        }
    }

    void MoveCardToTop(GameObject card, int index)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1.5f);

            Vector2 currentSize = rect.sizeDelta;
            Vector2 currentAnchorMin = rect.anchorMin;
            Vector2 currentAnchorMax = rect.anchorMax;
            Vector2 currentPivot = rect.pivot;          
            Vector2 currentPos = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(currentPos.x, -index * 110 - 20);
            
            rect.sizeDelta = currentSize;
            rect.anchorMin = currentAnchorMin;
            rect.anchorMax = currentAnchorMax;
            rect.pivot = currentPivot;
        }
    }

    void BuyItem(int index)
    {
        if (index < 0 || index >= itemsToSell.Count) return;

        ShopItem item = itemsToSell[index];

        if (playerMoney < item.price) return;
        if (item.itemToShow != null && item.itemToShow.activeSelf) return;

        playerMoney -= item.price;
        moneyText.text = playerMoney.ToString();

        if (item.itemToHide != null)
            item.itemToHide.SetActive(false);

        if (item.itemToShow != null)
            item.itemToShow.SetActive(true);

        CreateItems();
    }
}
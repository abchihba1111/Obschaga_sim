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

    public List<ShopItemData> itemsToSell = new List<ShopItemData>();
    public int playerMoney = 1000;

    private bool isShopOpen = false;

    [System.Serializable]
    public class ShopItemData
    {
        public GameObject itemToShow;
        public GameObject itemToHide;
        public string itemName = "Товар";
        public int price = 100;

        public bool isMultiItem = false;
        public int maxAmount = 4;
        public List<GameObject> specificItems = new List<GameObject>();

        [HideInInspector] public int currentIndex = 0;

        public bool hasRequirements = false;
        public List<int> requiredItemIndices = new List<int>();
        public string requirementDescription = "Требуется купить другие предметы";

        public string GetRequiredItemNames(List<ShopItemData> allItems)
        {
            if (requiredItemIndices.Count == 0)
                return "";

            List<string> names = new List<string>();
            foreach (int index in requiredItemIndices)
            {
                if (index >= 0 && index < allItems.Count)
                {
                    names.Add(allItems[index].itemName);
                }
            }
            return string.Join(", ", names);
        }
    }

    void Start()
    {
        shopPanel.SetActive(false);

        if (moneyText != null)
            moneyText.text = playerMoney.ToString();

        foreach (ShopItemData item in itemsToSell)
        {
            if (!item.isMultiItem)
            {
                if (item.itemToShow != null)
                    item.itemToShow.SetActive(false);

                if (item.itemToHide != null)
                    item.itemToHide.SetActive(true);
            }
            else
            {
                foreach (GameObject specificItem in item.specificItems)
                {
                    if (specificItem != null)
                        specificItem.SetActive(false);
                }
                item.currentIndex = 0;
            }
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
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < itemsToSell.Count; i++)
        {
            GameObject card = Instantiate(itemCardPrefab, itemsContainer);

            Transform nameObj = card.transform.Find("ItemName");
            Transform priceObj = card.transform.Find("ItemPrice");

            if (nameObj != null)
            {
                Text nameText = nameObj.GetComponent<Text>();
                if (nameText != null)
                {
                    string itemNameText = itemsToSell[i].itemName;

                    if (itemsToSell[i].isMultiItem)
                    {
                        int totalItems = itemsToSell[i].specificItems.Count;
                        int purchasedItems = itemsToSell[i].currentIndex;
                        itemNameText += $" ({purchasedItems}/{totalItems})";
                    }

                    if (itemsToSell[i].hasRequirements && !AreRequirementsMet(i))
                    {
                        itemNameText += " [🔒]";
                    }

                    nameText.text = itemNameText;
                }
            }

            if (priceObj != null)
            {
                Text priceText = priceObj.GetComponent<Text>();
                if (priceText != null)
                {
                    bool isFullyBought = IsItemFullyBought(i);
                    bool requirementsMet = AreRequirementsMet(i);
                    bool isAvailable = !itemsToSell[i].hasRequirements || requirementsMet;

                    if (itemsToSell[i].isMultiItem)
                    {
                        if (itemsToSell[i].currentIndex >= itemsToSell[i].specificItems.Count)
                        {
                            priceText.text = "Распродано";
                            priceText.color = Color.gray;
                        }
                        else if (isFullyBought)
                        {
                            priceText.text = "Куплено";
                            priceText.color = Color.gray;
                        }
                        else if (!isAvailable)
                        {
                            priceText.text = "Заблокировано";
                            priceText.color = Color.yellow;
                        }
                        else
                        {
                            priceText.text = itemsToSell[i].price + " $";
                            priceText.color = playerMoney >= itemsToSell[i].price ?
                                            Color.green : Color.red;
                        }
                    }
                    else
                    {
                        if (isFullyBought)
                        {
                            priceText.text = "Куплено";
                            priceText.color = Color.gray;
                        }
                        else if (!isAvailable)
                        {
                            priceText.text = "Заблокировано";
                            priceText.color = Color.yellow;
                        }
                        else
                        {
                            priceText.text = itemsToSell[i].price + " $";
                            priceText.color = playerMoney >= itemsToSell[i].price ?
                                            Color.green : Color.red;
                        }
                    }
                }
            }

            Button buyButton = card.GetComponentInChildren<Button>();
            if (buyButton != null)
            {
                Text buttonText = null;
                Text[] allTextsInButton = buyButton.GetComponentsInChildren<Text>(true);

                foreach (Text text in allTextsInButton)
                {
                    if (text.gameObject.name == "ItemName" || text.gameObject.name == "ItemPrice")
                        continue;

                    if (text.transform.parent == buyButton.transform ||
                        text.transform == buyButton.transform)
                    {
                        buttonText = text;
                        break;
                    }
                }

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

                bool isFullyBought = IsItemFullyBought(i);
                bool canBuy = CanBuyItem(i);
                bool requirementsMet = AreRequirementsMet(i);
                bool isAvailable = !itemsToSell[i].hasRequirements || requirementsMet;

                if (buttonText != null)
                {
                    if (itemsToSell[i].isMultiItem)
                    {
                        if (itemsToSell[i].currentIndex >= itemsToSell[i].specificItems.Count)
                            buttonText.text = "Распродано";
                        else if (isFullyBought)
                            buttonText.text = "Куплено";
                        else if (!isAvailable)
                            buttonText.text = "Заблокировано";
                        else
                            buttonText.text = "Купить";
                    }
                    else
                    {
                        if (isFullyBought)
                            buttonText.text = "Куплено";
                        else if (!isAvailable)
                            buttonText.text = "Заблокировано";
                        else
                            buttonText.text = "Купить";
                    }
                }

                buyButton.interactable = canBuy;

                if (itemsToSell[i].hasRequirements && !requirementsMet)
                {
                    buyButton.image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
                else
                {
                    buyButton.image.color = Color.white;
                }

                int index = i;
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() => BuyItem(index));
            }

            if (itemsToSell[i].hasRequirements && !AreRequirementsMet(i))
            {
                GameObject requirementTextObj = new GameObject("RequirementText");
                requirementTextObj.transform.SetParent(card.transform, false);

                Text requirementText = requirementTextObj.AddComponent<Text>();

                string requiredNames = itemsToSell[i].GetRequiredItemNames(itemsToSell);
                if (!string.IsNullOrEmpty(requiredNames))
                {
                    requirementText.text = $"Требуется: {requiredNames}";
                }
                else
                {
                    requirementText.text = itemsToSell[i].requirementDescription;
                }

                requirementText.color = Color.yellow;
                requirementText.fontSize = 10;
                requirementText.alignment = TextAnchor.MiddleCenter;

                RectTransform rt = requirementTextObj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(200, 30);
                rt.anchoredPosition = new Vector2(0, -80);
            }

            MoveCardToTop(card, i);
        }
    }

    bool AreRequirementsMet(int itemIndex)
    {
        ShopItemData item = itemsToSell[itemIndex];

        if (!item.hasRequirements || item.requiredItemIndices.Count == 0)
            return true;

        foreach (int requiredIndex in item.requiredItemIndices)
        {
            if (requiredIndex < 0 || requiredIndex >= itemsToSell.Count)
            {
                Debug.LogWarning($"Неверный индекс требуемого товара: {requiredIndex}");
                continue;
            }

            if (!IsItemFullyBought(requiredIndex))
                return false;
        }

        return true;
    }

    bool IsItemFullyBought(int index)
    {
        ShopItemData item = itemsToSell[index];

        if (item.isMultiItem)
        {
            return item.currentIndex >= item.specificItems.Count;
        }
        else
        {
            return item.itemToShow != null && item.itemToShow.activeSelf;
        }
    }

    bool CanBuyItem(int index)
    {
        ShopItemData item = itemsToSell[index];

        if (item.hasRequirements && !AreRequirementsMet(index))
            return false;

        if (item.isMultiItem)
        {
            return playerMoney >= item.price &&
                   item.currentIndex < item.specificItems.Count;
        }
        else
        {
            return playerMoney >= item.price &&
                   !(item.itemToShow != null && item.itemToShow.activeSelf);
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

        ShopItemData item = itemsToSell[index];

        if (!CanBuyItem(index)) return;

        playerMoney -= item.price;
        UpdateMoneyText();

        if (item.isMultiItem)
        {
            if (item.currentIndex < item.specificItems.Count)
            {
                GameObject nextItem = item.specificItems[item.currentIndex];
                if (nextItem != null)
                {
                    nextItem.SetActive(true);
                    Debug.Log($"Активирован предмет: {nextItem.name}");
                }
                item.currentIndex++;

                Debug.Log($"Куплен предмет из мультитовара: {item.itemName}. Куплено: {item.currentIndex}/{item.specificItems.Count}");
            }
        }
        else
        {
            if (item.itemToHide != null)
                item.itemToHide.SetActive(false);

            if (item.itemToShow != null)
                item.itemToShow.SetActive(true);
        }

        CreateItems();
    }

    // НОВЫЙ МЕТОД для обновления текста денег
    public void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = playerMoney.ToString();
        }
    }
}
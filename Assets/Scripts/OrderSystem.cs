using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class OrderSystem : MonoBehaviour
{
    public KeyCode openKey = KeyCode.O;
    public GameObject ordersPanel;
    public GameObject orderCardPrefab;
    public Transform ordersContainer;
    public Text paperText;
    public Text moneyText;

    public MonoBehaviour cameraScript;
    public MonoBehaviour playerScript;
    public PrinterController printer;
    public ShopController shopController;

    public int maxOrders = 5;
    public int paperInventory = 100;
    private bool isPanelOpen = false;
    private GameObject currentCustomer;

    public float orderSpawnDelay = 10f;
    public int maxSimultaneousOrders = 3;

    [Header("Настройки клиента")]
    public GameObject customerPrefab;
    public Transform customerSpawnPoint;

    private List<Order> orders = new List<Order>();
    private bool isGeneratingOrders = true;

    [System.Serializable]
    public class Order
    {
        public int id;
        public int sheets;
        public int reward;
        public bool accepted;
        public bool printed;
        public bool customerCalled;
        public bool paperSpent;
        public GameObject customerInstance;

        public Order(int orderId, int sheetsCount, int rewardAmount)
        {
            id = orderId;
            sheets = sheetsCount;
            reward = rewardAmount;
            accepted = false;
            printed = false;
            customerCalled = false;
            paperSpent = false;
            customerInstance = null;
        }
    }

    void Start()
    {
        if (ordersPanel != null)
            ordersPanel.SetActive(false);

        UpdateUI();

        AddNewOrder();
        StartCoroutine(GenerateOrdersWithDelay());
    }

    IEnumerator GenerateOrdersWithDelay()
    {
        while (isGeneratingOrders)
        {
            yield return new WaitForSeconds(orderSpawnDelay);

            if (orders.Count < maxSimultaneousOrders)
            {
                AddNewOrder();

                if (isPanelOpen)
                {
                    CreateCards();
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            if (isPanelOpen)
                ClosePanel();
            else
                OpenPanel();
        }
    }

    void AddNewOrder()
    {
        if (orders.Count >= maxOrders) return;

        int sheets = Random.Range(1, 51);
        int reward = sheets * 10 + Random.Range(0, 50);

        Order newOrder = new Order(orders.Count + 1, sheets, reward);
        orders.Add(newOrder);
    }

    void OpenPanel()
    {
        isPanelOpen = true;
        ordersPanel.SetActive(true);

        if (cameraScript != null) cameraScript.enabled = false;
        if (playerScript != null) playerScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        CreateCards();
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        ordersPanel.SetActive(false);

        if (cameraScript != null) cameraScript.enabled = true;
        if (playerScript != null) playerScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    void CreateCards()
    {
        foreach (Transform child in ordersContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < orders.Count; i++)
        {
            Order order = orders[i];

            GameObject card = Instantiate(orderCardPrefab, ordersContainer);

            RectTransform rect = card.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.pivot = new Vector2(0.5f, 1.25f);
                float yPosition = -i * (120 + 40);
                rect.anchoredPosition = new Vector2(0, yPosition);
                rect.sizeDelta = new Vector2(600, 150);
            }

            TMP_Text countText = FindTMPText(card.transform, "CountTxt");
            TMP_Text priceText = FindTMPText(card.transform, "PriceTxt");

            if (countText != null)
                countText.text = $"Количество листов: {order.sheets}";

            if (priceText != null)
                priceText.text = $"Сумма: {order.reward}";

            Button acceptButton = FindButton(card.transform, "AcceptButton");
            Button callButton = FindButton(card.transform, "CallCustomerButton");

            if (acceptButton != null)
            {
                acceptButton.gameObject.SetActive(!order.accepted);
                acceptButton.interactable = !order.accepted;

                TMP_Text buttonText = acceptButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = order.accepted ? "Принято" : "Принять";
                }

                int index = i;
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(() => AcceptOrder(index));
            }

            if (callButton != null)
            {
                bool shouldShow = order.printed && !order.customerCalled;
                callButton.gameObject.SetActive(shouldShow);
                callButton.interactable = shouldShow;

                TMP_Text buttonText = callButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = "Позвать заказчика";
                }

                int index = i;
                callButton.onClick.RemoveAllListeners();
                callButton.onClick.AddListener(() => CallCustomer(index));
            }
        }

        UpdateUI();
    }

    TMP_Text FindTMPText(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        if (child != null)
            return child.GetComponent<TMP_Text>();
        return null;
    }

    Button FindButton(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        if (child != null)
            return child.GetComponent<Button>();
        return null;
    }

    void AcceptOrder(int index)
    {
        if (index < 0 || index >= orders.Count) return;

        Order order = orders[index];

        if (order.accepted) return;

        order.accepted = true;

        CreateCards();
    }

    public void StartPrinting()
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].accepted && !orders[i].printed && !orders[i].paperSpent)
            {
                if (paperInventory >= orders[i].sheets)
                {
                    paperInventory -= orders[i].sheets;
                    orders[i].paperSpent = true;

                    if (printer != null)
                    {
                        printer.StartPrintingProcess(orders[i].sheets);
                    }

                    UpdateUI();
                    return;
                }
                else
                {
                    return;
                }
            }
        }
    }

    public void CompletePrinting()
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].accepted && orders[i].paperSpent && !orders[i].printed)
            {
                orders[i].printed = true;

                if (isPanelOpen)
                {
                    CreateCards();
                }
                return;
            }
        }
    }

    void CallCustomer(int index)
    {
        if (index < 0 || index >= orders.Count) return;

        Order order = orders[index];

        if (!order.printed || order.customerCalled) return;

        order.customerCalled = true;

        // Находим клиента в сцене
        CustomerController customerController = FindObjectOfType<CustomerController>();
        if (customerController != null)
        {
            Debug.Log("Найден клиент в сцене, активируем...");
            customerController.Setup(index, this);
        }
        else
        {
            Debug.LogError("Клиент не найден в сцене!");
        }

        CreateCards();
    }

    public void DeliverOrder(int orderIndex)
    {
        if (orderIndex < 0 || orderIndex >= orders.Count) return;

        Order order = orders[orderIndex];

        if (!order.printed || !order.customerCalled) return;

        Debug.Log($"Выдаем заказ #{order.id}, награда: {order.reward}");

        // Добавляем деньги
        if (shopController != null)
        {
            shopController.playerMoney += order.reward;
            shopController.UpdateMoneyText();
            Debug.Log($"Деньги добавлены: {order.reward}");
        }

        // Удаляем заказ
        orders.RemoveAt(orderIndex);

        // Добавляем новый
        StartCoroutine(AddNewOrderWithDelay());

        UpdateUI();

        if (isPanelOpen)
        {
            CreateCards();
        }
    }

    IEnumerator AddNewOrderWithDelay()
    {
        yield return new WaitForSeconds(2f);

        if (orders.Count < maxSimultaneousOrders)
        {
            AddNewOrder();

            if (isPanelOpen)
            {
                CreateCards();
            }
        }
    }

    void UpdateUI()
    {
        if (paperText != null)
            paperText.text = paperInventory.ToString();

        if (moneyText != null && shopController != null)
        {
            moneyText.text = shopController.playerMoney.ToString();
        }
    }

    // ДОБАВИТЬ ЭТОТ МЕТОД
    public void AddPaper(int amount)
    {
        paperInventory += amount;
        UpdateUI();

        if (isPanelOpen)
        {
            CreateCards();
        }

        Debug.Log($"Добавлено {amount} листов бумаги. Всего: {paperInventory}");
    }

    // ДОБАВИТЬ ЭТОТ МЕТОД ДЛЯ ПОКУПКИ КОРОБКИ
    public void AddPaperBox(int papersPerBox = 120)
    {
        AddPaper(papersPerBox);
    }

    void OnDestroy()
    {
        isGeneratingOrders = false;
        StopAllCoroutines();
    }

}
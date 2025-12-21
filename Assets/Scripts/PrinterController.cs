using UnityEngine;
using UnityEngine.UI;

public class PrinterController : MonoBehaviour
{
    public OrderSystem orderSystem;
    public GameObject printerUI;
    public Text printerStatusText;
    public Slider progressSlider;

    private bool isPrinting = false;
    private float printTimer = 0f;
    private int sheetsToPrint = 0;
    private int sheetsPrinted = 0;

    void Start()
    {
        if (printerUI != null)
            printerUI.SetActive(false);
    }

    void Update()
    {
        if (isPrinting)
        {
            printTimer += Time.deltaTime;

            if (printTimer >= 1f)
            {
                printTimer = 0f;
                sheetsPrinted++;

                if (progressSlider != null)
                {
                    progressSlider.value = (float)sheetsPrinted / sheetsToPrint;
                }

                if (printerStatusText != null)
                {
                    printerStatusText.text = $"Печатается: {sheetsPrinted}/{sheetsToPrint}";
                }

                if (sheetsPrinted >= sheetsToPrint)
                {
                    CompletePrinting();
                }
            }
        }
    }

    public void InteractWithPrinter()
    {
        Debug.Log("Взаимодействие с принтером");

        if (isPrinting)
        {
            Debug.Log("Принтер уже печатает");
            return;
        }

        if (orderSystem == null)
        {
            Debug.LogError("OrderSystem не назначен!");
            return;
        }

        // Просим OrderSystem начать печать
        orderSystem.StartPrinting();
    }

    // Этот метод вызывается из OrderSystem
    public void StartPrintingProcess(int sheets)
    {
        if (isPrinting) return;

        sheetsToPrint = sheets;
        sheetsPrinted = 0;
        isPrinting = true;

        if (printerUI != null)
        {
            printerUI.SetActive(true);

            if (progressSlider != null)
            {
                progressSlider.value = 0f;
            }

            if (printerStatusText != null)
            {
                printerStatusText.text = $"Печатается: 0/{sheets}";
            }
        }

        Debug.Log($"Начата печать: {sheets} листов");
    }

    void CompletePrinting()
    {
        isPrinting = false;

        if (orderSystem != null)
        {
            orderSystem.CompletePrinting();
        }

        if (printerUI != null)
        {
            printerUI.SetActive(false);
        }

        Debug.Log("Печать завершена");
    }
}
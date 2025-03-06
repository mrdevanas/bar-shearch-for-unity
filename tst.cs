using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonFilter : MonoBehaviour
{
    public InputField inputField;  // حقل الإدخال
    public List<Button> buttons;   // قائمة الأزرار
    public Vector2 targetPosition = new Vector2(-70.893f, 29.4f); // الإحداثيات الجديدة لأول زر مطابق
    public float spacing = 50f; // المسافة بين الأزرار المتقاربة
    public int maxHighlightedButtons = 3; // عدد الأزرار التي ستظهر قريبة من الموقع المحدد

    private Dictionary<Button, Vector2> originalPositions = new Dictionary<Button, Vector2>(); // حفظ أماكن الأزرار الأصلية

    void Start()
    {
        // حفظ المواقع الأصلية للأزرار عند التشغيل
        foreach (Button b in buttons)
        {
            originalPositions[b] = b.GetComponent<RectTransform>().anchoredPosition;
        }

        inputField.onValueChanged.AddListener(FilterButtons);
    }

    void FilterButtons(string input)
    {
        string normalizedInput = NormalizeText(input);
        List<Button> matchingButtons = new List<Button>();

        if (string.IsNullOrEmpty(normalizedInput)) // إذا كان الإدخال فارغًا، نظهر كل الأزرار في أماكنها الأصلية
        {
            foreach (Button b in buttons)
            {
                b.gameObject.SetActive(true);
                b.GetComponent<RectTransform>().anchoredPosition = originalPositions[b]; // إرجاعه إلى مكانه الأصلي
            }
        }
        else
        {
            foreach (Button b in buttons)
            {
                string buttonText = NormalizeText(b.GetComponentInChildren<Text>().text);
                bool shouldShow = buttonText.Contains(normalizedInput);

                if (shouldShow)
                {
                    matchingButtons.Add(b);
                }
                else
                {
                    b.gameObject.SetActive(false); // إخفاء الأزرار غير المطابقة
                }
            }

            // تحريك أول 3 أزرار متطابقة إلى أماكن متقاربة
            for (int i = 0; i < matchingButtons.Count; i++)
            {
                matchingButtons[i].gameObject.SetActive(true);

                if (i < maxHighlightedButtons) // إذا كان ضمن أول 3 نتائج
                {
                    matchingButtons[i].GetComponent<RectTransform>().anchoredPosition = 
                        new Vector2(targetPosition.x, targetPosition.y - (i * spacing)); // ترتيبهم عموديًا بمسافات
                }
                else
                {
                    matchingButtons[i].GetComponent<RectTransform>().anchoredPosition = originalPositions[matchingButtons[i]]; // إرجاع الباقي إلى أماكنهم
                }
            }
        }
    }

    string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return text.Trim().ToLower(); // تجاهل الأحرف الكبيرة والصغيرة والمسافات
    }
}

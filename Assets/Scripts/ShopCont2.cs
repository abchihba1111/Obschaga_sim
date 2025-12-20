#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ShopController))]
public class ShopControllerEditor : Editor
{
    private ShopController shopController;
    private SerializedProperty itemsToSellProp;
    private List<string> itemNames = new List<string>();

    private void OnEnable()
    {
        shopController = (ShopController)target;
        itemsToSellProp = serializedObject.FindProperty("itemsToSell");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        itemNames.Clear();
        itemNames.Add("(None)");
        for (int i = 0; i < shopController.itemsToSell.Count; i++)
        {
            itemNames.Add($"{i}: {shopController.itemsToSell[i].itemName}");
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("openKey"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shopPanel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemCardPrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemsContainer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("moneyText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraScript"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerScript"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerMoney"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Список товаров", EditorStyles.boldLabel);

        for (int i = 0; i < itemsToSellProp.arraySize; i++)
        {
            SerializedProperty itemProp = itemsToSellProp.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField($"Товар {i}", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("itemName"));
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("price"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Объекты для отображения:", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("itemToShow"));
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("itemToHide"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("isMultiItem"));
            if (itemProp.FindPropertyRelative("isMultiItem").boolValue)
            {
                EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("maxAmount"));
                EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("specificItems"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("hasRequirements"));
            if (itemProp.FindPropertyRelative("hasRequirements").boolValue)
            {
                SerializedProperty requiredIndicesProp = itemProp.FindPropertyRelative("requiredItemIndices");

                EditorGUILayout.LabelField("Требуемые товары:");
                for (int j = 0; j < requiredIndicesProp.arraySize; j++)
                {
                    EditorGUILayout.BeginHorizontal();

                    SerializedProperty indexProp = requiredIndicesProp.GetArrayElementAtIndex(j);
                    int currentIndex = indexProp.intValue;

                    int displayIndex = currentIndex + 1;
                    if (displayIndex < 0) displayIndex = 0;
                    if (displayIndex >= itemNames.Count) displayIndex = 0;

                    int newDisplayIndex = EditorGUILayout.Popup($"Товар {j + 1}", displayIndex, itemNames.ToArray());

                    int newIndex = newDisplayIndex - 1;

                    if (newDisplayIndex == 0)
                    {
                        newIndex = -1;
                    }

                    if (newIndex != currentIndex)
                    {
                        indexProp.intValue = newIndex;
                    }

                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        requiredIndicesProp.DeleteArrayElementAtIndex(j);
                        j--;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("+ Добавить требование"))
                {
                    requiredIndicesProp.arraySize++;
                    requiredIndicesProp.GetArrayElementAtIndex(requiredIndicesProp.arraySize - 1).intValue = -1;
                }

                EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("requirementDescription"));
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Удалить товар", GUILayout.Width(150)))
            {
                itemsToSellProp.DeleteArrayElementAtIndex(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("+ Добавить товар"))
        {
            itemsToSellProp.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
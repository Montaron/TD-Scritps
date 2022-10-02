using UnityEngine;
using UnityEditor;
[System.Serializable]
public struct FloatRange
{
    [SerializeField]
    float min, max;
    public float Min => min;
    public float Max => max;
    public float RandomValueInRange
    {
        get
        {
            return Random.Range(min, max);
        }
    }
    public FloatRange(float value)
    {
        min = max = value;
    }
    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max < min ? min : max;
    }
}


public class FloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; private set; }
    public float Max { get; private set; }
    public FloatRangeSliderAttribute(float min, float max)
    {
        Min = min;
        Max = max < min ? min : max;
    }
}

[CustomPropertyDrawer(typeof(FloatRangeSliderAttribute))]
public class FloatRangeSliderDrawer : PropertyDrawer
{
    public override void OnGUI(
     Rect position, SerializedProperty property, GUIContent label
    )
    {
        int originalIndentLevel = EditorGUI.indentLevel;
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(
        position, GUIUtility.GetControlID(FocusType.Passive), label
        );
        EditorGUI.indentLevel = 0;
        SerializedProperty minProperty = property.FindPropertyRelative("min");
        SerializedProperty maxProperty = property.FindPropertyRelative("max");
        float minValue = minProperty.floatValue;
        float maxValue = maxProperty.floatValue;
        float fieldWidth = position.width / 4f - 4f;
        float sliderWidth = position.width / 2f;
        position.width = fieldWidth;
        minValue = EditorGUI.FloatField(position, minValue);
        position.x += fieldWidth + 4f;
        position.width = sliderWidth;
        FloatRangeSliderAttribute limit = attribute as FloatRangeSliderAttribute;
        EditorGUI.MinMaxSlider(
        position, ref minValue, ref maxValue, limit.Min, limit.Max
        );
        position.x += sliderWidth + 4f;
        position.width = fieldWidth;
        maxValue = EditorGUI.FloatField(position, maxValue);
        if (minValue < limit.Min)
        {
            minValue = limit.Min;
        }
        if (maxValue < minValue)
        {
            maxValue = minValue;
        }
        else if (maxValue > limit.Max)
        {
            maxValue = limit.Max;
        }
        minProperty.floatValue = minValue;
        maxProperty.floatValue = maxValue;
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originalIndentLevel;
    }
}

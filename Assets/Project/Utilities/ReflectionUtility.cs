using System;
using System.Reflection;
using UnityEngine;

public static class ReflectionUtility
{
    public static void LogFieldsAndProperties(object obj)
    {
        if (obj == null)
        {
            Debug.Log("Object is null");
            return;
        }

        Type type = obj.GetType();
        Debug.Log("Inspecting fields and properties of: " + type.Name);

        // Log fields
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(obj);
            Debug.Log("Field: " + field.Name + ", Value: " + value);
        }

        // Log properties
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            object value = property.GetValue(obj);
            Debug.Log("Property: " + property.Name + ", Value: " + value);
        }
    }
}

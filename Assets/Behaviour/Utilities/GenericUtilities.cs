using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class GenericUtilities
{
    public static void ChangeLayer(this GameObject gameObject, int layer, bool changeChildren = true)
    {
        gameObject.layer = layer;
        if (!changeChildren) return;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.ChangeLayer(layer, true);
        }
    }
    public static T DeepClone<T>(this T a)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, a);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }
    }
    public static T CopyTo<T>(this T original, T copy)
    {
        System.Type type = original.GetType();
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
    public static T CopyFrom<T>(this T copy, T original)
    {
        System.Type type = original.GetType();
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
    public static float ToPercent01(float min, float max, float value)
    {
        float range = max - min;
        float output = (value - min) / range;
        return output;
    }

    public static T CopyComponent<T>(this GameObject destination, T original) where T : Component
    {
        System.Type type = typeof(T);
        T copy;
        if (destination.TryGetComponent(out T component))
             copy = component;
        else copy = destination.AddComponent(type) as T;
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}
public class trigger
{
    bool intbool;
    public bool boolean
    {
        get
        {
            var buff = intbool;
            intbool = false;
            return buff;
        }
        set
        {
            intbool = value;
        }
    }
    public static implicit operator bool(trigger trig) => trig.boolean;
    public static implicit operator trigger(bool boolean) => new trigger { boolean = boolean };
}

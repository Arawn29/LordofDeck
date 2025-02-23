using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class SerializedDictioanry<Type1, Type2> : ISerializationCallbackReceiver
{
   
   [SerializeField] private SerializedDictionaryItems<Type1,Type2>[] DictioanaryItems = new SerializedDictionaryItems<Type1, Type2>[4];
   public Dictionary<Type1, Type2> Dictionary = new Dictionary<Type1, Type2>();


    public void OnBeforeSerialize()
    {
     
    }

    public void OnAfterDeserialize()
    {
        foreach (var item in DictioanaryItems)
        {
            Dictionary.Add(item._Tkey, item._Tvalue);
        }
    }
}

[Serializable]
public class SerializedDictionaryItems<Tkey, Tvalue>
{
    [SerializeField]
    public Tkey _Tkey;
    [SerializeField]
    public Tvalue _Tvalue;

 
}
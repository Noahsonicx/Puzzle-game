using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetNotInDictionaryException : Exception
{
    public AssetNotInDictionaryException(string message) : base(message)
    {

    }
}

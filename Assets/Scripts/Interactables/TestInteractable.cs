using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : Interactable
{
    string objectName;

    public override void Awake()
    {
        base.Awake();
        objectName = name;
    }

    public override void OnInteract()
    {
        print($"interacting with, {objectName}");
    }

    public override void OnFocus()
    {
        print($"focused on, {objectName}");
    }

    public override void OnUnfocus()
    {
        print($"unfocused on, {objectName}");
    }
}

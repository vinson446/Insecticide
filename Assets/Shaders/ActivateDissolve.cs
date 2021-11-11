using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDissolve : MonoBehaviour
{
    Material _render = null;

    // Start is called before the first frame update
    void Start()
    {
        _render = gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(DieSequence());
        }
    }

    IEnumerator DieSequence()
    {
        float _alphaClipping = _render.GetFloat("_AlpClipping");

        while(_alphaClipping <= 1)
        {
            yield return new WaitForEndOfFrame();
            _render.SetFloat("_AlpClipping", _alphaClipping);
            _alphaClipping += .005f;
        }
    }
}

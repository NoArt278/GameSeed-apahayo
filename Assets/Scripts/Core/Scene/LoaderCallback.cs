using System.Collections;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool _isFirstUpdate = true;
    private void Update()
    {
        if (_isFirstUpdate)
        {
            _isFirstUpdate = false;
            StartCoroutine(Callback());
        }
    }

    private IEnumerator Callback() {
        SceneLoader.LoaderCallback();
        yield return new WaitForSeconds(2f);
        SceneLoader.Instance.AllowSceneActivation();
    }
}
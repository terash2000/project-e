using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private GameObject _progress;
    private Vector3 _progressLocalScale;
    void Start()
    {
        _progressLocalScale = _progress.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        _progressLocalScale.x = SceneChanger.Instance.SceneLoading.progress;
        _progress.transform.localScale = _progressLocalScale;
    }
}

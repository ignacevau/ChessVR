﻿using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class DictationScript : MonoBehaviour
{
    private DictationRecognizer m_DictationRecognizer;

    void Start()
    {
        //m_DictationRecognizer = new DictationRecognizer();

        //m_DictationRecognizer.DictationResult += (text, confidence) =>
        //{
        //    Debug.LogFormat("Dictation result: {0}", text);
        //};

        //m_DictationRecognizer.DictationHypothesis += (text) =>
        //{
        //    Debug.LogFormat("Dictation hypothesis: {0}", text);
        //};

        //m_DictationRecognizer.DictationComplete += (completionCause) =>
        //{
        //    if (completionCause != DictationCompletionCause.Complete)
        //        Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        //};

        //m_DictationRecognizer.DictationError += (error, hresult) =>
        //{
        //    Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        //};

        //m_DictationRecognizer.Start();
    }
}
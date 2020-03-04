using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System;

public class MenuController : MonoBehaviour {

    public InputField _inputField = null;
    public InputField _outputField = null;
    public MidiGenerator generator = null;
    public Text feedbackText = null;

    [HideInInspector] public static string _inputPath = "";
    [HideInInspector] public static string _outputPath = "";

    ExtensionFilter[] midiExtensions = new ExtensionFilter[] {
            new ExtensionFilter("Midi Files", "mid"),
            new ExtensionFilter("All Files", "*" ),
    };

    public void Start() {
        InitPaths();
    }

    private void InitPaths() {
        _inputPath = PlayerPrefs.GetString("input");
        _outputPath = PlayerPrefs.GetString("output");
        _inputField.text = _inputPath;
        _outputField.text = _outputPath;
    }

    public void BrowseInput() {       
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Input File", _inputPath, midiExtensions, false);
        if (paths.Length == 0) return;
        _inputPath = paths[0];
        _inputField.text = _inputPath;
        PlayerPrefs.SetString("input", _inputPath);
        feedbackText.text = "Input Set!";
    }

    public void BrowseOutput() {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Open Output Folder", _outputPath, false);
        if (paths.Length == 0) return;
        _outputPath = paths[0];
        _outputField.text = _outputPath;
        PlayerPrefs.SetString("output", _outputPath);
        feedbackText.text = "Output Set!";
    }

    public void Generate() {
        generator.GenerateMidi(_inputPath, _outputPath);
        feedbackText.text = "Generated Midi!";
    }

}

﻿/*
 * Copyright(c) 2017-2018 Sketchfab Inc.
 * License: https://github.com/sketchfab/UnityGLTF/blob/master/LICENSE
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGLTF;

namespace Sketchfab
{
    public class SketchfabModelWindow : EditorWindow
    {
        private bool _addToCurrentScene;
        private SketchfabModel _currentModel;
        private string _importDirectory;
        private byte[] _lastArchive;
        private SketchfabRequest _modelRequest;

        private string _prefabName;

        private Vector2 _scrollView;
        private SketchfabUI _ui;
        private SketchfabBrowser _window;

        private bool show;

        private void OnDestroy()
        {
            if (_window != null)
                _window.closeModelPage();
        }

        private void OnGUI()
        {
            if (_currentModel != null && show)
            {
                _scrollView = GUILayout.BeginScrollView(_scrollView);
                var model = _currentModel;

                // Model name, author, view On Sketchfab bloc
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    {
                        // Name
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(model.name, _ui.getSketchfabModelName());
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        // Author
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(model.author, _ui.getSketchfabContentLabel());
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        // View on Sketchfab
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        var viewSkfb = new GUIContent("View on Sketchfab", _ui.SKETCHFAB_ICON);
                        if (GUILayout.Button(viewSkfb, GUILayout.Height(24), GUILayout.Width(140)))
                            Application.OpenURL(SketchfabPlugin.Urls.modelUrl + "/" + _currentModel.uid);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                // Model preview
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(4);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(model._preview);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                // Import settings
                GUILayout.BeginHorizontal();
                {
                    displayImportSettings();
                }
                GUILayout.EndHorizontal();


                // Model info title
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    _ui.displayTitle("ABOUT THE MODEL");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                // Model info data
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        if (model.licenseJson != null && model.licenseJson["label"] != null)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginVertical();

                                // License label
                                GUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                GUILayout.Label(model.licenseJson["label"], EditorStyles.boldLabel);
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();

                                // License detail
                                GUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                GUILayout.Label(model.formattedLicenseRequirements, EditorStyles.miniLabel);
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();

                                GUILayout.FlexibleSpace();
                                GUILayout.EndVertical();

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                        }

                        else if (model.vertexCount != 0)
                        {
                            _ui.displayContent("Personal");
                            _ui.displaySubContent("You own this model");
                        }
                        else
                        {
                            _ui.displaySubContent("Fetching license data");
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        _ui.displayModelStats(" Vertex count", " " + Utils.humanifySize(model.vertexCount));
                        _ui.displayModelStats(" Face count", " " + Utils.humanifySize(model.faceCount));
                        if (model.hasAnimation != "")
                            _ui.displayModelStats(" Animation", model.hasAnimation);

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(20);
                }
                GUILayout.EndHorizontal();

                GUILayout.EndScrollView();
            }
        }

        private static void Init()
        {
            var window = (SketchfabModelWindow) GetWindow(typeof(SketchfabModelWindow));
            window.titleContent.text = "Model";
            window.Show();
        }

        public void displayModelPage(SketchfabModel model, SketchfabBrowser browser)
        {
            _window = browser;
            if (_currentModel == null || model.uid != _currentModel.uid)
            {
                _currentModel = model;
                _prefabName = GLTFUtils.cleanName(_currentModel.name).Replace(" ", "_");
                _importDirectory = Application.dataPath + "/Import/" + _prefabName.Replace(" ", "_");
            }
            else
            {
                _currentModel = model;
            }

            _ui = SketchfabPlugin.getUI();
            show = true;
        }

        private void onChangImportDirectoryClick()
        {
            var newImportDir = EditorUtility.OpenFolderPanel("Choose import directory", Application.dataPath, "");
            if (GLTFUtils.isFolderInProjectDirectory(newImportDir))
                _importDirectory = newImportDir;
            else if (newImportDir != "")
                EditorUtility.DisplayDialog("Error",
                    "Please select a path within your current Unity project (with Assets/)", "Ok");
        }

        private void onImportModelClick()
        {
            if (!assetAlreadyExists() || EditorUtility.DisplayDialog("Override asset",
                    "The asset " + _prefabName + " already exists in project. Do you want to override it ?", "Override",
                    "Cancel"))
            {
                // Reuse if still valid
                if (_currentModel.tempDownloadUrl.Length > 0 &&
                    EditorApplication.timeSinceStartup - _currentModel.downloadRequestTime <
                    _currentModel.urlValidityDuration)
                    requestArchive(_currentModel.tempDownloadUrl);
                else
                    fetchGLTFModel(_currentModel.uid, OnArchiveUpdate, _window._logger.getHeader());
            }
        }

        private void displayImportButton(bool isUserLoggedIn, bool modelIsAvailable)
        {
            string buttonText;
            if (!isUserLoggedIn)
            {
                buttonText = "You need to log in to download models";
                GUI.enabled = false;
            }
            else if (modelIsAvailable)
            {
                buttonText = "Download model";
                if (_currentModel.archiveSize > 0)
                    buttonText += " (" + Utils.humanifyFileSize(_currentModel.archiveSize) + ")";
            }
            else
            {
                buttonText = "Model not yet available";
            }

            var old = GUI.color;
            GUI.color = SketchfabUI.SKFB_BLUE;
            GUILayout.FlexibleSpace();

            var htmlCaption = "<color=" + Color.white + ">" + buttonText + "</color>";
            if (GUILayout.Button(htmlCaption, _ui.getSketchfabBigButton(), GUILayout.Height(64), GUILayout.Width(450)))
                onImportModelClick();

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.color = old;
            GUI.enabled = true;
        }

        private void displayImportSettings()
        {
            var modelIsAvailable = _currentModel.isModelAvailable;
            var isUserLoggedIn = _window._logger.isUserLogged();
            GUI.enabled = modelIsAvailable;

            GUILayout.BeginVertical("Box");
            {
                // Import options title
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                _ui.displayTitle("IMPORT OPTIONS");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                // Import directory in project
                GUILayout.BeginHorizontal();
                {
                    _ui.displayModelStats("Import into      ", GLTFUtils.getPathProjectFromAbsolute(_importDirectory));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Change", GUILayout.Width(80), GUILayout.Height(18)))
                        onChangImportDirectoryClick();
                }
                GUILayout.EndHorizontal();

                // random space
                GUILayout.Space(2);

                // Prefab name
                GUILayout.BeginHorizontal();
                GUILayout.Label("Prefab name  ", _ui.getKeyStyle());
                _prefabName = GUILayout.TextField(_prefabName, GUILayout.MaxWidth(300));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                // random space
                GUILayout.Space(10);

                // Big import button
                GUILayout.BeginHorizontal();
                {
                    displayImportButton(isUserLoggedIn, modelIsAvailable);
                }
                GUILayout.EndHorizontal();

                // random space
                GUILayout.Space(3);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    _addToCurrentScene = GUILayout.Toggle(_addToCurrentScene, "Instanciate prefab into current scene");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                // random final space
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private bool assetAlreadyExists()
        {
            var prefabPath = _importDirectory + "/" + _prefabName + ".prefab";
            return File.Exists(prefabPath);
        }

        private void OnArchiveUpdate()
        {
            EditorUtility.ClearProgressBar();
            var _unzipDirectory = Application.temporaryCachePath + "/unzip";
            _window._browserManager.setImportProgressCallback(UpdateProgress);
            _window._browserManager.setImportFinishCallback(OnFinishImport);
            _window._browserManager.importArchive(_lastArchive, _unzipDirectory, _importDirectory, _prefabName,
                _addToCurrentScene);
        }

        private void handleDownloadCallback(float current)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Download", "Downloading model archive ", current))
            {
                if (_modelRequest != null)
                {
                    _window._browserManager._api.dropRequest(ref _modelRequest);
                    _modelRequest = null;
                }

                clearProgress();
            }
        }

        private void clearProgress()
        {
            EditorUtility.ClearProgressBar();
        }

        private void OnFinishImport()
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Import successful",
                "Model \n" + _currentModel.name + " by " + _currentModel.author + " has been successfully imported",
                "OK");
        }

        public void fetchGLTFModel(string uid, RefreshCallback fetchedCallback, Dictionary<string, string> headers)
        {
            var url = SketchfabPlugin.Urls.modelEndPoint + "/" + uid + "/download";
            _modelRequest = new SketchfabRequest(url, headers);
            _modelRequest.setCallback(handleDownloadAPIResponse);
            _window._browserManager._api.registerRequest(_modelRequest);
        }

        private void handleArchive(byte[] data)
        {
            _lastArchive = data;
            OnArchiveUpdate();
        }


        private void handleDownloadAPIResponse(string response)
        {
            var responseJson = Utils.JSONParse(response);
            if (responseJson["gltf"] != null)
            {
                _currentModel.tempDownloadUrl = responseJson["gltf"]["url"];
                _currentModel.urlValidityDuration = responseJson["gltf"]["expires"].AsInt;
                _currentModel.downloadRequestTime = EditorApplication.timeSinceStartup;
                requestArchive(_currentModel.tempDownloadUrl);
            }
            else
            {
                Debug.Log("Unexpected Error: Model archive is not available");
            }

            Repaint();
        }

        private void requestArchive(string modelUrl)
        {
            var request = new SketchfabRequest(_currentModel.tempDownloadUrl);
            request.setCallback(handleArchive);
            request.setProgressCallback(handleDownloadCallback);
            SketchfabPlugin.getAPI().registerRequest(request);
        }

        public void UpdateProgress(GLTFEditorImporter.IMPORT_STEP step, int current, int total)
        {
            var element = "";
            switch (step)
            {
                case GLTFEditorImporter.IMPORT_STEP.BUFFER:
                    element = "Buffer";
                    break;
                case GLTFEditorImporter.IMPORT_STEP.IMAGE:
                    element = "Image";
                    break;
                case GLTFEditorImporter.IMPORT_STEP.TEXTURE:
                    element = "Texture";
                    break;
                case GLTFEditorImporter.IMPORT_STEP.MATERIAL:
                    element = "Material";
                    break;
                case GLTFEditorImporter.IMPORT_STEP.MESH:
                    element = "Mesh";
                    break;
                case GLTFEditorImporter.IMPORT_STEP.NODE:
                    element = "Node";
                    break;
                case GLTFEditorImporter.IMPORT_STEP.ANIMATION:
                    element = "Animation";
                    break;
                case GLTFEditorImporter.IMPORT_STEP.SKIN:
                    element = "Skin";
                    break;
            }

            EditorUtility.DisplayProgressBar("Importing glTF",
                "Importing " + element + " (" + current + " / " + total + ")", current / (float) total);
            Repaint();
        }
    }
}

#endif
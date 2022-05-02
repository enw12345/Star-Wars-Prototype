/*
 * Copyright(c) 2017-2018 Sketchfab Inc.
 * License: https://github.com/sketchfab/UnityGLTF/blob/master/LICENSE
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityGLTF;

namespace Sketchfab
{
    public class SketchfabBrowser : EditorWindow
    {
        public Texture2D _defaultThumbnail;
        private bool _animated;

        // Sketchfab elements
        public SketchfabBrowserManager _browserManager;

        // Upload params and options
        private string[] _categoriesNames;
        private int _categoryIndex;
        private string _categoryName = "";

        // Exporter UI: dynamic elements
        private string _currentUid = "";
        private string[] _license;
        private int _licenseIndex;
        public SketchfabLogger _logger;
        private string[] _polyCount;
        private int _polyCountIndex;
        private string _query = "";
        private Vector2 _scrollView;
        private string[] _searchIn;
        private SEARCH_IN _searchInIndex = SEARCH_IN.ALL_FREE_DOWNLOADABLE;
        private SketchfabModelWindow _skfbWin;

        // Search parameters
        private string[] _sortBy;
        private int _sortByIndex;
        private bool _staffpicked = true;

        private readonly int _thumbnailSize = 128;
        public SketchfabUI _ui;

        private float framesSinceLastSearch;
        private readonly float nbFrameSearchCooldown = 30.0f;

        private void Update()
        {
            if (_browserManager != null)
            {
                _browserManager.Update();
                if (_categoriesNames.Length == 0 && _browserManager.getCategories().Count > 0)
                {
                    _categoriesNames = _browserManager.getCategories().ToArray();
                    Repaint();
                }

                if (_browserManager.hasResults() /* && _browserManager.getResults()[0]._preview == null*/)
                {
                    _browserManager.fetchModelPreview();
                    Repaint();
                }

                framesSinceLastSearch++;
            }
        }

        private void OnEnable()
        {
            SketchfabPlugin.Initialize();
            _searchInIndex = SEARCH_IN.ALL_FREE_DOWNLOADABLE;
        }

        private void OnDestroy()
        {
            if (_skfbWin != null)
                _skfbWin.Close();
        }

        // UI
        private void OnGUI()
        {
            checkValidity();
            SketchfabPlugin.displayHeader();

            if (_currentUid.Length > 0) displaySeparatedModelPage();

            displaySearchOptions();
            displayNextPrev();
            _scrollView = GUILayout.BeginScrollView(_scrollView);
            displayResults();
            GUILayout.EndScrollView();

            if (_searchInIndex == SEARCH_IN.MY_STORE_PURCHASES && !_browserManager.hasResults())
            {
                if (_query.Length > 0)
                    displayCenteredMessage("There is no result for '" + _query + "' in your purchases.");
                else
                    displayCenteredMessage("It looks like you didn't do any purchase yet on Sketchfab Store");
            }

            if (_searchInIndex == SEARCH_IN.MY_MODELS && _logger.isUserLogged() && !_logger.canAccessOwnModels())
            {
                if (_query.Length > 0)
                    displayCenteredMessage("There is no result for '" + _query + "' in your .");
                else
                    displayCenteredMessage(
                        "It look like you don't have any model or your plan doesn't allow you to access them");

                displayFooter();
            }

            SketchfabPlugin.displayFooter();
        }

        [MenuItem("Sketchfab/Browse Sketchfab")]
        private static void Init()
        {
            var window = (SketchfabBrowser) GetWindow(typeof(SketchfabBrowser));
            window.titleContent.image = Resources.Load<Texture>("icon");
            window.titleContent.image.filterMode = FilterMode.Bilinear;
            window.titleContent.text = "Browse";
            window.Show();
        }

        private void checkValidity()
        {
            if (_browserManager == null)
            {
                _browserManager = new SketchfabBrowserManager(OnRefreshUpdate);
                resetFilters();
                _currentUid = "";
                _categoryName = "";
                _categoriesNames = new string[0];

                // Setup sortBy
                _sortBy = new[] {"Relevance", "Likes", "Views", "Recent"};
                _polyCount = new[] {"Any", "Up to 10k", "10k to 50k", "50k to 100k", "100k to 250k", "250k +"};
                _searchIn = new[] {"free downloadable", "my models", "store purchases"};
                _license = new[]
                {
                    "any", "CC BY", "CC BY SA", "CC BY-ND", "CC BY-NC", "CC BY-NC-SA", "CC BY-NC-ND", "CC0"
                }; // No search for store models so only CC licenses here
                Repaint();
                GL.sRGBWrite = true;
            }

            SketchfabPlugin.checkValidity();
            _ui = SketchfabPlugin.getUI();
            _logger = SketchfabPlugin.getLogger();
        }

        private void resizeWindow(int width, int height)
        {
            var size = minSize;
            minSize = new Vector2(width, height);
            Repaint();
            minSize = size;
        }

        private void triggerSearch()
        {
            if (framesSinceLastSearch < nbFrameSearchCooldown)
                return;

            if (_skfbWin != null)
                _skfbWin.Close();

            string licenseSmug;
            switch (_licenseIndex)
            {
                case 0:
                    licenseSmug = "";
                    break;
                case 1:
                    licenseSmug = "by";
                    break;
                case 2:
                    licenseSmug = "by-sa";
                    break;
                case 3:
                    licenseSmug = "by-nd";
                    break;
                case 4:
                    licenseSmug = "by-nc";
                    break;
                case 5:
                    licenseSmug = "by-nc-sa";
                    break;
                case 6:
                    licenseSmug = "by-nc-nd";
                    break;
                case 7:
                    licenseSmug = "cc0";
                    break;
                default:
                    licenseSmug = "";
                    break;
            }

            SORT_BY sort;
            switch (_sortByIndex)
            {
                case 0:
                    sort = SORT_BY.RELEVANCE;
                    break;
                case 1:
                    sort = SORT_BY.LIKES;
                    break;
                case 2:
                    sort = SORT_BY.VIEWS;
                    break;
                case 3:
                    sort = SORT_BY.RECENT;
                    break;
                default:
                    sort = SORT_BY.RELEVANCE;
                    break;
            }

            // Point clouds are not supported in Unity so check that polycount is not 0
            // here. It won't prevent model that are parially point clouds but it's better
            // than nothing
            var _minFaceCount = "1";
            var _maxFaceCount = "";
            switch (_polyCountIndex)
            {
                case 0:
                    break;
                case 1:
                    _maxFaceCount = "10000";
                    break;
                case 2:
                    _minFaceCount = "10000";
                    _maxFaceCount = "50000";
                    break;
                case 3:
                    _minFaceCount = "50000";
                    _maxFaceCount = "100000";
                    break;
                case 4:
                    _minFaceCount = "100000";
                    _maxFaceCount = "250000";
                    break;
                case 5:
                    _minFaceCount = "250000";
                    break;
            }

            var endpoint = SEARCH_ENDPOINT.DOWNLOADABLE;
            switch (_searchInIndex)
            {
                case SEARCH_IN.MY_MODELS:
                    endpoint = SEARCH_ENDPOINT.MY_MODELS;
                    break;
                case SEARCH_IN.MY_STORE_PURCHASES:
                    endpoint = SEARCH_ENDPOINT.STORE_PURCHASES;
                    break;
                default:
                    endpoint = SEARCH_ENDPOINT.DOWNLOADABLE;
                    break;
            }

            _browserManager.search(_query, _staffpicked, _animated, _categoryName, licenseSmug, _maxFaceCount,
                _minFaceCount, endpoint, sort);
            framesSinceLastSearch = 0.0f;
        }

        private void resetFilters()
        {
            _licenseIndex = 0;
            _categoryIndex = 0;
            _sortByIndex = 3;
            _polyCountIndex = 0;

            _query = "";
            _animated = false;
            _staffpicked = true;
            _categoryName = "All";
        }

        private void resetFilersOwnModels()
        {
            _licenseIndex = 0;
            _categoryIndex = 0;
            _sortByIndex = 3;
            _polyCountIndex = 0;

            _query = "";
            _animated = false;
            _staffpicked = false;
            _categoryName = "All";
        }

        private void displaySearchOptions()
        {
            // Query
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                displaySearchBox();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            {
                GUI.enabled = _searchInIndex != SEARCH_IN.MY_STORE_PURCHASES;
                displayCategories();

                displayFeatures();
                displayMaxFacesCount();
                GUILayout.FlexibleSpace();
                displaySortBy();
            }
            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        private void displaySearchIn()
        {
            var old = (int) _searchInIndex;
            _searchInIndex = (SEARCH_IN) EditorGUILayout.Popup((int) _searchInIndex, _searchIn, GUILayout.Width(130));
            if ((int) _searchInIndex != old)
            {
                if (_searchInIndex != SEARCH_IN.ALL_FREE_DOWNLOADABLE)
                    resetFilersOwnModels();
                else
                    resetFilters();
                triggerSearch();
            }
        }

        private void displayLicenseFilter()
        {
            var old = _licenseIndex;
            _licenseIndex = EditorGUILayout.Popup(_licenseIndex, _license, GUILayout.Width(130));
            if (_licenseIndex != old) triggerSearch();
        }

        private void displaySearchBox()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Search in:", GUILayout.Width(60));
                GUILayout.EndVertical();

                // Disable choice if user is not logged in
                var isEnabled = GUI.enabled;
                GUI.enabled = _logger.isUserLogged();
                GUILayout.BeginVertical();
                displaySearchIn();
                GUILayout.EndVertical();
                GUI.enabled = isEnabled;

                GUILayout.BeginVertical();
                GUI.SetNextControlName("SearchTextField");
                _query = EditorGUILayout.TextField(_query);
                GUILayout.EndVertical();

                // Trigger search on RETURN key
                if (Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "SearchTextField")
                    triggerSearch();

                // License filter is not available for store purchases
                GUI.enabled = _searchInIndex != SEARCH_IN.MY_STORE_PURCHASES;
                GUILayout.Label("with license");
                displayLicenseFilter();
                GUI.enabled = true;

                // Search button
                if (GUILayout.Button("Search", GUILayout.Width(120))) triggerSearch();
            }

            GUILayout.EndHorizontal();
        }

        private void displayCategories()
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(240));
            {
                GUILayout.Space(1);
                GUILayout.BeginHorizontal();
                if (_categoriesNames.Length > 0)
                {
                    GUILayout.Label("Categories");
                    var prev = _categoryIndex;
                    _categoryIndex = EditorGUILayout.Popup(_categoryIndex, _categoriesNames, GUILayout.MaxWidth(168));
                    _categoryName = _categoriesNames[_categoryIndex];
                    if (_categoryIndex != prev)
                        triggerSearch();
                }
                else
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Fetching categories");
                    _categoryName = "";
                    GUILayout.FlexibleSpace();
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void displayFeatures()
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(180));
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5);

                    var previous = _animated;
                    _animated = GUILayout.Toggle(_animated, "Animated");
                    if (_animated != previous)
                        triggerSearch();
                    previous = _staffpicked;
                    _staffpicked = GUILayout.Toggle(_staffpicked, "Staff Picked");
                    if (_staffpicked != previous)
                        triggerSearch();

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void displayMaxFacesCount()
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(120));
            {
                GUILayout.Space(1);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5);

                    GUILayout.Label("Max face count: ");
                    var old = _polyCountIndex;
                    _polyCountIndex = EditorGUILayout.Popup(_polyCountIndex, _polyCount, GUILayout.Width(100));
                    if (_polyCountIndex != old)
                        triggerSearch();

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void displaySortBy()
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(240));
            {
                GUILayout.Space(1);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Sort by");
                    var old = _sortByIndex;
                    _sortByIndex = EditorGUILayout.Popup(_sortByIndex, _sortBy, GUILayout.Width(80));
                    if (_sortByIndex != old)
                        triggerSearch();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void displayNextPrev()
        {
            GUILayout.BeginHorizontal();
            {
                if (_browserManager.hasPreviousResults())
                    if (GUILayout.Button("Previous"))
                    {
                        closeModelWindow();
                        _browserManager.requestPreviousResults();
                    }

                GUILayout.FlexibleSpace();
                if (_browserManager.hasNextResults())
                    if (GUILayout.Button("Next"))
                    {
                        closeModelWindow();
                        _browserManager.requestNextResults();
                    }
            }
            GUILayout.EndHorizontal();
        }

        private void displayFooter()
        {
            GUILayout.BeginVertical(GUILayout.Height(75));
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    var old = GUI.color;
                    var whitebackground = new GUIStyle(GUI.skin.button);
                    whitebackground.richText = true;

                    GUILayout.Label("<b>Gain full API access</b> to your personal library of 3D models",
                        SketchfabPlugin.getUI().getSketchfabBigLabel(), GUILayout.Height(48));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Upgrade to PRO", GUILayout.Height(48), GUILayout.Width(225)))
                        Application.OpenURL(SketchfabPlugin.Urls.plans);
                    GUI.color = old;
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
        }

        private void displayCenteredMessage(string message)
        {
            GUILayout.BeginVertical();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(message);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void displayResults()
        {
            var count = 0;
            var buttonLineLength = Mathf.Max(1, Mathf.Min((int) position.width / _thumbnailSize, 6));
            var needClose = false;
            var models = _browserManager.getResults();

            if (models != null && models.Count > 0) // Replace by "is ready"
                foreach (SketchfabModel model in models.Values)
                {
                    if (count % buttonLineLength == 0)
                    {
                        GUILayout.BeginHorizontal();
                        needClose = true;
                    }

                    GUILayout.FlexibleSpace();
                    displayResult(model);
                    GUILayout.FlexibleSpace();

                    if (count % buttonLineLength == buttonLineLength - 1)
                    {
                        GUILayout.EndHorizontal();
                        needClose = false;
                    }

                    count++;
                }
            else if (_browserManager._isFetching) displayCenteredMessage("Fetching models ....");

            if (needClose) GUILayout.EndHorizontal();
        }

        private void displayResult(SketchfabModel model)
        {
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button(new GUIContent(model._thumbnail), GUI.skin.label,
                        GUILayout.MaxHeight(_thumbnailSize), GUILayout.MaxWidth(_thumbnailSize)))
                {
                    _currentUid = model.uid;
                    _browserManager.fetchModelInfo(_currentUid);
                    if (_skfbWin != null)
                        _skfbWin.Focus();
                }

                GUILayout.BeginVertical(GUILayout.Width(_thumbnailSize), GUILayout.Height(50));
                GUILayout.Label(model.name, _ui.getSketchfabMiniModelName());
                GUILayout.Label("by " + model.author, _ui.getSketchfabMiniAuthorName());
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        // Model page
        private void displaySeparatedModelPage()
        {
            if (_skfbWin == null)
            {
                _skfbWin = CreateInstance<SketchfabModelWindow>();
                _skfbWin.displayModelPage(_browserManager.getModel(_currentUid), this);
                _skfbWin.position = new Rect(position.position, new Vector2(530, 660));
                _skfbWin.titleContent.text = "Model details";
                _skfbWin.Show();
                _skfbWin.Repaint();
            }

            _skfbWin.displayModelPage(_browserManager.getModel(_currentUid), this);
            _skfbWin.Show();
            _skfbWin.Repaint();
        }

        public void closeModelPage()
        {
            _currentUid = "";
        }

        private void closeModelWindow()
        {
            if (_skfbWin != null)
                _skfbWin.Close();
        }

        // Callbacks
        private void OnRefreshUpdate()
        {
            Repaint();
        }

        private void OnFinishImport()
        {
            var model = _browserManager.getModel(_currentUid);
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Import successful",
                "Model \n" + model.name + " by " + model.author + " has been successfully imported", "OK");
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

        private enum SEARCH_IN
        {
            ALL_FREE_DOWNLOADABLE = 0,
            MY_MODELS = 1,
            MY_STORE_PURCHASES = 2
        }
    }
}

#endif
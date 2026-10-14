using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class SetupPreMadeUI {

    static SetupPreMadeUI() {
        EditorApplication.delayCall += () => CheckAndBuildUI();
        EditorSceneManager.sceneOpened += (scene, mode) => {
            if (scene.name == "GameScene") CheckAndBuildUI();
        };
    }

    [MenuItem("Tools/Create Pre-Made UI In Scene")]
    public static void CreateAllUI() {
        if (EditorSceneManager.GetActiveScene().name != "GameScene") {
            EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        }
        BuildUIInActiveScene(true);
    }

    private static void CheckAndBuildUI() {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name == "GameScene") {
            BuildUIInActiveScene(false);
        }
    }

    private static void BuildUIInActiveScene(bool forceRebuild) {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        Transform canvasTr = canvas.transform;

        // Check if already created
        Transform existingDPS = canvasTr.Find("DPSBar");
        if (existingDPS != null && !forceRebuild) return;

        // Remove previous custom objects if force rebuild
        string[] toRemove = new string[] { "DPSBar", "SettingsButton", "PrestigeButton", "AutoShootBoostButton", "SettingsModal", "PrestigeModal" };
        foreach (var name in toRemove) {
            var child = canvasTr.Find(name);
            if (child != null) Object.DestroyImmediate(child.gameObject);
        }

        Font font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/LilitaOne-Regular.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Sprite winSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/Window.png");
        Sprite btnSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/Button.png");
        Sprite greenBtnSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/Green_Button.png");
        Sprite yellowBtnSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/Yellow_Button.png");

        // 1. DPSBar (Anchored top center below level bar)
        GameObject dpsBar = new GameObject("DPSBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline), typeof(DPSDisplay));
        dpsBar.transform.SetParent(canvasTr, false);
        var dpsRT = dpsBar.GetComponent<RectTransform>();
        dpsRT.anchorMin = new Vector2(0.5f, 1f);
        dpsRT.anchorMax = new Vector2(0.5f, 1f);
        dpsRT.pivot = new Vector2(0.5f, 1f);
        dpsRT.anchoredPosition = new Vector2(0f, -145f);
        dpsRT.sizeDelta = new Vector2(380f, 44f);

        var dpsImg = dpsBar.GetComponent<Image>();
        dpsImg.color = new Color(0.06f, 0.08f, 0.14f, 0.85f);

        var dpsOutline = dpsBar.GetComponent<Outline>();
        dpsOutline.effectColor = new Color(0.18f, 0.28f, 0.48f, 0.8f);
        dpsOutline.effectDistance = new Vector2(1.5f, -1.5f);

        var dpsTextObj = CreateTextObj(dpsBar.transform, "DPSText", "⚡ DPS: 0", font, 18, Color.cyan, new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(95f, 0f), new Vector2(170f, 38f));
        var dmgTextObj = CreateTextObj(dpsBar.transform, "TotalDamageText", "💥 DMG: 0", font, 18, new Color(1f, 0.85f, 0.2f), new Vector2(0.5f, 0f), new Vector2(1f, 1f), new Vector2(-95f, 0f), new Vector2(170f, 38f));

        var dpsDisplay = dpsBar.GetComponent<DPSDisplay>();
        dpsDisplay.dpsText = dpsTextObj.GetComponent<Text>();
        dpsDisplay.totalDamageText = dmgTextObj.GetComponent<Text>();

        // 2. SettingsButton (Anchored top right)
        GameObject settingsBtnObj = CreateButtonObj(canvasTr, "SettingsButton", btnSprite, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -320f), new Vector2(75f, 75f));
        CreateTextObj(settingsBtnObj.transform, "Text", "⚙", font, 36, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(70f, 70f));

        // 3. PrestigeButton (Anchored top right, below settings)
        GameObject prestigeBtnObj = CreateButtonObj(canvasTr, "PrestigeButton", btnSprite, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -405f), new Vector2(75f, 75f));
        var pTxt = CreateTextObj(prestigeBtnObj.transform, "Text", "🌌\nREBIRTH", font, 13, new Color(1f, 0.9f, 0.3f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(70f, 70f));
        pTxt.GetComponent<Text>().lineSpacing = 0.9f;

        // 4. AutoShootBoostButton (Anchored bottom right)
        GameObject boostBtnObj = CreateButtonObj(canvasTr, "AutoShootBoostButton", yellowBtnSprite != null ? yellowBtnSprite : btnSprite, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-55f, 60f), new Vector2(115f, 95f));
        var bTxt = CreateTextObj(boostBtnObj.transform, "Text", "🚀 FREE\nAUTO-FIRE\n(AD)", font, 14, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(110f, 85f));
        bTxt.GetComponent<Text>().lineSpacing = 0.9f;
        var boostComp = boostBtnObj.AddComponent<RewardedBoostButton>();
        boostComp.buttonLabel = bTxt.GetComponent<Text>();

        // 5. SettingsModal
        GameObject settingsModal = CreateModalBackdrop(canvasTr, "SettingsModal");
        var settingsMenu = settingsModal.AddComponent<SettingsMenu>();
        settingsMenu.settingsPanel = settingsModal;

        GameObject sWin = CreateWindowObj(settingsModal.transform, "Window", winSprite, new Vector2(440f, 480f));
        CreateTextObj(sWin.transform, "Title", "SETTINGS", font, 32, Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -60f), new Vector2(300f, 50f));

        GameObject mBtnObj = CreateButtonObj(sWin.transform, "Btn_MusicToggle", greenBtnSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 50f), new Vector2(280f, 68f));
        var mTxt = CreateTextObj(mBtnObj.transform, "Text", "MUSIC: ON", font, 22, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(270f, 60f));
        settingsMenu.musicToggleButton = mBtnObj.GetComponent<Button>();
        settingsMenu.musicText = mTxt.GetComponent<Text>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(mBtnObj.GetComponent<Button>().onClick, settingsMenu.OnMusicToggleClicked);

        GameObject sfxBtnObj = CreateButtonObj(sWin.transform, "Btn_SFXToggle", greenBtnSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -35f), new Vector2(280f, 68f));
        var sfxTxt = CreateTextObj(sfxBtnObj.transform, "Text", "SFX: ON", font, 22, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(270f, 60f));
        settingsMenu.sfxToggleButton = sfxBtnObj.GetComponent<Button>();
        settingsMenu.sfxText = sfxTxt.GetComponent<Text>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(sfxBtnObj.GetComponent<Button>().onClick, settingsMenu.OnSFXToggleClicked);

        GameObject sCloseBtnObj = CreateButtonObj(sWin.transform, "Btn_Close", yellowBtnSprite != null ? yellowBtnSprite : btnSprite, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 65f), new Vector2(220f, 60f));
        CreateTextObj(sCloseBtnObj.transform, "Text", "CLOSE", font, 22, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(200f, 50f));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(sCloseBtnObj.GetComponent<Button>().onClick, settingsMenu.CloseSettings);

        // Hook SettingsButton click to open settings
        UnityEditor.Events.UnityEventTools.AddPersistentListener(settingsBtnObj.GetComponent<Button>().onClick, settingsMenu.OpenSettings);
        settingsModal.SetActive(false);

        // 6. PrestigeModal
        GameObject prestigeModal = CreateModalBackdrop(canvasTr, "PrestigeModal");
        var prestigeMgr = prestigeModal.AddComponent<PrestigeManager>();
        prestigeMgr.prestigePanel = prestigeModal;
        prestigeMgr.minLevelToPrestige = 3; // Allows testing from level 3!

        GameObject pWin = CreateWindowObj(prestigeModal.transform, "Window", winSprite, new Vector2(460f, 580f));
        CreateTextObj(pWin.transform, "Title", "COSMIC REBIRTH", font, 30, new Color(1f, 0.85f, 0.2f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -50f), new Vector2(360f, 50f));
        var descObj = CreateTextObj(pWin.transform, "Desc", "Reset your planet level and coins\nto receive powerful Cosmic Orbs!\nEach Orb grants +10% permanent damage.", font, 15, new Color(0.9f, 0.9f, 0.9f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -115f), new Vector2(380f, 70f));
        descObj.GetComponent<Text>().lineSpacing = 1.15f;

        var orbsTxtObj = CreateTextObj(pWin.transform, "OrbsText", "COSMIC ORBS: 0", font, 22, Color.cyan, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(360f, 35f));
        prestigeMgr.currentOrbsText = orbsTxtObj.GetComponent<Text>();

        var bonusTxtObj = CreateTextObj(pWin.transform, "BonusText", "PERMANENT DMG: +0%", font, 20, new Color(0.4f, 1f, 0.4f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 25f), new Vector2(360f, 35f));
        prestigeMgr.bonusMultiplierText = bonusTxtObj.GetComponent<Text>();

        // Cosmic Tech Upgrade Button
        GameObject techBtnObj = CreateButtonObj(pWin.transform, "Btn_CosmicPower", yellowBtnSprite != null ? yellowBtnSprite : btnSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(340f, 60f));
        var techTxt = CreateTextObj(techBtnObj.transform, "Text", "COSMIC POWER (LVL 0)\nCost: 2 Orbs (+25% DMG)", font, 16, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(320f, 55f));
        prestigeMgr.cosmicPowerButton = techBtnObj.GetComponent<Button>();
        prestigeMgr.cosmicPowerText = techTxt.GetComponent<Text>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(techBtnObj.GetComponent<Button>().onClick, prestigeMgr.BuyCosmicPower);

        // Rebirth Action Button
        GameObject actBtnObj = CreateButtonObj(pWin.transform, "Btn_Rebirth", greenBtnSprite, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 125f), new Vector2(320f, 65f));
        var actTxt = CreateTextObj(actBtnObj.transform, "Text", "REBIRTH NOW", font, 20, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(300f, 60f));
        prestigeMgr.prestigeButton = actBtnObj.GetComponent<Button>();
        prestigeMgr.orbsToGainText = actTxt.GetComponent<Text>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(actBtnObj.GetComponent<Button>().onClick, prestigeMgr.DoPrestige);

        // Close Button
        GameObject pCloseBtnObj = CreateButtonObj(pWin.transform, "Btn_Close", btnSprite, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 50f), new Vector2(220f, 55f));
        CreateTextObj(pCloseBtnObj.transform, "Text", "CLOSE", font, 20, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(200f, 50f));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(pCloseBtnObj.GetComponent<Button>().onClick, prestigeMgr.ClosePrestigePanel);

        // Hook PrestigeButton click to open prestige modal
        UnityEditor.Events.UnityEventTools.AddPersistentListener(prestigeBtnObj.GetComponent<Button>().onClick, prestigeMgr.OpenPrestigePanel);
        prestigeModal.SetActive(false);

        // Mark scene dirty and save
        EditorUtility.SetDirty(canvas.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        Debug.Log(">>> [SetupPreMadeUI] Pre-Made UI GameObjects successfully created and saved in GameScene.unity! <<<");
    }

    private static GameObject CreateButtonObj(Transform parent, string name, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size) {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        var img = go.GetComponent<Image>();
        if (sprite != null) {
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
        }
        return go;
    }

    private static GameObject CreateTextObj(Transform parent, string name, string text, Font font, int fontSize, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size) {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(Outline));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        var txt = go.GetComponent<Text>();
        txt.text = text;
        txt.font = font;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontStyle = FontStyle.Bold;

        var outline = go.GetComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.95f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return go;
    }

    private static GameObject CreateModalBackdrop(Transform canvas, string name) {
        GameObject backdrop = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        backdrop.transform.SetParent(canvas, false);
        var rt = backdrop.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        var img = backdrop.GetComponent<Image>();
        img.color = new Color(0.04f, 0.05f, 0.08f, 0.85f);
        return backdrop;
    }

    private static GameObject CreateWindowObj(Transform parent, string name, Sprite sprite, Vector2 size) {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;

        var img = go.GetComponent<Image>();
        if (sprite != null) {
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
        } else {
            img.color = new Color(0.12f, 0.15f, 0.25f, 0.98f);
        }
        return go;
    }
}

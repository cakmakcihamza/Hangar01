#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Rota02Setup
{
    [MenuItem("Tools/Rota02/Sahneyi Hazirla")]
    public static void SahneyiHazirla()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("Rota02 setup Edit Mode'da calistirilmalidir.");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "Rota02" && scene.name != "Rota2")
        {
            Debug.LogWarning("Once Rota02 (veya Rota2) sahnesini acin.");
            return;
        }

        GameObject ucak = UcagiBul(scene);
        if (ucak == null)
        {
            Debug.LogError("Aktif sahnede NuD36_LowPoly ucagi bulunamadi.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Rota02 sahnesini hazirla");

        Undo.RecordObject(ucak.transform, "Ucagi konumlandir");
        ucak.transform.position = new Vector3(0f, 20f, 0f);

        GameObject balon = NesneyiBul(scene, "Balon");
        if (balon == null)
            balon = PrimitiveOlustur(scene, "Balon", PrimitiveType.Sphere);
        KokeTasi(balon);
        Undo.RecordObject(balon.transform, "Balonu konumlandir");
        balon.transform.position = new Vector3(40f, 30f, 60f);
        balon.transform.localScale = new Vector3(6f, 6f, 6f);

        GameObject ucaksavar = NesneyiBul(scene, "Uçaksavar");
        if (ucaksavar == null)
        {
            ucaksavar = new GameObject("Uçaksavar");
            SceneManager.MoveGameObjectToScene(ucaksavar, scene);
            Undo.RegisterCreatedObjectUndo(ucaksavar, "Ucaksavar olustur");
        }
        KokeTasi(ucaksavar);
        Undo.RecordObject(ucaksavar.transform, "Ucaksavari konumlandir");
        ucaksavar.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        ucaksavar.transform.localScale = Vector3.one;
        ParcayiHazirla(scene, ucaksavar.transform, "Gövde", PrimitiveType.Cylinder,
            new Vector3(0f, 1f, 0f), new Vector3(3f, 1f, 3f));
        ParcayiHazirla(scene, ucaksavar.transform, "Namlu", PrimitiveType.Cube,
            new Vector3(0f, 2f, 3f), new Vector3(0.35f, 0.35f, 6f));

        HedefeGit hareket = ucak.GetComponent<HedefeGit>();
        if (hareket == null)
            hareket = Undo.AddComponent<HedefeGit>(ucak);
        Undo.RecordObject(hareket, "Ucak hedefini ayarla");
        hareket.hedef = balon.transform;
        hareket.hiz = 14f;
        hareket.donusHizi = 60f;
        hareket.varisMesafesi = 8f;

        Radar radar = ucaksavar.GetComponent<Radar>();
        if (radar == null)
            radar = Undo.AddComponent<Radar>(ucaksavar);
        Undo.RecordObject(radar, "Radar hedefini ayarla");
        radar.hedef = ucak.transform;
        radar.yarimAci = 35f;
        radar.menzil = 120f;

        PrefabUtility.RecordPrefabInstancePropertyModifications(ucak.transform);
        PrefabUtility.RecordPrefabInstancePropertyModifications(hareket);
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);
        Selection.activeGameObject = ucak;
        Debug.Log("Rota02 hazirlandi. Sahne kaydedilmedi; kontrol ettikten sonra elle kaydedin.");
    }

    private static GameObject UcagiBul(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                Object source = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                string path = source == null ? "" : AssetDatabase.GetAssetPath(source);
                if (path == "Assets/NuD36_LowPoly.fbx"
                    || child.name.StartsWith("NuD36_LowPoly", System.StringComparison.Ordinal))
                    return root;
            }
        }
        return null;
    }

    private static GameObject NesneyiBul(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == name)
                    return child.gameObject;
        return null;
    }

    private static void KokeTasi(GameObject obj)
    {
        if (obj.transform.parent != null)
            Undo.SetTransformParent(obj.transform, null, "Nesneyi sahne kokune tasi");
    }

    private static GameObject PrimitiveOlustur(Scene scene, string name, PrimitiveType type)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        SceneManager.MoveGameObjectToScene(obj, scene);
        Undo.RegisterCreatedObjectUndo(obj, name + " olustur");
        return obj;
    }

    private static void ParcayiHazirla(Scene scene, Transform parent, string name,
        PrimitiveType type, Vector3 position, Vector3 scale)
    {
        Transform part = parent.Find(name);
        if (part == null)
        {
            part = PrimitiveOlustur(scene, name, type).transform;
            Undo.SetTransformParent(part, parent, name + " bagla");
        }
        Undo.RecordObject(part, name + " konumlandir");
        part.localPosition = position;
        part.localRotation = Quaternion.identity;
        part.localScale = scale;
    }
}
#endif

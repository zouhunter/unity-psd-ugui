#if UNITY_EDITOR
public class Menu
{
    [UnityEditor.MenuItem("Tools/Psd-Preview")]
    static void CallPreviewWindow()
    {
        UnityEditor.EditorApplication.ExecuteMenuItem("Window/Psd-Preview");
    }
    [UnityEditor.MenuItem("Tools/Psd-Exporter")]
    static void CallCreateExporter()
    {
        UnityEditor.EditorApplication.ExecuteMenuItem("Assets/Create/Psd-Exporter");
    }
}
#endif

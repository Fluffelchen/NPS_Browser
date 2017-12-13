using Microsoft.Win32;
using System.IO;

[System.Serializable]
public class Settings
{
    [System.NonSerialized]
    static string keyName = Path.Combine("HKEY_CURRENT_USER", "SOFTWARE", "NoPayStationBrowser");

    [System.NonSerialized]
    public static Settings Instance;

	// Settings
    public string downloadDir, pkgPath, pkgParams;
	public bool deleteAfterUnpack = false;
	public int simultaneousDl = 2;

	// Game URIs
	public string PS3Uri, PSMUri, PSPUri, GamesUri, PSXUri;

	// DLC URIs
	public string PS3DLCUri, PSPDLCUri, DLCUri;

	// Theme URIs
	public string PS3ThemeUri, PSPThemeUri, ThemeUri;

	// Update URIs
	public string UpdateUri;

	public Settings()
    {
        Instance = this;

		// Settings
		downloadDir = Registry.GetValue(keyName, "downloadDir", "")?.ToString();
		pkgPath = Registry.GetValue(keyName, "pkgPath", "")?.ToString();
		pkgParams = Registry.GetValue(keyName, "pkgParams", null)?.ToString();
		if (pkgParams == null) pkgParams = "-x {pkgFile} \"{zRifKey}\"";
		string deleteAfterUnpackString = Registry.GetValue(keyName, "deleteAfterUnpack", false)?.ToString();
		if (!string.IsNullOrEmpty(deleteAfterUnpackString))
			bool.TryParse(deleteAfterUnpackString, out deleteAfterUnpack);
		else deleteAfterUnpack = true;
		string simultanesulString = Registry.GetValue(keyName, "simultaneousDl", 2)?.ToString();
		if (!string.IsNullOrEmpty(simultanesulString))
			int.TryParse(simultanesulString, out simultaneousDl);
		else simultaneousDl = 2;

		// Game URIs
		PS3Uri = Registry.GetValue(keyName, "PS3Uri", "")?.ToString();
		PSMUri = Registry.GetValue(keyName, "PSMUri", "")?.ToString();
		PSPUri = Registry.GetValue(keyName, "PSPUri", "")?.ToString();
		GamesUri = Registry.GetValue(keyName, "GamesUri", "")?.ToString();
		PSXUri = Registry.GetValue(keyName, "PSXUri", "")?.ToString();

		// DLC URIs
		PS3DLCUri = Registry.GetValue(keyName, "PS3DLCUri", "")?.ToString();
		PSPDLCUri = Registry.GetValue(keyName, "PSPDLCUri", "")?.ToString();
		DLCUri = Registry.GetValue(keyName, "DLCUri", "")?.ToString();

		// Theme URIs
		PS3ThemeUri = Registry.GetValue(keyName, "PS3ThemeUri", "")?.ToString();
		PSPThemeUri = Registry.GetValue(keyName, "PSPThemeUri", "")?.ToString();
		ThemeUri = Registry.GetValue(keyName, "ThemeUri", "")?.ToString();

		// Update URIs
		UpdateUri = Registry.GetValue(keyName, "UpdateUri", "")?.ToString();
	}

    public void Store()
    {
		// Settings
		if (downloadDir != null)
			Registry.SetValue(keyName, "downloadDir", downloadDir);
		if (pkgPath != null)
			Registry.SetValue(keyName, "pkgPath", pkgPath);
		if (pkgParams != null)
			Registry.SetValue(keyName, "pkgParams", pkgParams);
		Registry.SetValue(keyName, "deleteAfterUnpack", deleteAfterUnpack);
		Registry.SetValue(keyName, "simultaneousDl", simultaneousDl);

		// Game URIs
		if (PS3Uri != null)
			Registry.SetValue(keyName, "PS3Uri", PS3Uri);
		if (PSMUri != null)
			Registry.SetValue(keyName, "PSMUri", PSMUri);
		if (PSPUri != null)
			Registry.SetValue(keyName, "PSPUri", PSPUri);
		if (GamesUri != null)
			Registry.SetValue(keyName, "GamesUri", GamesUri);
		if (PSXUri != null)
			Registry.SetValue(keyName, "PSXUri", PSXUri);

		// DLC URIs
		if (PS3DLCUri != null)
			Registry.SetValue(keyName, "PS3DLCUri", PS3DLCUri);
		if (PSPDLCUri != null)
			Registry.SetValue(keyName, "PSPDLCUri", PSPDLCUri);
		if (DLCUri != null)
			Registry.SetValue(keyName, "DLCUri", DLCUri);

		// Theme URIs
		if (PS3ThemeUri != null)
			Registry.SetValue(keyName, "PS3ThemeUri", PS3ThemeUri);
		if (PSPThemeUri != null)
			Registry.SetValue(keyName, "PSPThemeUri", PSPThemeUri);
		if (ThemeUri != null)
			Registry.SetValue(keyName, "ThemeUri", ThemeUri);

		// Update URIs
		if (UpdateUri != null)
			Registry.SetValue(keyName, "UpdateUri", UpdateUri);
    }
}





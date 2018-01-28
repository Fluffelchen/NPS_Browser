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
	public string PSVUri, PSMUri, PSXUri, PSPUri, PS3Uri, PS4Uri;

	// Avatar URIs
	public string PS3AvatarUri;

	// DLC URIs
	public string PSVDLCUri, PSPDLCUri, PS3DLCUri, PS4DLCUri;

	// Theme URIs
	public string PSVThemeUri, PSPThemeUri, PS3ThemeUri, PS4ThemeUri;

	// Update URIs
	public string PSVUpdateUri, PS4UpdateUri;

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
		PSVUri = Registry.GetValue(keyName, "GamesUri", "")?.ToString();
		PSMUri = Registry.GetValue(keyName, "PSMUri", "")?.ToString();
		PSXUri = Registry.GetValue(keyName, "PSXUri", "")?.ToString();
		PSPUri = Registry.GetValue(keyName, "PSPUri", "")?.ToString();
		PS3Uri = Registry.GetValue(keyName, "PS3Uri", "")?.ToString();
		PS4Uri = Registry.GetValue(keyName, "PS4Uri", "")?.ToString();

		// Avatar URIs
		PS3AvatarUri = Registry.GetValue(keyName, "PS3AvatarUri", "")?.ToString();

		// DLC URIs
		PSVDLCUri = Registry.GetValue(keyName, "DLCUri", "")?.ToString();
		PSPDLCUri = Registry.GetValue(keyName, "PSPDLCUri", "")?.ToString();
		PS3DLCUri = Registry.GetValue(keyName, "PS3DLCUri", "")?.ToString();
		PS4DLCUri = Registry.GetValue(keyName, "PS4DLCUri", "")?.ToString();

		// Theme URIs
		PSVThemeUri = Registry.GetValue(keyName, "ThemeUri", "")?.ToString();
		PSPThemeUri = Registry.GetValue(keyName, "PSPThemeUri", "")?.ToString();
		PS3ThemeUri = Registry.GetValue(keyName, "PS3ThemeUri", "")?.ToString();
		PS4ThemeUri = Registry.GetValue(keyName, "PS4ThemeUri", "")?.ToString();

		// Update URIs
		PSVUpdateUri = Registry.GetValue(keyName, "UpdateUri", "")?.ToString();
		PS4UpdateUri = Registry.GetValue(keyName, "PS4UpdateUri", "")?.ToString();
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
		if (PSVUri != null)
			Registry.SetValue(keyName, "GamesUri", PSVUri);
		if (PSMUri != null)
			Registry.SetValue(keyName, "PSMUri", PSMUri);
		if (PSXUri != null)
			Registry.SetValue(keyName, "PSXUri", PSXUri);
		if (PSPUri != null)
			Registry.SetValue(keyName, "PSPUri", PSPUri);
		if (PS3Uri != null)
			Registry.SetValue(keyName, "PS3Uri", PS3Uri);
		if (PS4Uri != null)
			Registry.SetValue(keyName, "PS4Uri", PS4Uri);

		// Avatar URIs
		if (PS3AvatarUri != null)
			Registry.SetValue(keyName, "PS3AvatarUri", PS3AvatarUri);

		// DLC URIs
		if (PSVDLCUri != null)
			Registry.SetValue(keyName, "DLCUri", PSVDLCUri);
		if (PSPDLCUri != null)
			Registry.SetValue(keyName, "PSPDLCUri", PSPDLCUri);
		if (PS3DLCUri != null)
			Registry.SetValue(keyName, "PS3DLCUri", PS3DLCUri);
		if (PS4DLCUri != null)
			Registry.SetValue(keyName, "PS4DLCUri", PS4DLCUri);

		// Theme URIs
		if (PSVThemeUri != null)
			Registry.SetValue(keyName, "ThemeUri", PSVThemeUri);
		if (PSPThemeUri != null)
			Registry.SetValue(keyName, "PSPThemeUri", PSPThemeUri);
		if (PS3ThemeUri != null)
			Registry.SetValue(keyName, "PS3ThemeUri", PS3ThemeUri);
		if (PS4ThemeUri != null)
			Registry.SetValue(keyName, "PS4ThemeUri", PS4ThemeUri);

		// Update URIs
		if (PSVUpdateUri != null)
			Registry.SetValue(keyName, "UpdateUri", PSVUpdateUri);
		if (PS4UpdateUri != null)
			Registry.SetValue(keyName, "PS4UpdateUri", PS4UpdateUri);
	}
}





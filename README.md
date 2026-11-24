# BBTUD Unofficial iOS Port

Unofficial iOS port of **Baldi's Basics The Ultra Decompile (BBTUD)** — open source fan project.

## Credits

Original game by **SeenWonderAlex**  
https://seenwonderalex.itch.io/baldis-basics-tud

This port is not affiliated with or endorsed by the original author or Basically Games.  
"Baldi's Basics" is a fan project based on *Baldi's Basics in Education and Learning*.

## Build for iOS

### Requirements
- macOS with Xcode installed
- Unity 2022.3.21f1 LTS (with **iOS Build Support** module via Unity Hub)
- Apple ID (free) for signing on device, or Apple Developer account for distribution

### Local build steps
1. Open the project in Unity 2022.3.21f1.
2. `File → Build Settings` → select **iOS** → `Switch Platform`.
3. `Player Settings → iOS`:
   - Scripting Backend: **IL2CPP**
   - Architecture: **ARM64**
   - Bundle Identifier: your own (e.g. `com.yourname.bbtud-ios`)
   - Signing: Automatically (sign in with Apple ID in Xcode)
4. `Build` to a folder.
5. Open the generated `.xcodeproj` in Xcode, plug in your iPhone, select the device, then **Run**.

> Free Apple ID can sign and install on your own device for 7 days; reconnect to Xcode to renew.

### GitHub Actions

A workflow skeleton is provided at `.github/workflows/unity-ios.yml`.  
GitHub-hosted macOS runners do **not** come with Unity preinstalled, so each run must download and install Unity (~several GB), which consumes Actions minutes quickly. Private repos on GitHub Free include ~$12 compute/month (~190 minutes of macOS). Public repo runners are free but the code becomes public.

## License / Notice

This is an unofficial fan port. Original assets belong to SeenWonderAlex.  
Do not publish on App Store without permission from the rights holders.

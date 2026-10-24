# Patcher

![Platform](https://img.shields.io/badge/platform-Windows-blue)
![Language](https://img.shields.io/badge/language-C%2B%2B%20%2F%20C%23%20%2F%20Rust-green)
![License](https://img.shields.io/badge/license-MIT-orange)

**Patcher** is a lightweight console utility for applying binary patches (hex patches) to game executables. With its help, you can unlock DLCs, bonuses, or modify specific functions in a game without resorting to complex HEX editors.

## 🚀 Features

*   Applying patches at specified offsets.
*   Support for configuration files for different games.
*   Verification of original bytes (`Orig_Byte`) before replacement to prevent file corruption.
*   Works around the Steam API (does not require replacing DLLs such as CreamAPI or SmokeAPI).

## 📦 Usage

The utility operates via the command line.

```bash
patcher <exe game> <config for game>
```

**Parameters:**
*   `<exe game>` — path to the game's executable file (e.g., `100orange.exe`).
*   `<config for game>` — path to the configuration file containing the patch map (offsets).

**Example:**
```bash
patcher "C:\Games\100 Orange Juice\100orange.exe" "100orange.txt"
```

## ⚙️ Configuration File Format

The configuration file contains information about which bytes to replace and at which addresses. The format is simple and readable.

**Config example:**

```ini
Offset_File: 0x11B19B5
Patch_Byte: 75
Orig_Byte: 74

Offset_File: 0x11B19F5
Patch_Byte: 90 90 48 8B 4C 24 48 48 31 E1 48 8B 15 7A 27 42
Orig_Byte: 31 C0 48 8B 4C 24 48 48 31 E1 48 8B 15 7A 27 42
```

*   `Offset_File` — address in the file (in hexadecimal format).
*   `Patch_Byte` — bytes to write.
*   `Orig_Byte` — original bytes (used for integrity verification so the patch doesn't break the game if it updates).

## ⚠️ Disclaimer

*   **Make a backup** of the original `.exe` file before using the patcher!
*   The author is not responsible for any file damage, account bans, or other issues arising from the use of this tool.
*   The tool is provided for educational purposes only.

# AutoC4Giver CS2
Automatically transfers the C4 to a nearby alive teammate if it's dropped shortly after spawn, ensuring the bomb always stays in terrorist hands during the critical early round period.

---

## 🚀 Installation

### Basic Installation
1. Install [CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp) and [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)
2. Download the latest release from releases
3. Extract and upload to your game server at `addons/counterstrikesharp/plugins/AutoC4Giver/`
4. Start server and configure the generated config file

---

## ⚙️ Configuration

The plugin will automatically generate a configuration file at `addons/counterstrikesharp/configs/plugins/AutoC4Giver/AutoC4Giver.json` after the first run.

### Main Configuration Parameters
| Parameter         | Description                                                                                         | Default | Required |
|-------------------|-----------------------------------------------------------------------------------------------------|---------|----------|
| `SpawnDuration`   | Duration in seconds after round start when C4 auto-transfer is active                              | `20`    | **YES**  |
| `TransferDelay`   | Delay in seconds before checking and transferring dropped C4                                       | `0.5`   | **YES**  |
| `EnableDebug`     | Enable detailed debug logging to console                                                           | `false` | **YES**  |
| `ConfigVersion`   | Configuration version (used for automatic updates)                                                 | `1`     | **YES**  |

---

## 🎯 How It Works

1. **Round Start**: Plugin activates the spawn period monitoring for the configured duration
2. **C4 Drop Detection**: When a terrorist drops the C4 during the spawn period, the plugin tracks the dropping player
3. **Auto-Transfer**: After the configured delay, the plugin finds the nearest alive terrorist (excluding the original dropper) and automatically transfers the C4
4. **Player Feedback**: The receiving player gets a chat notification confirming they received the C4
5. **Automatic Cleanup**: The plugin handles round end cleanup and player disconnections

---

## 📊 Support

For issues, questions, or feature requests, please visit our [GitHub Issues](https://github.com/wiruwiru/SpectatorList-CS2/issues) page.
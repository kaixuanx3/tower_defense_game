# CLAUDE.md — Tower Defense Game

A 2D squad tower defense inspired by *Greatest Estate Dev: Squad TD*, built in Unity 6.3 with C#.
The owner is learning Unity and C# from zero on this project. Every change is small, explained in
plain English, and shipped as its own branch + pull request so the history doubles as a tutorial.

## The game we are building

Core loop, in our own words:

1. Before a stage you pick a **squad of 4 heroes** from your roster.
2. Enemies come in **waves** and walk a fixed path toward your **estate**. Each enemy that reaches
   it removes estate HP. HP 0 = defeat.
3. You **drop heroes** onto placement slots beside the path. Heroes attack on their own.
4. Kills give **coins**. Coins buy a **random skill card** (the "spin"): pick 1 of 3, it buffs one
   hero or the whole squad. This is the roguelite part, so no two runs play the same.
5. Every 10th wave is a **boss with breakable parts** and telegraphed attack patterns.
6. Winning gives gold to **level heroes and build the estate**, which grants passive bonuses in
   later runs.

Original heroes, names, and art only. The manhwa and the official game are copyrighted; we borrow
the mechanics, not the content.

## Tech facts

| Thing | Value |
|---|---|
| Unity | 6.3 LTS (`6000.3.24f1`), 2D URP template, macOS |
| Packages already installed | Input System, 2D Tilemap (+Extras), 2D Animation, Aseprite importer, Unity UI (uGUI) + TextMeshPro, Test Framework |
| Language | C#. Unity scripts are classes that inherit `MonoBehaviour` and get attached to objects in a scene |
| Repo | `github.com/kaixuanx3/tower_defense_game`, default branch `main` |
| Code editor | VS Code (`.vscode/` is committed) |
| Unity binary (command-line checks) | `/Applications/Unity/Hub/Editor/6000.3.24f1/Unity.app/Contents/MacOS/Unity` |

## How we work together

**Claude does:** all C# scripts, ScriptableObject definitions, editor tools, tests, this file.
Claude edits `.unity` / `.prefab` / `.asset` YAML by hand only when clicking would be much slower,
and only after you have saved the scene (Cmd+S) and said so. Otherwise Unity overwrites the change.

**You do (in the Unity Editor):** create GameObjects and prefabs, drag references into Inspector
fields, press Play, read the Console. Claude gives exact click-by-click steps every time. Doing
this part yourself is how you learn the editor.

**After Claude adds or changes scripts:** click into the Unity window, wait for the compile
spinner (bottom-right) to finish, open the Console (Cmd+Shift+C). If anything is red, paste the
first error line back to Claude.

**How to ask for the next step:** "start Phase 1 branch 2" (or just "next"). Claude creates the
branch, writes the code, tells you what to click, and you confirm the "Done when" check before
the PR is merged.

### Git workflow: one feature = one branch = one pull request

Never commit on `main`. Every branch below is a separate PR, merged before the next one starts.

```bash
git checkout main && git pull
git checkout -b feat/p1-enemy-path        # prefix: feat/ fix/ chore/ docs/  then phase + name
# ... build the feature, test it in Play mode, Console shows 0 errors ...
git add -A                                # picks up the .meta files Unity generated
git commit -m "feat(p1): enemies walk the waypoint path"
git push -u origin feat/p1-enemy-path
gh pr create --fill
gh pr merge --merge --delete-branch
git checkout main && git pull
```

Rules:
- Every asset has a `.meta` file beside it. Commit them together. Never hand-write `.meta` files;
  Unity creates them when the editor regains focus.
- Commits are authored by the owner only. No Co-Authored-By trailers.
- A PR is done when: 0 compile errors, the phase's "Done when" check passes in Play mode, and
  EditMode tests (once they exist) are green.
- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `*.csproj`, `*.slnx` are generated and ignored.

### Verifying

In the editor: Console shows 0 errors (and no warnings from our own scripts), then run the
Play-mode check listed in the phase.

Command line, only when the Unity editor is **closed** (it locks the project):

```bash
# compile check: look for lines containing "error CS"
"/Applications/Unity/Hub/Editor/6000.3.24f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode -nographics -quit -projectPath . -logFile -

# EditMode tests (do not add -quit here)
"/Applications/Unity/Hub/Editor/6000.3.24f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode -nographics -runTests -testPlatform EditMode \
  -projectPath . -testResults Logs/TestResults.xml -logFile -
```

## Code conventions

- Folders: `Assets/_Game/{Scenes,Scripts,Prefabs,Data,Sprites,Audio,UI}`. Scripts are grouped by
  feature: `Scripts/{Core,Enemies,Heroes,Waves,Cards,UI}`. The underscore keeps our folder at the top.
- One class per file, file name equals class name. Unity requires this for `MonoBehaviour`s.
- Namespace `TowerDefense`. Types, methods, properties in `PascalCase`; fields and locals in
  `camelCase`. Inspector fields are `[SerializeField] private float speed = 2f;`, never public
  fields. Expose a property only when another class needs the value.
- Numbers a designer would tweak (HP, speed, cost, rewards) live in ScriptableObjects
  (`EnemyData`, `HeroData`, `WaveData`, `SkillCardData`), not in code.
- Small components that do one thing (`Health`, `EnemyMovement`, `HeroAttack`) beat one big
  script. Components talk through C# events; nothing searches the scene every frame.
- Simplicity first: build the smallest thing that works for the current phase. No options,
  managers, or abstractions "for later". If a script grows past ~150 lines, split it.
- Only one singleton, `GameManager`. No asset-store code packages unless we agree first.
- Comments explain *why*, in plain English, for a C# beginner. Every new C# feature (interface,
  event, coroutine, generic) gets a one-line comment the first time it appears in the project.

## Ten-second glossary

| Word | Meaning |
|---|---|
| Scene | One screen or level: a file listing GameObjects |
| GameObject | Anything placed in a scene (empty, sprite, camera, UI) |
| Component | A behaviour attached to a GameObject: `Transform`, `SpriteRenderer`, your scripts |
| MonoBehaviour | Base class a script inherits so Unity can attach it and call `Start()` / `Update()` |
| Prefab | A saved GameObject template you can spawn many copies of |
| Inspector | Panel showing the selected object's components and editable fields |
| Serialized field | A script variable Unity shows in the Inspector (`[SerializeField]`) |
| ScriptableObject | A data asset (numbers, sprites, lists) stored in the Project, not in a scene |
| Canvas | Root object of on-screen UI (uGUI) |
| `.meta` file | Unity's ID card for an asset. Always committed with the asset |

## The plan

Each phase is a handful of small feature branches. Finish a phase's **Done when** before starting
the next. **Learn** lists the Unity and C# ideas that phase introduces, in order.

### Phase 0 — Workspace and your first script

Goal: understand the editor, run a script you wrote, make a correct commit.

1. `chore/p0-project-layout`: create `Assets/_Game/...` folders; move `SampleScene` to
   `_Game/Scenes/Game.unity`; add it to the Scene List (File > Build Profiles); set
   Edit > Project Settings > Version Control > Mode to **Visible Meta Files** (we use git, not
   Unity Version Control).
2. `feat/p0-hello-world`: `Bootstrap.cs` with a `[SerializeField] string message` and
   `Debug.Log(message)` in `Start()`, attached to an empty `Game` object.

Learn: Scene, GameObject, Component, Inspector; `MonoBehaviour`, `Start()`, `Update()`,
`Debug.Log`, serialized fields.

Done when: Play prints the message in the Console; changing the text in the Inspector changes
the output without touching code; the PR contains the `.meta` files.

### Phase 1 — Enemies walk to the estate

1. `feat/p1-path`: `Path.cs` holds an ordered list of waypoint child `Transform`s and draws the
   route with `OnDrawGizmos` so you can see it in the Scene view.
2. `feat/p1-enemy-move`: `EnemyMovement.cs` moves waypoint to waypoint with `Vector2.MoveTowards`;
   an `Enemy` prefab with a placeholder circle sprite; a temporary `EnemySpawner.cs` that
   `Instantiate`s one enemy every 2 seconds.
3. `feat/p1-estate-health`: `Estate.cs` with `maxHp`; an enemy reaching the last waypoint calls
   `estate.TakeDamage(1)` and destroys itself; at 0 HP log "Game over".

Learn: `Transform`, `Vector2`, `Time.deltaTime`, prefabs, `Instantiate` / `Destroy`, `List<T>`,
`foreach`, gizmos.

Done when: enemies spawn, follow the drawn path, reduce estate HP, and "Game over" appears at 0.

### Phase 2 — Heroes fight back

1. `feat/p2-health`: `Health.cs` (enemies now, boss parts later) with `TakeDamage` and an
   `OnDied` event; `IDamageable` interface. `Enemy.Active` static list so heroes can find targets
   without physics.
2. `feat/p2-hero-attack`: `HeroAttack.cs` finds the nearest enemy within `range` every
   `attackInterval` seconds and spawns `Projectile.cs`, which flies to its target and calls
   `TakeDamage`. Hero prefab with a placeholder square.
3. `feat/p2-placement`: `PlacementSlot.cs` colliders beside the path; `SquadPlacer.cs` reads
   clicks/taps via the Input System (`Pointer.current` + `Physics2D.OverlapPoint`) and places the
   next hero of a 4-hero squad on the clicked slot.
4. `feat/p2-coins`: `Wallet.cs`; an enemy death adds its `coinReward`; temporary on-screen text.

Learn: interfaces, C# events (`event Action`), static members, 2D colliders and layers, physics
queries, Input System basics.

Done when: you place 4 heroes, they shoot, enemies die, coins go up. Bad placement still leaks.

### Phase 3 — Waves, HUD, win and lose

1. `feat/p3-data-assets`: `EnemyData` and `WaveData` ScriptableObjects (`[CreateAssetMenu]`);
   2–3 enemy types (fast and weak, slow and tanky).
2. `feat/p3-wave-manager`: `WaveManager.cs` runs waves from `WaveData` with a countdown and a
   "Send next wave" button; `GameManager.cs` state machine Preparing → Playing → Won / Lost.
3. `feat/p3-hud`: uGUI Canvas showing estate HP, coins, wave X / N; Win and Lose panels with a
   Restart button (`SceneManager.LoadScene`).
4. `chore/p3-asmdef-and-tests`: assembly definitions for game code and tests; first EditMode tests
   for pure logic (wave scheduling).

Learn: ScriptableObjects, enums, state machines, coroutines, uGUI + TextMeshPro, scene reload,
unit tests with the Test Runner (Window > General > Test Runner).

Done when: a 10-wave stage plays start to finish; Win panel shows; Lose at 0 HP; Restart works;
tests are green.

### Phase 4 — Skill cards (the roguelite "spin")

1. `feat/p4-stats`: `Stat.cs` = base value + list of modifiers (flat or percent). Heroes read
   damage, attack speed, and range through it.
2. `feat/p4-card-data`: `SkillCardData` ScriptableObject: name, rarity (Common / Rare / Epic),
   target (one hero or squad), effect (enum + value), icon.
3. `feat/p4-card-roll`: `CardRoller.cs` spends coins, draws 3 cards by rarity weight, you pick 1,
   it applies; cost rises every roll. Card picker UI panel.

Learn: weighted random, modifiers and composition, `switch` on enums, UI buttons and events.

Done when: mid-battle you spend coins, pick a card, and the chosen hero visibly kills faster.

### Phase 5 — Hero and enemy variety

1. `feat/p5-hero-data`: `HeroData` ScriptableObject (stats, attack type single / splash / chain,
   sprite). Four launch heroes: Guardian (melee, high HP), Archer (long range), Mage (splash),
   Bard (slows enemies in range).
2. `feat/p5-status-effects`: Slow, Burn (damage over time), Stun on enemies.
3. `feat/p5-active-skills`: tap a hero portrait to fire an ultimate with a cooldown.
4. `feat/p5-enemy-variety`: flying (only ranged heroes hit it), swarm, armored (flat damage
   reduction).
5. `chore/p5-object-pool`: reuse projectiles and enemies instead of `Instantiate` / `Destroy`.

Learn: inheritance vs composition, timers, generics, object pooling.

Done when: each hero feels different and each enemy type needs a different counter.

### Phase 6 — Boss with part breaking

1. `feat/p6-boss-parts`: `Boss.cs` plus `BossPart.cs` child objects, each with its own `Health`;
   breaking a part disables one attack and stuns the boss briefly.
2. `feat/p6-boss-patterns`: timed patterns (charge that stuns heroes, summon adds) with a warning
   sprite telegraph.
3. `feat/p6-boss-ui`: boss HP bar with part icons.

Done when: wave 10 spawns a boss with at least 2 breakable parts and 2 patterns, and breaking
parts changes the fight.

### Phase 7 — Art, animation, sound

1. `feat/p7-map`: Tilemap ground and path tiles.
2. `feat/p7-sprites`: replace placeholders with real sprites (see Art below).
3. `feat/p7-animation`: Animator with idle / walk / attack / die clips.
4. `feat/p7-juice`: hit flash, floating damage numbers, particles, screen shake.
5. `feat/p7-audio`: SFX and music through `AudioSource`, a tiny `AudioManager`.

Learn: Tilemap, Animator and animation clips, ParticleSystem, AudioSource, sprite atlases.

Done when: a 20-second clip of play looks and sounds like a real game.

### Phase 8 — Meta game: roster, stages, estate

1. `feat/p8-save`: `SaveData` written as JSON (`JsonUtility`) to `Application.persistentDataPath`.
2. `feat/p8-menus`: Main Menu scene, Stage Select (list of `StageData`), Squad Select (pick 4).
3. `feat/p8-hero-progression`: gold from wins levels heroes up and unlocks new ones.
4. `feat/p8-estate`: Estate screen with buildings bought with gold that grant run bonuses
   (+estate HP, +starting coins, cheaper card rolls).

Learn: multiple scenes and `SceneManager`, persistence and serialization, `DontDestroyOnLoad`.

Done when: quit and reopen keeps progress; beating a stage unlocks the next; estate bonuses apply
in battle.

### Phase 9 — Mobile build and polish

`feat/p9-touch-input`, `feat/p9-safe-area-ui`, `feat/p9-performance` (Profiler, sprite atlas),
`feat/p9-settings` (volume), `feat/p9-tutorial`, `chore/p9-android-build`.

Done when: an Android or iOS (or desktop) build runs the full loop at 60 fps.

**Out of scope for now:** real-time PvP, gacha and monetization, live-ops. Revisit after Phase 9.

## Art and characters

Claude cannot draw production anime or pixel art. Claude can generate placeholder shapes from
code, set up sprite slicing, Animators, and importers once PNGs exist, and point to sources.
Plan: placeholders until Phase 7, then either free CC0 packs (Kenney.nl, itch.io, OpenGameArt) or
your own drawings in Aseprite (the importer is already installed; `.aseprite` files drop straight
into `Assets`). Never use characters or art from the manhwa or the official game.

## Status

- [x] Plan written: this file (2026-09-17)
- [ ] Phase 0 — workspace and first script
- [ ] Phase 1 — enemies walk to the estate
- [ ] Phase 2 — heroes fight back
- [ ] Phase 3 — waves, HUD, win and lose
- [ ] Phase 4 — skill cards
- [ ] Phase 5 — hero and enemy variety
- [ ] Phase 6 — boss with part breaking
- [ ] Phase 7 — art, animation, sound
- [ ] Phase 8 — meta game
- [ ] Phase 9 — mobile build and polish

Update this list (with the date) every time a phase's last PR is merged.

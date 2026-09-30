# FlaUI Automation Context and Guidelines

This document serves as the master context and set of guidelines for automating test cases in this repository. It synthesizes the established standards for FlaUI Desktop Automation, WPF Element Identification (AutomationId), and Pre-PR Review requirements.

Use this context to correctly implement new functional tests (e.g., 'Add' functional test cases) or any provided manual test cases.

## 1. Automation Architecture & Flow

### 1.1 Three-Layer Pattern
The automation suite follows a strict three-layer architecture. Maintain this separation at all times:
1. **Test Layer**: Scenario orchestration using xUnit tests (`[Fact]`). Tests must inherit from `BaseSetup`.
2. **Window/Page Object Layer**: Screen-specific actions and assertions. Control identification belongs here, not in the test methods. Locator constants should be `private const string` fields at the top of the window class.
3. **AutomationHelper Layer**: Reusable, low-level FlaUI actions. Only add to this if a truly generic, reusable interaction is needed.

### 1.2 Test Construction & Flow
- **Manual Steps as Comments**: Insert the exact manual test steps as comments immediately before the corresponding automation code. Do not paraphrase or group comments (e.g., avoid `// STEP 17-23`). Each step must be on one line.
- **MainBaseWindow Flow**: Respect the existing `MainBaseWindow` flow control pattern. Register new windows only when genuinely required and use lazy-cached properties.
- **Window Reuse**: Prefer extending existing window classes and tests over duplicating code. Create new windows/tests only if no suitable counterpart exists.
- **Prompt & Keyboard**: If a prompt can be handled by existing keyboard patterns, do not create a new prompt window. Do not create new windows for picker menus or simple text blocks.

### 1.3 Locator Strategy
When locating controls, use the following priority:
1. Reuse an existing locator already present in the suite.
2. Check the WPF application code for stable locator evidence.
3. Use live inspection metadata (FlaUI Inspect/Live Property Explorer).
4. Prefer properties in this order: `AutomationId` > `Name` > visible text > stable fallback.
5. **TODO Strategy**: If a reliable locator cannot be found safely, do **not** guess. Add a targeted `TODO` comment stating what element is missing (e.g., `// TODO: Confirm AutomationId for the Add button...`).

---

## 2. Element Identification & AutomationIds

When modifying the WPF application UI to support testability, use the following rules to apply `AutomationProperties.AutomationId` safely without breaking functionality or creating duplicates.

### 2.1 Implementation Categories
- **Simple AutomationId**: For standard, unique controls. Use the `aid_` prefix + control name (e.g., `AutomationProperties.AutomationId="aid_btnSave"`).
- **Inherited AutomationId**: For `UserControls` or `ControlTemplates`. Pass through the parent's AutomationId to avoid duplicate static IDs across multiple instances.
- **Converter-Based AutomationId**: For lists, grids, or repeated elements. Use existing `AutomationIdConverter` or `MultiAutomationIdConverter` to dynamically compose unique IDs.
- **Icon ContentControls**: Standard `ContentControl` icons do not surface in UIA. You must use `AutomatisableViewbox` within the `ControlTemplate` and inherit the ID via `MultiBinding` to make them testable.
- **x:Name Fallback**: If a control has an `x:Name` that already surfaces uniquely in FlaUI Inspect, target that `x:Name` directly in the test. Do not force an explicit `aid_` AutomationId.

### 2.2 Naming Prefix Standards
Always use standard prefixes for your AutomationIds. Examples include:
- `Button` / `PickerButton` / `TextButton` -> `aid_btn`
- `TextBox` / `RPSTextBox` / `IconTextBox` -> `aid_txt`
- `TextBlock` -> `aid_txt`
- `ComboBox` -> `aid_cmb`
- `CheckBox` -> `aid_chk`
- `RadioButton` -> `aid_rdo`
- `ListBox` -> `aid_lst`
- `TabItem` -> `aid_tab`
- `DatePicker` -> `aid_dt`

---

## 3. Pre-PR Review & Code Quality

Before considering an automation test complete, perform a local sweep to ensure quality:

### 3.1 Type Safety & Variables
- **No `var` keyword**: Scan the entire file and replace explicit `var` usage with explicit types (e.g., `AutomationElement`, `string`). The only exceptions are when the type is extremely obvious from the right-hand side (like `new List<string>()`), but explicit typing is always preferred.
- **Unused Code**: Remove dead code, commented-out legacy code, unused `using` statements (verify with a build first), and unused constants/parameters.

### 3.2 Assertions & Logic
- **Strong Assertions**: Do not use `.IsAvailable` when you should be checking for selected state, text values, or enabled state. Assertions must prove the intended UI state.
- **No Save Assertions**: Do not generate assertions for "Save" actions unless the manual test step explicitly requires a separate outcome validation.
- **Wait Mechanisms**: **NEVER** use `Thread.Sleep()`. Use deterministic waits like `Wait.UntilInputIsProcessed()`, `WaitUntilElementExists`, or property-based wait helpers.
- **Property Access**: Access FlaUI properties safely using `.ValueOrDefault` (e.g., `Name.ValueOrDefault`) rather than `.Value` to prevent exceptions.

### 3.3 TODOs & Comments Alignment
- Resolve any TODOs that can be safely fixed inside the test project. Keep TODOs that require external app changes or manual inspection, but ensure they are highly specific.
- Verify that the step comment action matches the API call (e.g., if the comment says "Click Add button", ensure a button click is executed, not a keyboard shortcut, unless specified).
- Verify that expected outcome business values in comments exactly match the assertions in code.

---

## 4. Advanced Refactoring & Implementation Rules

### 4.1 Naming and ID Conventions
- **Automation IDs in Tests**: Use `PascalCase_with_underscores` for all automation ID string arguments (e.g., `New_Invoice`). Separate words with underscores and preserve all words from display text.
- **Child Element Lookups**: When looking up children of an expandable menu, use `byId` with the `aid_tvi_expander_` prefix. Never use `byName` for these elements.

### 4.2 Assertions & Validations
- **Multi-Level Assertions**: For expandable items, verify both the parent and ALL children. Parent-only checks give false confidence. Ensure you map methods to check if all are enabled, disabled, or in a mixed state.
- **Cross-Screen Data Validation**: When a step indicates data from a wizard or previous screen appears on a later screen, capture that data from the source screen into local variables and use them to explicitly assert the destination screen. Do not hardcode values or simply assert the screen is open.
- **Filtering Fixed List Entries**: If asserting row counts where a list contains both data rows and static entries (like an "Other" option), explicitly count the data rows by discounting the static entries by name. Do not rely on hardcoded total numbers or imprecise ranges.
- **Access Level Uniqueness**: Do not clone logic across different access levels or permissions. Each scenario must assert its unique restrictions independently.

### 4.3 Control Interactions
- **Left-Click Link Picker Menus**: If a picker menu (like a `PickerButton` or a `TextButton` with `ListItems`) opens via a left-click, use a dedicated helper (e.g., `SelectItemFromLinkMenu`). Do not use `SelectContextMenu` as that implies a right-click and targets the `ContextMenu` property.
- **Scoped Element Lookups**: If an element lacks a unique Automation ID and can only be safely reached through a specific container, place the container-scoped lookup inside the shared `AutomationHelper`. Keep window classes thin by just passing the container IDs to the helper.
- **Wait For Data-Bound Link Text**: When waiting for a data-bound link to load, wait for its *expected text* instead of just waiting for the element to exist or become enabled. It may temporarily show a placeholder (like "Select ...").

### 4.4 Maintenance and Documentation
- **DRY and Parameterisation**: Replace multiple nearly-identical Test methods with parameterised methods where callers pass business terms. Keep Test methods in the Window class, not the Test class.
- **Minimal Comment Convention**: Write minimal, structural comments (e.g., `// Constructor`). Avoid verbose rationale, WPF-internals narration, or repeating what the code name already says.
- **Resolve Actionable TODOs**: If a TODO can be resolved by reading the application source or checking standard conventions, resolve it immediately. Only leave TODOs for missing elements in the app or completely unknown business logic.

---

## 5. Master Governance & Shared Rules

### 5.1 Scope Discipline
- **Test Automation and Review Tasks**: Must ONLY change files within the test project.
- **Automation ID Tasks**: Must ONLY change application UI/XAML projects, never test logic.
- **Application Code Separation**: Never modify application business logic or backend projects to support testing. Only modify UI projects strictly for adding standard Automation ID plumbing.

### 5.2 Technology Stack
- Use **C#, FlaUI, and xUnit** only.
- Never introduce alternate test frameworks (e.g., NUnit, MSTest, SpecFlow) or broad abstractions not currently present in the suite.

### 5.3 Implementation Boundaries
- **No Broad Changes**: Make the smallest safe diff possible. Do not refactor, reformat, rename, or "clean up" unrelated code outside the immediate scope of the task.
- **No Invention**: Do not invent Automation IDs, locators, or business/assertion values. If you are blocked or missing a value, add a precise `TODO` comment and continue.
- **Build Validation**: After any change, always run a build and confirm zero compilation errors before considering a task complete.

### 5.4 Refactoring Capture Policy (Continuous Learning)
- **Knowledge Must Compound**: Any refactoring instruction, bug fix, convention clarification, or safe/unsafe pattern discovered during implementation MUST be captured and appended to the relevant guideline prompt/documentation.
- **Append-Only**: When capturing new lessons, always append a new entry. Never overwrite or delete existing guidance unless it is definitively proven wrong (in which case, supersede it with a dated note).
- **Rule Priorities**: When resolving ambiguity, apply principles in this priority order:
  1. Detailed multi-level assertions
  2. Extract common patterns/prefixes
  3. Parameterise identical logic
  4. Encapsulate technical detail behind business terms
  5. Follow existing conventions
  6. Place test/assertion methods in window classes, not test classes
- **Repository is the Source of Truth**: When a new lesson or theoretical best practice conflicts with an established, working repository pattern, prefer the existing repository pattern. Stay consistent with the surrounding code.

---

## 6. Detailed Pre-PR Quality Gates

This section contains strict, detailed rules for hardening automation code before it is considered PR-ready. 

### 6.1 Strict Type Safety (No `var`)
- **Whole-File Sweep**: When modifying a file, convert ALL `var` declarations in the entire file to their explicit types, not just the lines you changed.
- **Explicit Types**: Use the exact type (e.g., `LoginWindow`, `AutomationElement`, `string`). The only exception is for anonymous types (e.g., `new { ... }`).
- **No Leftover 'var'**: A touched file must never be left with a mix of new explicit types and legacy `var`.

### 6.2 Element Casting & Interaction Safety
- **Assert Before Casting**: Never chain `.AsListBox()`, `.AsButton()`, etc., directly onto `.FindFirstDescendant()`. Always store the located element in a variable, assert it is not null (`Assert.NotNull`), and *then* cast. If `.FindFirstDescendant()` returns null, direct casting will throw a generic `NullReferenceException` that masks the true failure.
- **No Silent Null-Conditional Clicks**: Do not guard required interactions with `?.` (e.g., `menuItem?.AsMenuItem().Click()`). If an element must be interacted with for the test to succeed, a missing element should fail the test immediately, not silently skip the action.
- **Discarded Searches**: If you call `FindAllMatchingElements` but discard the result, you are performing a slow full-tree scan that also acts as a brittle existence check. Remove the call unless you genuinely need the returned list; rely instead on the retry/wait logic of your actual interaction method.

### 6.3 Meaningful Assertions
- **Do Not Read the Lookup Element**: Do not assert business content by reading the same element (e.g., the window title) that you just used to locate the window. Assert the actual content element that renders the value.
- **Do Not Mask Discrepancies**: If a test hardcodes a count (e.g., `Assert.Equal(2, listItems)`) but the method name implies only one item should exist, extract the magic number into a named constant and add a `TODO` to investigate. Do not silently accept the discrepancy.
- **Handle Conditional Prompts**: If a manual step indicates a prompt might appear ("if prompted to Save..."), check for its existence conditionally with a short wait. Do not assume it will always/never appear.

### 6.4 Clean Code & Code Rot
- **Delete Commented Code**: Remove commented-out legacy code once superseded. Do not keep old code as "documentation".
- **Remove Orphaned Constants**: Delete any `private const` Automation IDs that are declared but never consumed by a method in the class.
- **Merge Duplicates**: Always scan for and remove duplicate `using` statements, duplicate field names, or duplicated methods that may have sneaked in from source control merges.
- **Global Usings**: Insert new `global using` entries in alphabetical order within their logical block. Do not just append them to the end.

### 6.5 Comment Formatting Fidelity
- **Comment Wrapping**: If a `//step` comment is too long, break it onto a new line *only at clause boundaries* (e.g., before "And" or "Then"). Never split a word across lines.
- **Minimal Rationale**: Keep comments structural (`// Constructor`, `// Helpers`). Do not write verbose rationale, WPF-internals narration, or repeat what the code plainly says inside method bodies.
- **Verbatim Steps**: Always keep the exact wording of the manual test step. If the code does something slightly different (a spec discrepancy), leave the step comment intact and add a concise 1-line `// Spec discrepancy:` note below it.

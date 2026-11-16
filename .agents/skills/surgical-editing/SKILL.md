---
name: surgical-editing
description: >-
  Use this skill when making code edits, modifying existing files, refactoring code,
  or debugging file modification failures such as 'Target not found' or ambiguous matches.
---

# Surgical File Editing & Precision Patching

This skill provides a dependable, step-by-step workflow for modifying existing files in the repository without corrupting files or encountering string-matching failures.

## Why File Edits Fail

1. **Exact-Match Requirement**: `edit_file` requires the `Target` parameter to match the file's exact bytes—including leading whitespace, indentation (4 spaces), and line endings (`\r\n` vs `\n`).
2. **Under-Anchoring**: Targets with only 1-2 generic lines (such as a closing brace `}` or a common statement) may match multiple places, triggering unintentional mass replacements or failing.
3. **Editing from Stale Memory**: Guessing line content instead of inspecting the file right before editing.
4. **Non-Contiguous Placeholders**: Inserting `// ... rest of code unchanged ...` inside `Target` or `Replacement`.
5. **Panic Overwrites**: Falling back to `write_file(Overwrite: true)` when `edit_file` errors, which wipes out large portions of large files.

---

## The Precision 4-Step Editing Workflow

### Step 1: Read the Exact Target Lines First
Always read the specific line range immediately before calling `edit_file`:
```json
read_file(Path: "C:/path/to/File.cs", StartLine: 45, EndLine: 65)
```

### Step 2: Select a Unique Anchored Target
Include 2–3 unchanged lines before and after your target change to ensure uniqueness:
- Bad target:
  ```csharp
  }
  ```
- Good target:
  ```csharp
      public void Render(RenderContext context, Size2D parent) {
          // original logic
      }
  ```

### Step 3: Match Leading Whitespace Exactly
- NeoKolors uses **4 spaces** indentation and **CRLF** line endings.
- Ensure every line in `Target` and `Replacement` matches the file's indentation level.

### Step 4: Perform Sequential Calls for Multiple Regions
For non-adjacent edits in the same file:
- Make separate calls to `edit_file`.
- Work from **bottom to top** (higher line numbers first) so earlier line positions do not shift for subsequent edits.

---

## Fallback: Line-Range Script (`replace-lines.ps1`)

If `edit_file` repeatedly fails due to stubborn whitespace, Unicode, or line ending differences, use the bundled PowerShell helper script:

```powershell
powershell -File .agents/skills/surgical-editing/scripts/replace-lines.ps1 `
    -Path "Src/Common/NKColor.cs" `
    -StartLine 42 `
    -EndLine 50 `
    -Content "    // Replacement code here"
```

For large multiline replacements, write the replacement snippet to a scratch file first, then run:
```powershell
powershell -File .agents/skills/surgical-editing/scripts/replace-lines.ps1 `
    -Path "Src/Common/NKColor.cs" `
    -StartLine 42 `
    -EndLine 50 `
    -ContentFile ".scratch/snippet.cs"
```

---

## Verification Checklist
- Run `git diff <filepath>` to verify that only the intended lines were modified.
- Build the affected project: `dotnet build <ProjectPath> -c Debug`.
- Never leave a file truncated or broken.

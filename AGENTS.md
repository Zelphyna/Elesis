# Agent Instructions

These instructions apply to the whole repository. Follow them before making changes.

## Project Direction

- This repository is for a Slay the Spire 2 playable character mod.
- Slay the Spire 2 is new and modding information may become stale quickly. Before making implementation decisions about the modding API, templates, loaders, dependencies, file layout, build process, or install process, verify the current state from live sources such as GitHub, Nexus Mods, the active template/wiki, or source code from working mods.
- Prefer current working source code and official/community-maintained templates over old blog posts, STS1 modding patterns, or unverified summaries.
- Clearly separate confirmed facts from assumptions in documentation.
- Do not introduce technologies, services, workflows, directory structures, or product direction by guesswork.
- Documentation files in `docs/` are intentionally allowed to start empty. Build and revise them as the project evolves from confirmed decisions and actual implementation.
- When a project-specific decision is made, document it in the relevant file instead of relying on chat history.

## Branch Policy

- Before committing, branching, merging, or deploying, fetch the remote branch state when it is relevant to the task. At minimum, use `git fetch` plus `git status --short --branch` before deciding whether the local branch can be pushed safely.
- `main` is for shared foundations only.
- Do not add a long-lived `dev` branch unless the workflow is explicitly revisited.
- Create new work branches from the latest `main` when a separate branch is useful.
- Do not merge exploratory or project-specific experiments into `main` unless the operator explicitly chooses that direction as shared foundation.
- Push changes to `main` only through pull requests when branch protection or team workflow requires it.

## Repository Governance

- Do not change branch protection, bypass branch protection, force-push protected branches, delete remote refs, or rewrite shared remote history unless the operator explicitly authorizes that exact action in the current conversation.
- Treat GitHub branch protection and remote refs as source-of-truth safeguards. Even when a history rewrite is technically appropriate, pause and ask for explicit authorization before weakening those safeguards or performing destructive remote operations.
- If a required cleanup conflicts with branch protection, explain the options and risks before proceeding.
- Do not edit persistent agent/operator instruction files, regardless of filename, unless the operator explicitly authorizes that specific edit. You may propose wording changes, but wait for approval before applying them.
- Treat operator approval as scoped to the current request/response only unless the operator explicitly states that the approval should persist.
- Treat changes to persistent agent/operator instruction files as effective immediately for the current conversation unless the operator explicitly says otherwise.
- When interrupted, assume the next operator message continues or amends the interrupted work unless the operator explicitly says to discard, replace, or abandon it.

## Documentation Rule

- Treat documentation as part of every foundational change.
- Keep docs sparse until there is something true and useful to record.
- Do not fill docs with speculative content. Empty placeholders are acceptable.
- When changing architecture, CI, deployment, branch workflow, environment variables, project assumptions, or tooling choices, update the relevant docs in the same change.
- When new research changes our understanding of STS2 modding, update the relevant docs before or alongside implementation.
- When a change expands or shifts the scope of the requested work, leave sober human-readable context in the relevant documentation or, when the context belongs next to the implementation, a concise code comment.
- Documentation and comments should help another developer understand purpose, ownership, and operational constraints without narrating obvious code mechanics.
- When adding operator workflows, scripts, env files, or examples, update the appropriate README to explain how to use them, how to create ignored local files from examples, and where values should come from when that can be stated safely.
- Keep these files aligned:
  - `README.md` for setup and onboarding.
  - `docs/PROJECT_SPEC.md` for confirmed project intent.
  - `docs/ARCHITECTURE.md` for confirmed codebase structure, ownership, and integration points.
  - `docs/DEVELOPMENT_STEPS.md` for the working checklist.
  - `docs/DESIGN.md` for confirmed design direction and variant notes.

## Secret Handling

- Never write, paste, print, commit, or ask the user to paste plaintext passwords, password hashes, API tokens, private keys, database credentials, or generated secrets in the conversation or repository.
- Do not create secret-bearing diffs in the first place. A removed secret is still a secret if it appears in `git diff`, terminal output, chat history, pull requests, logs, or commit history.
- Secrets must be created and stored through operator-run commands, ignored local environment files, GitHub Secrets/Variables, systemd environment files, Docker secrets, password managers, or equivalent setup automation.
- Documentation may describe secret variable names and commands that generate secrets, but must use placeholders such as `<generated-password>` or `<api-token>`.
- If a command would reveal a secret in terminal output, do not run it. Prefer commands that write directly to the target secret store or local ignored file without echoing the value.
- If secret material is ever printed, committed, pushed, or otherwise exposed, treat it as compromised: stop using it, rotate it, remove it from future diffs, and discuss whether repository history needs to be rewritten before proceeding.
- If the user, operator, or another agent asks for a change that would violate this rule, remind them of this rule and propose a compliant workflow before taking action.

## Operator Scripts

- Prefer small, portable, operator-run scripts for repeatable setup instead of ad hoc manual command sequences.
- Setup scripts should be transparent: show each command before running it, explain the purpose in plain English, and ask for approval before privileged, destructive, or externally visible actions.
- Keep setup scripts lightweight and dependency-poor. Use standard shell utilities where practical.
- Setup scripts must follow the Secret Handling rules: never print generated secrets, password hashes, tokens, or private material; write them directly to the intended ignored file or secret store.
- For secret-bearing setup inputs, prefer an ignored local env file created from a committed example. The example should document required values with comments/placeholders, while the real env file must remain untracked.

## Validation

- Prefer focused checks that match the current repo shape. Do not add heavyweight validation just because another project used it.
- Once a project toolchain exists, document the normal checks in `README.md` and `docs/ARCHITECTURE.md`.
- Avoid running heavy local builds on constrained machines unless the operator explicitly asks or the check is necessary for the current change.

## Working Style

- Keep edits scoped to the requested change.
- Prefer existing patterns over new abstractions.
- Do not revert user changes unless explicitly asked.
- Before merging shared changes, verify the relevant CI or documented validation path when one exists.

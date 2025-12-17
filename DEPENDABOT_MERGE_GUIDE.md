# Merging Main into Dependabot Branches

## Background

This repository has multiple dependabot branches that need to be kept up-to-date with the `main` branch. Due to branch protection rules, automated merges cannot be pushed directly to dependabot branches from the Claude Code environment.

## Current Dependabot Branches

The following dependabot branches were identified:

1. `dependabot/docker/dotnet/aspnet-10.0`
2. `dependabot/docker/dotnet/sdk-10.0`
3. `dependabot/nuget/src/TheOfficeAPI/aspnetcore-dependencies-2109638c46`
4. `dependabot/nuget/src/TheOfficeAPI/aspnetcore-dependencies-eae14cf27c`
5. `dependabot/nuget/src/TheOfficeAPI/microsoft-dependencies-1efc30326e`
6. `dependabot/nuget/src/TheOfficeAPI/microsoft-dependencies-e8a1aa53f2`
7. `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Integration/microsoft-dependencies-279cd96dd8`
8. `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Integration/test-dependencies-7e6fcf1d89`
9. `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Unit/microsoft-dependencies-f078703165`
10. `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Unit/test-dependencies-6136edda5f`

## Solution

### Option 1: Automated Script (Recommended)

Run the provided script to merge `main` into all dependabot branches automatically:

```bash
./merge-main-to-dependabot.sh
```

This script will:
- Fetch the latest changes from origin
- Iterate through all dependabot branches
- Merge `main` into each branch
- Push the changes to the remote repository
- Report any conflicts or failures

### Option 2: Manual Merge

To manually merge `main` into a specific dependabot branch:

```bash
# Fetch latest changes
git fetch origin

# Checkout the dependabot branch
git checkout <branch-name>

# Merge main
git merge origin/main --no-edit

# Push changes
git push origin <branch-name>
```

### Option 3: GitHub CLI

If you have the GitHub CLI installed:

```bash
# List all open dependabot PRs
gh pr list --author "app/dependabot" --state open

# For each PR, you can merge the base branch
gh pr view <PR-number> --json headRefName -q .headRefName | xargs -I {} sh -c 'git checkout {} && git merge origin/main && git push origin {}'
```

## Handling Merge Conflicts

If you encounter merge conflicts during the merge process:

1. The script will abort the merge and report the conflict
2. Manually checkout the branch: `git checkout <branch-name>`
3. Attempt the merge: `git merge origin/main`
4. Resolve conflicts in the affected files
5. Stage the resolved files: `git add <files>`
6. Complete the merge: `git commit`
7. Push the changes: `git push origin <branch-name>`

## Notes

- These merges ensure that dependabot PRs include the latest changes from `main`
- This can help prevent conflicts when merging dependabot PRs
- Run this script periodically or whenever significant changes are merged to `main`

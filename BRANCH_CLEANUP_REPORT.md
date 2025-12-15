# Branch Cleanup and Sync Report

Generated: 2025-12-15

## Summary

- **Total Branches Analyzed**: 24 remote branches
- **Merged Branches (to delete)**: 6
- **Active PR Branches**: 14
- **Orphaned Branches (no PR)**: 4

## 1. Merged Branches (Safe to Delete)

These branches have been fully merged into `main` and have no unique commits:

| Branch Name | Status | Action |
|-------------|--------|--------|
| `claude/debug-failing-chore-6zS28` | ✓ Merged | DELETE |
| `claude/fix-ci-cd-pipeline-01P4h391Q7SMX7gHypwDUDDt` | ✓ Merged | DELETE |
| `claude/fix-cd-test-project-0122zoUrdbVr9aLRUz7aPpLD` | ✓ Merged | DELETE |
| `claude/fix-mocked-integration-tests-01L1fr9BvZSwnXmcUTz8DnT2` | ✓ Merged | DELETE |
| `claude/fix-railway-display-01LgbbSjFVuGSSVer653L15s` | ✓ Merged | DELETE |
| `claude/fix-swagger-endpoints-01DnMUvhc7UFWWbwEbSx8r8Q` | ✓ Merged | DELETE |

### Delete Command:
```bash
git push origin --delete \
  claude/debug-failing-chore-6zS28 \
  claude/fix-ci-cd-pipeline-01P4h391Q7SMX7gHypwDUDDt \
  claude/fix-cd-test-project-0122zoUrdbVr9aLRUz7aPpLD \
  claude/fix-mocked-integration-tests-01L1fr9BvZSwnXmcUTz8DnT2 \
  claude/fix-railway-display-01LgbbSjFVuGSSVer653L15s \
  claude/fix-swagger-endpoints-01DnMUvhc7UFWWbwEbSx8r8Q
```

## 2. Active Pull Requests (Need Sync with Main)

### Claude PRs

| PR # | Branch | Title | Status |
|------|--------|-------|--------|
| #37 | `claude/add-contributing-security-01CgQ6z1CmrvatM5FBUMqX9a` | Add contributor and security guidelines | ⚠️ Needs sync |
| #6 | `claude/fix-sonarcloud-issues-011CV5xtxZCdpSTLCTEcE6WR` | Resolve SonarCloud code smells | ⚠️ Needs sync |

### Dependabot PRs

| PR # | Branch | Title |
|------|--------|-------|
| #44 | `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Integration/aspnetcore-dependencies-6babc3cb34` | Bump Microsoft.AspNetCore.Mvc.Testing |
| #43 | `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Unit/microsoft-dependencies-f078703165` | Bump Microsoft.NET.Test.Sdk |
| #42 | `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Integration/microsoft-dependencies-279cd96dd8` | Bump Microsoft.AspNetCore.Mvc.Testing and Microsoft.NET.Test.Sdk |
| #41 | `dependabot/nuget/src/TheOfficeAPI/microsoft-dependencies-e8a1aa53f2` | Bump Microsoft.AspNetCore.OpenApi |
| #40 | `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Unit/test-dependencies-6136edda5f` | Bump xunit and xunit.runner.visualstudio |
| #38 | `dependabot/nuget/src/TheOfficeAPI/aspnetcore-dependencies-eae14cf27c` | Bump Microsoft.AspNetCore.OpenApi and 2 others |
| #35 | `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Integration/multi-9c9872bf5c` | Bump coverlet.collector |
| #34 | `dependabot/nuget/src/TheOfficeAPI/aspnetcore-dependencies-2109638c46` | Bump the aspnetcore-dependencies group |
| #33 | `dependabot/nuget/tests/TheOfficeAPI.Level0.Tests.Integration/test-dependencies-7e6fcf1d89` | Bump the test-dependencies group |
| #32 | `dependabot/nuget/src/TheOfficeAPI/microsoft-dependencies-1efc30326e` | Bump the microsoft-dependencies group |
| #31 | `dependabot/docker/dotnet/aspnet-10.0` | Bump dotnet/aspnet from 9.0 to 10.0 |
| #30 | `dependabot/docker/dotnet/sdk-10.0` | Bump dotnet/sdk from 9.0 to 10.0 |

**Note**: Dependabot PRs typically auto-sync and resolve conflicts. Manual sync may not be necessary.

## 3. Orphaned Branches (No Open PR)

These branches have unique commits but no open pull requests:

| Branch Name | Unique Commit | Recommendation |
|-------------|---------------|----------------|
| `claude/add-exception-handler-editorconfig-dependabot-01GCLMK57dyHgFuUeB5NgCCu` | feat: implement structured logging with Serilog | Create PR or delete |
| `claude/add-richardson-maturity-level-2-01HYbw12FYWeZ7grBe6vxENB` | feat: add Richardson Maturity Model Level 3 (HATEOAS) | Create PR or delete |
| `claude/fix-smoke-tests-01SSGB5sndJGfWHYx33YM1vW` | fix: add XML file existence check in Swagger | Create PR or delete |
| `claude/plan-app-deployment-01DybVnmXgyGsAhEnuuoQb9w` | docs: add deployment guide for Raspberry Pi 5 | Create PR or delete |

## 4. Sync Instructions

To sync each PR branch with main, use the provided script `sync-pr-branches.sh` or run manually:

```bash
# For each branch:
git checkout <branch-name>
git merge origin/main -m "chore: sync with main"
# Resolve any conflicts
git push origin <branch-name>
```

### Known Conflicts

**PR #37** (`claude/add-contributing-security-01CgQ6z1CmrvatM5FBUMqX9a`):
- File: `.github/workflows/ci.yaml`
- Conflict: GitHub Pages deployment permissions
- Resolution: Keep PR branch version (includes `pages: write` permission)

## 5. Recommendations

1. **Immediate Actions**:
   - Delete the 6 merged branches to reduce clutter
   - Sync PR #37 and PR #6 with main

2. **Review Needed**:
   - Decide on the 4 orphaned branches:
     - Create PRs if the features are wanted
     - Delete if no longer needed

3. **Dependabot PRs**:
   - Consider merging or closing old dependabot PRs
   - Some may have conflicts with newer versions

## 6. Next Steps

1. Run `./sync-pr-branches.sh` to automatically sync all PR branches
2. Review and resolve any merge conflicts
3. Delete merged branches using the command in section 1
4. Review orphaned branches and decide their fate

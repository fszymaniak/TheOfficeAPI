# Branch Maintenance Guide

This guide helps maintain a clean and organized repository by managing branches and keeping PRs synchronized with the main branch.

## Quick Start

1. **Review the analysis**: Check `BRANCH_CLEANUP_REPORT.md` for detailed findings
2. **Delete merged branches**: Run `./delete-merged-branches.sh`
3. **Sync PR branches**: Run `./sync-pr-branches.sh`

## Available Scripts

### 1. `delete-merged-branches.sh`

Safely deletes branches that have been fully merged into `main`.

**Usage:**
```bash
./delete-merged-branches.sh
```

**Features:**
- Verifies each branch is actually merged before deletion
- Requires confirmation before deleting
- Provides detailed output and summary
- Safe: Won't delete branches with unique commits

**Branches to be deleted (6 total):**
- `claude/debug-failing-chore-6zS28`
- `claude/fix-ci-cd-pipeline-01P4h391Q7SMX7gHypwDUDDt`
- `claude/fix-cd-test-project-0122zoUrdbVr9aLRUz7aPpLD`
- `claude/fix-mocked-integration-tests-01L1fr9BvZSwnXmcUTz8DnT2`
- `claude/fix-railway-display-01LgbbSjFVuGSSVer653L15s`
- `claude/fix-swagger-endpoints-01DnMUvhc7UFWWbwEbSx8r8Q`

### 2. `sync-pr-branches.sh`

Syncs all active PR branches with the latest changes from `main`.

**Usage:**
```bash
./sync-pr-branches.sh
```

**Features:**
- Automatically fetches latest changes
- Merges main into each PR branch
- Pushes changes (if permissions allow)
- Handles merge conflicts gracefully
- Returns to your original branch when done

**Branches to sync:**
- PR #37: `claude/add-contributing-security-01CgQ6z1CmrvatM5FBUMqX9a`
- PR #6: `claude/fix-sonarcloud-issues-011CV5xtxZCdpSTLCTEcE6WR`

## Manual Sync Process

If you prefer to sync branches manually or need to handle merge conflicts:

```bash
# 1. Fetch latest changes
git fetch origin

# 2. Checkout the PR branch
git checkout <branch-name>

# 3. Merge main into the branch
git merge origin/main -m "chore: sync with main"

# 4. If conflicts occur:
#    - Resolve conflicts in your editor
#    - Stage resolved files: git add <file>
#    - Commit: git commit

# 5. Push changes
git push origin <branch-name>

# 6. Return to your working branch
git checkout -
```

## Known Issues

### PR #37 Merge Conflict

**File:** `.github/workflows/ci.yaml`

**Conflict:** The PR branch includes GitHub Pages deployment configuration with additional permissions:
```yaml
permissions:
  contents: write       # Required for GitHub Pages deployment
  pages: write          # Required for GitHub Pages deployment
  checks: write         # Required for test-reporter action
  pull-requests: write  # Required for test-reporter action on PRs
```

**Resolution:** Keep the PR branch version if you want to maintain GitHub Pages deployment functionality.

## Orphaned Branches

The following branches have commits but no open PRs. You should decide whether to:
- Create a PR for them
- Merge them manually
- Delete them

| Branch | Feature | Action Needed |
|--------|---------|---------------|
| `claude/add-exception-handler-editorconfig-dependabot-01GCLMK57dyHgFuUeB5NgCCu` | Structured logging with Serilog | Create PR or delete |
| `claude/add-richardson-maturity-level-2-01HYbw12FYWeZ7grBe6vxENB` | HATEOAS implementation | Create PR or delete |
| `claude/fix-smoke-tests-01SSGB5sndJGfWHYx33YM1vW` | Swagger XML file check | Create PR or delete |
| `claude/plan-app-deployment-01DybVnmXgyGsAhEnuuoQb9w` | Raspberry Pi deployment guide | Create PR or delete |

## Dependabot PRs

The repository has 12 open Dependabot PRs. Consider:

1. **Review and merge** dependency updates in batches:
   - Docker updates (#31, #30)
   - ASP.NET Core updates (#34, #38, #41, #44)
   - Testing framework updates (#33, #40, #42, #43)

2. **Close outdated PRs** if newer versions are available

3. **Configure Dependabot** to reduce PR count:
   - Group related dependencies
   - Set update schedule to weekly instead of daily

## Best Practices

1. **Regular Maintenance**: Run these scripts weekly to keep branches clean
2. **Sync Before Review**: Always sync PR branches with main before code review
3. **Delete After Merge**: Delete branches immediately after merging PRs
4. **Review Orphaned Branches**: Regularly check for branches without PRs

## Troubleshooting

### Permission Errors

If you get 403 errors when pushing:
- You may not have write permissions to certain branches
- Contact repository admin for access
- Or manually push from a machine with correct credentials

### Merge Conflicts

If automatic merge fails:
1. The script will show which files have conflicts
2. Checkout the branch: `git checkout <branch-name>`
3. Check conflict markers: `git diff --name-only --diff-filter=U`
4. Resolve conflicts manually
5. Commit and push: `git add . && git commit && git push`

### Branch Already Deleted

If a branch was already deleted:
- The scripts will skip it automatically
- No action needed

## Additional Resources

- Full analysis: `BRANCH_CLEANUP_REPORT.md`
- GitHub PR page: https://github.com/fszymaniak/TheOfficeAPI/pulls
- Git documentation: https://git-scm.com/doc

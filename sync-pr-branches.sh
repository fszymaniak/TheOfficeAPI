#!/bin/bash

# Branch Sync Script
# This script syncs all PR branches with the main branch

set -e  # Exit on error

MAIN_BRANCH="main"
CURRENT_BRANCH=$(git branch --show-current)

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# PR branches to sync
PR_BRANCHES=(
    "claude/add-contributing-security-01CgQ6z1CmrvatM5FBUMqX9a"
    "claude/fix-sonarcloud-issues-011CV5xtxZCdpSTLCTEcE6WR"
)

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}PR Branch Sync Script${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Fetch latest from origin
echo -e "${YELLOW}Fetching latest changes from origin...${NC}"
git fetch origin

# Function to sync a branch
sync_branch() {
    local branch=$1
    echo ""
    echo -e "${BLUE}----------------------------------------${NC}"
    echo -e "${BLUE}Syncing: $branch${NC}"
    echo -e "${BLUE}----------------------------------------${NC}"

    # Check if branch exists
    if ! git rev-parse --verify "origin/$branch" >/dev/null 2>&1; then
        echo -e "${RED}✗ Branch does not exist on remote${NC}"
        return 1
    fi

    # Checkout branch
    echo -e "${YELLOW}Checking out branch...${NC}"
    if ! git checkout "$branch" 2>/dev/null; then
        git checkout -b "$branch" "origin/$branch"
    fi

    # Check if already up to date
    BEHIND=$(git rev-list --count HEAD..origin/$MAIN_BRANCH)
    if [ "$BEHIND" -eq 0 ]; then
        echo -e "${GREEN}✓ Branch is already up to date with $MAIN_BRANCH${NC}"
        return 0
    fi

    echo -e "${YELLOW}Branch is $BEHIND commits behind $MAIN_BRANCH${NC}"

    # Try to merge
    echo -e "${YELLOW}Merging $MAIN_BRANCH into $branch...${NC}"
    if git merge "origin/$MAIN_BRANCH" -m "chore: sync with main"; then
        echo -e "${GREEN}✓ Merge successful${NC}"

        # Push changes
        echo -e "${YELLOW}Pushing changes...${NC}"
        if git push origin "$branch"; then
            echo -e "${GREEN}✓ Successfully pushed to origin${NC}"
        else
            echo -e "${RED}✗ Failed to push (check permissions)${NC}"
            echo -e "${YELLOW}  You may need to push manually${NC}"
            return 1
        fi
    else
        echo -e "${RED}✗ Merge conflict detected${NC}"
        echo -e "${YELLOW}  Files with conflicts:${NC}"
        git diff --name-only --diff-filter=U
        echo ""
        echo -e "${YELLOW}  Please resolve conflicts manually:${NC}"
        echo -e "${YELLOW}  1. Fix conflicts in the files listed above${NC}"
        echo -e "${YELLOW}  2. Run: git add <resolved-files>${NC}"
        echo -e "${YELLOW}  3. Run: git commit${NC}"
        echo -e "${YELLOW}  4. Run: git push origin $branch${NC}"
        echo ""
        return 1
    fi
}

# Sync each PR branch
FAILED_BRANCHES=()
SUCCESS_COUNT=0

for branch in "${PR_BRANCHES[@]}"; do
    if sync_branch "$branch"; then
        ((SUCCESS_COUNT++))
    else
        FAILED_BRANCHES+=("$branch")
    fi
done

# Return to original branch
echo ""
echo -e "${YELLOW}Returning to original branch: $CURRENT_BRANCH${NC}"
git checkout "$CURRENT_BRANCH"

# Summary
echo ""
echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Sync Summary${NC}"
echo -e "${BLUE}========================================${NC}"
echo -e "${GREEN}Successful: $SUCCESS_COUNT${NC}"
echo -e "${RED}Failed: ${#FAILED_BRANCHES[@]}${NC}"

if [ ${#FAILED_BRANCHES[@]} -gt 0 ]; then
    echo ""
    echo -e "${YELLOW}Branches that need manual attention:${NC}"
    for branch in "${FAILED_BRANCHES[@]}"; do
        echo -e "${RED}  - $branch${NC}"
    done
    exit 1
fi

echo -e "${GREEN}All branches synced successfully!${NC}"
exit 0

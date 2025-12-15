#!/bin/bash

# Delete Merged Branches Script
# This script deletes branches that have been fully merged into main

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Merged branches to delete
MERGED_BRANCHES=(
    "claude/debug-failing-chore-6zS28"
    "claude/fix-ci-cd-pipeline-01P4h391Q7SMX7gHypwDUDDt"
    "claude/fix-cd-test-project-0122zoUrdbVr9aLRUz7aPpLD"
    "claude/fix-mocked-integration-tests-01L1fr9BvZSwnXmcUTz8DnT2"
    "claude/fix-railway-display-01LgbbSjFVuGSSVer653L15s"
    "claude/fix-swagger-endpoints-01DnMUvhc7UFWWbwEbSx8r8Q"
)

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Delete Merged Branches Script${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Fetch latest to ensure we have up-to-date info
echo -e "${YELLOW}Fetching latest changes...${NC}"
git fetch origin

# Verify each branch is actually merged
echo -e "${YELLOW}Verifying branches are merged...${NC}"
SAFE_TO_DELETE=()
NOT_MERGED=()

for branch in "${MERGED_BRANCHES[@]}"; do
    if git rev-parse --verify "origin/$branch" >/dev/null 2>&1; then
        # Check if branch has any commits not in main
        UNIQUE_COMMITS=$(git log origin/main..origin/$branch --oneline | wc -l)
        if [ "$UNIQUE_COMMITS" -eq 0 ]; then
            echo -e "${GREEN}✓ $branch - fully merged${NC}"
            SAFE_TO_DELETE+=("$branch")
        else
            echo -e "${RED}✗ $branch - has $UNIQUE_COMMITS unique commits${NC}"
            NOT_MERGED+=("$branch")
        fi
    else
        echo -e "${YELLOW}⊘ $branch - already deleted${NC}"
    fi
done

# Show summary
echo ""
echo -e "${BLUE}Summary:${NC}"
echo -e "${GREEN}Safe to delete: ${#SAFE_TO_DELETE[@]}${NC}"
echo -e "${RED}Not merged: ${#NOT_MERGED[@]}${NC}"

if [ ${#NOT_MERGED[@]} -gt 0 ]; then
    echo ""
    echo -e "${YELLOW}WARNING: The following branches have unique commits:${NC}"
    for branch in "${NOT_MERGED[@]}"; do
        echo -e "${RED}  - $branch${NC}"
    done
    echo ""
fi

if [ ${#SAFE_TO_DELETE[@]} -eq 0 ]; then
    echo -e "${GREEN}No branches to delete.${NC}"
    exit 0
fi

# Confirm deletion
echo ""
echo -e "${YELLOW}The following branches will be deleted:${NC}"
for branch in "${SAFE_TO_DELETE[@]}"; do
    echo -e "  - $branch"
done
echo ""
read -p "Are you sure you want to delete these branches? (yes/no): " CONFIRM

if [ "$CONFIRM" != "yes" ]; then
    echo -e "${YELLOW}Deletion cancelled.${NC}"
    exit 0
fi

# Delete branches
echo ""
echo -e "${YELLOW}Deleting branches...${NC}"
SUCCESS_COUNT=0
FAILED=()

for branch in "${SAFE_TO_DELETE[@]}"; do
    echo -e "${YELLOW}Deleting $branch...${NC}"
    if git push origin --delete "$branch" 2>/dev/null; then
        echo -e "${GREEN}✓ Deleted $branch${NC}"
        ((SUCCESS_COUNT++))
    else
        echo -e "${RED}✗ Failed to delete $branch${NC}"
        FAILED+=("$branch")
    fi
done

# Final summary
echo ""
echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Deletion Summary${NC}"
echo -e "${BLUE}========================================${NC}"
echo -e "${GREEN}Successfully deleted: $SUCCESS_COUNT${NC}"
echo -e "${RED}Failed: ${#FAILED[@]}${NC}"

if [ ${#FAILED[@]} -gt 0 ]; then
    echo ""
    echo -e "${YELLOW}Failed to delete:${NC}"
    for branch in "${FAILED[@]}"; do
        echo -e "${RED}  - $branch${NC}"
    done
    echo ""
    echo -e "${YELLOW}Note: Failures may be due to permissions or branch protection rules.${NC}"
    exit 1
fi

echo -e "${GREEN}All merged branches deleted successfully!${NC}"
exit 0

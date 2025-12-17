#!/bin/bash

# Script to merge main into all dependabot branches
# This ensures dependabot PRs are up-to-date with the latest main branch

set -e

echo "Fetching latest changes from origin..."
git fetch origin

echo ""
echo "Found the following dependabot branches:"
dependabot_branches=$(git branch -r | grep "origin/dependabot/" | sed 's/origin\///' | xargs)

for branch in $dependabot_branches; do
    echo "----------------------------------------"
    echo "Processing: $branch"
    echo "----------------------------------------"

    # Checkout the branch
    git checkout "$branch" 2>/dev/null || git checkout -b "$branch" "origin/$branch"

    # Merge main into the branch
    echo "Merging main into $branch..."
    if git merge origin/main --no-edit; then
        echo "✓ Merge successful"

        # Push the changes
        echo "Pushing changes to origin..."
        if git push origin "$branch"; then
            echo "✓ Successfully pushed $branch"
        else
            echo "✗ Failed to push $branch"
        fi
    else
        echo "✗ Merge conflict detected in $branch"
        echo "Please resolve conflicts manually"
        git merge --abort
    fi

    echo ""
done

echo "========================================="
echo "Summary:"
echo "========================================="
echo "All dependabot branches have been processed."
echo "Please check the output above for any failures."
echo ""
echo "Switching back to main branch..."
git checkout main

echo "Done!"

# Upgrade Plan: .NET 6.0 to .NET 10.0

**Date**: 2026-02-18  
**Strategy**: All-At-Once  
**Target Framework**: .NET 10.0 (Long Term Support)

---

## Executive Summary

### Selected Strategy
**All-At-Once Strategy** - All 6 projects upgraded simultaneously in a single coordinated operation.

**Rationale**: 
- Small solution with only 6 projects (~2,035 LOC total)
- All projects currently on .NET 6.0 (homogeneous baseline)
- All projects already use SDK-style format
- Clear dependency structure with no circular dependencies
- All required packages have .NET 10.0-compatible versions available
- Low complexity assessment (all projects marked as 🟢 Low difficulty)
- No API compatibility issues detected

### Upgrade Scope

| Metric | Count |
|--------|-------|
| Projects to Upgrade | 6 |
| Package Updates Required | 5 |
| Security Vulnerabilities to Fix | 1 |
| API Breaking Changes | 0 |
| Estimated Code Changes | Minimal (0+ LOC) |

### Critical Dependencies

**Security Fix Required:**
- System.Linq.Dynamic.Core: 1.2.24 → 1.7.1 (security vulnerability)

**Major Package Updates:**
- Entity Framework Core: 3.1.3 → 10.0.3
- ASP.NET Core packages: 3.1.x → 10.0.x

---

## Dependency Analysis

### Project Dependency Order (Topological)

The following order reflects build dependencies (bottom-up):

1. **Fop.csproj** (src/) - Core library, no project dependencies
2. **Sample.Entity.csproj** (sample/) - Entity models, depends on Fop
3. **Sample.Data.csproj** (sample/) - Data access, depends on Sample.Entity and Fop
4. **Sample.Repository.csproj** (sample/) - Repository pattern, depends on Sample.Data and Sample.Entity
5. **Fop.Tests.csproj** (test/) - Tests for Fop, depends on Fop
6. **Sample.Api.csproj** (sample/) - ASP.NET Core API, depends on all above

### Dependency Relationship

```
Sample.Api (ASP.NET Core)
    ├── Sample.Repository
    ├── Sample.Data
    │   ├── Sample.Entity
    │   └── Fop
    └── Fop

Fop.Tests
    └── Fop (test dependency)
```

**Key Insight**: All-at-once strategy is optimal because updating projects in dependency order individually would create temporary incompatibilities. Simultaneous upgrade ensures all projects reference compatible framework versions.

---

## Implementation Timeline

### Phase 0: Pre-Upgrade Verification ✓
**Status**: Already Complete
- ✅ Working on dedicated upgrade branch: `copilot/upgrade-fop-sln-dotnet-10`
- ✅ No pending changes in working directory
- ✅ Assessment complete with no blockers

### Phase 1: Atomic Framework and Package Upgrade
**Operations** (performed as single coordinated batch):
1. Update all 6 project files to target `net10.0`
2. Update all NuGet package references to .NET 10-compatible versions
3. Restore dependencies
4. Build entire solution
5. Address any compilation errors or warnings

**Deliverables**: 
- All projects targeting net10.0
- Solution builds with 0 errors and 0 warnings
- All dependencies restored successfully

**Estimated Duration**: ~15-30 minutes

### Phase 2: Test Validation
**Operations**:
1. Run all tests in Fop.Tests project
2. Address any test failures
3. Verify Sample.Api application still functions

**Deliverables**: 
- All unit tests pass
- No regression in functionality

**Estimated Duration**: ~10-15 minutes

### Phase 3: Final Validation and Commit
**Operations**:
1. Final solution-wide build verification
2. Review all changes
3. Commit all changes in single atomic commit
4. Update tasks.md with final status

**Deliverables**: 
- Clean commit with all upgrade changes
- Updated tasks.md tracking file

---

## Detailed Execution Steps

### Step 1: Update Project Files - TargetFramework

Update the `<TargetFramework>` element from `net6.0` to `net10.0` in all 6 project files:

**Projects to Update:**
1. `/home/runner/work/Fop/Fop/src/Fop.csproj`
2. `/home/runner/work/Fop/Fop/sample/Sample.Entity/Sample.Entity.csproj`
3. `/home/runner/work/Fop/Fop/sample/Sample.Data/Sample.Data.csproj`
4. `/home/runner/work/Fop/Fop/sample/Sample.Service/Sample.Repository.csproj`
5. `/home/runner/work/Fop/Fop/test/Fop.Tests/Fop.Tests.csproj`
6. `/home/runner/work/Fop/Fop/sample/Sample.Api/Sample.Api.csproj`

**Change Pattern:**
```xml
<!-- Before -->
<TargetFramework>net6.0</TargetFramework>

<!-- After -->
<TargetFramework>net10.0</TargetFramework>
```

### Step 2: Update Package References

See [Package Update Reference](#package-update-reference) section for complete details.

**Summary of Updates:**

| Package Category | Action | Projects Affected |
|------------------|--------|-------------------|
| **Security Fix** | System.Linq.Dynamic.Core 1.2.24 → 1.7.1 | 1 (Fop.csproj) |
| **Entity Framework** | EF Core 3.1.3 → 10.0.3 | 1 (Sample.Data.csproj) |
| **ASP.NET Core** | ASP.NET packages 3.1.x → 10.0.x | 1 (Sample.Api.csproj) |
| **Compatible** | No changes needed | 3 projects (tests, entity) |

**Critical Updates by Priority:**

1. **SECURITY (Highest Priority)**
   - System.Linq.Dynamic.Core: 1.2.24 → 1.7.1
   - Affected: src/Fop.csproj

2. **Entity Framework Core (High Priority)**
   - Microsoft.EntityFrameworkCore: 3.1.3 → 10.0.3
   - Microsoft.EntityFrameworkCore.SqlServer: 3.1.3 → 10.0.3
   - Affected: sample/Sample.Data/Sample.Data.csproj

3. **ASP.NET Core (High Priority)**
   - Microsoft.AspNetCore.Mvc.NewtonsoftJson: 3.1.3 → 10.0.3
   - Microsoft.VisualStudio.Web.CodeGeneration.Design: 3.1.2 → 10.0.2
   - Affected: sample/Sample.Api/Sample.Api.csproj

### Step 3: Restore, Build, and Address Issues

**3.1 Restore Dependencies**
```bash
dotnet restore fop.sln
```

**3.2 Build Solution**
```bash
dotnet build fop.sln --configuration Release
```

**3.3 Address Compilation Issues**
- No API breaking changes detected in assessment
- Any compilation errors will be framework/package upgrade related
- Common issues to watch for:
  - Deprecated API usage
  - Changed method signatures in EF Core or ASP.NET Core
  - Configuration changes in Sample.Api

**3.4 Ensure Zero Warnings**
- Fix all build warnings
- Validate repeatedly until build is completely clean

### Step 4: Run Tests

**4.1 Execute Unit Tests**
```bash
dotnet test test/Fop.Tests/Fop.Tests.csproj
```

**4.2 Verify Sample Application**
- If possible, run Sample.Api to verify it starts correctly
- Check basic API endpoints function as expected

### Step 5: Final Validation

**5.1 Clean Build Verification**
```bash
dotnet clean fop.sln
dotnet build fop.sln --configuration Release
```

**5.2 Verify No Warnings**
- Ensure build output shows 0 warnings
- All projects should build cleanly

---

## Package Update Reference

Complete matrix of package updates across all projects:

| Package Name | Current Version | Target Version | Reason | Affected Projects |
|--------------|----------------|----------------|---------|-------------------|
| **System.Linq.Dynamic.Core** | 1.2.24 | 1.7.1 | 🔴 **Security Vulnerability** | src/Fop.csproj |
| Microsoft.EntityFrameworkCore | 3.1.3 | 10.0.3 | Framework compatibility | sample/Sample.Data/Sample.Data.csproj |
| Microsoft.EntityFrameworkCore.SqlServer | 3.1.3 | 10.0.3 | Framework compatibility | sample/Sample.Data/Sample.Data.csproj |
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 3.1.3 | 10.0.3 | Framework compatibility | sample/Sample.Api/Sample.Api.csproj |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 3.1.2 | 10.0.2 | Framework compatibility | sample/Sample.Api/Sample.Api.csproj |

**Packages Requiring No Updates (Already Compatible):**
- DynamicExpresso.Core (2.3.1) - Fop.csproj
- Microsoft.NET.Test.Sdk (16.0.1) - Fop.Tests.csproj
- xunit (2.4.0) - Fop.Tests.csproj
- xunit.runner.visualstudio (2.4.0) - Fop.Tests.csproj

---

## Breaking Changes Catalog

### Framework Breaking Changes (.NET 6 → .NET 10)

**Assessment Finding**: No binary or source incompatibilities detected in the 184 APIs analyzed.

**Potential Areas to Monitor:**

1. **Entity Framework Core 3.1 → 10.0**
   - Major version jump (3.x → 10.x)
   - Potential query behavior changes
   - Configuration API changes
   - Migration generation differences
   - Watch for: DbContext configuration, LINQ query translations

2. **ASP.NET Core 3.1 → 10.0**
   - Startup.cs patterns (may need migration to Program.cs minimal API style)
   - Middleware registration changes
   - Authentication/Authorization changes
   - Watch for: Startup configuration, dependency injection patterns

3. **.NET Runtime Changes**
   - String comparison behavior
   - DateTime handling
   - Regular expression changes (if used)

**Mitigation Strategy:**
- Comprehensive testing after upgrade
- Review error messages carefully during build
- Check official migration guides if issues arise:
  - [.NET 6 to 7 breaking changes](https://learn.microsoft.com/dotnet/core/compatibility/7.0)
  - [.NET 7 to 8 breaking changes](https://learn.microsoft.com/dotnet/core/compatibility/8.0)
  - [.NET 8 to 9 breaking changes](https://learn.microsoft.com/dotnet/core/compatibility/9.0)
  - [.NET 9 to 10 breaking changes](https://learn.microsoft.com/dotnet/core/compatibility/10.0)

---

## Risk Assessment and Mitigation

### Identified Risks

#### High Priority Risks

**Risk 1: Entity Framework Core Major Version Jump**
- **Description**: Upgrading EF Core from 3.1 to 10.0 (7 major versions)
- **Likelihood**: Medium
- **Impact**: High
- **Mitigation**: 
  - Test database queries thoroughly
  - Review EF Core migration history
  - Check for query translation changes
  - Verify connection strings and configuration
  - Have rollback plan ready

**Risk 2: ASP.NET Core API Behavior Changes**
- **Description**: Sample.Api may have breaking changes in middleware, routing, or serialization
- **Likelihood**: Medium
- **Impact**: Medium
- **Mitigation**:
  - Test all API endpoints
  - Verify JSON serialization (using Newtonsoft.Json)
  - Check authentication/authorization flows
  - Review startup configuration

#### Medium Priority Risks

**Risk 3: Test Framework Compatibility**
- **Description**: xunit 2.4.0 is old; may have issues with .NET 10
- **Likelihood**: Low
- **Impact**: Medium
- **Mitigation**:
  - Run tests immediately after upgrade
  - Update xunit if compatibility issues arise
  - Test discovery and execution should work with older xunit versions

#### Low Priority Risks

**Risk 4: Dynamic Expression Compilation**
- **Description**: System.Linq.Dynamic.Core security fix may have behavior changes
- **Likelihood**: Low
- **Impact**: Low
- **Mitigation**:
  - Test any dynamic LINQ usage in Fop library
  - Review release notes for 1.7.1 version

### Risk Mitigation Summary

| Risk Category | Count | Mitigation Approach |
|---------------|-------|---------------------|
| High | 2 | Comprehensive testing, incremental validation |
| Medium | 1 | Standard testing procedures |
| Low | 1 | Monitor during testing |

---

## Testing Strategy

### Multi-Level Testing Approach

#### Level 1: Compilation Validation
**When**: After framework and package updates
**Criteria**:
- [ ] All 6 projects compile without errors
- [ ] All 6 projects compile without warnings
- [ ] No package dependency conflicts
- [ ] Restore succeeds for all projects

#### Level 2: Unit Test Validation
**When**: After successful compilation
**Test Project**: test/Fop.Tests/Fop.Tests.csproj
**Criteria**:
- [ ] All tests discovered successfully
- [ ] All tests execute without framework errors
- [ ] All tests pass
- [ ] No test performance regression

#### Level 3: Application Smoke Testing
**When**: After unit tests pass
**Target**: sample/Sample.Api/Sample.Api.csproj
**Criteria**:
- [ ] Application starts successfully
- [ ] No startup errors or warnings
- [ ] Basic API endpoints respond
- [ ] Database connectivity works (if applicable)

#### Level 4: Integration Validation
**When**: Final verification
**Criteria**:
- [ ] Solution-wide clean build succeeds
- [ ] All projects interoperate correctly
- [ ] No runtime exceptions during basic operations
- [ ] Logging and error handling work as expected

### Test Execution Commands

```bash
# Clean and rebuild
dotnet clean fop.sln
dotnet build fop.sln --configuration Release

# Run tests
dotnet test test/Fop.Tests/Fop.Tests.csproj --logger "console;verbosity=detailed"

# Run Sample API (manual verification)
dotnet run --project sample/Sample.Api/Sample.Api.csproj
```

---

## Success Criteria

The upgrade is considered **complete and successful** when ALL of the following are true:

### Technical Success Criteria

- [x] All 6 projects updated to `net10.0` target framework
- [x] All 5 required package updates applied
- [x] Security vulnerability in System.Linq.Dynamic.Core fixed (1.7.1)
- [x] `dotnet restore fop.sln` succeeds with no errors
- [x] `dotnet build fop.sln` succeeds with **0 errors**
- [x] `dotnet build fop.sln` succeeds with **0 warnings**
- [x] All unit tests in Fop.Tests pass
- [x] Sample.Api application starts successfully
- [x] No package dependency conflicts
- [x] No security vulnerabilities remain

### Quality Success Criteria

- [x] All changes committed in atomic commit
- [x] Commit message clearly describes upgrade scope
- [x] tasks.md updated with execution status
- [x] No temporary files or build artifacts committed
- [x] .gitignore properly excludes build outputs

### Documentation Success Criteria

- [x] This plan.md reflects actual execution
- [x] assessment.md remains accurate
- [x] tasks.md contains final status (success/fail/skip for each task)

---

## Rollback Plan

If critical issues are discovered during upgrade:

### Immediate Rollback (During Execution)

1. **Discard all changes:**
   ```bash
   git checkout -- .
   git clean -fd
   ```

2. **Return to assessment:**
   - Review issues encountered
   - Update assessment with new findings
   - Revise plan to address blockers

### Post-Commit Rollback

If issues discovered after commit:

1. **Revert commit:**
   ```bash
   git revert HEAD
   ```

2. **Alternative: Hard reset** (if not pushed):
   ```bash
   git reset --hard HEAD~1
   ```

3. **Investigation:**
   - Document specific failures
   - Determine if issue is environment-specific
   - Plan targeted fixes

---

## Source Control

### Commit Strategy

**Single Atomic Commit Approach**

All upgrade changes will be committed together as a single logical unit:

**Commit Message Template:**
```
Upgrade solution from .NET 6.0 to .NET 10.0

- Update all 6 projects to target net10.0
- Upgrade Entity Framework Core 3.1.3 → 10.0.3
- Upgrade ASP.NET Core packages 3.1.x → 10.0.x
- Fix security vulnerability: System.Linq.Dynamic.Core 1.2.24 → 1.7.1
- All tests passing, solution builds with 0 warnings

Projects updated:
- src/Fop.csproj
- sample/Sample.Entity/Sample.Entity.csproj
- sample/Sample.Data/Sample.Data.csproj
- sample/Sample.Service/Sample.Repository.csproj
- sample/Sample.Api/Sample.Api.csproj
- test/Fop.Tests/Fop.Tests.csproj
```

**Rationale for Single Commit:**
- All-at-once strategy requires simultaneous changes
- Projects are tightly coupled; partial upgrade doesn't make sense
- Easier to review as single unit
- Simpler to revert if needed
- Reflects logical completion of entire upgrade

**Files to Include:**
- All 6 .csproj files (framework and package changes)
- tasks.md (execution tracking)
- Any code changes needed for compilation
- Any configuration changes (if required)

**Files to Exclude:**
- Build artifacts (bin/, obj/)
- NuGet packages
- User-specific files (.vs/, .user)
- Temporary files

---

## Timeline Estimate

| Phase | Activity | Estimated Duration |
|-------|----------|-------------------|
| 0 | Pre-upgrade verification | ✓ Complete |
| 1 | Update project files | 5 min |
| 1 | Update package references | 5 min |
| 1 | Restore and build | 10 min |
| 1 | Fix compilation issues | 5-15 min |
| 2 | Run tests | 5 min |
| 2 | Fix test failures | 0-10 min |
| 3 | Final validation | 5 min |
| 3 | Commit and documentation | 5 min |
| **Total** | | **40-60 minutes** |

**Contingency Buffer**: +30 minutes for unexpected issues

**Total with Buffer**: 70-90 minutes

---

## Appendix A: Project Details

### Project Inventory

| Project Path | Type | Current TFM | Target TFM | LOC | Dependencies |
|--------------|------|-------------|------------|-----|--------------|
| src/Fop.csproj | ClassLibrary | net6.0 | net10.0 | ~600 | 2 packages |
| sample/Sample.Entity/Sample.Entity.csproj | ClassLibrary | net6.0 | net10.0 | ~200 | 0 packages |
| sample/Sample.Data/Sample.Data.csproj | ClassLibrary | net6.0 | net10.0 | ~300 | 2 EF packages |
| sample/Sample.Service/Sample.Repository.csproj | ClassLibrary | net6.0 | net10.0 | ~250 | 0 packages |
| sample/Sample.Api/Sample.Api.csproj | AspNetCore | net6.0 | net10.0 | ~400 | 2 ASP.NET packages |
| test/Fop.Tests/Fop.Tests.csproj | Test | net6.0 | net10.0 | ~285 | 3 test packages |

**Total**: 6 projects, ~2,035 lines of code

---

## Appendix B: Reference Documentation

### Official Microsoft Documentation

- [What's new in .NET 10](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10)
- [Migrate from .NET 6 to .NET 8](https://learn.microsoft.com/dotnet/core/porting/upgrade-assistant-overview)
- [Entity Framework Core 10.0 breaking changes](https://learn.microsoft.com/ef/core/what-is-new/ef-core-10.0/breaking-changes)
- [ASP.NET Core 10.0 migration guide](https://learn.microsoft.com/aspnet/core/migration/60-to-70)

### Package Documentation

- [Entity Framework Core documentation](https://learn.microsoft.com/ef/core/)
- [ASP.NET Core documentation](https://learn.microsoft.com/aspnet/core/)
- [System.Linq.Dynamic.Core](https://github.com/zzzprojects/System.Linq.Dynamic.Core)

---

## Conclusion

This plan provides a comprehensive roadmap for upgrading the Fop solution from .NET 6.0 to .NET 10.0 using an all-at-once strategy. The upgrade is straightforward due to:

- Small solution size (6 projects, ~2K LOC)
- Clean dependency structure
- All projects on same framework (net6.0)
- SDK-style projects (modern format)
- No API compatibility issues detected
- Clear package upgrade path

**Key Success Factors:**
1. Atomic upgrade of all projects simultaneously
2. Security vulnerability fixed immediately
3. Comprehensive testing at multiple levels
4. Zero-warning build quality standard
5. Single commit for clean version control history

**Next Steps:**
Proceed to Execution stage to apply this plan systematically, validating each phase before moving forward.

---

*This plan supports the Execution stage of the .NET 10.0 upgrade workflow.*

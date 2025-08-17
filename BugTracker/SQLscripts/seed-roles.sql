-- SQL Server Script to Populate ASP.NET Identity Roles
-- Run this script after your database has been created and migrations applied
-- This script creates the roles used in the BugTracker application

-- Clear existing roles if needed (uncomment if you want to start fresh)
-- DELETE FROM [AspNetUserRoles];
-- DELETE FROM [AspNetRoles];

-- Insert AspNetRoles with the roles used in your BugTracker application
INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES 
    (NEWID(), 'Admin', 'ADMIN', NEWID()),
    (NEWID(), 'Project Manager', 'PROJECT MANAGER', NEWID()),
    (NEWID(), 'Developer', 'DEVELOPER', NEWID()),
    (NEWID(), 'Architect', 'ARCHITECT', NEWID()),
    (NEWID(), 'Demo', 'DEMO', NEWID()),
    (NEWID(), 'Submitter', 'SUBMITTER', NEWID());

-- Verification query to display created roles
SELECT 
    [Id],
    [Name],
    [NormalizedName],
    [ConcurrencyStamp]
FROM [AspNetRoles]
ORDER BY [Name];

PRINT 'Successfully created ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' roles for BugTracker application';
PRINT 'Roles created: Admin, Project Manager, Developer, Architect, Demo, Submitter';
-- SQL Server Script to Create Demo Data for BugTracker Application
-- Run this script after roles have been created and migrations applied
-- This creates a complete demo environment with user, projects, and tickets

-- IMPORTANT: This script assumes the roles have already been created by running seed-roles.sql

BEGIN TRY
    -- Check prerequisites first using IF EXISTS for better scope handling
    IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Demo')
    BEGIN
        PRINT 'ERROR: Demo role not found. Please run seed-roles.sql first to create the required roles.';
        RETURN
    END
    
    IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = 'demouser')
    BEGIN
        PRINT 'ERROR: Demo user with username ''demouser'' not found. Please create the demo user first before running this script.';
        RETURN
    END
    
    -- Prerequisites met, declare variables and proceed
    DECLARE @DemoUserId NVARCHAR(450);
    DECLARE @DemoRoleId NVARCHAR(450);
    DECLARE @Project1Id INT;
    DECLARE @Project2Id INT;
    DECLARE @Project3Id INT;
    
    -- Get the Demo role ID and user ID
    SELECT @DemoRoleId = Id FROM AspNetRoles WHERE Name = 'Demo'
    SELECT @DemoUserId = Id FROM AspNetUsers WHERE UserName = 'demouser'

    -- 2. Assign Demo role to Demo user
    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    VALUES (@DemoUserId, @DemoRoleId);

    -- 3. Create Demo Projects
    INSERT INTO [Projects] ([Title], [Description], [Date], [Demo])
VALUES 
    ('E-Commerce Platform', 'Modern online shopping platform with user authentication, product catalog, shopping cart, and payment integration. Built with ASP.NET Core and Entity Framework.', DATEADD(day, -30, GETDATE()), 1),
    ('Mobile Banking App', 'Secure mobile banking application with account management, transactions, bill payments, and advanced security features including two-factor authentication.', DATEADD(day, -25, GETDATE()), 1),
    ('Inventory Management System', 'Comprehensive warehouse and inventory tracking system with real-time stock monitoring, automated reordering, and detailed reporting capabilities.', DATEADD(day, -20, GETDATE()), 1);

    -- Get the project IDs
    SELECT @Project1Id = Id FROM Projects WHERE Title = 'E-Commerce Platform' AND Demo = 1
    SELECT @Project2Id = Id FROM Projects WHERE Title = 'Mobile Banking App' AND Demo = 1
    SELECT @Project3Id = Id FROM Projects WHERE Title = 'Inventory Management System' AND Demo = 1

    -- 4. Assign Demo User to all Demo Projects
    INSERT INTO [ApplicationUserProject] ([AssignedProjectsId], [AssignedUsersId])
    VALUES 
        (@Project1Id, @DemoUserId),
        (@Project2Id, @DemoUserId),
        (@Project3Id, @DemoUserId);

    -- 5. Create Sample Tickets for Project 1 (E-Commerce Platform)
    INSERT INTO [Tickets] ([Title], [Description], [SubmitterId], [DeveloperId], [Priority], [Status], [Type], [Created], [ProjectId])
    VALUES 
        ('Login page not responsive on mobile', 'The login form is not properly displayed on mobile devices. Users cannot access the username and password fields on screens smaller than 768px.', @DemoUserId, @DemoUserId, 2, 1, 0, DATEADD(day, -15, GETDATE()), @Project1Id),
        ('Add product review feature', 'Implement a 5-star rating system and text reviews for products. Include review moderation and average rating display.', @DemoUserId, NULL, 1, 0, 2, DATEADD(day, -12, GETDATE()), @Project1Id),
        ('Shopping cart total calculation error', 'Cart total is not updating correctly when discount codes are applied. Sometimes shows negative totals.', @DemoUserId, @DemoUserId, 3, 2, 0, DATEADD(day, -10, GETDATE()), @Project1Id),
        ('Implement wishlist functionality', 'Allow users to save products to a wishlist for future purchase. Include sharing capabilities.', @DemoUserId, @DemoUserId, 1, 3, 2, DATEADD(day, -8, GETDATE()), @Project1Id),
        ('Payment gateway timeout issues', 'Payment processing occasionally times out during high traffic periods, causing transaction failures.', @DemoUserId, NULL, 3, 0, 0, DATEADD(day, -5, GETDATE()), @Project1Id);

    -- 6. Create Sample Tickets for Project 2 (Mobile Banking App)
    INSERT INTO [Tickets] ([Title], [Description], [SubmitterId], [DeveloperId], [Priority], [Status], [Type], [Created], [ProjectId])
    VALUES 
    ('Two-factor authentication not working', 'SMS codes for 2FA are not being delivered consistently. Users are locked out of their accounts.', @DemoUserId, @DemoUserId, 3, 1, 0, DATEADD(day, -18, GETDATE()), @Project2Id),
    ('Add biometric login support', 'Implement fingerprint and face recognition login options for enhanced security and user convenience.', @DemoUserId, @DemoUserId, 2, 1, 2, DATEADD(day, -14, GETDATE()), @Project2Id),
    ('Transaction history export feature', 'Allow users to export their transaction history in PDF and CSV formats for record keeping.', @DemoUserId, NULL, 1, 0, 2, DATEADD(day, -11, GETDATE()), @Project2Id),
    ('Balance display incorrect after transfer', 'Account balance sometimes shows outdated information after completing a transfer. Requires manual refresh.', @DemoUserId, @DemoUserId, 2, 3, 0, DATEADD(day, -7, GETDATE()), @Project2Id),
    ('Improve transfer speed performance', 'Optimize the money transfer process to reduce processing time from 5 seconds to under 2 seconds.', @DemoUserId, @DemoUserId, 1, 2, 1, DATEADD(day, -4, GETDATE()), @Project2Id);

-- 7. Create Sample Tickets for Project 3 (Inventory Management)
INSERT INTO [Tickets] ([Title], [Description], [SubmitterId], [DeveloperId], [Priority], [Status], [Type], [Created], [ProjectId])
VALUES 
    ('Barcode scanner integration not working', 'Barcode scanner is not properly integrating with the inventory system. Manual entry required for all items.', @DemoUserId, @DemoUserId, 3, 1, 0, DATEADD(day, -16, GETDATE()), @Project3Id),
    ('Add low stock alert notifications', 'Implement automated email alerts when inventory levels fall below defined thresholds.', @DemoUserId, NULL, 2, 0, 2, DATEADD(day, -13, GETDATE()), @Project3Id),
    ('Report generation performance issues', 'Monthly inventory reports are taking over 10 minutes to generate. Need to optimize database queries.', @DemoUserId, @DemoUserId, 2, 1, 1, DATEADD(day, -9, GETDATE()), @Project3Id),
    ('Multi-location inventory tracking', 'Add support for tracking inventory across multiple warehouse locations with transfer capabilities.', @DemoUserId, @DemoUserId, 1, 3, 2, DATEADD(day, -6, GETDATE()), @Project3Id),
    ('Stock count discrepancy in reports', 'Physical stock counts do not match system records. Investigation needed for data integrity issues.', @DemoUserId, NULL, 2, 0, 0, DATEADD(day, -3, GETDATE()), @Project3Id);

    -- Verification Queries
    PRINT 'Demo data created successfully!';
    PRINT '';
    PRINT 'Found Demo User:';
    SELECT Email, FirstName, LastName FROM AspNetUsers WHERE Id = @DemoUserId

    PRINT '';
    PRINT 'Created Demo Projects:';
    SELECT Id, Title, Demo FROM Projects WHERE Demo = 1

    PRINT '';
    PRINT 'Created Tickets Summary:';
    SELECT 
        p.Title as ProjectName,
        COUNT(t.Id) as TicketCount,
        SUM(CASE WHEN t.Status = 0 THEN 1 ELSE 0 END) as Pending,
        SUM(CASE WHEN t.Status = 1 THEN 1 ELSE 0 END) as InProgress,
        SUM(CASE WHEN t.Status = 2 THEN 1 ELSE 0 END) as Closed,
        SUM(CASE WHEN t.Status = 3 THEN 1 ELSE 0 END) as Finished
    FROM Projects p
    LEFT JOIN Tickets t ON p.Id = t.ProjectId
    WHERE p.Demo = 1
    GROUP BY p.Title

END TRY
BEGIN CATCH
    PRINT 'ERROR: An error occurred while creating demo data.';
    PRINT ERROR_MESSAGE();
END CATCH;
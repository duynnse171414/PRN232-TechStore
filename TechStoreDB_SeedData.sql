USE [TechStoreDB];
GO

SET NOCOUNT ON;
GO

/* Seed dữ liệu mẫu:
   - Role: customer/staff/admin
   - User: admin@techstore.local, staff@techstore.local
   - Customer: 1 record cho customer@techstore.local
   - 3 product + 1 product_image mỗi product

   Mật khẩu '123456789' được hash theo logic BE: SHA256 (UTF-8 bytes) + Base64.
*/

DECLARE @PwdHash nvarchar(1000) = N'FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=';

/* 1) Roles */
IF NOT EXISTS (SELECT 1 FROM dbo.[role] WHERE [name] = N'customer')
    INSERT INTO dbo.[role] ([id], [name], [description]) VALUES (1, N'customer', N'Khach hang');
IF NOT EXISTS (SELECT 1 FROM dbo.[role] WHERE [name] = N'staff')
    INSERT INTO dbo.[role] ([id], [name], [description]) VALUES (2, N'staff', N'Nhan vien');
IF NOT EXISTS (SELECT 1 FROM dbo.[role] WHERE [name] = N'admin')
    INSERT INTO dbo.[role] ([id], [name], [description]) VALUES (3, N'admin', N'Quan tri');

/* 2) Users */
IF NOT EXISTS (SELECT 1 FROM dbo.[user] WHERE [email] = N'admin@techstore.local')
    INSERT INTO dbo.[user] ([email], [password_hash], [role_id], [created_at])
    VALUES (N'admin@techstore.local', @PwdHash, 3, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.[user] WHERE [email] = N'staff@techstore.local')
    INSERT INTO dbo.[user] ([email], [password_hash], [role_id], [created_at])
    VALUES (N'staff@techstore.local', @PwdHash, 2, GETDATE());

DECLARE @CustomerUserEmail nvarchar(255) = N'customer@techstore.local';
DECLARE @CustomerUserId bigint = (SELECT TOP 1 id FROM dbo.[user] WHERE [email] = @CustomerUserEmail);

IF @CustomerUserId IS NULL
BEGIN
    INSERT INTO dbo.[user] ([email], [password_hash], [role_id], [created_at])
    VALUES (@CustomerUserEmail, @PwdHash, 1, GETDATE());
    SET @CustomerUserId = SCOPE_IDENTITY();
END

/* 3) Customer profile */
IF NOT EXISTS (
    SELECT 1
    FROM dbo.customer c
    WHERE c.user_id = @CustomerUserId
)
BEGIN
    INSERT INTO dbo.customer ([user_id], [name], [email], [phone], [created_at])
    VALUES (
        @CustomerUserId,
        N'Nguyen Van A',
        @CustomerUserEmail,
        N'0901234567',
        GETDATE()
    );
END

/* 4) Brands, Categories, PC component types */
/* Brand */
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'ASUS')
    INSERT INTO dbo.brand ([name]) VALUES (N'ASUS');
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'Samsung')
    INSERT INTO dbo.brand ([name]) VALUES (N'Samsung');
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'MSI')
    INSERT INTO dbo.brand ([name]) VALUES (N'MSI');

DECLARE @BrandMainboardId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'ASUS');
DECLARE @BrandSsdId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'Samsung');
DECLARE @BrandGpuId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'MSI');

/* Category */
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'Mainboard')
    INSERT INTO dbo.category ([name]) VALUES (N'Mainboard');
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'SSD')
    INSERT INTO dbo.category ([name]) VALUES (N'SSD');
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'Card man hinh')
    INSERT INTO dbo.category ([name]) VALUES (N'Card man hinh');

DECLARE @CatMainboardId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'Mainboard');
DECLARE @CatSsdId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'SSD');
DECLARE @CatGpuId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'Card man hinh');

/* PC component types (dung cho component_type_id trong product) */
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'Mainboard')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'Mainboard', 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'SSD')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'SSD', 1, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'GPU')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'GPU', 1, 3);

DECLARE @TypeMainboardId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'Mainboard');
DECLARE @TypeSsdId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'SSD');
DECLARE @TypeGpuId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'GPU');

/* 5) Products (3 san pham) */
DECLARE @SkuMainboard nvarchar(100) = N'MB-ASUS-01';
DECLARE @SkuSsd nvarchar(100) = N'SSD-SAMSUNG-01';
DECLARE @SkuGpu nvarchar(100) = N'GPU-MSI-01';

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuMainboard)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'Mainboard ASUS (Mainboard)', @SkuMainboard, N'Mainboard cho PC (demo seed).', N'12 thang',
         @BrandMainboardId, @CatMainboardId, @TypeMainboardId, 25000000, 10, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuSsd)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'SSD Samsung (SSD)', @SkuSsd, N'So lid seeding (demo).', N'24 thang',
         @BrandSsdId, @CatSsdId, @TypeSsdId, 1800000, 20, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuGpu)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'Card man hinh MSI (GPU)', @SkuGpu, N'Card man hinh cho PC (demo seed).', N'12 thang',
         @BrandGpuId, @CatGpuId, @TypeGpuId, 12000000, 5, GETDATE());
END

/* 6) Product images (1 hinh / product) */
DECLARE @ImgMainboard nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/VK8P2XicDcnbDkMwAADQLxoSW6SSZTEZc4lNkeGN1spQpV1cvmq_s7_Zzuv5fmohGNdluaJoWpmo8E6UVJUIF4VokISGXub1wFhDyWk8_k83AgxsFIMqLc5LuGJhuRkgc7p3ALQSlBoWdu2ibl4avx1iQQOlxPm7j2ItWLqtBhNNLAyKQWGohRrdymcONzZD34ShMqY0mBzYX1nnDEQdI3phjzbT_LvJvYM3kx_mxUAG';

DECLARE @ImgSsd nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/QD_Va3icDclJDoIwAADAFwkEo6QkxiAqa5UGUORGC7JECpRGCq_yO_5G5zrfT8V5P-qyXFDC5p4X-YpjupbKkWe8JhLpWnmsur6vabkfdv_TjUsOLILCWwqfDBxOlRsthgmtckgDhdRCozZEb5NpHj4GG5qQOMzw2ZmVbZbCu3ZdzawkHmr8JbIdijXoM_gYAJiaQQHtyxauuLRurE6oS3AwqeFDxKZoAxidrmT6AZVWPvQ';

DECLARE @ImgGpu nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/HpALgnicDcnbEkJAAADQL8JqrWKmaWgjMSh2Jr00rMtqio2dLr6q3-lv6rye74cJwUdTUaqODm8uqlISRQflZhS5aKlM-5sysp7ztmtW9-X_TCssDZcm9SycNEwSGGUED9CpOYXI9dE7YwNAySOKXziIn-erpxe-t7XPUlpYSDuSlz0bTrFOu2gvgSxfQOvgzfONBZgTAjXFRbJLp4ntJsfQEAbEuJ6C5_p-IaqHefMD8uw7rA';

DECLARE @MainboardId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuMainboard);
DECLARE @SsdId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuSsd);
DECLARE @GpuId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuGpu);

IF NOT EXISTS (
    SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @MainboardId AND pi.image_url = @ImgMainboard
)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@MainboardId, @ImgMainboard);

IF NOT EXISTS (
    SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @SsdId AND pi.image_url = @ImgSsd
)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@SsdId, @ImgSsd);

IF NOT EXISTS (
    SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @GpuId AND pi.image_url = @ImgGpu
)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@GpuId, @ImgGpu);

PRINT N'Seed data OK.';
GO


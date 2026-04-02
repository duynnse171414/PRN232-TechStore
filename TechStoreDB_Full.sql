USE [master]
GO
IF DB_ID(N'TechStoreDB') IS NOT NULL
BEGIN
    ALTER DATABASE [TechStoreDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TechStoreDB];
END
GO
/****** Object:  Database [TechStoreDB]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE DATABASE [TechStoreDB]
GO
DECLARE @majorVersion INT = TRY_CAST(SERVERPROPERTY('ProductMajorVersion') AS INT);
IF @majorVersion >= 15
    ALTER DATABASE [TechStoreDB] SET COMPATIBILITY_LEVEL = 150;
ELSE
    ALTER DATABASE [TechStoreDB] SET COMPATIBILITY_LEVEL = 140;
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TechStoreDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TechStoreDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TechStoreDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TechStoreDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TechStoreDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TechStoreDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [TechStoreDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TechStoreDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TechStoreDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TechStoreDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TechStoreDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TechStoreDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TechStoreDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TechStoreDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TechStoreDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TechStoreDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TechStoreDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TechStoreDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TechStoreDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TechStoreDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TechStoreDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TechStoreDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TechStoreDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TechStoreDB] SET RECOVERY FULL 
GO
ALTER DATABASE [TechStoreDB] SET  MULTI_USER 
GO
ALTER DATABASE [TechStoreDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TechStoreDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TechStoreDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TechStoreDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TechStoreDB] SET DELAYED_DURABILITY = DISABLED 
GO
-- OPTIMIZED_LOCKING can throw syntax errors on older SQL Server engines.
GO
ALTER DATABASE [TechStoreDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'TechStoreDB', N'ON'
GO
ALTER DATABASE [TechStoreDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [TechStoreDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TechStoreDB]
GO
/****** Object:  Table [dbo].[address]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[address](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[customer_id] [bigint] NOT NULL,
	[address_line] [nvarchar](500) NULL,
	[city] [nvarchar](100) NULL,
	[district] [nvarchar](100) NULL,
	[ward] [nvarchar](100) NULL,
	[is_default] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[brand]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[brand](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[cart]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cart](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[cart_item]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cart_item](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[cart_id] [bigint] NOT NULL,
	[product_id] [bigint] NOT NULL,
	[quantity] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[category]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[category](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[customer]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customer](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NULL,
	[name] [nvarchar](255) NOT NULL,
	[email] [nvarchar](255) NULL,
	[phone] [nvarchar](50) NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[order_item]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order_item](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[order_id] [bigint] NOT NULL,
	[product_id] [bigint] NOT NULL,
	[quantity] [int] NULL,
	[price] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[orders]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orders](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[customer_id] [bigint] NOT NULL,
	[address_id] [bigint] NULL,
	[voucher_id] [bigint] NULL,
	[status] [nvarchar](50) NULL,
	[total_amount] [decimal](18, 2) NULL,
	[discount_amount] [decimal](18, 2) NULL,
	[shipping_fee] [decimal](18, 2) NULL,
	[notes] [nvarchar](500) NULL,
	[tracking_number] [nvarchar](100) NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[payment]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payment](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[order_id] [bigint] NOT NULL,
	[method] [nvarchar](50) NULL,
	[status] [nvarchar](50) NULL,
	[paid_at] [datetime] NULL,
	[amount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK__payment__3213E83F061E10C7] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pc_build]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pc_build](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NULL,
	[name] [nvarchar](255) NULL,
	[total_price] [decimal](18, 2) NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pc_build_item]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pc_build_item](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[build_id] [bigint] NOT NULL,
	[component_type_id] [bigint] NOT NULL,
	[product_id] [bigint] NOT NULL,
	[quantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pc_component_type]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pc_component_type](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[is_required] [bit] NOT NULL,
	[sort_order] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[sku] [nvarchar](100) NULL,
	[description] [nvarchar](max) NULL,
	[warranty] [nvarchar](500) NULL,
	[brand_id] [bigint] NULL,
	[category_id] [bigint] NULL,
	[component_type_id] [bigint] NULL,
	[price] [decimal](18, 2) NULL,
	[stock] [int] NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product_image]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_image](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_id] [bigint] NOT NULL,
	[image_url] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product_promotion]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_promotion](
	[product_id] [bigint] NOT NULL,
	[promotion_id] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[promotion_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product_spec]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_spec](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_id] [bigint] NOT NULL,
	[spec_key] [nvarchar](100) NULL,
	[spec_value] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[promotion]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[promotion](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[discount_percent] [int] NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[voucher]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[voucher](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[name] [nvarchar](255) NULL,
	[discount_percent] [int] NULL,
	[max_discount] [decimal](18, 2) NULL,
	[min_order_amount] [decimal](18, 2) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[is_active] [bit] NOT NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_voucher_code] UNIQUE NONCLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[role]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[role](
	[id] [int] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_role_name] UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user]    Script Date: 3/12/2026 4:45:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[password_hash] [nvarchar](500) NOT NULL,
	[role_id] [int] NOT NULL,
	[is_active] [bit] NOT NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_user_email] UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_address_customer_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_address_customer_id] ON [dbo].[address]
(
	[customer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_cart_user_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_cart_user_id] ON [dbo].[cart]
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_cart_item_cart_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_cart_item_cart_id] ON [dbo].[cart_item]
(
	[cart_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_cart_item_product_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_cart_item_product_id] ON [dbo].[cart_item]
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_customer_user_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_customer_user_id] ON [dbo].[customer]
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_order_item_order_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_order_item_order_id] ON [dbo].[order_item]
(
	[order_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_order_item_product_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_order_item_product_id] ON [dbo].[order_item]
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_orders_address_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_orders_address_id] ON [dbo].[orders]
(
	[address_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_orders_voucher_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_orders_voucher_id] ON [dbo].[orders]
(
	[voucher_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_orders_created_at]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_orders_created_at] ON [dbo].[orders]
(
	[created_at] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_orders_customer_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_orders_customer_id] ON [dbo].[orders]
(
	[customer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_orders_status]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_orders_status] ON [dbo].[orders]
(
	[status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_payment_order_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_payment_order_id] ON [dbo].[payment]
(
	[order_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_product_brand_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_product_brand_id] ON [dbo].[product]
(
	[brand_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_product_category_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_product_category_id] ON [dbo].[product]
(
	[category_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_product_component_type_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_product_component_type_id] ON [dbo].[product]
(
	[component_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_product_image_product_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_product_image_product_id] ON [dbo].[product_image]
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_product_spec_product_id]    Script Date: 3/12/2026 4:45:32 PM ******/
CREATE NONCLUSTERED INDEX [IX_product_spec_product_id] ON [dbo].[product_spec]
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[address] ADD  CONSTRAINT [DF_address_is_default]  DEFAULT ((0)) FOR [is_default]
GO
ALTER TABLE [dbo].[cart] ADD  CONSTRAINT [DF_cart_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[cart_item] ADD  CONSTRAINT [DF_cart_item_quantity]  DEFAULT ((1)) FOR [quantity]
GO
ALTER TABLE [dbo].[customer] ADD  CONSTRAINT [DF_customer_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[order_item] ADD  CONSTRAINT [DF_order_item_quantity]  DEFAULT ((1)) FOR [quantity]
GO
ALTER TABLE [dbo].[orders] ADD  CONSTRAINT [DF_orders_status]  DEFAULT ('pending') FOR [status]
GO
ALTER TABLE [dbo].[orders] ADD  CONSTRAINT [DF_orders_discount_amount]  DEFAULT ((0)) FOR [discount_amount]
GO
ALTER TABLE [dbo].[orders] ADD  CONSTRAINT [DF_orders_shipping_fee]  DEFAULT ((0)) FOR [shipping_fee]
GO
ALTER TABLE [dbo].[orders] ADD  CONSTRAINT [DF_orders_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[payment] ADD  CONSTRAINT [DF_payment_status]  DEFAULT ('pending') FOR [status]
GO
ALTER TABLE [dbo].[voucher] ADD  CONSTRAINT [DF_voucher_is_active]  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[voucher] ADD  CONSTRAINT [DF_voucher_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[pc_build] ADD  CONSTRAINT [DF_pcbuild_created]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[pc_build_item] ADD  CONSTRAINT [DF_pbi_qty]  DEFAULT ((1)) FOR [quantity]
GO
ALTER TABLE [dbo].[pc_component_type] ADD  CONSTRAINT [DF_pct_required]  DEFAULT ((1)) FOR [is_required]
GO
ALTER TABLE [dbo].[pc_component_type] ADD  CONSTRAINT [DF_pct_sort]  DEFAULT ((0)) FOR [sort_order]
GO
ALTER TABLE [dbo].[product] ADD  CONSTRAINT [DF_product_stock]  DEFAULT ((0)) FOR [stock]
GO
ALTER TABLE [dbo].[product] ADD  CONSTRAINT [DF_product_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_user_role_id]  DEFAULT ((1)) FOR [role_id]
GO
ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_user_is_active]  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_user_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[address]  WITH CHECK ADD  CONSTRAINT [FK_address_customer] FOREIGN KEY([customer_id])
REFERENCES [dbo].[customer] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[address] CHECK CONSTRAINT [FK_address_customer]
GO
ALTER TABLE [dbo].[cart]  WITH CHECK ADD  CONSTRAINT [FK_cart_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[cart] CHECK CONSTRAINT [FK_cart_user]
GO
ALTER TABLE [dbo].[cart_item]  WITH CHECK ADD  CONSTRAINT [FK_cart_item_cart] FOREIGN KEY([cart_id])
REFERENCES [dbo].[cart] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[cart_item] CHECK CONSTRAINT [FK_cart_item_cart]
GO
ALTER TABLE [dbo].[cart_item]  WITH CHECK ADD  CONSTRAINT [FK_cart_item_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[cart_item] CHECK CONSTRAINT [FK_cart_item_product]
GO
ALTER TABLE [dbo].[customer]  WITH CHECK ADD  CONSTRAINT [FK_customer_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[customer] CHECK CONSTRAINT [FK_customer_user]
GO
ALTER TABLE [dbo].[order_item]  WITH CHECK ADD  CONSTRAINT [FK_order_item_order] FOREIGN KEY([order_id])
REFERENCES [dbo].[orders] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[order_item] CHECK CONSTRAINT [FK_order_item_order]
GO
ALTER TABLE [dbo].[order_item]  WITH CHECK ADD  CONSTRAINT [FK_order_item_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
GO
ALTER TABLE [dbo].[order_item] CHECK CONSTRAINT [FK_order_item_product]
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD  CONSTRAINT [FK_orders_address] FOREIGN KEY([address_id])
REFERENCES [dbo].[address] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[orders] CHECK CONSTRAINT [FK_orders_address]
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD  CONSTRAINT [FK_orders_customer] FOREIGN KEY([customer_id])
REFERENCES [dbo].[customer] ([id])
GO
ALTER TABLE [dbo].[orders] CHECK CONSTRAINT [FK_orders_customer]
GO
ALTER TABLE [dbo].[payment]  WITH CHECK ADD  CONSTRAINT [FK_payment_order] FOREIGN KEY([order_id])
REFERENCES [dbo].[orders] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[payment] CHECK CONSTRAINT [FK_payment_order]
GO
ALTER TABLE [dbo].[pc_build]  WITH CHECK ADD  CONSTRAINT [FK_pcbuild_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[pc_build] CHECK CONSTRAINT [FK_pcbuild_user]
GO
ALTER TABLE [dbo].[pc_build_item]  WITH CHECK ADD  CONSTRAINT [FK_pbi_build] FOREIGN KEY([build_id])
REFERENCES [dbo].[pc_build] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[pc_build_item] CHECK CONSTRAINT [FK_pbi_build]
GO
ALTER TABLE [dbo].[pc_build_item]  WITH CHECK ADD  CONSTRAINT [FK_pbi_component_type] FOREIGN KEY([component_type_id])
REFERENCES [dbo].[pc_component_type] ([id])
GO
ALTER TABLE [dbo].[pc_build_item] CHECK CONSTRAINT [FK_pbi_component_type]
GO
ALTER TABLE [dbo].[pc_build_item]  WITH CHECK ADD  CONSTRAINT [FK_pbi_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
GO
ALTER TABLE [dbo].[pc_build_item] CHECK CONSTRAINT [FK_pbi_product]
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD  CONSTRAINT [FK_product_brand] FOREIGN KEY([brand_id])
REFERENCES [dbo].[brand] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[product] CHECK CONSTRAINT [FK_product_brand]
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD  CONSTRAINT [FK_product_category] FOREIGN KEY([category_id])
REFERENCES [dbo].[category] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[product] CHECK CONSTRAINT [FK_product_category]
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD  CONSTRAINT [FK_product_component_type] FOREIGN KEY([component_type_id])
REFERENCES [dbo].[pc_component_type] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[product] CHECK CONSTRAINT [FK_product_component_type]
GO
ALTER TABLE [dbo].[product_image]  WITH CHECK ADD  CONSTRAINT [FK_product_image_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[product_image] CHECK CONSTRAINT [FK_product_image_product]
GO
ALTER TABLE [dbo].[product_promotion]  WITH CHECK ADD  CONSTRAINT [FK_product_promotion_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[product_promotion] CHECK CONSTRAINT [FK_product_promotion_product]
GO
ALTER TABLE [dbo].[product_promotion]  WITH CHECK ADD  CONSTRAINT [FK_product_promotion_promotion] FOREIGN KEY([promotion_id])
REFERENCES [dbo].[promotion] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[product_promotion] CHECK CONSTRAINT [FK_product_promotion_promotion]
GO
ALTER TABLE [dbo].[product_spec]  WITH CHECK ADD  CONSTRAINT [FK_product_spec_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[product_spec] CHECK CONSTRAINT [FK_product_spec_product]
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD  CONSTRAINT [FK_orders_voucher] FOREIGN KEY([voucher_id])
REFERENCES [dbo].[voucher] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[orders] CHECK CONSTRAINT [FK_orders_voucher]
GO
ALTER TABLE [dbo].[user]  WITH CHECK ADD  CONSTRAINT [FK_user_role] FOREIGN KEY([role_id])
REFERENCES [dbo].[role] ([id])
GO
ALTER TABLE [dbo].[user] CHECK CONSTRAINT [FK_user_role]
GO
USE [master]
GO
ALTER DATABASE [TechStoreDB] SET  READ_WRITE 
GO

/* ==================== SEED DATA ==================== */
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
    INSERT INTO dbo.[user] ([email], [password_hash], [role_id], [is_active], [created_at])
    VALUES (N'admin@techstore.local', @PwdHash, 3, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.[user] WHERE [email] = N'staff@techstore.local')
    INSERT INTO dbo.[user] ([email], [password_hash], [role_id], [is_active], [created_at])
    VALUES (N'staff@techstore.local', @PwdHash, 2, 1, GETDATE());

DECLARE @CustomerUserEmail nvarchar(255) = N'customer@techstore.local';
DECLARE @CustomerUserId bigint = (SELECT TOP 1 id FROM dbo.[user] WHERE [email] = @CustomerUserEmail);

IF @CustomerUserId IS NULL
BEGIN
    INSERT INTO dbo.[user] ([email], [password_hash], [role_id], [is_active], [created_at])
    VALUES (@CustomerUserEmail, @PwdHash, 1, 1, GETDATE());
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

/* 4) Voucher */
IF NOT EXISTS (SELECT 1 FROM dbo.voucher WHERE [code] = N'WELCOME10')
BEGIN
    INSERT INTO dbo.voucher ([code], [name], [discount_percent], [max_discount], [min_order_amount], [start_date], [end_date], [is_active], [created_at])
    VALUES (
        N'WELCOME10',
        N'Giam 10% don dau',
        10,
        1000000,
        500000,
        DATEADD(DAY, -1, GETDATE()),
        DATEADD(MONTH, 6, GETDATE()),
        1,
        GETDATE()
    );
END

/* 5) Brands, Categories, PC component types */
/* Brand */
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'ASUS')
    INSERT INTO dbo.brand ([name]) VALUES (N'ASUS');
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'Samsung')
    INSERT INTO dbo.brand ([name]) VALUES (N'Samsung');
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'MSI')
    INSERT INTO dbo.brand ([name]) VALUES (N'MSI');
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'Intel')
    INSERT INTO dbo.brand ([name]) VALUES (N'Intel');
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'Corsair')
    INSERT INTO dbo.brand ([name]) VALUES (N'Corsair');
IF NOT EXISTS (SELECT 1 FROM dbo.brand WHERE [name] = N'Cooler Master')
    INSERT INTO dbo.brand ([name]) VALUES (N'Cooler Master');

DECLARE @BrandMainboardId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'ASUS');
DECLARE @BrandSsdId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'Samsung');
DECLARE @BrandGpuId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'MSI');
DECLARE @BrandCpuId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'Intel');
DECLARE @BrandRamId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'Corsair');
DECLARE @BrandPsuId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'Cooler Master');
DECLARE @BrandCaseId bigint = (SELECT TOP 1 id FROM dbo.brand WHERE [name] = N'Cooler Master');

/* Category */
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'CPU')
    INSERT INTO dbo.category ([name]) VALUES (N'CPU');
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'GPU')
    INSERT INTO dbo.category ([name]) VALUES (N'GPU');
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'RAM')
    INSERT INTO dbo.category ([name]) VALUES (N'RAM');
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'SSD')
    INSERT INTO dbo.category ([name]) VALUES (N'SSD');
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'Mainboard')
    INSERT INTO dbo.category ([name]) VALUES (N'Mainboard');
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'PSU')
    INSERT INTO dbo.category ([name]) VALUES (N'PSU');
IF NOT EXISTS (SELECT 1 FROM dbo.category WHERE [name] = N'Case')
    INSERT INTO dbo.category ([name]) VALUES (N'Case');

DECLARE @CatCpuId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'CPU');
DECLARE @CatGpuId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'GPU');
DECLARE @CatRamId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'RAM');
DECLARE @CatSsdId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'SSD');
DECLARE @CatMainboardId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'Mainboard');
DECLARE @CatPsuId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'PSU');
DECLARE @CatCaseId bigint = (SELECT TOP 1 id FROM dbo.category WHERE [name] = N'Case');

/* PC component types (build PC 7 thanh phan bat buoc) */
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'CPU')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'CPU', 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'GPU')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'GPU', 1, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'RAM')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'RAM', 1, 3);
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'SSD')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'SSD', 1, 4);
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'Mainboard')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'Mainboard', 1, 5);
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'PSU')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'PSU', 1, 6);
IF NOT EXISTS (SELECT 1 FROM dbo.pc_component_type WHERE [name] = N'Case')
    INSERT INTO dbo.pc_component_type ([name], [is_required], [sort_order]) VALUES (N'Case', 1, 7);

DECLARE @TypeCpuId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'CPU');
DECLARE @TypeGpuId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'GPU');
DECLARE @TypeRamId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'RAM');
DECLARE @TypeSsdId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'SSD');
DECLARE @TypeMainboardId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'Mainboard');
DECLARE @TypePsuId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'PSU');
DECLARE @TypeCaseId bigint = (SELECT TOP 1 id FROM dbo.pc_component_type WHERE [name] = N'Case');

/* 6) Products (7 san pham theo 7 component) */
DECLARE @SkuCpu nvarchar(100) = N'CPU-INTEL-01';
DECLARE @SkuGpu nvarchar(100) = N'GPU-MSI-01';
DECLARE @SkuRam nvarchar(100) = N'RAM-CORSAIR-01';
DECLARE @SkuSsd nvarchar(100) = N'SSD-SAMSUNG-01';
DECLARE @SkuMainboard nvarchar(100) = N'MB-ASUS-01';
DECLARE @SkuPsu nvarchar(100) = N'PSU-CM-01';
DECLARE @SkuCase nvarchar(100) = N'CASE-CM-01';

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuCpu)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'CPU Intel Core (CPU)', @SkuCpu, N'CPU cho PC (demo seed).', N'36 thang',
         @BrandCpuId, @CatCpuId, @TypeCpuId, 6500000, 15, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuGpu)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'Card đồ họa MSI (GPU)', @SkuGpu, N'Card đồ họa cho PC (demo seed).', N'12 thang',
         @BrandGpuId, @CatGpuId, @TypeGpuId, 12000000, 5, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuRam)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'RAM Corsair DDR5 (RAM)', @SkuRam, N'RAM cho PC (demo seed).', N'36 thang',
         @BrandRamId, @CatRamId, @TypeRamId, 2200000, 30, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuSsd)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'SSD Samsung (SSD)', @SkuSsd, N'SSD cho PC (demo seed).', N'24 thang',
         @BrandSsdId, @CatSsdId, @TypeSsdId, 1800000, 20, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuMainboard)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'Mainboard ASUS (Mainboard)', @SkuMainboard, N'Mainboard cho PC (demo seed).', N'12 thang',
         @BrandMainboardId, @CatMainboardId, @TypeMainboardId, 4500000, 10, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuPsu)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'Nguồn Cooler Master 650W (PSU)', @SkuPsu, N'Nguồn máy tính (demo seed).', N'24 thang',
         @BrandPsuId, @CatPsuId, @TypePsuId, 1900000, 18, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE sku = @SkuCase)
BEGIN
    INSERT INTO dbo.product
        ([name], [sku], [description], [warranty], [brand_id], [category_id], [component_type_id], [price], [stock], [created_at])
    VALUES
        (N'Vỏ case Cooler Master (Case)', @SkuCase, N'Vỏ case cho PC (demo seed).', N'12 thang',
         @BrandCaseId, @CatCaseId, @TypeCaseId, 1500000, 12, GETDATE());
END

/* 7) Product images (1 hinh / product) */
DECLARE @ImgCpu nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/zC0GHXicDclJDoIwAADAFwmiEpHEGBbLKhCrELgYLasEKLRQ8VH-x9_oXOf7KSnFROb5rEXDjGmWLuijXXIFoXdaIQ51DU_KDuOqLQ79_n-y4qU7A0HpekxYXavIB1EBJCldRb0AcZ1Dx-9r8ak1lmHFOXpjPM4ABtgUFt02ZLlCNiQEc1yeJDAHVPdvMVPEtlzrr7M7BcogXJDVOPZgqpMXaVBPiBUyaodj5_4ANrg-jA';

DECLARE @ImgGpu nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/d7m62nicDcndCoIwGADQJ8q_EakQYaYSwsKWhFdiU3TKdG6f1XySXq23qXN7vp8OQCjfNJuRSi2gqTfwGC2jVVABowaduKm6SQg2tod5_z8_wLWXUIJOd6mpw2BGuhxmaYWpKj1cMcGiyl6eHdktOj-Wt-j8btWUuhNfswyvCS-CO0_7vC-I61xjieyQQDCEQTygLQfMiuQCrx9Xpjdd';

DECLARE @ImgRam nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/9A5cTXicDcndCoIwGADQJ5quIkshwsqiSHFqpt6IzeHm78wvqZ6q1-lt6tye74cDyMFQVdbS-0sCyxHc2olSDJCBoArtGnXgnZSiLdb96n-G6eT6gXq2XWJzgL3n56iq57hv8O34tGRK7Evs6dPkGfsEWa8suIYojSZxvXR9ws-uE5XNHtyZ0KiWjgljVRRmmVO8-bsSNSAi6s0Jj7MdxpX2WIiAB-WWBc4h7a2u-AGB7j6U';

DECLARE @ImgSsd nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/9A5cTXicDcndCoIwGADQJ5quIkshwsqiSHFqpt6IzeHm78wvqZ6q1-lt6tye74cDyMFQVdbS-0sCyxHc2olSDJCBoArtGnXgnZSiLdb96n-G6eT6gXq2XWJzgL3n56iq57hv8O34tGRK7Evs6dPkGfsEWa8suIYojSZxvXR9ws-uE5XNHtyZ0KiWjgljVRRmmVO8-bsSNSAi6s0Jj7MdxpX2WIiAB-WWBc4h7a2u-AGB7j6U';

DECLARE @ImgMainboard nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/MzVgpHicDclJDoIwAADAFwnIomJiDDEEBKWWsJ5IKVsRsNpigEf5H3-jc53vp-Gcsr0olgN-zZSXxYrngyzUjCNOsIAfvciaB6VkqI_Pw__2hlfoFoZ-XCF_UFNQ2FYO4YaQFVIoiLUpBVHi9aDq7N73FF0mmh3cLByp124bXvzsJC1SWjuJvqCZ0XTata3jMHMMI4ubCLjKtD7Lb8ZN1wjvAfRksyNwfGdBGP8A7Y49fQ';

DECLARE @ImgPsu nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/s3Yl5nicDcndEoFAGADQJ6qUNNWMMZoII0Oin7u11SLbrt2vlKfyOt6Gc3u-nysAl66mlQ0WA4eyUODSGCqRgOCGVcyoJq-M81tDZs_p_9z5rnACfCBIMQUJV3OPwsLhQ6onDAjpIxQvN69t9kK0qtYsammY7EVn98zPM5RIMOvxhPq4oF3qTN5G7Ys-F611PhwT_W6PqLRMEefho1iepCEzsHUaSG-w2rQ0CPkBDzBAFw';

DECLARE @ImgCase nvarchar(1000) =
N'https://images.openai.com/thumbnails/url/Ss8qYHicDclRCoIwAADQE6UyhjMhwpTUUZo4Eb9Cp7mJOs0tseN0q25T7_d9P0zKabF1vRnpc5tkU-9kNQKtXWQpOdWoGPSFiWniY3ucD_-znaje-5RsBTHw6itPZEHXX1mY4Hcjum4W4AFByk1rs2YFCWFQQs-x1jXDaOBxift0Se95laPEgQBkpVm4gXHKopdB8eUUdreNoFilSLnnpFDVD0s7OSE';

DECLARE @CpuId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuCpu);
DECLARE @GpuId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuGpu);
DECLARE @RamId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuRam);
DECLARE @SsdId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuSsd);
DECLARE @MainboardId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuMainboard);
DECLARE @PsuId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuPsu);
DECLARE @CaseId bigint = (SELECT TOP 1 id FROM dbo.product WHERE sku = @SkuCase);

IF NOT EXISTS (SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @CpuId AND pi.image_url = @ImgCpu)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@CpuId, @ImgCpu);

IF NOT EXISTS (SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @GpuId AND pi.image_url = @ImgGpu)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@GpuId, @ImgGpu);

IF NOT EXISTS (SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @RamId AND pi.image_url = @ImgRam)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@RamId, @ImgRam);

IF NOT EXISTS (SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @SsdId AND pi.image_url = @ImgSsd)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@SsdId, @ImgSsd);

IF NOT EXISTS (SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @MainboardId AND pi.image_url = @ImgMainboard)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@MainboardId, @ImgMainboard);

IF NOT EXISTS (SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @PsuId AND pi.image_url = @ImgPsu)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@PsuId, @ImgPsu);

IF NOT EXISTS (SELECT 1 FROM dbo.product_image pi WHERE pi.product_id = @CaseId AND pi.image_url = @ImgCase)
    INSERT INTO dbo.product_image ([product_id], [image_url]) VALUES (@CaseId, @ImgCase);

PRINT N'Seed data OK.';
GO

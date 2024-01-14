USE [master]
GO
/****** Object:  Database [SWP1]    Script Date: 1/11/2024 5:15:05 PM ******/
CREATE DATABASE [SWP]
GO
USE [SWP]
GO
/****** Object:  Table [dbo].[Blog]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Blog](
	[BlogId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Content] [nvarchar](max) NULL,
	[Image] [nvarchar](255) NULL,
	[CreateDate] [datetime] NULL,
	[Status] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK__Blog__54379E30945B471F] PRIMARY KEY CLUSTERED 
(
	[BlogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Brand]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brand](
	[BrandId] [int] IDENTITY(1,1) NOT NULL,
	[BrandName] [nvarchar](255) NULL,
	[Status] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdateBy] [nvarchar](255) NULL,
 CONSTRAINT [PK__Brand__DAD4F05EC1936766] PRIMARY KEY CLUSTERED 
(
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](255) NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdateBy] [nvarchar](255) NULL,
	[Status] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK__Category__19093A0B1C8CFF17] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Color]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Color](
	[ColorId] [int] IDENTITY(1,1) NOT NULL,
	[ColorName] [nvarchar](255) NULL,
	[Status] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK__Color__8DA7674DE061313B] PRIMARY KEY CLUSTERED 
(
	[ColorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeedBack]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeedBack](
	[FeedBackID] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[DetailId] [int] NULL,
	[Comment] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK__FeedBack__E2CB3867B83C8815] PRIMARY KEY CLUSTERED 
(
	[FeedBackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CustomerName] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[CustomerEmail] [nvarchar](255) NULL,
	[CustomerPhoneNumber] [nvarchar](20) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Status] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK__Order__C3905BCF8B173F23] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orderdetails]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orderdetails](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[DetailId] [int] NULL,
	[Quantity] [int] NULL,
	[TotalPrice] [decimal](18, 2) NULL,
	[Discount] [decimal](18, 2) NULL,
	[Status] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdateBy] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductDetail]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductDetail](
	[DetailId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[SizeId] [int] NULL,
	[ColorId] [int] NULL,
	[Quantity] [int] NULL,
	[Status] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdateBy] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK__ProductD__135C316D80D9C867] PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductImage]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductImage](
	[ProductImageId] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NULL,
	[Status] [int] NULL,
	[CreateDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdateBy] [nvarchar](255) NULL,
 CONSTRAINT [PK__ProductI__07B2B1B895F9D470] PRIMARY KEY CLUSTERED 
(
	[ProductImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[BrandId] [int] NOT NULL,
	[Price] [decimal](18, 2) NULL,
	[Quantity] [int] NULL,
	[CategoryId] [int] NOT NULL,
	[IsSale] [bit] NULL,
	[Status] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdateBy] [nvarchar](255) NULL,
 CONSTRAINT [PK__Products__B40CC6CDAA4A365F] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](255) NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK__Role__8AFACE1A87A339F8] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Size]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Size](
	[SizeID] [int] IDENTITY(1,1) NOT NULL,
	[SizeName] [nvarchar](255) NULL,
	[Status] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdateBy] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK__Size__83BD095A364A0891] PRIMARY KEY CLUSTERED 
(
	[SizeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 1/11/2024 5:15:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[Email] [nvarchar](255) NULL,
	[Password] [nvarchar](255) NULL,
	[Address] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[FirstName] [nchar](10) NULL,
	[LastName] [nvarchar](255) NULL,
	[Gender] [bit] NULL,
	[Status] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[Image] [nvarchar](255) NULL,
	[otp] [nvarchar](50) NULL,
	[otp_expired] [Datetime] NULL
 CONSTRAINT [PK__User__1788CC4C49576765] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Blog]  WITH CHECK ADD  CONSTRAINT [FK_Blog_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Blog] CHECK CONSTRAINT [FK_Blog_User]
GO
ALTER TABLE [dbo].[FeedBack]  WITH CHECK ADD  CONSTRAINT [FK_FeedBack_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[FeedBack] CHECK CONSTRAINT [FK_FeedBack_User]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_User]
GO
ALTER TABLE [dbo].[Orderdetails]  WITH CHECK ADD  CONSTRAINT [FK_Orderdetails_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([OrderId])
GO
ALTER TABLE [dbo].[Orderdetails] CHECK CONSTRAINT [FK_Orderdetails_Order]
GO
ALTER TABLE [dbo].[Orderdetails]  WITH CHECK ADD  CONSTRAINT [FK_Orderdetails_ProductDetail] FOREIGN KEY([DetailId])
REFERENCES [dbo].[ProductDetail] ([DetailId])
GO
ALTER TABLE [dbo].[Orderdetails] CHECK CONSTRAINT [FK_Orderdetails_ProductDetail]
GO
ALTER TABLE [dbo].[ProductDetail]  WITH CHECK ADD  CONSTRAINT [FK_ProductDetail_Color] FOREIGN KEY([ColorId])
REFERENCES [dbo].[Color] ([ColorId])
GO
ALTER TABLE [dbo].[ProductDetail] CHECK CONSTRAINT [FK_ProductDetail_Color]
GO
ALTER TABLE [dbo].[ProductDetail]  WITH CHECK ADD  CONSTRAINT [FK_ProductDetail_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[ProductDetail] CHECK CONSTRAINT [FK_ProductDetail_Products]
GO
ALTER TABLE [dbo].[ProductDetail]  WITH CHECK ADD  CONSTRAINT [FK_ProductDetail_Size] FOREIGN KEY([SizeId])
REFERENCES [dbo].[Size] ([SizeID])
GO
ALTER TABLE [dbo].[ProductDetail] CHECK CONSTRAINT [FK_ProductDetail_Size]
GO
ALTER TABLE [dbo].[ProductImage]  WITH CHECK ADD  CONSTRAINT [FK_ProductImage_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[ProductImage] CHECK CONSTRAINT [FK_ProductImage_Products]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Brand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brand] ([BrandId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Brand]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Category]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role]
GO
USE [master]
GO
ALTER DATABASE [SWP] SET  READ_WRITE 
GO

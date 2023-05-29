USE [EventmanagerNew]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AddressType] [int] NULL,
	[AddressLine] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Country_Id] [int] NULL,
	[ZipCode] [nvarchar](50) NULL,
	[IsDefault] [bit] NULL,
	[FullName] [varchar](200) NULL,
	[Landmark] [nvarchar](max) NULL,
	[latitude] [nvarchar](50) NULL,
	[longitude] [nvarchar](50) NULL,
	[AddressArea] [nvarchar](200) NULL,
 CONSTRAINT [PK_AddressLineUserMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminSetup]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminSetup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventApprovalTime] [int] NULL,
	[AutoTransferMinAmount] [decimal](18, 2) NULL,
	[AutoTransferTime] [int] NULL,
	[PlatinumRow] [int] NULL,
	[GoldRow] [int] NULL,
	[SilverRow] [int] NULL,
	[BronzeRow] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssignUssd]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignUssd](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Company_Id] [int] NULL,
	[Ussd_Code] [varchar](200) NULL,
	[IsDeleted] [bit] NULL,
	[Event_Name] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EventId] [int] NULL,
	[EventCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_AssignUssd] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AWSStreamingPage]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AWSStreamingPage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SiteName] [varchar](100) NULL,
	[SiteLogo] [varchar](max) NULL,
	[ServerUrl] [varchar](max) NULL,
	[StreamKey] [varchar](50) NULL,
	[ChannelId] [int] NULL,
	[IsStreaming] [bit] NULL,
	[StreamStartDate] [datetime] NULL,
	[StreamStopDate] [datetime] NULL,
	[ChannelForWebSite] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BankDetails]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BankName] [varchar](max) NULL,
	[Type] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_BankDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BankDetalMapping]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankDetalMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BankDetail_Id] [int] NULL,
	[MobileMoneyDetail_Id] [int] NULL,
	[Mobile_Money_Unique] [nvarchar](max) NULL,
	[AccountHolder] [nvarchar](max) NULL,
	[AccountNumber] [nvarchar](max) NULL,
	[BankRegNo] [nvarchar](max) NULL,
	[Company_Id] [int] NULL,
	[UserId] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_BankDetalMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BannerTimer]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BannerTimer](
	[Id] [int] NOT NULL,
	[ImagePath] [nvarchar](max) NULL,
	[StartTime] [varchar](10) NULL,
	[EndTime] [varchar](10) NULL,
	[IsTimeEnable] [int] NULL,
	[IsImageEnble] [int] NULL,
	[AddDate] [datetime] NULL,
	[StartDate] [varchar](20) NULL,
	[EndDate] [varchar](20) NULL,
	[UserID] [varchar](50) NULL,
	[Bannerforwebsite] [int] NULL,
	[MobileImagePath] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BoughtTickets]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BoughtTickets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Guid_Id] [nvarchar](50) NOT NULL,
	[TicketCode] [nvarchar](50) NOT NULL,
	[TicketName] [nvarchar](50) NOT NULL,
	[EventDate] [nvarchar](50) NOT NULL,
	[TicketDate] [nvarchar](50) NOT NULL,
	[TicketBookedAt] [nvarchar](50) NOT NULL,
	[Venue] [varchar](50) NULL,
	[BarcodeNumber] [nvarchar](50) NOT NULL,
	[Address] [varchar](50) NULL,
	[Event] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK_BoughtTickets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Broadcast]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Broadcast](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailAddress] [varchar](50) NULL,
	[MobileNumber] [varchar](50) NULL,
	[IsMailSend] [bit] NULL,
	[Event_Id] [int] NULL,
	[Created_at] [datetime] NULL,
	[CompanyId] [int] NULL,
	[PaymentId] [int] NULL,
	[BroadcastMessageId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BroadcastMessage]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BroadcastMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[Subject] [varchar](200) NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[Event_Id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BusinessOwner]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessOwner](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nchar](100) NOT NULL,
	[LastName] [nchar](100) NULL,
	[MobileNumber] [nchar](10) NULL,
	[Company_Id] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_BusinessOwner] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Country_Id] [int] NOT NULL,
	[Name_of_business] [nvarchar](250) NOT NULL,
	[Business_contact_number] [nvarchar](14) NOT NULL,
	[Business_Email_address] [nvarchar](max) NOT NULL,
	[Address_Id] [int] NULL,
	[Status] [int] NOT NULL,
	[Created_at] [datetime] NULL,
	[Updated_at] [datetime] NULL,
	[UserName] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[Parent_Id] [int] NULL,
	[GroupId] [int] NULL,
	[Logo] [varchar](200) NULL,
	[website] [varchar](200) NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Code] [varchar](5) NOT NULL,
	[AccessCode] [varchar](50) NULL,
	[Status] [int] NULL,
	[Zone] [int] NULL,
	[Phonecode] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CountryNew]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryNew](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[iso] [varchar](2) NOT NULL,
	[name] [varchar](80) NOT NULL,
	[nicename] [varchar](80) NOT NULL,
	[iso3] [varchar](3) NULL,
	[numcode] [int] NULL,
	[phonecode] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Coupon]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Coupon](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OfferId] [int] NOT NULL,
	[CoupanCode] [nvarchar](50) NULL,
	[Mobile] [varchar](100) NULL,
	[Isused] [bit] NULL,
	[OfferForwebSite] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Currency]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](100) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Status] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ErrText] [nvarchar](max) NULL,
	[ErrTime] [datetime] NULL,
	[FilePath] [nvarchar](max) NULL,
	[Method] [nvarchar](max) NULL,
	[ParentMethod] [varchar](500) NULL,
	[ParentFilePath] [varchar](500) NULL,
	[RequestString] [varchar](500) NULL,
	[ResponseString] [varchar](500) NULL,
	[User_Id] [varchar](500) NULL,
	[Company_Id] [varchar](500) NULL,
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Events]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Events](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Event_name] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Direction] [nvarchar](max) NULL,
	[GPS Location] [nvarchar](max) NULL,
	[Address_Id] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[longitude] [decimal](12, 9) NULL,
	[latitude] [decimal](12, 9) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Created_at] [datetime] NULL,
	[Updated_at] [datetime] NULL,
	[Venue] [varchar](500) NULL,
	[Company_Id] [int] NULL,
	[User_Id] [int] NULL,
	[SubscriptionType] [int] NULL,
	[Status] [int] NULL,
	[PublishDate] [datetime] NULL,
	[IsPopularEvent] [bit] NULL,
	[PaymetIdSubscription] [int] NULL,
	[Eventtype] [int] NULL,
	[LiveURL] [varchar](250) NULL,
	[eventCatg] [int] NULL,
	[streamType] [int] NULL,
	[webrtcURL] [varchar](250) NULL,
	[CreatedOnWebsite] [int] NULL,
	[IsStreamimg] [bit] NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventTicket]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventTicket](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ticket_Type] [int] NULL,
	[Section_Details] [nvarchar](max) NULL,
	[Quantity] [int] NULL,
	[AvailableQuantity] [int] NULL,
	[Event_Id] [int] NULL,
	[Price] [decimal](18, 2) NULL,
	[Created_at] [datetime] NULL,
	[Updated_at] [datetime] NULL,
	[TicketName] [nvarchar](max) NULL,
	[Seat] [nvarchar](max) NULL,
	[tableNo] [nvarchar](max) NULL,
	[GateNo] [nvarchar](max) NULL,
	[ColorArea] [nvarchar](max) NULL,
	[IsEnable] [bit] NULL,
	[Inviation_Id] [int] NULL,
	[TicketStatus] [int] NULL,
	[ValidDays] [int] NULL,
	[PaymentType] [int] NULL,
	[PaymentCurrency] [int] NULL,
	[StartDate] [datetime] NULL,
 CONSTRAINT [PK_EventTicket] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GroupObjectMap]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupObjectMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupMasterId] [int] NOT NULL,
	[ObjectMasterId] [int] NOT NULL,
	[Rights] [varchar](4) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HubtelPaymentResponse]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HubtelPaymentResponse](
	[Session] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[OrderInfo] [varchar](max) NULL,
	[SessionId] [nvarchar](50) NULL,
	[OrderId] [nvarchar](50) NULL,
	[ExtraData] [varchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[TicketCode] [nvarchar](50) NULL,
	[EventId] [int] NULL,
	[CompanyId] [int] NULL,
	[GuidId] [varchar](50) NULL,
	[Amount] [decimal](18, 2) NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_HubtelPaymentResponse] PRIMARY KEY CLUSTERED 
(
	[Session] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invitation]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invitation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NULL,
	[PaymentId] [int] NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[EmailAddress] [varchar](50) NULL,
	[MobileNumber] [varchar](50) NULL,
	[IsMailSend] [bit] NULL,
	[Event_Id] [int] NULL,
	[Created_at] [datetime] NULL,
	[Updated_at] [datetime] NULL,
	[Status] [int] NULL,
	[UserId] [int] NULL,
	[CompanyId] [int] NULL,
	[IsSmsSend] [bit] NULL,
	[SendType] [int] NULL,
	[Remark] [nvarchar](max) NULL,
 CONSTRAINT [PK_Invitation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Likedislike]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Likedislike](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[eventID] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[Likes] [int] NULL,
	[UpdatedOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveStreamChat]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiveStreamChat](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[eventID] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[Message] [varchar](500) NOT NULL,
	[Date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiveStreamUser]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiveStreamUser](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[eventID] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[IPAddress] [varchar](50) NULL,
	[LastActive] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MultimediaContent]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MultimediaContent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[URL] [nvarchar](max) NULL,
	[Event_Id] [int] NULL,
	[Mul_Type] [int] NULL,
	[Created_at] [datetime] NULL,
	[Updated_at] [datetime] NULL,
	[Mul_MainPic] [bit] NULL,
	[videotype] [int] NULL,
	[videoId] [nvarchar](20) NULL,
	[validdays] [int] NULL,
 CONSTRAINT [PK_MultimediaContent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReceiverId] [int] NULL,
	[GeneratedBy] [int] NULL,
	[GeneratedFor] [int] NULL,
	[Text] [nvarchar](max) NULL,
	[Isseen] [bit] NULL,
	[MsgType] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[MsgTitle] [varchar](100) NULL,
 CONSTRAINT [PK_MessagesNotification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotificationMessage]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MsgTitle] [varchar](200) NULL,
	[MsgText] [nvarchar](max) NOT NULL,
	[MsgId] [int] NULL,
	[UserId] [int] NULL,
	[CompanyId] [int] NULL,
	[ReplyedBy] [int] NULL,
	[Status] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ObjectGroup]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ObjectGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Groupname] [nvarchar](max) NULL,
	[CompanyId] [int] NULL,
	[InsertDate] [datetime] NULL,
	[LastChanged] [datetime] NULL,
 CONSTRAINT [PK_ObjectGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ObjectMaster]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ObjectMaster](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObjectName] [varchar](100) NULL,
	[CreatedBy] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[ModuleType] [varchar](10) NULL,
 CONSTRAINT [PK_ObjectMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Offers]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Offers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OfferType] [int] NULL,
	[Value] [decimal](18, 2) NULL,
	[EventId] [int] NULL,
	[OfferName] [nvarchar](50) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CompanyId] [int] NULL,
	[CoupenCode] [nvarchar](max) NULL,
	[IsDeleted] [bit] NULL,
	[PaymentID] [int] NULL,
	[Noofcoupons] [int] NULL,
	[OfferPageCategory] [int] NULL,
	[TicketType] [int] NULL,
	[OfferForwebSite] [int] NULL,
 CONSTRAINT [PK_Offers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OTPVerification]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OTPVerification](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[OTP] [varchar](20) NOT NULL,
	[created] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[BaseCurrencyId] [int] NOT NULL,
	[ExchangeRate] [decimal](18, 5) NOT NULL,
	[UserId] [int] NOT NULL,
	[RefundAmount] [decimal](18, 2) NULL,
	[TransactionId] [nvarchar](max) NOT NULL,
	[GatewayName] [nvarchar](max) NOT NULL,
	[GatewayRemark] [nvarchar](max) NULL,
	[PaymentRQURI] [nvarchar](max) NULL,
	[PaymentRSURI] [nvarchar](max) NULL,
	[PaymentDate] [datetime] NOT NULL,
	[RefundDate] [datetime] NULL,
	[CompanyId] [int] NULL,
	[adminfee] [decimal](18, 2) NULL,
	[PaymentFor] [nvarchar](100) NULL,
	[LastUpdated] [datetime] NULL,
	[ResponseTransactionId] [nvarchar](50) NULL,
	[MinimumFee] [decimal](18, 2) NULL,
	[Token] [nvarchar](max) NULL,
	[Session_id_Stripe] [nvarchar](max) NULL,
	[PaymentCurrencyType] [int] NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentSetup]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentSetup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvitationOrg] [decimal](18, 2) NULL,
	[InvitationUser] [decimal](18, 2) NULL,
	[Broadcast] [decimal](18, 2) NULL,
	[Platinum] [decimal](18, 2) NULL,
	[Gold] [decimal](18, 2) NULL,
	[Silver] [decimal](18, 2) NULL,
	[Bronze] [decimal](18, 2) NULL,
	[Adminfee] [decimal](18, 2) NULL,
	[InvitationUserSms] [decimal](18, 2) NULL,
	[InvitationOrgSms] [decimal](18, 2) NULL,
	[MinimumFee] [decimal](18, 2) NULL,
	[CoupanFee] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentSupportt]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentSupportt](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[email] [varchar](100) NOT NULL,
	[transactionId] [varchar](max) NOT NULL,
	[created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RSVP]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RSVP](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Namer] [varchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[Event_Id] [int] NULL,
 CONSTRAINT [PK_RSVP] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScanCompanyUser]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScanCompanyUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](190) NOT NULL,
	[Name] [varchar](280) NOT NULL,
	[Password] [varchar](280) NOT NULL,
	[CompanyId] [int] NULL,
	[Status] [int] NOT NULL,
	[Createdat] [datetime] NULL,
	[Updateat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScannerEventRel]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScannerEventRel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScanerId] [int] NULL,
	[EventId] [int] NULL,
	[status] [int] NOT NULL,
	[Createdat] [datetime] NULL,
	[Updateat] [datetime] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[test1]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[test1](
	[Trxn] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicketType]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TicketTypes] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TickeUserMap]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TickeUserMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TicketId] [int] NOT NULL,
	[Status] [int] NULL,
	[InvitationId] [int] NULL,
	[UserId] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[OfferCode] [int] NULL,
	[OfferRemark] [nvarchar](max) NULL,
	[ActualAmount] [decimal](18, 2) NULL,
	[ExpectedDeliveryDate] [datetime] NULL,
	[PaymemtId] [int] NULL,
	[OrderDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[Name] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[Qty] [int] NULL,
	[BarCodeNumber] [nvarchar](max) NULL,
	[IsCheckIn] [bit] NULL,
	[UpdateDate] [datetime] NULL,
	[IsTicketSendToUser] [bit] NULL,
	[Ticketforwebsite] [int] NULL,
	[PaymentCurrencyType] [int] NULL,
	[ScanUserId] [int] NULL,
 CONSTRAINT [PK_OrdersHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User_Heave]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_Heave](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NULL,
	[WhichEvent] [varchar](200) NULL,
	[Package] [varchar](200) NULL,
	[Firsttimejoin] [varchar](200) NULL,
	[PhoneNo] [varchar](50) NULL,
	[Address] [varchar](150) NULL,
	[Email] [varchar](150) NULL,
	[HearSource] [varchar](50) NULL,
	[Emergency_Contact] [varchar](50) NULL,
	[Medical_condition] [bit] NULL,
	[Above_condition] [varchar](150) NULL,
	[REFUND_POLICY] [bit] NULL,
	[Release_of_Liability] [bit] NULL,
	[Social_Media] [bit] NULL,
	[Payment] [bit] NULL,
	[user_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLoggedTable]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLoggedTable](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[VideoId] [int] NULL,
	[LastActiveDate] [datetime] NULL,
	[IsSessionStop] [bit] NULL,
	[VideoSession] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[PhoneNo] [nvarchar](14) NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[UserType] [int] NOT NULL,
	[UserStatus] [int] NOT NULL,
	[CreditBalanceTotal] [decimal](18, 2) NULL,
	[CreditBalanceDaily] [decimal](18, 2) NULL,
	[CreditBalanceMonthly] [decimal](18, 2) NULL,
	[CountryId] [int] NULL,
	[CurrencyId] [int] NULL,
	[NativeCurrencyId] [int] NULL,
	[ExchangeRate] [decimal](18, 5) NULL,
	[Password] [nvarchar](max) NULL,
	[ProfilePic] [nvarchar](max) NULL,
	[LastModified] [datetime] NULL,
	[IsEmailEnable] [bit] NULL,
	[IsNotiEnable] [bit] NULL,
	[IsMessageEnable] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[GroupId] [int] NULL,
	[Parent_Id] [int] NULL,
	[Phone_CountryCode] [varchar](10) NULL,
	[Token] [varchar](100) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users1]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users1](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[PhoneNo] [nvarchar](14) NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[UserType] [int] NOT NULL,
	[UserStatus] [int] NOT NULL,
	[CreditBalanceTotal] [decimal](18, 2) NULL,
	[CreditBalanceDaily] [decimal](18, 2) NULL,
	[CreditBalanceMonthly] [decimal](18, 2) NULL,
	[CountryId] [int] NULL,
	[CurrencyId] [int] NULL,
	[NativeCurrencyId] [int] NULL,
	[ExchangeRate] [decimal](18, 5) NULL,
	[Password] [nvarchar](max) NULL,
	[ProfilePic] [nvarchar](max) NULL,
	[LastModified] [datetime] NULL,
	[IsEmailEnable] [bit] NULL,
	[IsNotiEnable] [bit] NULL,
	[IsMessageEnable] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[GroupId] [int] NULL,
	[Parent_Id] [int] NULL,
	[Phone_CountryCode] [varchar](10) NULL,
	[Token] [varchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[USSD_Data]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[USSD_Data](
	[TICKET_CODES] [nvarchar](50) NOT NULL,
	[EVENT] [nvarchar](max) NOT NULL,
	[TICKET_NAME] [nvarchar](max) NOT NULL,
	[PRICE] [nvarchar](50) NOT NULL,
	[DATE] [nvarchar](50) NOT NULL,
	[IsCheckIn] [bit] NULL,
	[UpdateDate] [datetime] NULL,
	[PaymentId] [int] NULL,
	[UserId] [int] NULL,
	[Status] [int] NULL,
	[IsSmsSend] [bit] NULL,
	[EventId] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[ScannerId] [int] NULL,
	[Currency_Type] [int] NULL,
	[ticketid] [int] NULL,
	[ActualAmount] [decimal](18, 2) NULL,
	[Qty] [int] NULL,
	[BarCodeNumber] [varchar](50) NULL,
	[TicketCode] [nvarchar](50) NULL,
	[TicketDate] [nvarchar](50) NULL,
 CONSTRAINT [PK_USSD_Data] PRIMARY KEY CLUSTERED 
(
	[TICKET_CODES] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wallet]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wallet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[AdminFee] [decimal](18, 2) NULL,
	[EarnedAmount] [decimal](18, 2) NULL,
	[WithdrawAmount] [decimal](18, 2) NULL,
	[Balance] [decimal](18, 2) NULL,
	[PaymentId] [int] NULL,
	[Status] [int] NULL,
	[withdrawTransactionId] [nvarchar](max) NULL,
	[PaymentDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Withdraw]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Withdraw](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventId] [int] NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[AdminFee] [decimal](18, 2) NULL,
	[WithdrawAmount] [decimal](18, 2) NULL,
	[CompanyId] [int] NULL,
	[TransactionId] [nvarchar](max) NULL,
	[CurrencyId] [int] NULL,
	[BaseCurrencyId] [int] NULL,
	[ExchangeRate] [decimal](18, 5) NULL,
	[GatewayName] [nchar](10) NULL,
	[GatewayRemark] [nvarchar](max) NULL,
	[PaymentRQURI] [nvarchar](max) NULL,
	[PaymentRSURI] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[PaymentDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WithdrawReq]    Script Date: 5/30/2023 12:48:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WithdrawReq](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NULL,
	[RequestAmount] [decimal](18, 2) NULL,
	[Status] [int] NULL,
	[walletId] [int] NULL,
	[RequestDate] [datetime] NULL,
	[withdrawDate] [datetime] NULL,
	[PaymentMode] [int] NULL,
	[PaymentId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CountryNew] ADD  DEFAULT (NULL) FOR [iso3]
GO
ALTER TABLE [dbo].[CountryNew] ADD  DEFAULT (NULL) FOR [numcode]
GO
ALTER TABLE [dbo].[Address]  WITH NOCHECK ADD  CONSTRAINT [FK_Address_Country] FOREIGN KEY([Country_Id])
REFERENCES [dbo].[Country] ([Id])
GO
ALTER TABLE [dbo].[Address] NOCHECK CONSTRAINT [FK_Address_Country]
GO
ALTER TABLE [dbo].[BankDetalMapping]  WITH CHECK ADD  CONSTRAINT [FK_BankDetalMapping_BankDetails] FOREIGN KEY([MobileMoneyDetail_Id])
REFERENCES [dbo].[BankDetails] ([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[BankDetalMapping] CHECK CONSTRAINT [FK_BankDetalMapping_BankDetails]
GO
ALTER TABLE [dbo].[BankDetalMapping]  WITH CHECK ADD  CONSTRAINT [FK_BankDetalMapping_Company] FOREIGN KEY([Company_Id])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[BankDetalMapping] CHECK CONSTRAINT [FK_BankDetalMapping_Company]
GO
ALTER TABLE [dbo].[BankDetalMapping]  WITH CHECK ADD  CONSTRAINT [FK_BankDetalMapping_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[BankDetalMapping] CHECK CONSTRAINT [FK_BankDetalMapping_Users]
GO
ALTER TABLE [dbo].[Broadcast]  WITH CHECK ADD FOREIGN KEY([BroadcastMessageId])
REFERENCES [dbo].[BroadcastMessage] ([Id])
GO
ALTER TABLE [dbo].[Broadcast]  WITH CHECK ADD  CONSTRAINT [FK_Broadcast_Payments] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[Broadcast] CHECK CONSTRAINT [FK_Broadcast_Payments]
GO
ALTER TABLE [dbo].[BusinessOwner]  WITH CHECK ADD  CONSTRAINT [FK_BusinessOwner_Company] FOREIGN KEY([Company_Id])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[BusinessOwner] CHECK CONSTRAINT [FK_BusinessOwner_Company]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_Address] FOREIGN KEY([Address_Id])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_Company_Address]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_Country] FOREIGN KEY([Country_Id])
REFERENCES [dbo].[Country] ([Id])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_Company_Country]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_ObjectGroup] FOREIGN KEY([GroupId])
REFERENCES [dbo].[ObjectGroup] ([Id])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_Company_ObjectGroup]
GO
ALTER TABLE [dbo].[Coupon]  WITH CHECK ADD FOREIGN KEY([OfferId])
REFERENCES [dbo].[Offers] ([Id])
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Address] FOREIGN KEY([Address_Id])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Address]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Company] FOREIGN KEY([Company_Id])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Company]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Payments] FOREIGN KEY([PaymetIdSubscription])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Payments]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Users] FOREIGN KEY([User_Id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Users]
GO
ALTER TABLE [dbo].[EventTicket]  WITH CHECK ADD  CONSTRAINT [FK_EventTicket_Events] FOREIGN KEY([Event_Id])
REFERENCES [dbo].[Events] ([Id])
GO
ALTER TABLE [dbo].[EventTicket] CHECK CONSTRAINT [FK_EventTicket_Events]
GO
ALTER TABLE [dbo].[EventTicket]  WITH CHECK ADD  CONSTRAINT [FK_EventTicket_Invitation] FOREIGN KEY([Inviation_Id])
REFERENCES [dbo].[Invitation] ([Id])
GO
ALTER TABLE [dbo].[EventTicket] CHECK CONSTRAINT [FK_EventTicket_Invitation]
GO
ALTER TABLE [dbo].[GroupObjectMap]  WITH CHECK ADD  CONSTRAINT [FK_GroupObjectMap_ObjectGroup] FOREIGN KEY([GroupMasterId])
REFERENCES [dbo].[ObjectGroup] ([Id])
GO
ALTER TABLE [dbo].[GroupObjectMap] CHECK CONSTRAINT [FK_GroupObjectMap_ObjectGroup]
GO
ALTER TABLE [dbo].[GroupObjectMap]  WITH CHECK ADD  CONSTRAINT [FK_GroupObjectMap_ObjectMaster] FOREIGN KEY([ObjectMasterId])
REFERENCES [dbo].[ObjectMaster] ([Id])
GO
ALTER TABLE [dbo].[GroupObjectMap] CHECK CONSTRAINT [FK_GroupObjectMap_ObjectMaster]
GO
ALTER TABLE [dbo].[Invitation]  WITH CHECK ADD  CONSTRAINT [FK_Invitation_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[Invitation] CHECK CONSTRAINT [FK_Invitation_Company]
GO
ALTER TABLE [dbo].[Invitation]  WITH CHECK ADD  CONSTRAINT [FK_Invitation_Events] FOREIGN KEY([Event_Id])
REFERENCES [dbo].[Events] ([Id])
GO
ALTER TABLE [dbo].[Invitation] CHECK CONSTRAINT [FK_Invitation_Events]
GO
ALTER TABLE [dbo].[Invitation]  WITH CHECK ADD  CONSTRAINT [FK_Invitation_Payments] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[Invitation] CHECK CONSTRAINT [FK_Invitation_Payments]
GO
ALTER TABLE [dbo].[Invitation]  WITH CHECK ADD  CONSTRAINT [FK_Invitation_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Invitation] CHECK CONSTRAINT [FK_Invitation_Users]
GO
ALTER TABLE [dbo].[LiveStreamChat]  WITH CHECK ADD FOREIGN KEY([eventID])
REFERENCES [dbo].[Events] ([Id])
GO
ALTER TABLE [dbo].[LiveStreamChat]  WITH CHECK ADD  CONSTRAINT [FK__LiveStrea__userI__2A164134] FOREIGN KEY([userId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[LiveStreamChat] CHECK CONSTRAINT [FK__LiveStrea__userI__2A164134]
GO
ALTER TABLE [dbo].[MultimediaContent]  WITH CHECK ADD  CONSTRAINT [FK_Event] FOREIGN KEY([Event_Id])
REFERENCES [dbo].[Events] ([Id])
GO
ALTER TABLE [dbo].[MultimediaContent] CHECK CONSTRAINT [FK_Event]
GO
ALTER TABLE [dbo].[Offers]  WITH CHECK ADD  CONSTRAINT [FK_Offers_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[Offers] CHECK CONSTRAINT [FK_Offers_Company]
GO
ALTER TABLE [dbo].[Offers]  WITH CHECK ADD  CONSTRAINT [FK_Offers_Events] FOREIGN KEY([EventId])
REFERENCES [dbo].[Events] ([Id])
GO
ALTER TABLE [dbo].[Offers] CHECK CONSTRAINT [FK_Offers_Events]
GO
ALTER TABLE [dbo].[RSVP]  WITH CHECK ADD  CONSTRAINT [FK_RSVP_Events] FOREIGN KEY([Event_Id])
REFERENCES [dbo].[Events] ([Id])
GO
ALTER TABLE [dbo].[RSVP] CHECK CONSTRAINT [FK_RSVP_Events]
GO
ALTER TABLE [dbo].[ScanCompanyUser]  WITH CHECK ADD FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[ScannerEventRel]  WITH CHECK ADD FOREIGN KEY([EventId])
REFERENCES [dbo].[Events] ([Id])
GO
ALTER TABLE [dbo].[ScannerEventRel]  WITH CHECK ADD FOREIGN KEY([ScanerId])
REFERENCES [dbo].[ScanCompanyUser] ([Id])
GO
ALTER TABLE [dbo].[TickeUserMap]  WITH CHECK ADD  CONSTRAINT [FK_TickeUserMap_EventTicket] FOREIGN KEY([TicketId])
REFERENCES [dbo].[EventTicket] ([Id])
GO
ALTER TABLE [dbo].[TickeUserMap] CHECK CONSTRAINT [FK_TickeUserMap_EventTicket]
GO
ALTER TABLE [dbo].[TickeUserMap]  WITH CHECK ADD  CONSTRAINT [FK_TickeUserMap_Invitation] FOREIGN KEY([InvitationId])
REFERENCES [dbo].[Invitation] ([Id])
GO
ALTER TABLE [dbo].[TickeUserMap] CHECK CONSTRAINT [FK_TickeUserMap_Invitation]
GO
ALTER TABLE [dbo].[TickeUserMap]  WITH CHECK ADD  CONSTRAINT [FK_TickeUserMap_Payments] FOREIGN KEY([PaymemtId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[TickeUserMap] CHECK CONSTRAINT [FK_TickeUserMap_Payments]
GO
ALTER TABLE [dbo].[TickeUserMap]  WITH CHECK ADD  CONSTRAINT [FK_TickeUserMap_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[TickeUserMap] CHECK CONSTRAINT [FK_TickeUserMap_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_ObjectGroup] FOREIGN KEY([GroupId])
REFERENCES [dbo].[ObjectGroup] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_ObjectGroup]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Users] FOREIGN KEY([Parent_Id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Users]
GO
ALTER TABLE [dbo].[Wallet]  WITH CHECK ADD FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[Wallet]  WITH CHECK ADD  CONSTRAINT [FK__Wallet__PaymentI__17036CC0] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[Wallet] CHECK CONSTRAINT [FK__Wallet__PaymentI__17036CC0]
GO
ALTER TABLE [dbo].[WithdrawReq]  WITH CHECK ADD FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[WithdrawReq]  WITH CHECK ADD FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[WithdrawReq]  WITH CHECK ADD FOREIGN KEY([walletId])
REFERENCES [dbo].[Wallet] ([Id])
GO

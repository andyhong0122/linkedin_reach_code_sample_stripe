USE [Welrus]
GO
/****** Object:  StoredProcedure [dbo].[Payments_Insert_V2]    Script Date: 2/12/2022 2:03:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Andy Hong>
-- Create date: <12/4/20>
-- Description:	Insert V2 of Payment Accounts. 
-- =============================================


ALTER PROC [dbo].[Payments_Insert_V2]
		 @AppointmentId int 
		,@PaymentTypeId int
		,@StripeCustomerId nvarchar(250)
		,@ChargeResponse NVARCHAR(MAX)
		,@ChargeId nvarchar(250)
		,@ReceiptId nvarchar(250)
		,@ReceiptUrl nvarchar(250)
		,@Amount decimal(18, 2)

as

/*
=====================================

DECLARE 
		 @AppointmentId int = 1
		,@PaymentTypeId int = 3
		,@StripeCustomerId nvarchar(250) = 'abc'
		,@ChargeResponse NVARCHAR(MAX) = 'abc'
		,@ChargeId nvarchar(250) = 'abc' 
		,@ReceiptId nvarchar(250) = 'abc'
		,@ReceiptUrl nvarchar(250) = 'abc'
		,@Amount decimal(18, 2) = 10.00

EXECUTE [dbo].[Payments_Insert_V2]
		 @AppointmentId
		,@PaymentTypeId 
		,@StripeCustomerId 
		,@ChargeResponse 
		,@ChargeId 
		,@ReceiptId
		,@ReceiptUrl
		,@Amount
		
		select * from dbo.Payments
		select * from dbo.ProviderAppointments
        
=====================================
*/

BEGIN


DECLARE @ReturnedValues TABLE
			([ReturnedId] INT NOT NULL
			,[StripeCustomerId] NVARCHAR(255) NOT NULL
			,[ReceiptId] NVARCHAR(250) NULL
			,[ChargeId] NVARCHAR(250) NOT NULL
			,[ReceiptUrl] NVARCHAR(250) NOT NULL
			,[CreatedBy] INT)

INSERT INTO [dbo].[Payments]
			([AppointmentId]
			,[PaymentTypeId]
			,[StripeCustomerId]
			,[ChargeResponse]
			,[ChargeId]
			,[ReceiptUrl]
			,[Amount])

    VALUES
        (@AppointmentId
        ,@PaymentTypeId
        ,@StripeCustomerId
        ,@ChargeResponse
		,@ChargeId
		,@ReceiptId
        ,@Amount)

SELECT 
	  [AppointmentId]
     ,[StripeCustomerId]
	 ,[ReceiptId]
	 ,[ChargeId]
	 ,[ReceiptUrl]

FROM dbo.Payments


END

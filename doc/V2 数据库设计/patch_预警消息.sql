--�����ƷԤ����ʾ


-- =============================================
-- Author:		zjk
-- Create date: 2014-1-2
-- Description:	��ȡ��ƷԤ����Ϣ
-- =============================================
ALTER PROCEDURE [dbo].[uspGetWarningGoods]
	@WarningType int--1:�۳�Ԥ�� 2������Ԥ��
	,@WarningDays int -- Ԥ��ʱ�䣺������Ʒ�쵽����ǰ������Ԥ�����۳���Ʒ�������Ԥ��
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @sql as varchar(max);
	--set @sql = N'
		SET @WarningType = CASE WHEN @WarningType IS NULL THEN 2 ELSE @WarningType END;
		SET @WarningDays = CASE WHEN @WarningDays IS NULL THEN 7 ELSE @WarningDays END;

		SELECT 
			[Goods].[Id] as GoodsId
			,[Goods].[CategoryId] as GoodsCategoryId
			,[Goods].[SupplierId] as GoodsSupplierId
			,[Goods].[Code] as GoodsCode
			,[Goods].[OriginalCode] as GoodsOriginalCode
			,[Goods].[SourceType] as GoodsSourceType
			,[Goods].[Name] as GoodsName
			,[Goods].[Status] as GoodsStatus
			,[Goods].[Image] as GoodsImage
			,[Goods].[Color] as GoodsColor
			,[Goods].[Quality] as GoodsQuality
			,[Goods].[Parts] as GoodsParts
			,[Goods].[MarkPrice] as GoodsMarkPrice
			,[Goods].[PrimePrice] as GoodsPrimePrice
			,[Goods].[SalePrice] as GoodsSalePrice
			,[Goods].[Prepay] as GoodsPrepay
			,[Goods].[Discount] as GoodsDiscount
			,[Goods].[Desc] as GoodsDesc
			,[Goods].[Paid] as GoodsPaid
			,[Goods].[CreatedDate] as GoodsCreatedDate
			,[Goods].[SaledDate] as GoodsSaledDate
			,[Goods].[ConsignStartDate] as GoodsConsignStartData
			,[Goods].[ConsignEndDate] as GoodsConsignEndDate
			,[Goods].[UpdatedDate] as GoodsUpdatedDate
			,[PurchaseRecord].[Id] as PurchaseRecordId
			,[PurchaseRecord].[UserId] as PurchaseRecordUserId
			,[PurchaseRecord].[PayType] as PurchaseRecordPayType
			,[PurchaseRecord].[Remark] as PurchaseRecordRemark
			,[PurchaseRecord].[Operator] as PurchaseRecordOperator
			,ISNULL([SaledRecord].[Id],0) as SaledRecordId
			,ISNULL([SaledRecord].[GoodsId],0) as SaledRecordGoodsId
			,ISNULL([SaledRecord].[UserId],0) as SaledRecordUserId
			,ISNULL([SaledRecord].[PayType],0) as SaledRecordPayType
			,[SaledRecord].[Remark] as SaledRecordRemark
			,[SaledRecord].[CustomerName] as SaledRecordCustomerName
      ,[SaledRecord].[CustomerPhone] as SaledRecordCustomerPhone
			,[SaledRecord].[Operator] as SaledRecordOperator
  FROM [Goods]
			left join [PurchaseRecord] on [Goods].[Id] = [PurchaseRecord].[GoodsId] 
			left join [SaledRecord] on [Goods].[Id] = [SaledRecord].[GoodsId]
  WHERE 
		-- �۳�δ����Ԥ��
		-- 1�ڿ⣬2Ԥ��,3���۳� 4��ȡ��
		[Goods].[Status] = 3 --CASE WHEN @WarningType = 1 THEN 3 ELSE 1 END
		--1:δ��� 2���Ѵ��
		AND [Goods].[Paid] = 1 -- CASE WHEN @WarningType = 1 THEN 1 ELSE [Goods].[Paid] END
		--AND GETDATE() >= CASE WHEN @WarningType = 1 THEN CONVERT(varchar(100),DATEADD(DAY, -@WarningDays, [Goods].[SaledDate]), 23) ELSE GETDATE() END 

		-- ���۵��� Ԥ��
		AND LEFT([Goods].[Code],2) = CASE WHEN @WarningType = 2 THEN 'JS' ELSE LEFT([Goods].[Code],2) END
		AND GETDATE() >= CASE WHEN @WarningType = 2 THEN CONVERT(varchar(100),DATEADD(DAY, -@WarningDays, [Goods].[ConsignEndDate]), 23) ELSE GETDATE() END 
		AND GETDATE() <= CASE WHEN @WarningType = 2 THEN CONVERT(varchar(100),DATEADD(DAY, 1, [Goods].[ConsignEndDate]),23) ELSE GETDATE() END
		ORDER BY  
			CASE WHEN @WarningType = 2 THEN [Goods].[ConsignEndDate] ELSE [Goods].[SaledDate] END
	
END

go




-- =============================================
-- Author:		zjk
-- Create date: 2014-1-3
-- Description:	��ȡ��ƷԤ����Ϣ
-- =============================================
ALTER PROCEDURE [dbo].[uspGetWarningGoodsMsg]
	@WarningType int--1:�۳�Ԥ�� 2������Ԥ��
	,@WarningDays int -- Ԥ��ʱ�䣺������Ʒ�쵽����ǰ������Ԥ�����۳���Ʒ�������Ԥ��
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @sql as varchar(max);
	--set @sql = N'
		SET @WarningType = CASE WHEN @WarningType IS NULL THEN 2 ELSE @WarningType END;
		SET @WarningDays = CASE WHEN @WarningDays IS NULL THEN 7 ELSE @WarningDays END;

		SELECT 
		COUNT(0) as GoodsCount
		,CASE WHEN @WarningType = 1 THEN MAX([Goods].[SaledDate]) ELSE MAX([Goods].[ConsignEndDate]) end as MaxDate
		,CASE WHEN @WarningType = 1 THEN MIN([Goods].[SaledDate]) ELSE MIN([Goods].[ConsignEndDate]) end as MinDate
			
  FROM [Goods]
  WHERE 
		-- �۳�δ����Ԥ��
		-- 1�ڿ⣬2Ԥ��,3���۳� 4��ȡ��
		[Goods].[Status] = 3 -- CASE WHEN @WarningType = 1 THEN 3 ELSE 1 END
		--1:δ��� 2���Ѵ��
		AND [Goods].[Paid] = 1 -- CASE WHEN @WarningType = 1 THEN 1 ELSE [Goods].[Paid] END
		--AND GETDATE() >= CASE WHEN @WarningType = 1 THEN CONVERT(varchar(100),DATEADD(DAY, -@WarningDays, [Goods].[SaledDate]), 23) ELSE GETDATE() END 

		-- ���۵��� Ԥ��
		AND LEFT([Goods].[Code],2) = CASE WHEN @WarningType = 2 THEN 'JS' ELSE LEFT([Goods].[Code],2) END
		AND GETDATE() >= CASE WHEN @WarningType = 2 THEN CONVERT(varchar(100),DATEADD(DAY, -@WarningDays, [Goods].[ConsignEndDate]), 23) ELSE GETDATE() END 
		AND GETDATE() <= CASE WHEN @WarningType = 2 THEN CONVERT(varchar(100),DATEADD(DAY, 1, [Goods].[ConsignEndDate]),23) ELSE GETDATE() END
		--ORDER BY  
		--	CASE WHEN @WarningType = 2 THEN [Goods].[ConsignEndDate] ELSE [Goods].[SaledDate] END
	
END


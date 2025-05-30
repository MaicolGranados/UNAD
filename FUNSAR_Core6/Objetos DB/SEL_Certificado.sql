USE [funsaro1_prod]
GO
/****** Object:  StoredProcedure [funsaro1_desar].[SEL_Certificado]    Script Date: 26/07/2023 11:08:31 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [funsaro1_desar].[SEL_Certificado]
@documento varchar(50),
@tdocumento varchar(10)
AS
BEGIN
DECLARE @tdoc int;
SET @tdoc = (SELECT Id from TDocumento where tDocumento = @tdocumento);
SELECT TOP (1) Id
FROM [funsaro1_desar].[Certificado] WHERE Documento = @documento and DocumentoId = @tdoc;
RETURN
END
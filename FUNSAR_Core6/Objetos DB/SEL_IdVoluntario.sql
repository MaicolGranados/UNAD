USE [funsaro1_prod]
GO
/****** Object:  StoredProcedure [funsaro1_desar].[SEL_IdVoluntario]    Script Date: 26/07/2023 11:09:33 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [funsaro1_desar].[SEL_IdVoluntario]
@documento varchar(50),
@tdocumento int
AS
BEGIN
SELECT TOP (1) Id
FROM [funsaro1_desar].[Voluntario] WHERE Documento = @documento and DocumentoId = @tdocumento;
RETURN
END
USE [funsaro1_prod]
GO
/****** Object:  StoredProcedure [funsaro1_desar].[SEL_NomBrigada]    Script Date: 26/07/2023 11:10:26 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [funsaro1_desar].[SEL_NomBrigada]
@idBrigada int
AS
BEGIN
SELECT Nombre FROM funsaro1_desar.Brigada WHERE Id = @idBrigada;
RETURN
END
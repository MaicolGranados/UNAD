USE [funsaro1_prod]
GO
/****** Object:  StoredProcedure [funsaro1_desar].[SEL_Parametros]    Script Date: 26/07/2023 11:10:50 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [funsaro1_desar].[SEL_Parametros]
@brigada varchar(50),
@tdocumento varchar(10),
@semestre varchar(2)
AS
BEGIN
DECLARE @parametros TABLE(
idbrigada int,
idtdoc int,
idsemestre int
);
INSERT INTO @parametros VALUES ((SELECT Id from Brigada where Nombre = @brigada),(SELECT Id from TDocumento where tDocumento = @tdocumento),(SELECT Id from Semestre where semestre = @semestre));
SELECT * FROM @parametros;
RETURN
END
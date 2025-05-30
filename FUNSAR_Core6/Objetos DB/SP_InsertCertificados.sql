USE [funsaro1_prod]
GO
/****** Object:  StoredProcedure [funsaro1_desar].[SP_InsertCertificados]    Script Date: 26/07/2023 11:11:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [funsaro1_desar].[SP_InsertCertificados]
AS
BEGIN
DECLARE @count int;
SET @count = (SELECT COUNT(*) FROM CertificadoTemp);
INSERT INTO Certificado ([Nombre]
      ,[Apellido]
      ,[DocumentoId]
      ,[Documento]
      ,[BrigadaId]
      ,[codCertificado]
      ,[AnoProceso]
      ,[SemestreId]
      ,[FechaExpedicion]
      ,[EstadoId]) SELECT [Nombre]
      ,[Apellido]
      ,[DocumentoId]
      ,[Documento]
      ,[BrigadaId]
      ,[codCertificado]
      ,[AnoProceso]
      ,[SemestreId]
      ,[FechaExpedicion]
      ,[EstadoId] FROM CertificadoTemp;
DELETE FROM CertificadoTemp;
DBCC CHECKIDENT(CertificadoTemp, RESEED,0);
RETURN @count;
END
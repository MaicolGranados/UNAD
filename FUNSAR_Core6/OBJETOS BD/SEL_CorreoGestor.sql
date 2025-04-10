USE [funsaro1_prod]
GO
/****** Object:  StoredProcedure [funsaro1_desar].[SEL_CorreoGestor]    Script Date: 26/07/2023 11:09:10 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [funsaro1_desar].[SEL_CorreoGestor]
@idVoluntario int
AS
BEGIN
DECLARE @colegio int;
SET @colegio = (SELECT ColegioId FROM funsaro1_desar.Voluntario WHERE Id = @idVoluntario);
DECLARE @brigada int;
SET @brigada = (SELECT BrigadaId FROM funsaro1_desar.Colegio WHERE Id = @colegio);
SELECT TOP(1) Email FROM funsaro1_desar.AspNetUsers U INNER JOIN funsaro1_desar.AspNetUserRoles R ON U.Id = R.UserId WHERE U.BrigadaId = @brigada;
RETURN
END
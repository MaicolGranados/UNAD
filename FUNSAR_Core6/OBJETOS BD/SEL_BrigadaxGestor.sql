USE [funsaro1_prod]
GO
/****** Object:  StoredProcedure [funsaro1_desar].[SEL_BrigadaxGestor]    Script Date: 26/07/2023 11:07:50 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [funsaro1_desar].[SEL_BrigadaxGestor]
@user varchar(500)
AS
BEGIN
SELECT BrigadaId FROM funsaro1_desar.AspNetUsers WHERE UserName = @user;
RETURN
END
IF exists (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[p_CW_ObracunSatniceZaPeriod]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[p_CW_ObracunSatniceZaPeriod] 
GO

IF exists (SELECT * FROM sys.types WHERE name = 'TempTabelaSatnica')
    DROP TYPE TempTabelaSatnica
GO

CREATE TYPE TempTabelaSatnica AS TABLE
(
	ID int identity,
	IDZaposlenog int,
	Radnik nvarchar(100),
	Datum datetime,
	Minut int,
	Greska nvarchar(100)
)
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*************************************************************************

    Created By:   Nikola Bacic
    Created On:   10/12/2020
	Version:	  1.0.0
    Ticket No:    Trelo No-1
    Description:  Računa satnicu zaposlenih u minutama i vraća podatke 
				  vezane za željeni upit
    
 ************************************************************************/

CREATE PROCEDURE DBO.[p_CW_ObracunSatniceZaPeriod]                                   
   @VremeOd datetime,               
   @VremeDo datetime,
   @Operator varchar(2),
   @Sati int          
AS  


DECLARE
	@brUkupno int,
	@brojac int,
	@IdZaposlenog int,
	@Radnik varchar(100),
	@RegKod varchar(8),
	@VremeReg datetime,
	@LogicAktivnost varchar(3),
	@prethodnaLogicAktivnost varchar(3),
	@prethodniRadnik varchar(100),
	@satnica int, -- u minutama
	@prethodnoVremeReg datetime,
	@odabraniDatum date,
	@prethodniZaposlID int,
	@tmpInt int,
	@greska varchar(100),
	@NezatvorenaSatnica varchar(100),
	@NeispravniUzastopniUnosi varchar(100)

SET @VremeDo = dateadd(second, -1, dateadd(day,1, @VremeDo))

SET @NezatvorenaSatnica = 'Dan nije pravilno zatvoren.'
SET @NeispravniUzastopniUnosi = 'Nepravilan raspored logičke aktivnosti (#!OPIS!#)'

DECLARE @tmpRegs TABLE
(
	ID int identity,
	IDZaposlenog int,
	Radnik varchar(100),
	RegKod varchar(8),
	VremeReg datetime,
	LogickaAktivnost varchar(3)
)

DECLARE @tmpSatnica TempTabelaSatnica

INSERT INTO @tmpRegs(IDZaposlenog, Radnik, RegKod, VremeReg, LogickaAktivnost)
SELECT CAST(z.IDZaposlenog as int), z.Ime + ' ' + z.Prezime as Radnik,
	 SUBSTRING(zm.IDNo,7,8) as RegKod, r.TerminalskoVremeRegistracije, r.IDLogickeAktivnosti	
FROM Zaposleni z
	INNER JOIN Zaposleni_IDMemorija zm ON z.IDZaposlenog = zm.IDZaposlenog
	LEFT JOIN tblReg r ON SUBSTRING(zm.IDNo,7,8) = r.IDNo
WHERE r.TerminalskoVremeRegistracije between @VremeOd AND @VremeDo
	AND z.statuszaposlenog IS NULL
ORDER BY CAST(z.IDZaposlenog as int), r.TerminalskoVremeRegistracije

SELECT @brojac = 0, 
	@odabraniDatum = CAST(@VremeOd as date), 
	@prethodniZaposlID = 0

SELECT @brojac = MIN(ID), @brUkupno = MAX(ID) FROM @tmpRegs

WHILE (@brojac <= @brUkupno AND @brojac > 0)
BEGIN
	SELECT 
		@IdZaposlenog = IDZaposlenog,
		@Radnik = Radnik,
		@RegKod = RegKod,
		@VremeReg = VremeReg,
		@LogicAktivnost = LogickaAktivnost
	FROM @tmpRegs WHERE ID = @brojac

	SET @greska = null

	IF @LogicAktivnost = 'UV'
	BEGIN
		SET @brojac = @brojac + 1
		CONTINUE
	END
	IF(@prethodniZaposlID = @IdZaposlenog)
	BEGIN
		IF(@odabraniDatum = CAST(@VremeReg as date))
		BEGIN
			SET @tmpInt = datediff(mi, @prethodnoVremeReg, @VremeReg)
			IF(@LogicAktivnost IN ('P', 'I', 'PI'))
			BEGIN
				SET @satnica = @satnica + @tmpInt
				IF(@LogicAktivnost = 'I' AND @prethodnaLogicAktivnost = 'I')
				BEGIN
					SET @greska = REPLACE(@NeispravniUzastopniUnosi, '#!OPIS!#', 'uzastopni Izlaz')
					SET @satnica = 0
				END
			END
			IF(@LogicAktivnost IN ('U'))
			BEGIN
				IF(@prethodnaLogicAktivnost = 'PI') 
					SET @satnica = @satnica - CASE WHEN @tmpInt > 10 THEN @tmpInt ELSE 0 END
				IF(@prethodnaLogicAktivnost = 'P') 
					SET @satnica = @satnica + CASE WHEN @tmpInt > 30 THEN 30 - @tmpInt ELSE @tmpInt END
				IF(@prethodnaLogicAktivnost = 'U')
				BEGIN
					SET @greska = REPLACE(@NeispravniUzastopniUnosi, '#!OPIS!#', 'uzastopni Ulaz')
					SET @satnica = 0
				END
			END
			IF(@greska IS NOT NULL)
			BEGIN
				INSERT INTO @tmpSatnica(
					IDZaposlenog, 
					Radnik, 
					Datum, 
					Minut, 
					Greska
				)
				VALUES(
					@IdZaposlenog, 
					@Radnik, 
					@odabraniDatum, 
					CASE WHEN @satnica > 8*60 AND @satnica < 8*60+20 THEN 8*60 ELSE @satnica END, 
					@greska
				)

				SET @brojac = @brojac + 1
				CONTINUE
			END
		END
		ELSE
		BEGIN	--Novi datum
			IF(@satnica > 0)
			BEGIN
				SET @greska = null
				IF(@prethodnaLogicAktivnost <> 'I')
					SET @greska = @NezatvorenaSatnica
				INSERT INTO @tmpSatnica(
					IDZaposlenog, 
					Radnik, 
					Datum, 
					Minut, 
					Greska
				)
				VALUES(
					@prethodniZaposlID, 
					@prethodniRadnik, 
					@odabraniDatum, 
					CASE WHEN @satnica > 8*60 AND @satnica < 8*60+20 THEN 8*60 ELSE @satnica END, 
					@greska
				)
			END
			SELECT @prethodniZaposlID = @IdZaposlenog,
				@odabraniDatum = CAST(@VremeReg as date),
				@satnica = 0
		END
	END
	ELSE
	BEGIN -- Sledeci radnik
		IF(@satnica > 0)
			BEGIN
				SET @greska = null
				IF(@prethodnaLogicAktivnost <> 'I')
					SET @greska = @NezatvorenaSatnica
				INSERT INTO @tmpSatnica(
					IDZaposlenog, 
					Radnik, 
					Datum, 
					Minut, 
					Greska
				)
				VALUES(
					@prethodniZaposlID, 
					@prethodniRadnik, 
					@odabraniDatum, 
					CASE WHEN @satnica > 8*60 AND @satnica < 8*60+20 THEN 8*60 ELSE @satnica END, 
					@greska
				)
			END
		SET @satnica = 0
	END
	SELECT @prethodniRadnik = @Radnik,
		@prethodniZaposlID = @IdZaposlenog,
		@prethodnoVremeReg = @VremeReg,
		@prethodnaLogicAktivnost = CASE WHEN @LogicAktivnost = 'UV' THEN @prethodnaLogicAktivnost ELSE @LogicAktivnost END,
		@odabraniDatum = @VremeReg,
		@brojac = @brojac + 1

END

SET @odabraniDatum = CAST(@VremeOd as date)

WHILE(@odabraniDatum <= @VremeDo)
BEGIN 
	INSERT INTO @tmpSatnica(IDZaposlenog, Radnik, Datum, Minut, Greska)
	SELECT CAST(z.IDZaposlenog as int) as IDZaposlenog, z.Ime + ' ' + z.Prezime as Radnik, 
		@odabraniDatum, 0, 'Nema unosa'	
	FROM Zaposleni z
		INNER JOIN Zaposleni_IDMemorija zm ON z.IDZaposlenog = zm.IDZaposlenog
	WHERE SUBSTRING(zm.IDNo,7,8) NOT IN (SELECT IDNo FROM tblReg WHERE CAST(TerminalskoVremeRegistracije as date) = @odabraniDatum)
		and z.statuszaposlenog is null

	SET @odabraniDatum = DATEADD(day,1,@odabraniDatum)
END

DECLARE @sqlUpit nvarchar(max),
	@sqlParams nvarchar(100),
	@prm int

SET @prm = @Sati * 60

SET @sqlUpit = N' 
	SELECT IDZaposlenog, 
		Radnik, 
		Datum, 
		FORMAT(CAST(dateadd(minute,Minut,cast(''00:00:00'' as time)) as datetime2),N''HH:mm'') as Satnica, 
		Minut, 
		ISNULL(Greska,'''')
	FROM @tmpSatnica 
	WHERE Minut ' + 
		CASE 
			WHEN @Operator = 'LT' THEN '< @prm AND Minut > 0' 
			WHEN @Operator = 'LE' THEN '<= @prm AND Minut > 0'
			WHEN @Operator = 'GT' THEN '> @prm'
			WHEN @Operator = 'GE' THEN '>= @prm'
			ELSE '= @prm'
		END + '
	ORDER BY Radnik, Datum'

	SET @sqlParams = N'@prm int, @tmpSatnica TempTabelaSatnica readonly'

Exec sp_executesql @sqlUpit, @sqlParams, @prm, @tmpSatnica

GO
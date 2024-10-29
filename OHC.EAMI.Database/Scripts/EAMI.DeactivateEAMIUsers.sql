use master;
/*******************************************************************
* PURPOSE:	This sql script deactivates the users in @UserNameList from
*			the databases in @DBNameList.
* NOTES:	To run, simply enter the values for @UserNameList and @DBNameList. 
* CREATED:	Alex Hoang	6/17/2022
* MODIFIED 
* DATE			AUTHOR				DESCRIPTION
*-------------------------------------------------------------------
* 6/30/2022	Alex Hoang			Created Script
*******************************************************************/

begin try    
	  begin tran DeactivateEAMIUsers
			DECLARE @DBName AS VARCHAR(100), @sqlCurrentDB AS VARCHAR(100)
			declare @DBNameList table (DBName VARCHAR(100))
			insert @DBNameList(DBName) 
			values('EAMI') ,('EAMI-RX')  


			DECLARE @DBCounter INT
			--declare @ResultsList table (StatusEntry VARCHAR(100));
			select @DBCounter=1;
			WHILE ( @DBCounter <= (select count(*) from @DBNameList) )
			BEGIN

				SELECT @DBName = DBName FROM (
				SELECT 
				DBName, ROW_NUMBER() OVER(ORDER BY DBName asc) AS ROW
				FROM @DBNameList 
				) AS TMP1
				WHERE ROW = @DBCounter

			
				IF DB_ID(@DBName) IS NOT NULL
				BEGIN
					SELECT @DBName as 'Targeting Database'
				END
				ELSE
				BEGIN
					SELECT @DBName + ' Does Not Exist on this Server so will be skipped.' as 'Targeting Database'
					GOTO Increment_DBCounter
				END
				
				SET @DBName = QUOTENAME(@DBName);
				DECLARE @sql as varchar(max)
				select @sql = N'USE ' + @DBName + N';' + CHAR(13)+CHAR(10) +
				'declare @ResultsList table (StatusEntry VARCHAR(100));' + CHAR(13)+CHAR(10) +
				'declare @UserName as VARCHAR(100), @UNCounter INT, @IsActive bit' + CHAR(13)+CHAR(10) +
				'declare @UserNameList table (UserName VARCHAR(100));' + CHAR(13)+CHAR(10) +

				'insert @UserNameList(UserName) values(''TChang''),(''ptempall''),(''AHankins''),(''TTran''),(''KMartine'')' + CHAR(13)+CHAR(10) +

				'select @UNCounter=1;' + CHAR(13)+CHAR(10) +
				'WHILE ( @UNCounter <= (select count(*) from @UserNameList) )' + CHAR(13)+CHAR(10) +
				'BEGIN' + CHAR(13)+CHAR(10) +
					'SELECT @UserName = UserName FROM (' + CHAR(13)+CHAR(10) +
					'SELECT ' + CHAR(13)+CHAR(10) +
					'UserName, ROW_NUMBER() OVER(ORDER BY UserName asc) AS ROW' + CHAR(13)+CHAR(10) +
					'FROM @UserNameList ' + CHAR(13)+CHAR(10) +
					') AS TMP2' + CHAR(13)+CHAR(10) +
					'WHERE ROW = @UNCounter' + CHAR(13)+CHAR(10) +
		
					'set @IsActive = (select IsActive from TB_USER where User_Name = @UserName)' + CHAR(13)+CHAR(10) +
					'if @IsActive is null' + CHAR(13)+CHAR(10) +
						'begin' + CHAR(13)+CHAR(10) +
							'insert @ResultsList(StatusEntry) values(''User '' + @UserName + '' is NOT in TB_USER.'')' + CHAR(13)+CHAR(10) + 
						'end;' + CHAR(13)+CHAR(10) +
					'else' + CHAR(13)+CHAR(10) +
						'begin' + CHAR(13)+CHAR(10) +
							'if @IsActive = 0' + CHAR(13)+CHAR(10) +
								'begin' + CHAR(13)+CHAR(10) +
									'insert @ResultsList(StatusEntry) values(''User '' + @UserName + '' is already DEACTIVE in TB_USER.'')' + CHAR(13)+CHAR(10) + 
								'end' + CHAR(13)+CHAR(10) +
						'else' + CHAR(13)+CHAR(10) +
							'begin' + CHAR(13)+CHAR(10) +
								'update TB_USER set IsActive = 0 where User_Name = @UserName' + CHAR(13)+CHAR(10) +
								'insert @ResultsList(StatusEntry) values(''User '' + @UserName + '' has been set to DEACTIVE in TB_USER.'')' + CHAR(13)+CHAR(10) + 
							'end' + CHAR(13)+CHAR(10) +
						'end;' + CHAR(13)+CHAR(10) +
					'set @UNCounter  = @UNCounter  + 1' + CHAR(13)+CHAR(10) +
				'END' + CHAR(13)+CHAR(10) +

				'select StatusEntry as ''User Status'' from @ResultsList;' + CHAR(13)+CHAR(10)

				exec (@sql)

				Increment_DBCounter:
					SET @DBCounter  = @DBCounter  + 1 
			END

	  commit tran
	  
end try
begin catch

	declare @ErrorMessage nvarchar(4000);
	declare @ErrorSeverity int;
	declare @ErrorState int;

	select @ErrorMessage = ERROR_MESSAGE(),
		   @ErrorSeverity = ERROR_SEVERITY(),
		   @ErrorState = ERROR_STATE();

	if @@TRANCOUNT > 0         
		rollback tran DeactivateEAMIUsers

		print '@ErrorMessage is ' + @ErrorMessage
				print @ErrorSeverity
						print @ErrorState

	RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState) 

end catch







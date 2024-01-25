ALTER DATABASE SCOPED CONFIGURATION CLEAR PROCEDURE_CACHE;
select count(*) from sys.dm_exec_cached_plans;

EXEC sp_updatestats;

SELECT st.text, qs.EXECUTION_COUNT --, qs.*, cp.* 
FROM sys.dm_exec_query_stats AS qs 
CROSS APPLY sys.dm_exec_sql_text(sql_handle) AS st
CROSS APPLY sys.dm_exec_query_plan(plan_handle) AS cp
--WHERE st.text like '%FROM [Genre]%'
OPTION(RECOMPILE);
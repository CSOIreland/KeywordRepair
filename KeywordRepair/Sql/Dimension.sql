﻿SELECT RLS_ID
,MDM_ID
,MDM_CODE
,MDM_VALUE 
,LNG_ISO_CODE 
,LNG_ID 
FROM TD_MATRIX_DIMENSION 
INNER JOIN
(
	select MTR_ID
	,RLS_ID 
	,LNG_ISO_CODE 
	,LNG_ID 
	from TD_MATRIX 
	INNER JOIN VW_RELEASE_LIVE_NOW 
	ON MTR_ID=VRN_MTR_ID 
	INNER JOIN TD_RELEASE 
	ON RLS_ID=VRN_RLS_ID
	INNER JOIN TS_LANGUAGE 
	ON MTR_LNG_ID=LNG_ID 
	AND LNG_DELETE_FLAG=0
) Q
ON Q.MTR_ID=MDM_MTR_ID 
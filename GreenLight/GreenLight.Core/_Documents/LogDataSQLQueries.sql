/*
Select * from GLOSAEventLog order by createdAt desc

Select IntersectionId, COUNT(IntersectionId) from GLOSAEventLog where RouteSession = '14d3f32f-5386-440e-90e1-4e574247cb9c' group by  IntersectionId 

select * from GLOSAMonitoringLog

select top 100 * from GLOSAEventLog order by createdAt desc

select [URL] ,Count([URL]) from GLOSAMonitoringLog where statuscode = 200 and createdAt >= '20180129'  group by [URL]

select top 50 AVG(Latency) from GLOSAMonitoringLog where StatusCode = 200 order by createdAt desc

select count(id) from GLOSAEventLog order by createdAt desc */

select VehicleId,
createdAt,
Longitude,
Latitude,
DeviceTime,
TimeOffset,
IntersectionId,
[Event],
Distance,
--RouteSession,
Speed,
CalculationAdvisory,
SPAT,
--heading,
MAP,
Lane from dbo.GLOSAEventLog 
--where VehicleId = 'cit09750'
--and 
where
createdAt between '2019-FEB-10' and '2019-FEB-20' 
order by createdAt desc
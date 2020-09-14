using Microsoft.EntityFrameworkCore.Migrations;

namespace TempLoggerService.Api.Migrations
{
    public partial class sp_GetTempRange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE PROCEDURE [dbo].[GetTempRange]
	-- Add the parameters for the stored procedure here
	@device uniqueidentifier,
	@start datetime2,
	@end datetime2
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT CAST(Timestamp as DATE) as d, datepart(HOUR, Timestamp) as h, avg(Value) as avgtemp
FROM [dbo].[Temperatures]
where timestamp > @start and timestamp < @end
and deviceID = @device
group by CAST(timestamp as DATE),datepart(HOUR, timestamp)
order by d, h

END
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[GetTempRange]");
        }
    }
}

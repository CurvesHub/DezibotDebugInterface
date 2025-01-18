using DezibotDebugInterface.Api.Tests.TestCommon;

namespace DezibotDebugInterface.Api.Tests.Endpoints.Sessions;

public class DeleteSessionEndpointsTests() : BaseDezibotTestFixture(nameof(DeleteSessionEndpointsTests))
{
    // TODO: Add tests for:
    // - DeleteAllSessions_WhenSessionExists_ShouldDeleteAllSessions
    // - DeleteAllSessions_WhenSessionNotExists_ShouldReturnNoContent
    
    // - DeleteSession_WhenSessionExistsAndNotActive_ShouldDeleteSession
    // - DeleteSession_WhenSessionExistsAndIsActive_ShouldReturnConflict
    // - DeleteSession_WhenSessionNotExists_ShouldReturnNotFound
    
    // - DeleteDezibotFromSession_WhenDezibotExistsInSession_ShouldDeleteDezibotFromSession
    // - DeleteDezibotFromSession_WhenDezibotNotExistsInSession_ShouldReturnNotFound (OnlyInOtherSession -> Not Deleted!)
    // - DeleteDezibotFromSession_WhenSessionNotExists_ShouldReturnNotFound
}
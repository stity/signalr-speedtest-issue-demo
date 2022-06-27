"use strict";
function buildConnection(url) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect([0, 2000, 10000, 30000]) // set the time in ms at which we should try to reconnect, if the last try fails connection is lost
        // For more control over the timing and number of automatic reconnect attempts, 
        // withAutomaticReconnect accepts an object implementing the IRetryPolicy interface, 
        // which has a single method named nextRetryDelayInMilliseconds
        .build();
    return connection;
}
const speedTest = buildConnection('/speedTest?session=ede7d2dd-77cc-4c51-9f38-1d7b419823d7');

speedTest.on('Log', function (value) {
    console.log(value);
});

// withAutomaticReconnect won't configure the HubConnection to retry initial start failures, so start failures need to be handled manually
async function start(signalRConnection) {
    try {
        await signalRConnection.start();
        console.assert(signalRConnection.state === signalR.HubConnectionState.Connected);
        console.log("SignalR Connected.");
    } catch (err) {
        console.assert(signalRConnection.state === signalR.HubConnectionState.Disconnected);
        console.log(err);
        setTimeout(() => start(), 5000);
    }
}


async function main() {

    await start(speedTest);
    await speedTest.invoke("StartSpeedTest");
}
main();

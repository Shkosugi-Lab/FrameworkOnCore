using System;
using System.Diagnostics;

// EventLogEntryType's values are the same everywhere; EventLog's members are called on Windows only.
#pragma warning disable CA1416

namespace FrameworkOnCore;

/// <summary>
/// The event log, as the application writes to it (EventLog.WriteEntry, and the source it checks for or creates first):
/// on Windows, the event log; elsewhere, where .NET throws PlatformNotSupportedException, a line on the standard error,
/// which the service's journal (systemd) or the container's log keeps. An application logging to it on every request
/// (openIMIS: every login, every error) fails there otherwise. The converter calls these for EventLog's
/// (rules/packages.json platformReplacements); a call on an EventLog passes it first.
/// </summary>
public static class EventLogs
{
    // ---- EventLog's static members

    public static void WriteEntry(string source, string message) => WriteEntry(source, message, EventLogEntryType.Information, 0, 0, null);

    public static void WriteEntry(string source, string message, EventLogEntryType type) => WriteEntry(source, message, type, 0, 0, null);

    public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID) => WriteEntry(source, message, type, eventID, 0, null);

    public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID, short category) => WriteEntry(source, message, type, eventID, category, null);

    public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID, short category, byte[] rawData)
    {
        if (OperatingSystem.IsWindows()) EventLog.WriteEntry(source, message, type, eventID, category, rawData);
        else Write(source, message, type, eventID);
    }

    /// <summary>Elsewhere every source exists: its entries go to the standard error.</summary>
    public static bool SourceExists(string source) => !OperatingSystem.IsWindows() || EventLog.SourceExists(source);

    public static bool SourceExists(string source, string machineName) => !OperatingSystem.IsWindows() || EventLog.SourceExists(source, machineName);

    public static void CreateEventSource(string source, string logName)
    {
        if (OperatingSystem.IsWindows()) EventLog.CreateEventSource(source, logName);
    }

    public static void CreateEventSource(string source, string logName, string machineName)
    {
        if (OperatingSystem.IsWindows()) EventLog.CreateEventSource(new EventSourceCreationData(source, logName) { MachineName = machineName });
    }

    public static void CreateEventSource(EventSourceCreationData sourceData)
    {
        if (OperatingSystem.IsWindows()) EventLog.CreateEventSource(sourceData);
    }

    // ---- An EventLog's (the receiver first). Elsewhere .NET's EventLog cannot be made (its constructor throws): Windows'.

    public static void WriteEntry(EventLog log, string message) => WriteEntry(log, message, EventLogEntryType.Information, 0, 0, null);

    public static void WriteEntry(EventLog log, string message, EventLogEntryType type) => WriteEntry(log, message, type, 0, 0, null);

    public static void WriteEntry(EventLog log, string message, EventLogEntryType type, int eventID) => WriteEntry(log, message, type, eventID, 0, null);

    public static void WriteEntry(EventLog log, string message, EventLogEntryType type, int eventID, short category) => WriteEntry(log, message, type, eventID, category, null);

    public static void WriteEntry(EventLog log, string message, EventLogEntryType type, int eventID, short category, byte[] rawData)
    {
        if (OperatingSystem.IsWindows()) log.WriteEntry(message, type, eventID, category, rawData);
        else Write(log.Source, message, type, eventID);
    }

    static void Write(string source, string message, EventLogEntryType type, int eventID) =>
        Console.Error.WriteLine($"{source}: {type} {eventID}: {message}");
}

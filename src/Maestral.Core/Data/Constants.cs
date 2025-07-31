// Copyright © 2025 xbabco. All rights reserved.

namespace Maestral.Core.Data;

public static class Constants
{
    public const string DatabaseFilename = "AppSQLite.db3";

    public static string DatabasePath =>
        $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}";
}

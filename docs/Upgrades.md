# Upgrades

## Version Compatibility

| From Version | To Version | Breaking Changes | Data Migration Required |
|-------------|------------|------------------|------------------------|
| 0.2.0-prerelease-2 | 0.3.0-prerelease-3 | Yes | Yes |
| 0.1.0-prerelease-1 | 0.2.0-prerelease-2 | No | No |

## Upgrade Instructions

### 0.2.0.prerelease-2 -> 0.3.0-prerelease-3

The following core custom module table has been changed:

- `KenticoAiraUnified_AiraUnifiedChatPromptGroup`

#### Steps to Upgrade:

1. **Backup Your Data**
   ```sql
   -- Create backup tables
   SELECT * INTO KenticoAiraUnified_AiraUnifiedChatPromptGroup_backup 
   FROM KenticoAiraUnified_AiraUnifiedChatPromptGroup;
   
   SELECT * INTO KenticoAiraUnified_AiraUnifiedChatPrompt_backup 
   FROM KenticoAiraUnified_AiraUnifiedChatPrompt;
   ```

2. **Install the New Package**
   ```powershell
   dotnet add package Kentico.Xperience.AiraUnified --version 0.3.0-prerelease-3
   ```

3. **Clean Up Old Tables**
   ```sql
   -- Drop old tables
   DROP TABLE KenticoAiraUnified_AiraUnifiedChatPromptGroup;
   DROP TABLE KenticoAiraUnified_AiraUnifiedChatPrompt;

   -- Remove old class definitions
   DELETE FROM [dbo].[CMS_Class] 
   WHERE ClassName IN (
       'KenticoAiraUnified.AiraUnifiedChatPromptGroup', 
       'KenticoAiraUnified.AiraUnifiedChatPrompt'
   );
   ```

4. **Restart Your Application**
   The new custom module classes will be automatically installed on startup.

5. **Verify the Upgrade**
   - Check that the Aira Unified application is accessible.
   - Verify that chat functionality works.
   - Confirm that the smart uploader is operational.

## Data Migration

Currently, there is no automated migration path for chat history and configuration data between versions. This is due to potential breaking changes in the database schema.

### Manual Migration Options

1. **Export/Import Data**
   - Export relevant data from backup tables.
   - Transform the data to match the new schema.
   - Import into the new tables.

2. **Start Fresh**
   - If the data is not critical, you can start with a fresh installation.
   - Users will need to reconfigure their settings.

## Uninstall

To completely remove the Aira Unified integration:

1. Remove the NuGet package from your solution:
   ```powershell
   dotnet remove package Kentico.Xperience.AiraUnified
   ```

2. Run the following SQL to clean up the database:
   ```sql
   -- Drop all Aira Unified tables
   DROP TABLE KenticoAiraUnified_AiraUnifiedChatMessage;
   DROP TABLE KenticoAiraUnified_AiraUnifiedChatPromptGroup;
   DROP TABLE KenticoAiraUnified_AiraUnifiedChatPrompt;
   DROP TABLE KenticoAiraUnified_AiraUnifiedChatSummary;
   DROP TABLE KenticoAiraUnified_AiraUnifiedChatThread;
   DROP TABLE KenticoAiraUnified_AiraUnifiedConfigurationItem;

   -- Remove class definitions
   DELETE FROM [dbo].[CMS_Class] 
   WHERE ClassName LIKE 'kenticoairaunified%';

   -- Remove the resource
   DELETE FROM [CMS_Resource] 
   WHERE ResourceName = 'CMS.Integration.AiraUnified';
   ```

3. Clean up any remaining files:
   - Remove the Aira Unified configuration from `appsettings.json`.
   - Remove any custom routes or middleware from `Program.cs`.
   - Delete any custom assets from the `wwwroot` directory.

## Troubleshooting

If you encounter issues during the upgrade:

1. **Check the Logs**
   - Review the application logs for any errors.
   - Check the database logs for SQL errors.

2. **Verify Permissions**
   - Ensure the application has proper database permissions.
   - Check that all required roles are assigned.

3. **Rollback Plan**
   - If the upgrade fails, restore from your backup.
   - Reinstall the previous version of the package.
   - Contact support if issues persist.